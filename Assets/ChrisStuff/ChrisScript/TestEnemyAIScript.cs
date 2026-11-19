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

    //patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float sightRange;
    public bool playerInSightRange;

    public float maxPatrolTime = 3f;
    float patrolTimer;
    public PlayerInteractor interactor;
    public Animator enemyAnim;

   // float lastX;
   // Vector3 lastPos;
    private void Awake()
    {
        enemy=GetComponent<Transform>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        enemyAnim=GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;
        agent=GetComponent<NavMeshAgent>();
        enemyCollider= GetComponent<Collider>();
        interactor=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractor>();
    }
    private void Start()
    {
        //lastX = enemy.position.x;
       // lastPos = enemy.position;
    }
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        // playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        if (!playerInSightRange) Patrolling();
        if (playerInSightRange&&!interactor.playerInsideVent) ChasePlayer();


       // float currentx=transform.position.x;
     
       // enemySprite.flipX = currentx <lastX;
        //lastX = currentx;

       // Vector3 currentPos = transform.position;
       // Vector3 dif = currentPos - lastPos;
        //float realdif = dif.magnitude;
        //if (realdif < 0.01||realdif==0)
       // {
            //enemyin idle state
       // }
        //if (horizontal != 0)
            //playerSprite.flipX = horizontal < 0;
            UpdateEnemySpriteSheet();
    }
    /*private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //WalkPoint reached;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }*/
    public void UpdateEnemySpriteSheet()
    {
        Vector3 vel=agent.velocity;
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
        if (!walkPointSet)
        {
            SearchWalkPoint();
            patrolTimer = 0f;
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            patrolTimer += Time.deltaTime;

            bool reached = !agent.pathPending &&
                           agent.remainingDistance <= agent.stoppingDistance + 0.5f;
            bool unreachable = !agent.pathPending &&
                               agent.pathStatus != NavMeshPathStatus.PathComplete;
            bool timedOut = patrolTimer > maxPatrolTime;

            if (reached || unreachable || timedOut)
                walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        int m = Random.Range(1, 3);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
      
        //int Random = Random.Range(1, 3);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
    }


}
