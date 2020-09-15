using UnityEngine;

[CreateAssetMenu(menuName = "Dither Pattern")]
public class DitherPattern : ScriptableObject
{
    public Texture2D pattern;
    public int stepHeight = 2;

    public Color GetPixel(int x, int y, float opacity)
    {
        var stepCount = Mathf.FloorToInt(pattern.height / stepHeight);
        var step = Mathf.FloorToInt(opacity * stepCount);

        var px = x % pattern.width;
        var py = step * stepHeight + y % stepHeight;

        return pattern.GetPixel(px, py);
    }
}
