using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public Transform player;
    public float interactionRadius = 3f;

    private Interactible highighted;

    void Update()
    {
        var items = GameObject.FindObjectsOfType<Interactible>().Where(item =>
            (player.transform.position - item.transform.position).LessThan(interactionRadius));

        if (!items.Any())
        {
            SetHighlighted(null);
            return;
        }

        var closest = null as Interactible;
        var closestDistSq = float.PositiveInfinity;
        foreach (var item in items)
        {
            var distSq = (item.transform.position - player.transform.position).sqrMagnitude;

            if (distSq < closestDistSq)
            {
                closest = item;
                closestDistSq = distSq;
            }
        }

        SetHighlighted(closest);

        if (Input.GetKeyDown(KeyCode.Space) && highighted != null)
        {
            highighted.Interact(player.gameObject);
        }
    }

    void SetHighlighted(Interactible value)
    {
        if (value != highighted)
        {
            if (highighted != null)
            {
                highighted.IsHighlighted = false;
            }

            if (value != null)
            {
                value.IsHighlighted = true;
            }

            highighted = value;
        }
    }
}
