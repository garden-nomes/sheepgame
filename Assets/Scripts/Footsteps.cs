using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footsteps;
    public AudioSource output;
    public float speed = 2f;

    private bool isPlaying = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var isPlayerMoving = rb.velocity.sqrMagnitude > 0f;

        if (isPlayerMoving && !isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(PlayCoroutine());
        }

        isPlaying = isPlayerMoving;
    }

    IEnumerator PlayCoroutine()
    {
        isPlaying = true;

        while (isPlaying)
        {
            var clip = footsteps[Mathf.FloorToInt(Random.value * footsteps.Length)];
            output.PlayOneShot(clip, 1f);
            yield return new WaitForSeconds(1f / speed);
        }
    }
}
