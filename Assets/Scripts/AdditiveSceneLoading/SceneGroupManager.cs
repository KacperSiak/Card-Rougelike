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
    readonly Dictionary<string, AsyncOperation> _deferredPreloads = new Dictionary<string, AsyncOperation>();
        readonly List<SceneGroup> _registeredGroups = new List<SceneGroup>();
        readonly HashSet<string> _externalPersistent = new HashSet<string>(StringComparer.Ordinal);

        public void RegisterExternalPersistent(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            _externalPersistent.Add(sceneName);
        }

        public void RegisterSceneGroups(IEnumerable<SceneGroup> groups)
        {
            _registeredGroups.Clear();
            if (groups == null) return;
            _registeredGroups.AddRange(groups);
        }

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            _deferredPreloads.Clear();
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

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

            foreach (var sceneData in ActiveSceneGroup.Scenes)
            {
                var name = sceneData.Name;
                if (name == activeSceneName) continue;
                if (loadedScenes.Contains(name)) continue;

                var preloadOp = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                if (preloadOp == null) continue;
                preloadOp.allowSceneActivation = false;
                _deferredPreloads[name] = preloadOp;
                while (preloadOp.progress < 0.9f)
                {
                    progress?.Report(preloadOp.progress);
                    await Task.Delay(50);
                }
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
                if (IsPersistent(sceneName)) continue;
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
            Camera[] allCameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.scene != activeScene)
                {
                    if (IsPersistent(cam.gameObject.scene.name)) continue;
                    cam.enabled = false;
                }
            }
        }

        void EnableActiveSceneCameras(Scene activeScene)
        {
            Camera[] allCameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
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
            Canvas[] allCanvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.gameObject.scene != activeScene && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    if (IsPersistent(canvas.gameObject.scene.name)) continue;
                    canvas.enabled = false;
                }
            }
        }

        void EnableActiveSceneUI(Scene activeScene)
        {
            Canvas[] allCanvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
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
            Light[] allLights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            
            foreach (Light light in allLights)
            {
                if (light.gameObject.scene != activeScene)
                {
                    if (IsPersistent(light.gameObject.scene.name)) continue;
                    light.enabled = false;
                }
            }
        }

        void EnableActiveSceneLights(Scene activeScene)
        {
            Light[] allLights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
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
                if (IsPersistent(scene.name))
                {
                    Debug.Log($"Skipping disable for persistent scene {scene.name}");
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
                    bool shouldBeActive = activeSceneName.Contains("Map");
                    Debug.Log($"Map object {obj.name}: setting to {shouldBeActive}");
                    obj.SetActive(shouldBeActive);
                }
                else if (obj.name.Contains("Menu"))
                {
                    bool shouldBeActive = activeSceneName.Contains("Menu");
                    Debug.Log($"Menu object {obj.name}: setting to {shouldBeActive}");
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
                await LoadScenes(targetGroup, null);
                var groupActive = targetGroup.FindSceneNameByType(SceneType.ActiveScene);
                if (groupActive == sceneName)
                {
            return;
                }
            }

            var target = SceneManager.GetSceneByName(sceneName);
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

        private bool IsPersistent(string sceneName)
        {
            if (_externalPersistent.Contains(sceneName)) return true;
            for (int g = 0; g < _registeredGroups.Count; g++)
            {
                var group = _registeredGroups[g];
                if (group?.Scenes == null) continue;
                for (int s = 0; s < group.Scenes.Count; s++)
                {
                    var data = group.Scenes[s];
                    if (data != null && data.Name == sceneName && data.Persistent)
                        return true;
                }
            }
            return false;
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
