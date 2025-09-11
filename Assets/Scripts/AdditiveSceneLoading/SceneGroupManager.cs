using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Systems.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        SceneGroup ActiveSceneGroup;
    // Scenes preloaded with allowSceneActivation=false (will finalize on ActivateScene)
    readonly Dictionary<string, AsyncOperation> _deferredPreloads = new Dictionary<string, AsyncOperation>();
        // All known groups (optional) so we can detect cross-group scene activation
        readonly List<SceneGroup> _registeredGroups = new List<SceneGroup>();

        public void RegisterSceneGroups(IEnumerable<SceneGroup> groups)
        {
            _registeredGroups.Clear();
            if (groups == null) return;
            _registeredGroups.AddRange(groups);
        }

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            // Clear old deferred operations when loading a fresh group
            _deferredPreloads.Clear();
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            // Load ONLY the designated ActiveScene from the group to avoid
            // running scripts (Awake/Start) from other scenes at bootstrap.
            var activeSceneName = ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene);
            if (string.IsNullOrEmpty(activeSceneName))
            {
                Debug.LogError("[SceneGroupManager] Active scene name not found in the scene group.");
                return;
            }

            if (reloadDupScenes == false && loadedScenes.Contains(activeSceneName))
            {
                Debug.Log($"[SceneGroupManager] Active scene '{activeSceneName}' already loaded.");
            }
            else
            {
                var op = SceneManager.LoadSceneAsync(activeSceneName, LoadSceneMode.Additive);
                while (!op.isDone)
                {
                    progress?.Report(op.progress);
                    await Task.Delay(50);
                }
                OnSceneLoaded.Invoke(activeSceneName);
            }

            // Preload remaining scenes in the group with deferred activation so their scripts don't run yet
            foreach (var sceneData in ActiveSceneGroup.Scenes)
            {
                var name = sceneData.Name;
                if (name == activeSceneName) continue;
                if (loadedScenes.Contains(name)) continue;

                var preloadOp = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                if (preloadOp == null) continue;
                preloadOp.allowSceneActivation = false;
                _deferredPreloads[name] = preloadOp;

                // Progress feedback while preloading to 0.9
                while (preloadOp.progress < 0.9f)
                {
                    progress?.Report(preloadOp.progress);
                    await Task.Delay(50);
                }
                // Now it's ready to be activated later; do NOT invoke OnSceneLoaded here
            }

            Scene activeScene = SceneManager.GetSceneByName(activeSceneName);

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
                EnableActiveSceneObjects(activeScene);
                EnableActiveSceneCameras(activeScene);
                EnableActiveSceneUI(activeScene);
                EnableActiveSceneLights(activeScene);
                DisableInactiveSceneCameras(activeScene);
                DisableInactiveSceneUI(activeScene);
                DisableInactiveSceneLights(activeScene);
                DisableInactiveSceneObjects(activeScene);
                ManageDontDestroyOnLoadObjects(activeScene.name);
            }

            OnSceneGroupLoaded.Invoke();

        }

        public async Task UnloadScenes()
        {
            var bootstrapper = SceneManager.GetSceneByName("Bootstrapper");
            if (bootstrapper.IsValid() && bootstrapper.isLoaded)
            {
                SceneManager.SetActiveScene(bootstrapper);
            }

            var scenesToUnload = new List<string>();

            int sceneCount = SceneManager.sceneCount;

            for (var i = sceneCount - 1; i >= 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName == "Bootstrapper" || sceneName == "DontDestroyOnLoad") continue;
                scenesToUnload.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenesToUnload.Count);

            foreach (var scene in scenesToUnload)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);
                OnSceneUnloaded.Invoke(scene);
            }

            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }

            await Resources.UnloadUnusedAssets();
        }

        void DisableInactiveSceneCameras(Scene activeScene)
        {
            Camera[] allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
            
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.scene != activeScene)
                {
                    cam.enabled = false;
                }
            }
        }

        void EnableActiveSceneCameras(Scene activeScene)
        {
            Camera[] allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.scene == activeScene)
                {
                    cam.enabled = true;
                }
            }
        }

        void DisableInactiveSceneUI(Scene activeScene)
        {
            Canvas[] allCanvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
            
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.gameObject.scene != activeScene && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas.enabled = false;
                }
            }
        }

        void EnableActiveSceneUI(Scene activeScene)
        {
            Canvas[] allCanvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.gameObject.scene == activeScene)
                {
                    canvas.enabled = true;
                }
            }
        }

        void DisableInactiveSceneLights(Scene activeScene)
        {
            Light[] allLights = UnityEngine.Object.FindObjectsOfType<Light>();
            
            foreach (Light light in allLights)
            {
                if (light.gameObject.scene != activeScene)
                {
                    light.enabled = false;
                }
            }
        }

        void EnableActiveSceneLights(Scene activeScene)
        {
            Light[] allLights = UnityEngine.Object.FindObjectsOfType<Light>();
            foreach (Light light in allLights)
            {
                if (light.gameObject.scene == activeScene)
                {
                    light.enabled = true;
                }
            }
        }

        void DisableInactiveSceneObjects(Scene activeScene)
        {
            Debug.Log($"DisableInactiveSceneObjects called for active scene: {activeScene.name}");
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"Processing scene {i}: {scene.name}, isLoaded: {scene.isLoaded}");
                
                if (scene == activeScene || !scene.isLoaded) 
                {
                    Debug.Log($"Skipping scene {scene.name} (active or not loaded)");
                    continue;
                }
                
                GameObject[] rootObjects = scene.GetRootGameObjects();
                Debug.Log($"Found {rootObjects.Length} root objects in scene {scene.name}");
                
                foreach (GameObject rootObj in rootObjects)
                {
                    if (rootObj.name.Contains("DontDestroyOnLoad") || 
                        rootObj.name.Contains("Bootstrapper") ||
                        rootObj.name.Contains("SceneLoader"))
                    {
                        Debug.Log($"Keeping essential object: {rootObj.name}");
                        continue;
                    }
                    
                    Debug.Log($"Disabling object: {rootObj.name} in scene {scene.name}");
                    rootObj.SetActive(false);
                }
            }
        }

        void EnableActiveSceneObjects(Scene activeScene)
        {
            Debug.Log($"EnableActiveSceneObjects called for scene: {activeScene.name}");
            
            GameObject[] rootObjects = activeScene.GetRootGameObjects();
            Debug.Log($"Found {rootObjects.Length} root objects in active scene");
            
            foreach (GameObject rootObj in rootObjects)
            {
                Debug.Log($"Enabling object: {rootObj.name}");
                rootObj.SetActive(true);
            }
        }

        void ManageDontDestroyOnLoadObjects(string activeSceneName)
        {
            Debug.Log($"ManageDontDestroyOnLoadObjects called for scene: {activeSceneName}");
            
            Scene dontDestroyScene = default;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"Found scene: {scene.name}");
                if (scene.name == "DontDestroyOnLoad")
                {
                    dontDestroyScene = scene;
                    break;
                }
            }
            
            if (!dontDestroyScene.IsValid()) 
            {
                Debug.LogWarning("DontDestroyOnLoad scene not found!");
                return;
            }
            
            GameObject[] dontDestroyObjects = dontDestroyScene.GetRootGameObjects();
            Debug.Log($"Found {dontDestroyObjects.Length} objects in DontDestroyOnLoad");
            
            foreach (GameObject obj in dontDestroyObjects)
            {
                Debug.Log($"Processing object: {obj.name}");
                
                if (obj.name.Contains("ShopScene") || obj.name.Contains("Shop"))
                {
                    bool shouldBeActive = activeSceneName == "ShopScene";
                    Debug.Log($"Shop object {obj.name}: setting to {shouldBeActive}");
                    obj.SetActive(shouldBeActive);
                }
                else if (obj.name.Contains("Combat") || obj.name.Contains("Battle"))
                {
                    bool shouldBeActive = activeSceneName.Contains("Combat") || activeSceneName.Contains("Battle");
                    Debug.Log($"Combat object {obj.name}: setting to {shouldBeActive}");
                    obj.SetActive(shouldBeActive);
                }
                else if (obj.name.Contains("Map") || obj.name.Contains("Generation"))
                {
                    bool shouldBeActive = activeSceneName == "MapGeneration";
                    Debug.Log($"Map object {obj.name}: setting to {shouldBeActive}");
                    obj.SetActive(shouldBeActive);
                }
                else if (obj.name.Contains("SceneLoader") || obj.name.Contains("Bootstrapper") ||
                         obj.name.Contains("EventSystem") || obj.name.Contains("Manager"))
                {
                    Debug.Log($"System object {obj.name}: keeping active");
                }
                else
                {
                    Debug.Log($"Unknown object {obj.name}: disabling");
                    obj.SetActive(false);
                }
            }
        }

        public async Task SwitchToSceneGroup(SceneGroup group)
        {
            await UnloadScenes();
            await LoadScenes(group, null);
        }

        public async Task SwitchToScene(string sceneName)
        {
            await UnloadScenes();
            
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            do
            {
                await Task.Delay(100);
            } while (operation.progress < 0.9f);

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                await Task.Delay(100);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            OnSceneLoaded.Invoke(sceneName);
            
            var activeScene = SceneManager.GetSceneByName(sceneName);
            EnableActiveSceneObjects(activeScene);
            EnableActiveSceneCameras(activeScene);
            EnableActiveSceneUI(activeScene);
            EnableActiveSceneLights(activeScene);
            DisableInactiveSceneCameras(activeScene);
            DisableInactiveSceneUI(activeScene);
            DisableInactiveSceneLights(activeScene);
            DisableInactiveSceneObjects(activeScene);
            ManageDontDestroyOnLoadObjects(sceneName);
        }

        public async Task ActivateScene(string sceneName, bool loadIfMissing = true)
        {
            // Detect if target scene belongs to another registered group; if so switch groups first
            SceneGroup targetGroup = null;
            if (_registeredGroups.Count > 0)
            {
                for (int i = 0; i < _registeredGroups.Count; i++)
                {
                    var g = _registeredGroups[i];
                    if (g?.Scenes != null && g.Scenes.Any(s => s.Name == sceneName))
                    {
                        targetGroup = g;
                        break;
                    }
                }
            }

            if (targetGroup != null && targetGroup != ActiveSceneGroup)
            {
                // Load the new group (unloads previous internally)
                await LoadScenes(targetGroup, null);
                // If the group's ActiveScene is the requested one, we're done.
                var groupActive = targetGroup.FindSceneNameByType(SceneType.ActiveScene);
                if (groupActive == sceneName)
                {
                    return; // Already active after group load
                }
                // Otherwise continue to finalize/activate the specific scene (likely preloaded now)
            }

            var target = SceneManager.GetSceneByName(sceneName);
            // If we have a deferred preload, finalize it first
            if (_deferredPreloads.TryGetValue(sceneName, out var deferredOp))
            {
                deferredOp.allowSceneActivation = true;
                while (!deferredOp.isDone)
                {
                    await Task.Delay(10);
                }
                _deferredPreloads.Remove(sceneName);
                target = SceneManager.GetSceneByName(sceneName);
                OnSceneLoaded.Invoke(sceneName);
            }

            if ((!target.IsValid() || !target.isLoaded) && loadIfMissing)
            {
                var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                operation.allowSceneActivation = false;
                while (operation.progress < 0.9f)
                {
                    await Task.Delay(50);
                }
                operation.allowSceneActivation = true;
                while (!operation.isDone)
                {
                    await Task.Delay(10);
                }
                target = SceneManager.GetSceneByName(sceneName);
                OnSceneLoaded.Invoke(sceneName);
            }

            if (!target.IsValid() || !target.isLoaded)
            {
                Debug.LogError($"[SceneGroupManager] ActivateScene failed. Scene not loaded: {sceneName}");
                return;
            }

            SceneManager.SetActiveScene(target);

            EnableActiveSceneObjects(target);
            EnableActiveSceneCameras(target);
            EnableActiveSceneUI(target);
            EnableActiveSceneLights(target);
            DisableInactiveSceneCameras(target);
            DisableInactiveSceneUI(target);
            DisableInactiveSceneLights(target);
            DisableInactiveSceneObjects(target);
            ManageDontDestroyOnLoadObjects(sceneName);
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}
