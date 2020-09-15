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
    private GameObject apple;
    public bool HasApple => apple != null;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (apple != null)
            {
                var sheep = GameObject
                    .FindObjectsOfType<Sheep>()
                    .FirstOrDefault(s =>
                        (s.transform.position - transform.position).sqrMagnitude < pickupRadiusSq);

                if (sheep != null)
                {
                    GameObject.Destroy(apple);
                    apple = null;
                    sheep.ReceiveApple();
                }
                else
                {
                    apple.transform.position = transform.position;
                    apple.SetActive(true);
                    apple = null;
                }
            }
            else
            {
                var apples = GameObject.FindGameObjectsWithTag(Tags.TREAT);
                var pickup = apples.FirstOrDefault(apple =>
                    (apple.transform.position - transform.position).sqrMagnitude < pickupRadiusSq);

                if (pickup != null)
                {
                    apple = pickup;
                    apple.SetActive(false);
                }
                else
                {
                    isNoisy = true;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isNoisy = false;
        }

        if (noiseIndicator != null) noiseIndicator.SetActive(isNoisy);
    }
}
