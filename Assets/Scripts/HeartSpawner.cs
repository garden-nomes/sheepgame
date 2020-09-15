using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{
    public float radius = .25f;
    public float spawnTime = .5f;
    public GameObject heartPrefab;
    public GameObject exclamationPrefab;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void ShowHearts(int count)
    {
        StartCoroutine(ShowEmoteCoroutine(heartPrefab, count));
    }

    public void ShowExclamations(int count)
    {
        StartCoroutine(ShowEmoteCoroutine(exclamationPrefab, count));
    }

    private IEnumerator ShowEmoteCoroutine(GameObject prefab, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var spawnPos = transform.position + (Vector3)Random.insideUnitCircle * radius;
            Instantiate(prefab, spawnPos, Quaternion.identity, transform);

            yield return new WaitForSeconds(spawnTime / (float)count);
        }
    }
}
