using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SteeredObject : MonoBehaviour
{
    public float speed;
    public float threshold;

    private Vector2 force = Vector2.zero;

    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity => velocity;

    private new Rigidbody2D rigidbody;
    private Vector2 Position => transform.position;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        var thresholdSq = threshold * threshold;

        rigidbody.velocity = force.sqrMagnitude >= thresholdSq ?
            force.normalized * speed :
            Vector2.zero;

        force = Vector2.zero;
    }

    public void AddForce(Vector2 force)
    {
        this.force += force;
    }

    public void MoveTowards(Vector2 target, float radius = 1f, float weight = 1f)
    {
        if (!(target - Position).LessThan(radius))
        {
            AddForce((target - Position).normalized * weight);
        }
    }

    public void FleeFrom(Vector2 target, float? radius = null, float weight = 1f)
    {
        if (radius == null || (Position - target).LessThan(radius.Value))
        {
            AddForce((Position - target).normalized * weight);
        }
    }

    public void Cohere(IEnumerable<SteeredObject> neighbors, float weight = 1f)
    {
        var list = neighbors.ToList();

        var center = list.Aggregate(
            Vector2.zero,
            (sum, neighbor) => sum + neighbor.Position) / list.Count;

        AddForce((center - Position).normalized * weight);
    }

    public void Seperate(IEnumerable<SteeredObject> neighbors, float radius = 1f, float weight = 1f)
    {
        var list = neighbors.ToList();

        foreach (var neighbor in list)
        {
            var multiplier = Mathf.Pow((neighbor.Position - Position).magnitude / radius, -2f);
            var force = (Position - neighbor.Position).normalized / list.Count * multiplier;
            AddForce(force * weight);
        }
    }
}
