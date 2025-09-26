using UnityEngine;

public class MaterialSetupManager : PersistenSingleton<MaterialSetupManager>
{
    private MaterialPropertyBlock _block;

    void Start()
    {
        _block = new();
    }

    public void SetupVisual(MeshRenderer obj, int col, int row)
    {
        _block.SetFloat("_ColIndex", col);
        _block.SetFloat("_RowIndex", row);

        obj.SetPropertyBlock(_block);

        obj.SetPropertyBlock(_block, 0);
    }
}
