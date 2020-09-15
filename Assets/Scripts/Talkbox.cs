using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Talkbox : MonoBehaviour
{
    public Text text;
    public float timeBetweenLines = 0.2f;

    Coroutine coroutine;

    public void Say(Interaction interaction, GameObject target)
    {
        if (coroutine != null)
        {
            if (interaction.interrupt)
            {
                StopCoroutine(coroutine);
            }
            else
            {
                return;
            }
        }

        coroutine = StartCoroutine(SayCoroutine(interaction, target));
    }

    private IEnumerator SayCoroutine(string message, float time)
    {
        text.text = message;
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        text.gameObject.SetActive(false);
    }

    private IEnumerator SayCoroutine(Interaction interaction, GameObject target)
    {
        foreach (var line in interaction.lines)
        {
            yield return SayCoroutine(line.Render(target), line.duration);
            yield return new WaitForSeconds(timeBetweenLines);
        }

        coroutine = null;
    }
}
