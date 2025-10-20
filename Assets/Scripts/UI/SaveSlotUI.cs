using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button deleteAllButton;
    
    [Header("Settings")]
    [SerializeField] private string slotName = "default";
    [SerializeField] private int maxDisplayedSaves = 10;
    
    private List<SaveSlotEntry> _slotEntries = new List<SaveSlotEntry>();
    
    private void Start()
    {
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshSaveList);
            
        if (deleteAllButton != null)
            deleteAllButton.onClick.AddListener(DeleteAllSaves);
            
        RefreshSaveList();
    }

    private void OnDestroy()
    {
        if (refreshButton != null)
            refreshButton.onClick.RemoveListener(RefreshSaveList);
            
        if (deleteAllButton != null)
            deleteAllButton.onClick.RemoveListener(DeleteAllSaves);
    }
    
    public void RefreshSaveList()
    {
        // Clear existing entries
        foreach (var entry in _slotEntries)
        {
            if (entry.gameObject != null)
                Destroy(entry.gameObject);
        }
        _slotEntries.Clear();
        
        // Get all saves
        var saves = JsonSaveManager.GetAllSaves(slotName);
        int displayCount = Mathf.Min(saves.Count, maxDisplayedSaves);
        
        for (int i = 0; i < displayCount; i++)
        {
            string savePath = saves[i];
            var info = JsonSaveManager.GetSaveInfo(savePath);
            if (info == null) continue;
            
            CreateSaveSlotEntry(info, i);
        }
        
        Debug.Log($"[SaveSlotUI] Displayed {_slotEntries.Count} save slots");
    }
    
    private void CreateSaveSlotEntry(SaveFileInfo info, int index)
    {
        if (saveSlotPrefab == null || saveSlotContainer == null) return;
        
        GameObject slotObj = Instantiate(saveSlotPrefab, saveSlotContainer);
        var entry = slotObj.GetComponent<SaveSlotEntry>();
        
        if (entry == null)
            entry = slotObj.AddComponent<SaveSlotEntry>();
            
        entry.Initialize(info, index);
        entry.OnLoadRequested += () => LoadSave(info.filePath);
        entry.OnDeleteRequested += () => DeleteSave(info.filePath);
        entry.OnRestoreRequested += () => RestoreBackup(info.filePath);
        
        _slotEntries.Add(entry);
    }
    
    private void LoadSave(string path)
    {
        Debug.Log($"[SaveSlotUI] Loading save: {path}");
        
        var data = JsonSaveManager.Load(path);
        if (data == null)
        {
            Debug.LogError($"[SaveSlotUI] Failed to load save from: {path}");
            return;
        }
        
        if (MapGenerator.Instance != null)
        {
            MapGenerator.Instance.ApplyJsonSave(data);
        }
        else
        {
            Debug.LogWarning("[SaveSlotUI] MapGenerator not found, loading map scene");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapGeneration");
        }
    }
    
    private void DeleteSave(string path)
    {
        if (JsonSaveManager.DeleteSave(path))
        {
            RefreshSaveList();
        }
    }
    
    private void RestoreBackup(string path)
    {
        if (JsonSaveManager.RestoreFromBackup(path))
        {
            RefreshSaveList();
        }
    }
    
    private void DeleteAllSaves()
    {
        int deleted = JsonSaveManager.DeleteAllInSlot(slotName);
        Debug.Log($"[SaveSlotUI] Deleted {deleted} saves");
        RefreshSaveList();
    }
}

public class SaveSlotEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button restoreButton;
    
    public event Action OnLoadRequested;
    public event Action OnDeleteRequested;
    public event Action OnRestoreRequested;
    
    private SaveFileInfo _info;
    
    public void Initialize(SaveFileInfo info, int index)
    {
        _info = info;
        
        if (nameText != null)
            nameText.text = $"Save #{index + 1}";
            
        if (infoText != null)
            infoText.text = $"Seed: {info.seed} | Nodes: {info.nodesCount} | Size: {FormatFileSize(info.fileSize)}";
            
        if (dateText != null)
        {
            try
            {
                if (!string.IsNullOrEmpty(info.savedAt))
                {
                    var savedTime = DateTime.Parse(info.savedAt);
                    dateText.text = savedTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    dateText.text = info.lastModified.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                }
            }
            catch
            {
                dateText.text = info.lastModified.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            }
        }
        
        if (loadButton != null)
            loadButton.onClick.AddListener(() => OnLoadRequested?.Invoke());
            
        if (deleteButton != null)
            deleteButton.onClick.AddListener(() => OnDeleteRequested?.Invoke());
            
        if (restoreButton != null)
            restoreButton.onClick.AddListener(() => OnRestoreRequested?.Invoke());
    }
    
    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
        return $"{bytes / (1024 * 1024)} MB";
    }
}
