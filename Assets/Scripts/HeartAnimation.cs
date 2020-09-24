using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartAnimation : MonoBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = new Color(1f, 1f, 1f, 0f);
    public float lift = .5f;
    public float lifeSpan = .5f;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Vector3 startPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        var t = timer / lifeSpan;

        spriteRenderer.color = Color.Lerp(startColor, endColor, t);
        transform.position = startPos + Vector3.up * lift * (t * t * t);

        if (t > 1f)
        {
            Destroy(gameObject);
        }
    }
}
