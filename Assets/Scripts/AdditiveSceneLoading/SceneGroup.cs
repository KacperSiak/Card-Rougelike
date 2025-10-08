using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;

namespace Systems.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
        }
    }

    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public SceneType SceneType;
        [Tooltip("If true, this scene will never be unloaded automatically by SceneGroupManager")] public bool Persistent;
        [Tooltip("If true, when this scene is not active, all its root objects will be disabled (kept loaded but inactive). If false, they stay active unless UnloadWhenInactive is true.")]
        public bool DeactivateWhenInactive = true;
        [Tooltip("If true and not Persistent, when this scene becomes inactive it will be unloaded.")]
        public bool UnloadWhenInactive = false;
    }

    public enum SceneType
    {
        ActiveScene,
        MainMenu,
        UI,
        HUD,
        Map,
        Enemy,
        Shop,
        Treasure,
        Heal,
        Gambling,
        Challenge
    }
}
