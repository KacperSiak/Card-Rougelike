using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class JsonSaveManager
{
    static string SavesRoot => Path.Combine(Application.persistentDataPath, "Saves");

    static string _currentSlot = "default";
    static int _maxSavesPerSlot = 10;

    public static void SetCurrentSlot(string slotName)
    {
        if (string.IsNullOrWhiteSpace(slotName)) slotName = "default";
        _currentSlot = SanitizeSlotName(slotName);
    }

    public static string GetCurrentSlot() => _currentSlot;

    public static void SetMaxSavesPerSlot(int max)
    {
        _maxSavesPerSlot = Mathf.Max(1, max);
    }

    static string GetSlotRoot(string slot = null)
    {
        var s = string.IsNullOrWhiteSpace(slot) ? _currentSlot : SanitizeSlotName(slot);
        return Path.Combine(SavesRoot, s);
    }

    static string SanitizeSlotName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "default" : cleaned;
    }

    public static string GetLatestSavePath(string slot = null)
    {
        var root = GetSlotRoot(slot);
        if (!Directory.Exists(root)) return null;
        var files = Directory.GetFiles(root, "*.json", SearchOption.TopDirectoryOnly);
        if (files == null || files.Length == 0) return null;
        return files.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).FirstOrDefault();
    }

    public static IReadOnlyList<string> GetAllSaves(string slot = null)
    {
        var root = GetSlotRoot(slot);
        if (!Directory.Exists(root)) return Array.Empty<string>();
        return Directory.GetFiles(root, "*.json", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(f => File.GetLastWriteTimeUtc(f))
                        .ToArray();
    }

    public static string CreateNewSavePath(string slot = null)
    {
        var root = GetSlotRoot(slot);
        Directory.CreateDirectory(root);
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(root, $"map_{timestamp}.json");
    }

    public static void Save(JsonMapSave data, string path = null, string slot = null)
    {
        if (data == null) { Debug.LogWarning("[JsonSaveManager] No data to save"); return; }
        try
        {
            if (string.IsNullOrEmpty(path)) path = CreateNewSavePath(slot);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            data.savedAt = DateTime.UtcNow.ToString("o");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"[JsonSaveManager] Saved to: {path}");

            EnforceRollingLimit(slot);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Save failed: {ex}");
        }
    }

    static void EnforceRollingLimit(string slot = null)
    {
        try
        {
            var all = GetAllSaves(slot).ToList();
            if (all.Count <= _maxSavesPerSlot) return;
            var toDelete = all.Skip(_maxSavesPerSlot).ToList();
            foreach (var f in toDelete)
            {
                try { File.Delete(f); } catch (Exception e) { Debug.LogWarning($"[JsonSaveManager] Failed to delete old save '{f}': {e.Message}"); }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JsonSaveManager] Rolling cleanup failed: {ex.Message}");
        }
    }

    public static bool DeleteSave(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return false;
            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Delete failed: {ex}");
            return false;
        }
    }

    public static int DeleteAllInSlot(string slot = null)
    {
        int count = 0;
        var root = GetSlotRoot(slot);
        if (!Directory.Exists(root)) return 0;
        foreach (var f in Directory.GetFiles(root, "*.json", SearchOption.TopDirectoryOnly))
        {
            try { File.Delete(f); count++; } catch { }
        }
        return count;
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