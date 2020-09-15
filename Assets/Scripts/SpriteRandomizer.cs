using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRandomizer : MonoBehaviour
{
    public Sprite[] sprites;

    public void Randomize()
    {
        if (sprites.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, sprites.Length);
        GetComponent<SpriteRenderer>().sprite = sprites[index];
    }
}
