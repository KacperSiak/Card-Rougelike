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

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;

            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var SceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(SceneData.Name)) continue;

                var operation = SceneManager.LoadSceneAsync(SceneData.Reference.Path, LoadSceneMode.Additive);

                operationGroup.Operations.Add(operation);

                OnSceneLoaded.Invoke(SceneData.Name);
            }

            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100);
            }

            Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
                EnableActiveSceneObjects(activeScene);
                DisableInactiveSceneCameras(activeScene);
                DisableInactiveSceneUI(activeScene);
                DisableInactiveSceneLights(activeScene);
                DisableInactiveSceneObjects(activeScene);
            }

            OnSceneGroupLoaded.Invoke();

        }

        public async Task UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            for (var i = sceneCount - 1; i < 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;
                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
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

        void DisableInactiveSceneObjects(Scene activeScene)
        {
            // Get all root GameObjects from all scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                
                if (scene == activeScene || !scene.isLoaded) continue;
                
                GameObject[] rootObjects = scene.GetRootGameObjects();
                
                foreach (GameObject rootObj in rootObjects)
                {
                    // Keep essential objects like DontDestroyOnLoad objects
                    if (rootObj.name.Contains("DontDestroyOnLoad") || 
                        rootObj.name.Contains("Bootstrapper") ||
                        rootObj.name.Contains("SceneLoader") ||
                        rootObj.name.Contains("Manager"))
                    {
                        continue;
                    }
                    
                    // Disable the entire object hierarchy
                    rootObj.SetActive(false);
                }
            }
        }

        void EnableActiveSceneObjects(Scene activeScene)
        {
            GameObject[] rootObjects = activeScene.GetRootGameObjects();
            
            foreach (GameObject rootObj in rootObjects)
            {
                // Enable all root objects in active scene
                rootObj.SetActive(true);
            }
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
