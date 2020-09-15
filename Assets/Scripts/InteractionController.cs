using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractionController : MonoBehaviour
{
    public Interaction[] interactions;
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
        var matches = interactions.Where(i => i.event_ == event_).ToList();

        if (!matches.Any())
        {
            return;
        }

        var interaction = matches[Mathf.FloorToInt(Random.value * matches.Count)];
        talkbox.Say(interaction, target);
    }
}
