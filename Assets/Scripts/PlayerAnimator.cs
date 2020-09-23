using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    public SpriteAnimation idleRight;
    public SpriteAnimation walkRight;
    public SpriteAnimation idleWithAppleRight;
    public SpriteAnimation walkWithAppleRight;

    private new Rigidbody2D rigidbody;
    private SpriteAnimator animator;
    private new SpriteRenderer renderer;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var dx = rigidbody.velocity.x;
        var hasApple = GetComponent<PlayerMovement>().IsHolding;

        if (dx < 0) renderer.flipX = true;
        if (dx > 0) renderer.flipX = false;

        if (rigidbody.velocity.sqrMagnitude == 0)
        {
            animator.SetAnimation(hasApple ? idleWithAppleRight : idleRight);
        }
        else
        {
            animator.SetAnimation(hasApple ? walkWithAppleRight : walkRight);
        }
    }
}
