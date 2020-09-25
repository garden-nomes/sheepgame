using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeedFast = 10f;
    public float moveSpeedSlow = 5f;
    public float minPetTime = 1f;
    public GameObject noiseIndicator;
    public Vector2 petOffset = new Vector2(1f, .1f);

    private bool isNoisy = false;
    public bool IsNoisy => isNoisy;

    private GameObject held;
    public bool IsHolding => held != null;

    private Sheep petting = null;
    public bool IsPetting => petting != null;
    private float petTime = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsPetting)
        {
            petTime += Time.deltaTime;

            var radius = InteractionController.instance.interactionRadius;

            if (!(transform.position - petting.transform.position).LessThan(radius) ||
                (petTime > minPetTime && !Input.GetKey(KeyCode.Space)))
            {
                petting = null;
            }
        }
        else
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.velocity = input * (isNoisy || IsPetting ? moveSpeedSlow : moveSpeedFast);
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

    public void Pet(Sheep sheep)
    {
        petting = sheep;
        petTime = 0f;
    }
}
