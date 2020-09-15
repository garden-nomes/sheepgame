using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Waterline : MonoBehaviour
{
    public Color waterColor = Color.blue;
    public Color foamColor = Color.white;
    public float foamHeight = 1f;
    public int width = 10;
    public int height = 30;
    [Range(0f, 1f)]
    public float rise = .5f;
    public float xFrequency = .2f;
    public float timeFrequency = 1f;
    public float fps = 12;

    private Texture2D texture;
    private int pixelWidth;
    private int pixelHeight;
    public float pixelsPerUnit = 8f;

    void Start()
    {
        pixelWidth = (int)(width * pixelsPerUnit);
        pixelHeight = (int)(height * pixelsPerUnit);

        texture = new Texture2D(pixelWidth, pixelHeight);
        texture.filterMode = FilterMode.Point;

        var pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        var spriteRect = new Rect(0, 0, texture.width, texture.height);
        var sprite = Sprite.Create(texture, spriteRect, new Vector2(0f, 0f), pixelsPerUnit);
        GetComponent<SpriteRenderer>().sprite = sprite;

        StartCoroutine(AnimationCoroutine());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var center = transform.position + Vector3.right * width / 2f - Vector3.down * height / 2f;
        var size = new Vector3(width, height, 1f);
        Gizmos.DrawWireCube(center, size);
    }

    IEnumerator AnimationCoroutine()
    {
        while (true)
        {
            if (IsOnscreen())
            {
                RenderTexture();
            }

            yield return new WaitForSeconds(1f / fps);
        }
    }

    bool IsOnscreen()
    {
        var rect = new Rect(transform.position.x, transform.position.y, width, height);

        var cameraSize = new Vector2(
            Camera.main.orthographicSize * Screen.width / Screen.height * 2f,
            Camera.main.orthographicSize * 2f);

        var cameraRect = new Rect(
            (Vector2)Camera.main.transform.position - cameraSize / 2f,
            cameraSize);

        return cameraRect.Overlaps(rect);
    }

    void RenderTexture()
    {
        var pixels = new Color[texture.width * texture.height];

        for (int x = 0; x < texture.width; x++)
        {
            var noiseX = transform.position.x * pixelsPerUnit + x + 1000f;
            var foamNoiseX = noiseX + 1000f;

            var oceanHeight =
                Mathf.PerlinNoise(noiseX * xFrequency, Time.time * 0.1f * timeFrequency) * height;

            var foamNoiseHeight =
                Mathf.PerlinNoise(foamNoiseX * xFrequency, Time.time * 0.1f * timeFrequency) * foamHeight;

            var oceanY = oceanHeight * pixelsPerUnit;

            for (int y = 0; y < pixelHeight; y++)
            {
                var color = y < oceanY
                    ? (y > oceanY - (foamNoiseHeight * pixelsPerUnit)
                        ? foamColor
                        : waterColor)
                    : Color.clear;

                pixels[y * texture.width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
}
