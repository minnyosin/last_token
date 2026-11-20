using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemyAIScript : MonoBehaviour
{
    public Transform enemy;
    public SpriteRenderer enemySprite;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Collider enemyCollider;
    public Transform[] patrolPoints;
    public float sightRange = 22;
    public bool playerInSightRange;
    public PlayerInteractor interactor;
    public Animator enemyAnim;
    public bool isChasing;
    public SpriteRenderer ExclamationMark;
    public AudioSource spottedAudio;
    public float patrolSpeed = 4.8f;
    public float chaseSpeed = 6.6f;

    int patrolIndex;
    float destinationTimer;
    float searchTime;
    Vector3 searchPosition;
    Vector3 progressPosition;
    float stuckTime;

    void Awake()
    {
        enemy = transform;
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        enemyAnim = GetComponentInChildren<Animator>();
        interactor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractor>();
        player = interactor.transform;
        ExclamationMark.enabled = false;
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            var points = GameObject.FindGameObjectsWithTag("PatrolPoint");
            System.Array.Sort(points,(a,b) => string.CompareOrdinal(a.name,b.name));
            patrolPoints = new Transform[points.Length];
            for(int i=0;i<points.Length;i++) { patrolPoints[i] = points[i].transform; }
        }
        progressPosition = transform.position;
    }

    void Update()
    {
        if (!agent.enabled || !agent.isOnNavMesh) { return; }
        if (!interactor.LMS.canInput)
        {
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;
        playerInSightRange = !interactor.playerInsideVent && Vector3.Distance(transform.position,player.position) < sightRange;
        if (playerInSightRange)
        {
            Vector3 from = transform.position + Vector3.up * 1.5f;
            Vector3 to = player.position + Vector3.up * 1.5f;
            RaycastHit hit;
            if (Physics.Raycast(from,(to-from).normalized,out hit,Vector3.Distance(from,to),~0,QueryTriggerInteraction.Ignore))
            {
                playerInSightRange = hit.collider.GetComponentInParent<PlayerInteractor>() == interactor;
            }
        }
        if (playerInSightRange)
        {
            if (!isChasing) { StartCoroutine(Spotted()); }
            isChasing = true;
            searchPosition = player.position;
            searchTime = 2.5f;
        }
        else
        {
            isChasing = false;
            searchTime -= Time.deltaTime;
        }
        if (interactor.playerInsideVent) { searchTime = 0; }
        agent.speed = searchTime > 0 ? chaseSpeed : patrolSpeed;
        destinationTimer -= Time.deltaTime;
        if (destinationTimer <= 0)
        {
            destinationTimer = .35f;
            if (searchTime > 0)
            {
                if (!GoTo(searchPosition)) { searchTime = 0; Patrol(); }
            }
            else { Patrol(); }
        }
        if (Vector3.Distance(progressPosition,transform.position) > .2f)
        {
            progressPosition = transform.position;
            stuckTime = 0;
        }
        else if (agent.hasPath && !agent.pathPending && agent.remainingDistance > 1)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > 1.5f)
            {
                agent.ResetPath();
                searchTime = 0;
                if (patrolPoints.Length > 0) { patrolIndex = (patrolIndex + 1) % patrolPoints.Length; }
                destinationTimer = 0;
                stuckTime = 0;
            }
        }
        UpdateEnemySpriteSheet();
    }

    bool GoTo(Vector3 position)
    {
        NavMeshHit hit;
        var path = new NavMeshPath();
        if (!NavMesh.SamplePosition(position,out hit,2,agent.areaMask)) { return false; }
        if (!agent.CalculatePath(hit.position,path) || path.status != NavMeshPathStatus.PathComplete) { return false; }
        return agent.SetPath(path);
    }
    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) { return; }
        Vector3 distance = Vector3.zero;
        if (patrolPoints[patrolIndex] != null)
        {
            distance = patrolPoints[patrolIndex].position - transform.position;
            distance.y = 0;
        }
        if (patrolPoints[patrolIndex] == null || distance.sqrMagnitude < 1)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
        for(int i=0;i<patrolPoints.Length;i++)
        {
            if (patrolPoints[patrolIndex] != null && GoTo(patrolPoints[patrolIndex].position)) { return; }
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
        agent.ResetPath();
    }
    public void HearToken(Vector3 position)
    {
        if (Vector3.Distance(transform.position,position) > 45) { return; }
        searchPosition = position;
        searchTime = 5;
        destinationTimer = 0;
    }
    public void UpdateEnemySpriteSheet()
    {
        Vector3 movement = agent.velocity;
        enemyAnim.SetBool("isWalking",movement.sqrMagnitude > .01f);
        if (movement.x > .1f) { enemySprite.flipX = false; }
        if (movement.x < -.1f) { enemySprite.flipX = true; }
    }
    IEnumerator Spotted()
    {
        ExclamationMark.enabled = true;
        spottedAudio.Play();
        yield return new WaitForSeconds(.7f);
        ExclamationMark.enabled = false;
    }
}
