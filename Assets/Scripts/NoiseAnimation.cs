using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseAnimation : MonoBehaviour
{
    public float minRadius = 1f;
    public float maxRadius = 5f;
    public Color startColor = new Color(1f, 1f, 1f, 0.25f);
    public Color endColor = new Color(1f, 1f, 1f, 0f);

    public float loopTime = 2f;

    private float t = 0;


    void Update()
    {
        t = (t + Time.deltaTime / loopTime) % 1f;

        var radius = Mathf.Lerp(minRadius, maxRadius, t);
        var color = Color.Lerp(startColor, endColor, t);

        transform.localScale = new Vector3(radius, radius, 1f);
        GetComponent<SpriteRenderer>().color = color;
    }
}
