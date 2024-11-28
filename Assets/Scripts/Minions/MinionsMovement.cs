using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionsMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    private GameObject previousTarget;

    private float attackRange = 3f;
    public float detectRange = 8f;

    public bool isCaster;

    public GameObject[] waypoints; // Assign waypoints in the inspector
    private int waypointIndex = 0;

    // Add LifeSystem to handle HP and Damage for minions
    private LifeSystem lifeSystem;

    //get animator
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(20, 80); // Assign random priority to avoid collisions
        agent.radius = 0.2f; // Adjust this based on your scene
    
        lifeSystem = GetComponent<LifeSystem>(); // Get the LifeSystem component

        animator = GetComponent<Animator>();

        if (lifeSystem != null)
        {
            lifeSystem.Initialize(100, 0, 0, 25, 0); // Initialize stats for minions
        }

        if (isCaster)
        {
            // Set the attackRange for caster minion
            attackRange = 5f;
        }
        else
        {
            // Set the attackRange for melee minion
            attackRange = 2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckForTarget();
        // Debug.Log("Target: " + target);
        // Debug.Log("Waypoint Index: " + waypointIndex);
    }

    private bool isInRangeAttack()
    {
        float adjustedRange = attackRange;

        if (target != null && (target.CompareTag("TowerRed") || target.CompareTag("TowerBlue")))
        {
            // Add extra range for towers
            adjustedRange += 1.5f;
        }

        // Check if the target is within the adjusted range
        return Vector3.Distance(transform.position, target.transform.position) <= adjustedRange;
    }

    private void Move()
    {
        if (target != null)
        {
            // Adjust stopping distance dynamically
            if (target.CompareTag("TowerRed") || target.CompareTag("TowerBlue"))
            {
                agent.stoppingDistance = Mathf.Min(attackRange, 3f); // Adjust stopping distance for towers
            }
            else
            {
                agent.stoppingDistance = 0f; // Default stopping distance
            }

            // Check if the agent is in range to attack
            if (isInRangeAttack())
            {
                Attack();
            }
            else
            {
                // Calculate the closest point near the tower if the target is a tower
                if (target.CompareTag("TowerRed") || target.CompareTag("TowerBlue"))
                {
                    Vector3 closestPoint = GetClosestPointOnBounds(target);
                    agent.SetDestination(closestPoint);
                }
                else
                {
                    agent.SetDestination(target.transform.position);
                }
            }
        }
    }

    private Vector3 GetClosestPointOnBounds(GameObject target)
    {
        Collider towerCollider = target.GetComponent<Collider>();
        if (towerCollider != null)
        {
            // Find the closest point on the collider bounds
            return towerCollider.ClosestPoint(transform.position);
        }
        
        // Fallback to the target's position if no collider is present
        return target.transform.position;
    }


    private void Attack()
    {
        if (lifeSystem != null && lifeSystem.CanAttack())
        {
            if (target != null && target.CompareTag("PlayerRed") || target.CompareTag("PlayerBlue")
            || target.CompareTag("MinionRed") || target.CompareTag("MinionBlue")
            || target.CompareTag("TowerRed") || target.CompareTag("TowerBlue"))
            {
                // Déclenche l'animation d'attaque
                animator.SetBool("isAttacking", true);

                LifeSystem targetLifeSystem = target.GetComponent<LifeSystem>();
                if (targetLifeSystem != null)
                {
                    targetLifeSystem.TakeDamage(lifeSystem.GetAttackDamage());
                }

                // Lance une coroutine pour attendre avant de désactiver l'animation
                StartCoroutine(ResetAttackAnimation());
            }
        }

        agent.SetDestination(transform.position);
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Ajustez le temps à la durée de l'animation d'attaque
        animator.SetBool("isAttacking", false);
    }

    private void CheckForTarget()
    {
        previousTarget = target;
        target = null;

        List<GameObject> minions = new List<GameObject>();
        List<GameObject> towers = new List<GameObject>();
        List<GameObject> players = new List<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRange);

        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            if (CompareTag("MinionRed") && obj.CompareTag("MinionBlue") ||
                CompareTag("MinionBlue") && obj.CompareTag("MinionRed"))
            {
                minions.Add(obj);
            }
            else if (CompareTag("MinionRed") && obj.CompareTag("TowerBlue") ||
                    CompareTag("MinionBlue") && obj.CompareTag("TowerRed"))
            {
                towers.Add(obj);
            }
            else if (CompareTag("MinionRed") && obj.CompareTag("PlayerBlue") ||
                    CompareTag("MinionBlue") && obj.CompareTag("PlayerRed"))
            {
                players.Add(obj);
            }
        }

        if (minions.Count > 0)
        {
            target = GetClosest(minions);
        }
        else if (towers.Count > 0)
        {
            target = GetClosest(towers);
        }
        else if (players.Count > 0)
        {
            target = GetClosest(players);
        }
        else
        {
            // Default to waypoint navigation if no target
            target = waypoints[waypointIndex];

            float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position);
            if (distanceToWaypoint < 0.5f) // Smaller threshold for reaching the waypoint
            {
                waypointIndex++; // Move to the next waypoint
                if (waypointIndex >= waypoints.Length)
                {
                    waypointIndex = 0; // Loop back to the first waypoint
                }
            }

            // Set the destination only if the target has changed
            if (previousTarget != target || agent.destination != waypoints[waypointIndex].transform.position)
            {
                agent.SetDestination(waypoints[waypointIndex].transform.position);
            }
        }
    }

    private GameObject GetClosest(List<GameObject> objects)
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
