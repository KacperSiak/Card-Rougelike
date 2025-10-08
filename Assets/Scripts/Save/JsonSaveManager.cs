using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class JsonSaveManager
{
    static string SavesRoot => Path.Combine(Application.persistentDataPath, "Saves");

    public static string GetLatestSavePath()
    {
        if (!Directory.Exists(SavesRoot)) return null;
        var files = Directory.GetFiles(SavesRoot, "*.json", SearchOption.TopDirectoryOnly);
        if (files == null || files.Length == 0) return null;
        return files.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).FirstOrDefault();
    }

    public static string CreateNewSavePath()
    {
        Directory.CreateDirectory(SavesRoot);
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(SavesRoot, $"map_{timestamp}.json");
    }

    public static void Save(JsonMapSave data, string path = null)
    {
        if (data == null) { Debug.LogWarning("[JsonSaveManager] No data to save"); return; }
        try
        {
            if (string.IsNullOrEmpty(path)) path = CreateNewSavePath();
            data.savedAt = DateTime.UtcNow.ToString("o");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"[JsonSaveManager] Saved to: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Save failed: {ex}");
        }
    }

    public static JsonMapSave Load(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<JsonMapSave>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Load failed: {ex}");
            return null;
        }
    }
}