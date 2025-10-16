using UnityEngine;

public static class AtlasHelper
{
    public static Sprite Cutoff(Texture2D atlas, int rows, int cols, int row, int col, bool rowZeroTop = true, float pixelsPerUnit = 100f)
    {
        if (atlas == null) return null;
        if (rowZeroTop) row = (rows - 1) - row;

        int tileWidth = atlas.width / cols;
        int tileHeight = atlas.height / rows;

        Rect rect = new Rect(col*tileWidth, row*tileHeight, tileWidth, tileHeight);

        return Sprite.Create(atlas, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit, 0, SpriteMeshType.FullRect);
    }
}
