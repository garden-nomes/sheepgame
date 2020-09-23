using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    public string Action;
    private bool isHighlighted;
    public Material highlightMaterial;

    public Action<GameObject> OnInteract;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;

    public bool IsHighlighted
    {
        get => isHighlighted;
        set => UpdateIsHighlighted(value);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
    }

    public void Interact(GameObject player)
    {
        if (OnInteract != null)
        {
            OnInteract.Invoke(player);
        }
    }

    void UpdateIsHighlighted(bool value)
    {
        if (spriteRenderer != null && value != isHighlighted)
        {
            if (value)
            {
                spriteRenderer.material = highlightMaterial;
            }
            else
            {
                spriteRenderer.material = originalMaterial;
            }

            isHighlighted = value;
        }
    }
}
