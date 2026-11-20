using UnityEngine;
using UnityEngine.AI;

public class anotherTest : MonoBehaviour
{
    public Transform enemy;
    public SpriteRenderer enemySprite;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Collider enemyCollider;

    //patrol (waypoints)
    public Transform[] patrolPoints;
    int patrolIndex;
    bool patrolDestinationSet;

    public float sightRange;
    public bool playerInSightRange;

    public PlayerInteractor interactor;
    public Animator enemyAnim;
    public bool isChasing = false;

    private void Awake()
    {
        enemy = GetComponent<Transform>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        enemyAnim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        interactor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractor>();
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange) Patrolling();
        if (playerInSightRange && !interactor.playerInsideVent) ChasePlayer();

        UpdateEnemySpriteSheet();
    }

    public void UpdateEnemySpriteSheet()
    {
        Vector3 vel = agent.velocity;
        if (vel.magnitude < 0.1f)
        {
            enemyAnim.SetBool("isWalking", false);
            // idle state
            return;
        }
        else
        {
            enemyAnim.SetBool("isWalking", true);
        }
        if (vel.x > 0.1f) enemySprite.flipX = false;
        else if (vel.x < -0.1f) enemySprite.flipX = true;
    }
  private void Patrolling()
    {
        isChasing = false;
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (!patrolDestinationSet)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            patrolDestinationSet = true;
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void ChasePlayer()
    {
        isChasing = true;
        patrolDestinationSet = false;
        agent.SetDestination(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}