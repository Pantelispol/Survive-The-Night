using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float baseHealth = 10f;
    private float currentHealth;
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject ragdoll;

    [Header("Combat")]
    [SerializeField] private float attackCD = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float baseAggroRange = 10f;
    [SerializeField] private float damageToDoor = 3f;
    private float aggroRange;

    [Header("Roaming")]
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private float roamInterval = 3f;
    private float roamTimer = 0f;
    private Vector3 roamTarget;

    [HideInInspector] public EnemySpawner spawner;
    private GameObject player;
    private NavMeshAgent agent;
    private Animator animator;
    private float timePassed;
    private float newDestinationCD = 0.5f;

    private Door currentTargetDoor;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        ScaleAggroRangeByDay();
        ScaleHealthBasedOnDay();
        StartCoroutine(ApplyDamageOverTime());
        ScaleAttackSpeedByDay();
        ScaleSpeedByDay();
    }

    void Update()
    {
        if (player == null) return;

        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        RaycastHit hit;
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        bool sawPlayer = Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, aggroRange) && hit.collider.CompareTag("Player");
        bool sawDoor = hit.collider != null && hit.collider.GetComponent<Door>() != null;

        if (sawPlayer)
        {
            currentTargetDoor = null;
            HandleAttack(player.transform.position);
        }
        else if (sawDoor)
        {
            currentTargetDoor = hit.collider.GetComponent<Door>();
            HandleAttack(currentTargetDoor.transform.position);
        }
        else
        {
            Roam();
        }

        newDestinationCD -= Time.deltaTime;
        timePassed += Time.deltaTime;

        if (sawPlayer)
        {
            transform.LookAt(player.transform);
        }
    }

    private void HandleAttack(Vector3 targetPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) <= attackRange && timePassed >= attackCD)
        {
            animator.SetTrigger("attack");
            timePassed = 0;
        }

        if (newDestinationCD <= 0)
        {
            agent.SetDestination(targetPosition);
            newDestinationCD = 0.5f;
        }
    }

    private void Roam()
    {
        roamTimer += Time.deltaTime;

        if (roamTimer >= roamInterval || Vector3.Distance(transform.position, roamTarget) < 1f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection.y = 0;
            Vector3 roamPosition = transform.position + randomDirection;

            if (NavMesh.SamplePosition(roamPosition, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
            {
                roamTarget = hit.position;
                agent.SetDestination(roamTarget);
            }

            roamTimer = 0f;
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (true)
        {
            if (TimeManager.Instance != null)
            {
                var currentTimeOfDay = TimeManager.Instance.CurrentTimeOfDay;
                if (currentTimeOfDay == TimeManager.TimeOfDay.Sunrise ||
                    currentTimeOfDay == TimeManager.TimeOfDay.Day ||
                    currentTimeOfDay == TimeManager.TimeOfDay.Sunset)
                {
                    TakeDamage(10f); 
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
        }
    }

    void Die()
    {
        if (CompareTag("Zombie"))
            SoundManager.PlaySound(SoundType.ZombieDeath, transform.position);
        else if (CompareTag("Skeleton"))
            SoundManager.PlaySound(SoundType.SkeletonDeath, transform.position);

        if (spawner != null)
            spawner.NotifyEnemyDeath(this.gameObject);

        if (ragdoll != null)
        {
            GameObject rb = Instantiate(ragdoll, transform.position, transform.rotation);
            Destroy(rb, 5f);
        }

        if (hitVFX != null)
        {
            GameObject fx = Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(fx, 3f);
        }

        Destroy(gameObject);
    }


    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        animator.SetTrigger("damage");

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            float shakeDistanceThreshold = 10f;

            if (distanceToPlayer <= shakeDistanceThreshold)
            {
                CameraShake.Instance.ShakeCamera(2f, 0.2f);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void StartDealDamage()
    {
        if (currentTargetDoor != null)
        {
            currentTargetDoor.TakeDamage(damageToDoor);
        }
        else
        {
            GetComponentInChildren<EnemyDamageDealer>().StartDealDamage();
        }
    }

    public void EndDealDamage()
    {
        if (currentTargetDoor == null)
        {
            GetComponentInChildren<EnemyDamageDealer>().EndDealDamage();
        }
    }

    public void HitVFX(Vector3 hitPosition)
    {
        GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        Destroy(hit, 3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }

    private void ScaleHealthBasedOnDay()
    {
        int day = TimeManager.Instance != null ? TimeManager.Instance.Days : 0;

        float dayMultiplier = 1 + (day * 0.25f); 

        if (CompareTag("Skeleton"))
            currentHealth = baseHealth * dayMultiplier;
        else if (CompareTag("Zombie"))
            currentHealth = baseHealth * (1 + (day * 0.4f)); 
        else
            currentHealth = baseHealth;

        Debug.Log($"[Enemy] Health scaled on Day {day}. Final health: {currentHealth}");
    }


    private void ScaleAggroRangeByDay()
    {
        if (TimeManager.Instance != null)
        {
            int currentDay = TimeManager.Instance.Days;
            aggroRange = baseAggroRange + (currentDay * 5f);
            Debug.Log($"[Enemy] Aggro range scaled on Day {currentDay}. Final aggro range: {aggroRange}");
        }
        else
        {
            aggroRange = baseAggroRange;
            Debug.LogWarning("[Enemy] TimeManager is null. Using default aggro range.");
        }
    }

    private void ScaleAttackSpeedByDay()
    {
        int day = TimeManager.Instance != null ? TimeManager.Instance.Days : 0;
        attackCD = Mathf.Max(1f, 5f - day * 1f);
        Debug.Log($"[Enemy] Attack cooldown scaled on Day {day}. Final attackCD: {attackCD}");
    }

    private void ScaleSpeedByDay()
    {
        int day = TimeManager.Instance != null ? TimeManager.Instance.Days : 0;

        float speedMultiplier = 1 + (day * 0.1f);
        agent.speed *= speedMultiplier;

        Debug.Log($"[Enemy] Speed scaled on Day {day}. Final speed: {agent.speed}");
    }


}
