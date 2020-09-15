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

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void AddForce(Vector2 force)
    {
        this.force += force;
    }

    void LateUpdate()
    {
        var thresholdSq = threshold * threshold;

        rigidbody.velocity = force.sqrMagnitude >= thresholdSq
            ? force.normalized * speed
            : Vector2.zero;

        force = Vector2.zero;
    }
}
