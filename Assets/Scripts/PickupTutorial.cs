using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickupTutorial : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float showIfPlayerWithinRadius = 5f;

    SpriteRenderer spriteRenderer;
    Transform player;
    bool isVisible = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag(Tags.PLAYER)?.transform;
        SetAlpha(0f);
        StartCoroutine(ShowHide());
    }

    IEnumerator ShowHide()
    {
        while (true)
        {
            if (!isVisible && IsInRange())
            {
                isVisible = true;
                yield return FadeIn(fadeDuration);
            }
            else if (isVisible && !IsInRange())
            {
                isVisible = false;
                yield return FadeOut(fadeDuration);
            }
            else
            {
                yield return new WaitForSeconds(.1f);
            }
        }
    }

    IEnumerator FadeIn(float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            SetAlpha(t / duration);
            yield return null;
        }
    }

    IEnumerator FadeOut(float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            SetAlpha(1f - (t / duration));
            yield return null;
        }
    }

    void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
    }

    bool IsInRange()
    {
        if (player == null)
        {
            return false;
        }

        return (player.position - transform.position).LessThan(showIfPlayerWithinRadius);
    }
}
