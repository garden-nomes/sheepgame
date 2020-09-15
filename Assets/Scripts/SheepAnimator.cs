using UnityEngine;

[RequireComponent(typeof(SteeredObject))]
[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(Sheep))]
public class SheepAnimator : MonoBehaviour
{
    public SpriteAnimation idleRight;
    public SpriteAnimation walkRight;
    public SpriteAnimation grazeRight;

    private new Rigidbody2D rigidbody;
    private SpriteAnimator animator;
    private Sheep sheep;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
        sheep = GetComponent<Sheep>();
    }

    void Update()
    {
        var dx = rigidbody.velocity.x;

        if (dx < 0) transform.localScale = new Vector3(1f, 1f, 1f);
        if (dx > 0) transform.localScale = new Vector3(-1f, 1f, 1f);

        if (rigidbody.velocity.sqrMagnitude == 0)
        {
            animator.SetAnimation(sheep.isHeadDown ? grazeRight : idleRight);
        }
        else
        {
            animator.SetAnimation(walkRight);
        }
    }
}
