using UnityEngine;

[RequireComponent(typeof(Interactible))]
public class Apple : MonoBehaviour
{
    void Start()
    {
        GetComponent<Interactible>().OnInteract += OnInteract;
    }

    void OnInteract(GameObject player)
    {
        var moveScript = player.GetComponent<PlayerMovement>();

        if (moveScript == null)
        {
            Debug.LogError("Player script not found");
        }

        moveScript.Pickup(gameObject);
    }
}
