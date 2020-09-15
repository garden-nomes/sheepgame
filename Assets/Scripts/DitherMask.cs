using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class DitherMask : MonoBehaviour
{
    public DitherPattern pattern;
    public float radius = 20;
    [Range(0f, 1f)]
    public float ditherWidth = 0.5f;
    public int pixelsPerUnit = 8;

    void Reset()
    {
        UpdateMask();
    }

    [ContextMenu("Update Mask")]
    void UpdateMask()
    {
        var texture = GenerateTexture();
        var rect = new Rect(0, 0, texture.width, texture.height);
        var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
        GetComponent<SpriteMask>().sprite = sprite;
    }

    Texture2D GenerateTexture()
    {
        int sideLength = (int)(radius * 2 * pixelsPerUnit);

        var texture = new Texture2D(sideLength, sideLength, TextureFormat.Alpha8, false);
        texture.filterMode = FilterMode.Point;

        var center = new Vector2(sideLength, sideLength) / 2f;
        var pixelRadius = radius * pixelsPerUnit;

        for (int x = 0; x < sideLength; x++)
        {
            for (int y = 0; y < sideLength; y++)
            {
                float dist = (new Vector2(x, y) - center).magnitude;

                if (dist < pixelRadius)
                {
                    float opacity = dist.Map(pixelRadius * ditherWidth, pixelRadius, 1f, 0f);
                    texture.SetPixel(x, y, pattern.GetPixel(x, y, opacity));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return texture;
    }
}
