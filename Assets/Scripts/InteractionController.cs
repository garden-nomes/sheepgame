using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public static InteractionController instance;

    public Transform player;
    public float interactionRadius = 3f;
    public bool HasTarget => target != null;

    private Interactible target;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple instances of InteractionControllers found");
        }

        instance = this;
    }

    void Update()
    {
        var items = GameObject.FindObjectsOfType<Interactible>().Where(item =>
            item.isActiveAndEnabled &&
            (player.transform.position - item.transform.position).LessThan(interactionRadius));

        if (!items.Any())
        {
            SetTarget(null);
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

        SetTarget(closest);

        if (Input.GetKeyDown(KeyCode.Space) && target != null)
        {
            target.Interact(player.gameObject);
        }
    }

    void SetTarget(Interactible value)
    {
        if (value != target)
        {
            if (target != null)
            {
                target.IsHighlighted = false;
            }

            if (value != null)
            {
                value.IsHighlighted = true;
            }

            target = value;
        }
    }
}
