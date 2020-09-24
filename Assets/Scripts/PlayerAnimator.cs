using System;
using UnityEngine;

[Serializable]
public class LeftRightAnimation
{
    public SpriteAnimation left;
    public SpriteAnimation right;

    public SpriteAnimation Get(bool isFacingRight)
    {
        return isFacingRight ? right : left;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    public LeftRightAnimation idle;
    public LeftRightAnimation walk;

    private new Rigidbody2D rigidbody;
    private SpriteAnimator animator;
    private bool isFacingRight = true;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
    }

    void Update()
    {
        var dx = rigidbody.velocity.x;

        if (dx > 0) isFacingRight = true;
        if (dx < 0) isFacingRight = false;

        if (rigidbody.velocity.sqrMagnitude == 0)
        {
            animator.SetAnimation(idle.Get(isFacingRight));
        }
        else
        {
            animator.SetAnimation(walk.Get(isFacingRight));
        }
    }
}
