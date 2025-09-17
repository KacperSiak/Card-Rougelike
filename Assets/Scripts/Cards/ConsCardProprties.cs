using UnityEngine;

[System.Serializable]   
public struct MaterialsByType
{
    public ConsCardType type;
    public Material material;
}
public class ConsCardProprties : MonoBehaviour
{
    [SerializeField] private MeshRenderer _front;
    [SerializeField] private MeshRenderer _back;
    [SerializeField] private MaterialsByType[] _materials;

    [SerializeField] private Texture2D _moiraeBack;
    [SerializeField] private Texture2D _oneUseBack;

    public void Setup(ConsCardType type, int row, int col)
    {
        var mat = FindMaterial(type);
        if (mat != null) _front.sharedMaterial = mat;

        MaterialSetupManager.Instance.SetupVisual(_front, col, row);

        if (type == ConsCardType.Moirae) _back.material.mainTexture = _moiraeBack;
        else _back.material.mainTexture = _oneUseBack;

    }

    private Material FindMaterial(ConsCardType type)
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].type == type) 
                return _materials[i].material;
        }
        return null;
    }
}
