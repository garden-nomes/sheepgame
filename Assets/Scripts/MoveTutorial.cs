using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MoveTutorial : MonoBehaviour
{
    public float timeBeforeAppear = 3f;
    public float timeBeforeHide = 1f;
    public float fadeDuration = 1f;

    bool hasMoved = false;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlpha(0f);
        StartCoroutine(ShowHide());
    }

    void Update()
    {
        if (
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow))
        {
            hasMoved = true;
        }
    }

    IEnumerator ShowHide()
    {
        yield return new WaitForSeconds(timeBeforeAppear);

        if (!hasMoved)
        {
            yield return FadeIn(fadeDuration);

            while (!hasMoved)
            {
                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(timeBeforeHide);
            yield return FadeOut(fadeDuration);
        }

        gameObject.SetActive(false);
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
}
