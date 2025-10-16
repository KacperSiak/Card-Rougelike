using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class JsonSaveManager
{
    const string DEFAULT_SLOT = "default";
    const int DEFAULT_MAX_SAVES = 10;
    const string SAVE_FILE_PATTERN = "*.json";
    const string TIMESTAMP_FORMAT = "yyyyMMdd_HHmmss";
    const string SAVE_FILE_PREFIX = "map_";
    const string BACKUP_SUFFIX = ".backup";
    
    static string SavesRoot => Path.Combine(Application.persistentDataPath, "Saves");

    static string _currentSlot = DEFAULT_SLOT;
    static int _maxSavesPerSlot = DEFAULT_MAX_SAVES;
    static string _lastSerializedJson;
    static JsonMapSave _lastSerializedData;

    public static void SetCurrentSlot(string slotName)
    {
        if (string.IsNullOrWhiteSpace(slotName)) slotName = DEFAULT_SLOT;
        _currentSlot = SanitizeSlotName(slotName);
    }

    public static string GetCurrentSlot() => _currentSlot;

    public static void SetMaxSavesPerSlot(int max)
    {
        _maxSavesPerSlot = Mathf.Max(1, max);
    }

    public static int GetMaxSavesPerSlot() => _maxSavesPerSlot;

    static string GetSlotRoot(string slot = null)
    {
        var s = string.IsNullOrWhiteSpace(slot) ? _currentSlot : SanitizeSlotName(slot);
        return Path.Combine(SavesRoot, s);
    }

    static string SanitizeSlotName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return DEFAULT_SLOT;
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Where(c => !invalid.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? DEFAULT_SLOT : cleaned;
    }

    public static string GetLatestSavePath(string slot = null)
    {
        var root = GetSlotRoot(slot);
        if (!Directory.Exists(root)) return null;
        
        try
        {
            var files = Directory.GetFiles(root, SAVE_FILE_PATTERN, SearchOption.TopDirectoryOnly);
            return files.Length == 0 ? null : files.OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JsonSaveManager] GetLatestSavePath failed: {ex.Message}");
            return null;
        }
    }

    public static IReadOnlyList<string> GetAllSaves(string slot = null)
    {
        var root = GetSlotRoot(slot);
        if (!Directory.Exists(root)) return Array.Empty<string>();
        
        try
        {
            return Directory.GetFiles(root, SAVE_FILE_PATTERN, SearchOption.TopDirectoryOnly)
                            .OrderByDescending(File.GetLastWriteTimeUtc)
                            .ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JsonSaveManager] GetAllSaves failed: {ex.Message}");
            return Array.Empty<string>();
        }
    }

    public static string CreateNewSavePath(string slot = null)
    {
        var root = GetSlotRoot(slot);
        Directory.CreateDirectory(root);
        string timestamp = DateTime.UtcNow.ToString(TIMESTAMP_FORMAT);
        return Path.Combine(root, $"{SAVE_FILE_PREFIX}{timestamp}.json");
    }

    public static void Save(JsonMapSave data, string path = null, string slot = null)
    {
        if (data == null)
        {
            Debug.LogWarning("[JsonSaveManager] Cannot save null data");
            return;
        }

        try
        {
            path ??= CreateNewSavePath(slot);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? SavesRoot);

            if (File.Exists(path))
            {
                string backupPath = path + BACKUP_SUFFIX;
                try
                {
                    File.Copy(path, backupPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[JsonSaveManager] Backup creation failed: {ex.Message}");
                }
            }
            
            data.savedAt = DateTime.UtcNow.ToString("o");
            
            string json;
            if (_lastSerializedData == data && !string.IsNullOrEmpty(_lastSerializedJson))
            {
                json = _lastSerializedJson;
            }
            else
            {
                json = JsonUtility.ToJson(data, prettyPrint: false);
                _lastSerializedData = data;
                _lastSerializedJson = json;
            }
            
            File.WriteAllText(path, json);
            Debug.Log($"[JsonSaveManager] Saved to: {path}");

            EnforceRollingLimit(slot);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Save failed: {ex}");
        }
    }

    public static async Task SaveAsync(JsonMapSave data, string path = null, string slot = null)
    {
        if (data == null)
        {
            Debug.LogWarning("[JsonSaveManager] Cannot save null data");
            return;
        }

        try
        {
            path ??= CreateNewSavePath(slot);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? SavesRoot);
            
            data.savedAt = DateTime.UtcNow.ToString("o");
            string json = JsonUtility.ToJson(data, prettyPrint: false);
            
            await File.WriteAllTextAsync(path, json);
            Debug.Log($"[JsonSaveManager] Async saved to: {path}");

            EnforceRollingLimit(slot);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Async save failed: {ex}");
        }
    }

    static void EnforceRollingLimit(string slot = null)
    {
        if (_maxSavesPerSlot <= 0) return;
        
        try
        {
            var all = GetAllSaves(slot);
            if (all.Count <= _maxSavesPerSlot) return;
            
            var toDelete = all.Skip(_maxSavesPerSlot);
            foreach (var file in toDelete)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[JsonSaveManager] Failed to delete old save '{Path.GetFileName(file)}': {e.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JsonSaveManager] Rolling cleanup failed: {ex.Message}");
        }
    }

    public static bool DeleteSave(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return false;
        
        try
        {
            File.Delete(path);
            Debug.Log($"[JsonSaveManager] Deleted save: {Path.GetFileName(path)}");
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
        var root = GetSlotRoot(slot);
        if (!Directory.Exists(root)) return 0;
        
        int count = 0;
        try
        {
            foreach (var file in Directory.GetFiles(root, SAVE_FILE_PATTERN, SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(file);
                    count++;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[JsonSaveManager] Failed to delete '{Path.GetFileName(file)}': {ex.Message}");
                }
            }
            Debug.Log($"[JsonSaveManager] Deleted {count} save(s) from slot '{slot ?? _currentSlot}'");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JsonSaveManager] DeleteAllInSlot failed: {ex.Message}");
        }
        return count;
    }

    public static JsonMapSave Load(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
        
        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return null;
            
            var data = JsonUtility.FromJson<JsonMapSave>(json);
            if (!ValidateSaveData(data))
            {
                Debug.LogWarning($"[JsonSaveManager] Invalid save data in '{Path.GetFileName(path)}'");
                return null;
            }
            
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Load failed: {ex}");
            return null;
        }
    }

    public static async Task<JsonMapSave> LoadAsync(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
        
        try
        {
            string json = await File.ReadAllTextAsync(path);
            if (string.IsNullOrWhiteSpace(json)) return null;
            
            var data = JsonUtility.FromJson<JsonMapSave>(json);
            if (!ValidateSaveData(data))
            {
                Debug.LogWarning($"[JsonSaveManager] Invalid save data in '{Path.GetFileName(path)}'");
                return null;
            }
            
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Async load failed: {ex}");
            return null;
        }
    }

    static bool ValidateSaveData(JsonMapSave data)
    {
        if (data == null) return false;
        if (data.nodes == null || data.edges == null) return false;
        if (string.IsNullOrEmpty(data.version)) return false;
        
        var nodeGuids = new HashSet<string>();
        foreach (var node in data.nodes)
        {
            if (string.IsNullOrEmpty(node.guid))
            {
                Debug.LogWarning("[JsonSaveManager] Found node with empty GUID");
                return false;
            }
            if (!nodeGuids.Add(node.guid))
            {
                Debug.LogWarning($"[JsonSaveManager] Duplicate node GUID: {node.guid}");
                return false;
            }
        }
        
        foreach (var edge in data.edges)
        {
            if (!nodeGuids.Contains(edge.fromGuid))
            {
                Debug.LogWarning($"[JsonSaveManager] Edge references non-existent node: {edge.fromGuid}");
                return false;
            }
            if (!nodeGuids.Contains(edge.toGuid))
            {
                Debug.LogWarning($"[JsonSaveManager] Edge references non-existent node: {edge.toGuid}");
                return false;
            }
        }
        
        if (!string.IsNullOrEmpty(data.currentPlayerNodeGuid))
        {
            if (!nodeGuids.Contains(data.currentPlayerNodeGuid))
            {
                Debug.LogWarning($"[JsonSaveManager] Player position references non-existent node: {data.currentPlayerNodeGuid}");
                data.currentPlayerNodeGuid = null;
            }
        }
        
        return true;
    }

    public static void ClearCache()
    {
        _lastSerializedData = null;
        _lastSerializedJson = null;
    }
    
    public static bool RestoreFromBackup(string savePath)
    {
        if (string.IsNullOrEmpty(savePath)) return false;
        
        string backupPath = savePath + BACKUP_SUFFIX;
        if (!File.Exists(backupPath))
        {
            Debug.LogWarning($"[JsonSaveManager] No backup found for: {Path.GetFileName(savePath)}");
            return false;
        }
        
        try
        {
            File.Copy(backupPath, savePath, overwrite: true);
            Debug.Log($"[JsonSaveManager] Restored from backup: {Path.GetFileName(savePath)}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Restore from backup failed: {ex}");
            return false;
        }
    }
    
    public static SaveFileInfo GetSaveInfo(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
        
        try
        {
            var fileInfo = new FileInfo(path);
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<JsonMapSave>(json);
            
            return new SaveFileInfo
            {
                filePath = path,
                fileName = Path.GetFileName(path),
                fileSize = fileInfo.Length,
                lastModified = fileInfo.LastWriteTimeUtc,
                seed = data?.seed ?? 0,
                version = data?.version ?? "unknown",
                nodesCount = data?.nodes?.Count ?? 0,
                savedAt = data?.savedAt
            };
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JsonSaveManager] GetSaveInfo failed for '{Path.GetFileName(path)}': {ex.Message}");
            return null;
        }
    }
    
    public static JsonMapSave TryRepairSave(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
        
        try
        {
            Debug.Log($"[JsonSaveManager] Attempting to repair save: {Path.GetFileName(path)}");
            
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<JsonMapSave>(json);
            
            if (data == null)
            {
                Debug.LogError("[JsonSaveManager] Could not deserialize data");
                return null;
            }
            
            if (data.nodes == null) data.nodes = new List<JsonNodeRecord>();
            if (data.edges == null) data.edges = new List<JsonEdgeRecord>();
            
            if (string.IsNullOrEmpty(data.version))
            {
                data.version = "1.0";
                Debug.Log("[JsonSaveManager] Fixed missing version");
            }
            
            int removedNodes = data.nodes.RemoveAll(n => string.IsNullOrEmpty(n.guid));
            if (removedNodes > 0)
                Debug.Log($"[JsonSaveManager] Removed {removedNodes} nodes with null GUIDs");
            
            var validGuids = new HashSet<string>(data.nodes.Select(n => n.guid));
            
            int removedEdges = data.edges.RemoveAll(e => 
                !validGuids.Contains(e.fromGuid) || !validGuids.Contains(e.toGuid));
            if (removedEdges > 0)
                Debug.Log($"[JsonSaveManager] Removed {removedEdges} invalid edges");
            
            if (!string.IsNullOrEmpty(data.currentPlayerNodeGuid) && !validGuids.Contains(data.currentPlayerNodeGuid))
            {
                Debug.Log("[JsonSaveManager] Cleared invalid player position");
                data.currentPlayerNodeGuid = null;
            }
            
            if (ValidateSaveData(data))
            {
                Debug.Log("[JsonSaveManager] Save successfully repaired");
                return data;
            }
            else
            {
                Debug.LogError("[JsonSaveManager] Repair validation failed");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonSaveManager] Repair failed: {ex}");
            return null;
        }
    }
}

[Serializable]
public class SaveFileInfo
{
    public string filePath;
    public string fileName;
    public long fileSize;
    public DateTime lastModified;
    public int seed;
    public string version;
    public int nodesCount;
    public string savedAt;
}