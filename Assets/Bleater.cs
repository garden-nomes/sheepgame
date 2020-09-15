using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleater : MonoBehaviour
{
    public AudioSource output;
    public AudioClip[] bleats;
    public float minWait = 1f;
    public float maxWait = 5f;

    void Start()
    {
        StartCoroutine(BleatCoroutine());
    }

    IEnumerator BleatCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            PlayBleat();
        }
    }

    public void Bleat()
    {
        PlayBleat();

        // restart timer for next bleat
        StopAllCoroutines();
        StartCoroutine(BleatCoroutine());
    }

    private void PlayBleat()
    {
        var clip = bleats[Mathf.FloorToInt(Random.value * bleats.Length)];
        output.PlayOneShot(clip, 1f);
    }
}
