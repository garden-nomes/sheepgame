using UnityEngine;

[ExecuteInEditMode]
public class Grass : MonoBehaviour
{
    public Sprite[] sprites;
    [Range(0f, 1f)]
    public float exhaustion = 0f;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var spriteIndex = Mathf.Min(Mathf.FloorToInt(exhaustion * (float)(sprites.Length - 1)), sprites.Length - 1);
        spriteRenderer.sprite = sprites[spriteIndex];
    }
}
