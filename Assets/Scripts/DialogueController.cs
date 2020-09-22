using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogueController : MonoBehaviour
{
    public Dialogue[] dialogues;
    public Talkbox talkbox;

    void Awake()
    {
        EventBus.Instance.ListenAll(OnEvent);
    }

    void Start()
    {
        EventBus.Instance.Notify(Event.GAME_START, null);
    }

    void OnEvent(Event event_, GameObject target)
    {
        var matches = dialogues.Where(i => i.event_ == event_).ToList();

        if (!matches.Any())
        {
            return;
        }

        var dialogue = matches[Mathf.FloorToInt(Random.value * matches.Count)];
        talkbox.Say(dialogue, target);
    }
}
