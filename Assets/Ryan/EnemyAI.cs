using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState
    {
        Patrol,
        Chase,
        Search
    }




    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;




    [Header("Detection")]
    public float detectionRange = 8f;
    public LayerMask obstacleMask;

    [Header("Search")]
    public float searchDuration = 3f;

    [Header("Movement")]
    public float repathInterval = 0.2f;



    private AIState currentState;


    private int patrolIndex;


    private float searchTimer;
    private float repathTimer;

    private Vector3 lastKnownPosition;

    void Start()
    {
        
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        currentState = AIState.Patrol;

        MoveToNextPatrolPoint();

    }

    void Update()
    {
        
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                break;

            case AIState.Chase:
                Chase();
                break;

            case AIState.Search:
                Search();
                break;
        }

    }



    // idle/patrolling block

    void Patrol()
    {
        
        if (CanSeePlayer())
        {
            currentState = AIState.Chase;
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            MoveToNextPatrolPoint();
        }

    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.SetDestination(
            patrolPoints[patrolIndex].position
        );

        patrolIndex++;

        if (patrolIndex >= patrolPoints.Length)
            patrolIndex = 0;

    }



    // move towards the player...

    void Chase()
    {
        
        if (CanSeePlayer())
        {
            lastKnownPosition = player.position;

            repathTimer -= Time.deltaTime;

            if (repathTimer <= 0f)
            {
                agent.SetDestination(lastKnownPosition);
                repathTimer = repathInterval;
            }

        }

        else
        {
            currentState = AIState.Search;

            agent.SetDestination(lastKnownPosition);

            searchTimer = searchDuration;
        }

    }

  //player-locating block

    void Search()
    {
        if (CanSeePlayer())
        {
            currentState = AIState.Chase;
            return;
        }

        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0f)
        {
            currentState = AIState.Patrol;

            MoveToNextPatrolPoint();
        }

    }





    // block that handles player detection;;

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up;

        Vector3 direction = target - origin;

        float distance = direction.magnitude;

        // Check detection range.
        if (distance > detectionRange)
            return false;

        // Check whether a wall or obstacle blocks visibility.
        if (Physics.Raycast(
            origin,
            direction.normalized,
            distance,
            obstacleMask,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;

    }

}
