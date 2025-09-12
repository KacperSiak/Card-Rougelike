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
