using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteFader))]
public class DistanceFader : MonoBehaviour
{
    public Transform target;

    public float near = 3f;
    public float far = 10f;
    public float maxFade = 0.8f;

    SpriteFader fader;

    void Start()
    {
        fader = GetComponent<SpriteFader>();
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        var dist = (transform.position - target.position).magnitude;
        fader.cutoff = Mathf.Clamp(maxFade - (dist - near) / (far - near) * maxFade, 0f, maxFade);
    }
}
