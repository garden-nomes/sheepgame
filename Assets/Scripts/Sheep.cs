using System.Collections.Generic;
using System.Linq;
using SheepAI;
using SheepAI.States;
using UnityEngine;

public class SheepAwareness
{
    public PlayerMovement player;
    public IEnumerable<Apple> apples;
    public IEnumerable<SteeredObject> neighbors;
}

[RequireComponent(typeof(SteeredObject))]
[RequireComponent(typeof(Bleater))]
[RequireComponent(typeof(Interactible))]
[RequireComponent(typeof(SheepAnimator))]
public class Sheep : MonoBehaviour
{
    // individual traits
    public float attitude = 0f;

    // settings
    public float pickupRadius = 1f;
    public float neighborRadius = 6f;
    public float wanderChangeTarget = 1f; // wander
    public float wanderRadius = 5f;
    public float wanderAlertRadius = 5f;
    public float wanderFleeRadius = 2f;
    public float grazeChangeTarget = .2f; // graze
    public float grazeRadius = 2f;
    public float greetResetTime = 10f; // greet
    public float greetResetRadius = 10f;
    public float greetRadius = 5f;
    public float fleeRadius = 5f; // flee
    public float fleeCohesionWeight = 1f;
    public float fleeSeparationWeight = 1f;
    public float fleeSeparationRadius = 1f;
    public float petSpawnHeartRate = 1f;

    // status
    public bool isHeadDown = false;

    // references
    public LayerMask blockingLayer;
    public PlayerMovement player;
    public HeartSpawner heartSpawner;

    // helpers
    public Vector2 Position => (Vector2) transform.position;

    // private status
    private SheepStateMachine stateMachine;
    public Pasture pasture;
    public bool IsOnPasture => pasture != null && !pasture.isExhausted;

    private Interactible interactible;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponent<PlayerMovement>();

            if (player == null)
            {
                throw new System.Exception("Couldn't find player object in scene");
            }
        }

        interactible = GetComponent<Interactible>();
        GetComponent<Interactible>().OnInteract += OnInteract;

        stateMachine = new SheepStateMachine(this);
        stateMachine.Push<WanderState>();
    }

    void Update()
    {
        var toPlayer = transform.position - player.transform.position;

        // universal state transitions
        if (!(stateMachine.CurrentState is FleePlayerState) && player.IsNoisy && toPlayer.LessThan(fleeRadius))
        {
            stateMachine.Push<FleePlayerState>();
        }

        stateMachine.Update();
    }

    void OnInteract(GameObject player)
    {
        if (attitude >= 1f)
        {
            player.GetComponent<PlayerMovement>().Pet(this);
            stateMachine.Push<PlayerPetState>();
        }
        else
        {
            stateMachine.Push<FleePlayerState>();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var pasture = collider.GetComponent<Pasture>();

        if (pasture != null)
        {
            this.pasture = pasture;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {

        if (pasture != null && collider.gameObject == pasture.gameObject)
        {
            pasture = null;
        }
    }

    public void ReceiveApple()
    {
        heartSpawner.ShowHearts(5);
        attitude += 1f;
    }

    public SheepAwareness Awareness => new SheepAwareness()
    {
        neighbors = FindNeighbors(),
        apples = FindApples(),
        player = player,
    };

    IEnumerable<SteeredObject> FindNeighbors()
    {
        return GameObject.FindObjectsOfType<Sheep>()
            .Where(other =>
                (other.Position - Position).sqrMagnitude < neighborRadius * neighborRadius &&
                other != this)
            .Select(other => other.GetComponent<SteeredObject>());
    }

    IEnumerable<Apple> FindApples()
    {
        var apples = GameObject.FindObjectsOfType<Apple>();

        return apples.Where(apple =>
            (apple.transform.position - transform.position).LessThan(wanderAlertRadius));
    }
}
