using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }
    
    [Header("Current Active Save")]
    [SerializeField] private MapSaveDataSO activeSave;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetActiveSave(MapSaveDataSO saveData)
    {
        activeSave = saveData;
        Debug.Log($"[GameSaveManager] Active save set: {(saveData != null ? saveData.name : "null")}");
    }
    
    public MapSaveDataSO GetActiveSave()
    {
        return activeSave;
    }
    
    public bool HasActiveSave()
    {
        return activeSave != null;
    }
    
    public void ClearActiveSave()
    {
        activeSave = null;
        Debug.Log("[GameSaveManager] Active save cleared");
    }
    
    public void CreateNewGameSave(MapSaveDataSO template = null)
    {
        MapSaveDataSO newSave = ScriptableObject.CreateInstance<MapSaveDataSO>();
        
        if (template != null)
        {
            newSave.numberOfLevels = template.numberOfLevels;
            newSave.minNodesPerLevel = template.minNodesPerLevel;
            newSave.maxNodesPerLevel = template.maxNodesPerLevel;
            newSave.horizontalSpacing = template.horizontalSpacing;
            newSave.verticalSpacing = template.verticalSpacing;
            newSave.horizontalRandomRange = template.horizontalRandomRange;
            newSave.verticalRandomRange = template.verticalRandomRange;
        }
        else
        {
            newSave.numberOfLevels = 8;
            newSave.minNodesPerLevel = 2;
            newSave.maxNodesPerLevel = 4;
            newSave.horizontalSpacing = 5f;
            newSave.verticalSpacing = 6f;
            newSave.horizontalRandomRange = 2f;
            newSave.verticalRandomRange = 1f;
        }
        
        newSave.mapSeed = 0;
        newSave.hasPlayerPosition = false;
        newSave.maxMapTime = 0f;
        newSave.currentMapTime = 0f;
        newSave.saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        
        SetActiveSave(newSave);
        
        Debug.Log("[GameSaveManager] New game save created");
    }
    
    public bool SaveToFile(string fileName = "CurrentSave")
    {
        if (activeSave == null)
        {
            Debug.LogWarning("[GameSaveManager] No active save to write to file");
            return false;
        }
        
        return SaveSystemManager.SaveToFile(activeSave, fileName);
    }
    
    public bool LoadFromFile(string fileName = "CurrentSave")
    {
        MapSaveDataSO loadedSave = SaveSystemManager.LoadFromFile(fileName);
        
        if (loadedSave != null)
        {
            SetActiveSave(loadedSave);
            return true;
        }
        
        return false;
    }
    
    public bool SaveFileExists(string fileName = "CurrentSave")
    {
        return SaveSystemManager.SaveFileExists(fileName);
    }
}
