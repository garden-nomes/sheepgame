using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SheepState
{
    Graze,
    GreetPlayer,
    FleePlayer,
    Alert
}

[RequireComponent(typeof(SteeredObject))]
[RequireComponent(typeof(Bleater))]
[RequireComponent(typeof(Interactible))]
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

    // status
    public bool isHeadDown = false;

    // references
    public LayerMask blockingLayer;
    public PlayerMovement player;
    public HeartSpawner heartSpawner;

    // helpers
    public Vector2 Position => (Vector2) transform.position;

    // private status
    private Vector2 wanderTarget;
    private Vector2 fleeTarget;
    private SteeredObject steering;
    private SheepState state;
    public SheepState State => state;
    private float greetPlayerResetTimer = 0f;
    private Pasture pasture;
    public bool IsOnPasture => pasture != null && !pasture.isExhausted;

    private Vector2 toPlayer;
    private bool isPlayerNoisy;
    private List<Sheep> neighbors;
    private Bleater bleater;

    #region Lifecycle methods

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

        bleater = GetComponent<Bleater>();
        steering = GetComponent<SteeredObject>();
        GetComponent<Interactible>().OnInteract += OnInteract;

        UpdateState(SheepState.Graze);
    }

    void Update()
    {
        // timers
        if (greetPlayerResetTimer > 0f) greetPlayerResetTimer -= Time.deltaTime;

        // update cached data
        toPlayer = player.transform.position - transform.position;
        isPlayerNoisy = player.IsNoisy;
        neighbors = FindNeighbors();

        // universal state transitions
        if (state != SheepState.FleePlayer && isPlayerNoisy && toPlayer.LessThan(fleeRadius))
        {
            // flee
            UpdateState(SheepState.FleePlayer);
        }

        switch (state)
        {
            case SheepState.Graze:
                Wander();
                break;
            case SheepState.GreetPlayer:
                Greet();
                break;
            case SheepState.FleePlayer:
                Flee();
                break;
            case SheepState.Alert:
                Alert();
                break;
        }
    }

    void OnInteract(GameObject player)
    {
        heartSpawner.ShowHearts(1);
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

    void UpdateState(SheepState state)
    {
        // state entry logic
        switch (state)
        {
            case SheepState.Graze:
                wanderTarget = Position;
                break;
            case SheepState.FleePlayer:
                fleeTarget = player.transform.position;
                break;
        }

        this.state = state;
    }

    public void ReceiveApple()
    {
        heartSpawner.ShowHearts(5);
        attitude += 1f;
        greetPlayerResetTimer = greetResetTime;
    }

    #endregion

    #region Behaviours

    void Wander()
    {
        // check if apple within range
        var apple = FindApple();

        if (apple != null)
        {
            // seek apple
            wanderTarget = apple.transform.position;

            // pick up apple
            if ((apple.transform.position - transform.position).LessThan(pickupRadius))
            {
                GameObject.Destroy(apple);
                ReceiveApple();
            }
        }
        else
        {
            // change target every so often
            var changeTargetChance = IsOnPasture ? grazeChangeTarget : wanderChangeTarget;

            if (Random.value < changeTargetChance * Time.deltaTime)
            {
                wanderTarget = PickNewWanderPoint();
            }
        }

        MoveTowards(wanderTarget);

        // graze when on pasture and not moving
        if (steering.Velocity.sqrMagnitude == 0 && IsOnPasture)
        {
            isHeadDown = true;
            pasture.exhaustion -= Time.deltaTime;
        }
        else
        {
            isHeadDown = false;
        }

        if (attitude < 1f)
        {
            // go alert when player approaches
            if (toPlayer.LessThan(wanderAlertRadius))
            {
                UpdateState(SheepState.Alert);
            }
        }
        else if (attitude >= 2f)
        {
            // green when player approaches
            if (greetPlayerResetTimer <= 0f && toPlayer.LessThan(greetRadius))
            {
                UpdateState(SheepState.GreetPlayer);
            }
        }
    }

    void Greet()
    {
        isHeadDown = false;

        if (toPlayer.LessThan(2f))
        {
            // boop
            heartSpawner.ShowHearts(1);
            EventBus.Instance.Notify(Event.SHEEP_GREET, gameObject);
            greetPlayerResetTimer = greetResetTime;
            UpdateState(SheepState.Graze);
        }
        else
        {
            MoveTowards(player.transform.position);
        }
    }

    void Flee()
    {
        isHeadDown = false;

        // if the sheep can hear player, update position
        if (isPlayerNoisy)
        {
            fleeTarget = player.transform.position;
        }

        // flocking behaviours
        FleeFrom(fleeTarget);
        Seperate(fleeSeparationWeight, fleeSeparationRadius);
        Cohere(fleeCohesionWeight);

        // if out of range revert to wander
        if (!(fleeTarget - Position).LessThan(fleeRadius + 3f))
        {
            UpdateState(SheepState.Graze);
        }
    }

    void Alert()
    {
        isHeadDown = false;

        if (toPlayer.LessThan(wanderFleeRadius))
        {
            UpdateState(SheepState.FleePlayer);
        }
        else if (!toPlayer.LessThan(wanderAlertRadius))
        {
            UpdateState(SheepState.Graze);
        }
    }

    #endregion

    #region Movement

    void MoveTowards(Vector2 target, float radius = 1f, float weight = 1f)
    {
        if (!(target - Position).LessThan(radius))
        {
            steering.AddForce((target - Position).normalized * weight);
        }
    }

    void FleeFrom(Vector2 target, float weight = 1f)
    {
        var fromTarget = Position - target;
        steering.AddForce(fromTarget.normalized * weight);
    }

    void Cohere(float weight = 1f)
    {
        var neighbors = FindNeighbors();

        var center = neighbors.Aggregate(
            Vector2.zero,
            (sum, sheep) => sum + sheep.Position) / neighbors.Count;

        steering.AddForce((center - Position).normalized * weight);
    }

    void Seperate(float weight = 1f, float radius = 1f)
    {
        var neighbors = FindNeighbors();

        foreach (var neighbor in neighbors)
        {
            var multiplier = Mathf.Pow((neighbor.Position - Position).magnitude / radius, -2f);
            var force = (Position - neighbor.Position).normalized / neighbors.Count * multiplier;
            steering.AddForce(force * weight);
        }
    }

    #endregion

    #region Helpers

    List<Sheep> FindNeighbors()
    {
        return GameObject.FindObjectsOfType<Sheep>()
            .Where(other =>
                (other.Position - Position).sqrMagnitude < neighborRadius * neighborRadius &&
                other != this)
            .ToList();
    }

    GameObject FindApple()
    {
        var treats = GameObject.FindGameObjectsWithTag(Tags.TREAT);

        return treats.FirstOrDefault(treat =>
        {
            return (treat.transform.position - transform.position).LessThan(wanderAlertRadius);
        });
    }

    Vector2 PickNewWanderPoint()
    {
        var radius = IsOnPasture ? grazeRadius : wanderRadius;

        for (int i = 0; i < 32; i++)
        {
            var sample = Position + Random.insideUnitCircle * radius;

            if (CanMoveTo(sample) && (i > 8 || IsPointOnPasture(sample)))
            {
                return sample;
            }
        }

        return Position;
    }

    bool IsPointOnPasture(Vector2 point)
    {
        return Physics2D.OverlapPointAll(point)
            .Any(collider =>
            {
                var pasture = collider.GetComponent<Pasture>();
                return pasture != null && !pasture.isExhausted;
            });
    }

    bool CanMoveTo(Vector2 point)
    {
        var results = new RaycastHit2D[1];
        var filter = new ContactFilter2D();
        filter.SetLayerMask(blockingLayer);
        var raycast = GetComponent<Collider2D>()
            .Cast(point - Position, filter, results, (point - Position).magnitude);
        return raycast == 0;
    }

    #endregion
}
