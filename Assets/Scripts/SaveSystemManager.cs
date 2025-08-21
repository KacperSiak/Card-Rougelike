using UnityEngine;
using System.IO;

public static class SaveSystemManager
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private static readonly string SAVE_EXTENSION = ".json";
    
    public static bool SaveToFile(MapSaveDataSO saveData, string fileName)
    {
        try
        {
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
            
            string json = JsonUtility.ToJson(saveData, true);
            string filePath = SAVE_FOLDER + fileName + SAVE_EXTENSION;
            
            File.WriteAllText(filePath, json);
            Debug.Log($"[SaveSystemManager] Save written to: {filePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystemManager] Failed to save to file: {e.Message}");
            return false;
        }
    }
    
    public static MapSaveDataSO LoadFromFile(string fileName)
    {
        try
        {
            string filePath = SAVE_FOLDER + fileName + SAVE_EXTENSION;
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[SaveSystemManager] Save file not found: {filePath}");
                return null;
            }
            
            string json = File.ReadAllText(filePath);
            MapSaveDataSO loadedSave = ScriptableObject.CreateInstance<MapSaveDataSO>();
            JsonUtility.FromJsonOverwrite(json, loadedSave);
            
            Debug.Log($"[SaveSystemManager] Save loaded from: {filePath}");
            return loadedSave;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystemManager] Failed to load from file: {e.Message}");
            return null;
        }
    }
    
    public static bool SaveFileExists(string fileName)
    {
        string filePath = SAVE_FOLDER + fileName + SAVE_EXTENSION;
        return File.Exists(filePath);
    }
    
    public static bool DeleteSaveFile(string fileName)
    {
        try
        {
            string filePath = SAVE_FOLDER + fileName + SAVE_EXTENSION;
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[SaveSystemManager] Save file deleted: {filePath}");
                return true;
            }
            else
            {
                Debug.LogWarning($"[SaveSystemManager] Save file not found for deletion: {filePath}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystemManager] Failed to delete save file: {e.Message}");
            return false;
        }
    }
    
    public static string[] GetAllSaveFiles()
    {
        try
        {
            if (!Directory.Exists(SAVE_FOLDER))
            {
                return new string[0];
            }
            
            string[] files = Directory.GetFiles(SAVE_FOLDER, "*" + SAVE_EXTENSION);
            
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            
            return files;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystemManager] Failed to get save files: {e.Message}");
            return new string[0];
        }
    }
}
