using UnityEngine;

public class ShaderParameterReader : MonoBehaviour
{
    // Przypisz w Inspectorze swój materia³ korzystaj¹cy z tego shadera
    public Material targetMaterial;

    void Start()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("Nie przypisano materia³u!");
            return;
        }

        // Odczyt Vector2 z materia³u
        Vector2 frontOffset = targetMaterial.GetVector("_FrontOffset");
        Vector2 backOffset = targetMaterial.GetVector("_BackOffset");
        Vector2 tiling = targetMaterial.GetVector("_Tiling");

        Debug.Log($"FrontOffset: {frontOffset}");
        Debug.Log($"BackOffset: {backOffset}");
        Debug.Log($"Tiling: {tiling}");
    }

    // Przyk³adowo mo¿esz te¿ odczytaæ teksturê
    public Texture GetAtlasTexture()
    {
        if (targetMaterial != null)
            return targetMaterial.GetTexture("_AtlasTexture");
        else
            return null;
    }
}
