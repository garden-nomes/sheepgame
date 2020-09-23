using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeedFast = 10f;
    public float moveSpeedSlow = 5f;
    public GameObject noiseIndicator;
    public float pickupRadius = 2f;

    private bool isNoisy = false;
    public bool IsNoisy => isNoisy;
    private GameObject held;
    public bool IsHolding => held != null;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var pickupRadiusSq = pickupRadius * pickupRadius;

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = input * (isNoisy ? moveSpeedSlow : moveSpeedFast);

        if (rb.velocity.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (rb.velocity.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !InteractionController.instance.HasTarget)
        {
            if (held != null) { DropItem(); }
            else { isNoisy = true; }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isNoisy = false;
        }

        if (noiseIndicator != null) noiseIndicator.SetActive(isNoisy);
    }

    public void Pickup(GameObject item)
    {
        held = item;
        held.SetActive(false);
    }

    public void DropItem()
    {
        held.transform.position = transform.position;
        held.SetActive(true);
        held = null;
    }
}
