using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pasture : MonoBehaviour
{
    public float density = .5f;
    public float grassDensity = .1f;

    [ContextMenuItem("Calculate", "ResetCapacity")]
    public float capacity;
    public float exhaustion;

    public bool isExhausted = false;

    public Grass grassPrefab;

    private List<Grass> grasses;
    private new Collider2D collider;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        ResetCapacity();
        PlaceGrasses();
    }

    void Update()
    {
        if (exhaustion <= 0)
        {
            isExhausted = true;
            exhaustion = 0;
        }

        foreach (var grass in grasses)
        {
            grass.exhaustion = 1 - (exhaustion / capacity);
        }
    }

    void PlaceGrasses()
    {
        grasses = new List<Grass>();

        var rect = new Rect(collider.bounds.min, collider.bounds.size);
        var sampler = new PoissonSampler(rect, 1f / grassDensity);

        foreach (var sample in sampler.Samples())
        {
            if (collider.OverlapPoint(sample))
            {
                var grass = Instantiate(grassPrefab, sample, Quaternion.identity);
                grass.transform.SetParent(transform);
                grasses.Add(grass);
            }
        }
    }

    void ResetCapacity()
    {
        capacity = GetArea() * density;
        exhaustion = capacity;
    }

    float GetArea()
    {
        if (collider is CircleCollider2D circle)
        {
            return Mathf.PI * circle.radius * circle.radius *
                transform.localScale.x * transform.localScale.y;
        }
        else if (collider is BoxCollider2D box)
        {
            var size = box.size * transform.localScale;
            return size.x * size.y;
        }
        else
        {
            var bounds = collider.bounds;
            var size = (Vector2)bounds.size * transform.localScale;
            return size.x * size.y;
        }
    }
}
