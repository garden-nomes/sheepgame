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
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimator : MonoBehaviour
{
    public LeftRightAnimation idle;
    public LeftRightAnimation walk;
    public LeftRightAnimation pet;
    public bool IsFacingRight => isFacingRight;

    private new Rigidbody2D rigidbody;
    private SpriteAnimator animator;
    private PlayerMovement movement;
    private bool isFacingRight = true;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
        movement = GetComponent<PlayerMovement>();

        InteractionController.instance.OnInteract += OnInteract;
    }

    void Update()
    {
        var dx = rigidbody.velocity.x;
        if (dx > 0) isFacingRight = true;
        if (dx < 0) isFacingRight = false;

        if (movement.IsPetting)
        {
            Set(pet);
        }
        else if (rigidbody.velocity.sqrMagnitude == 0)
        {
            Set(idle);
        }
        else
        {
            Set(walk);
        }
    }

    void Set(LeftRightAnimation animation)
    {
        animator.SetAnimation(animation.Get(isFacingRight));
    }

    void OnInteract(GameObject target)
    {
        isFacingRight = target.transform.position.x > transform.position.x;
    }
}
