using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public new SpriteAnimation animation;

    private new SpriteRenderer renderer;
    private float time;
    private int frame;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        time = 0f;
        frame = 0;
    }

    void Update()
    {
        if (animation == null)
        {
            return;
        }

        time += Time.deltaTime;

        if (time > 1f / animation.fps)
        {
            frame = (frame + 1) % animation.sprites.Count;
            renderer.sprite = animation.sprites[frame];
            time = 0;
        }
    }

    public void SetAnimation(SpriteAnimation animation)
    {
        if (animation != this.animation)
        {
            this.animation = animation;
            renderer.sprite = animation.sprites[0];
            frame = 0;
            time = 0f;
        }
    }
}
