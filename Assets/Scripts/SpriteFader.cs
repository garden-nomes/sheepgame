using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteFader : MonoBehaviour
{
    [Range(0f, 1f)]
    public float cutoff = 1f;
    public int gradientSize = 32;
    public DitherPattern ditherPattern;

    private Texture2D texture;
    private float previousCutoff;

    void Start()
    {
        previousCutoff = cutoff;
        UpdateTexture();
    }

    void Update()
    {
        if (cutoff != previousCutoff)
        {
            UpdateTexture();
            previousCutoff = cutoff;
        }
    }

    void Setup()
    {
        var sprite = GetComponent<SpriteRenderer>()?.sprite;

        if (sprite == null)
        {
            return;
        }

        var width = (int)sprite.rect.width;
        var height = (int)sprite.rect.height;

        texture = new Texture2D(width, height, TextureFormat.Alpha8, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }

        texture.Apply();

        var rect = new Rect(0, 0, texture.width, texture.height);
        var originalPivot = (sprite.pivot - sprite.rect.min) / sprite.rect.size;

        var maskSprite = Sprite.Create(texture, rect, originalPivot, sprite.pixelsPerUnit);
        maskSprite.name = "Fader Mask";

        var mask = GetComponent<SpriteMask>();

        if (mask == null)
        {
            mask = gameObject.AddComponent<SpriteMask>();
        }

        mask.sprite = maskSprite;
    }

    void UpdateTexture()
    {
        if (texture == null)
        {
            Setup();
        }

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                var opacity = Mathf.Clamp((y - texture.height * (1 - cutoff)) / gradientSize, 0f, 1f);
                texture.SetPixel(x, y, ditherPattern.GetPixel(x, y, opacity));
            }
        }

        texture.Apply();
    }
}
