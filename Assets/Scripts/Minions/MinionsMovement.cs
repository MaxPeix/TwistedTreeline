using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionsMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    private GameObject enemyCore;
    private GameObject previousTarget;

    private float attackRange = 3f;
    public float detectRange = 8f;

    public bool isCaster;

    // Add LifeSystem to handle HP and Damage for minions
    private LifeSystem lifeSystem;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(20, 80); // Assign random priority to avoid collisions
        agent.radius = 0.2f; // Adjust this based on your scene
    
        lifeSystem = GetComponent<LifeSystem>(); // Get the LifeSystem component

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

        if (CompareTag("MinionRed"))
        {
            enemyCore = GameObject.Find("BlueCore");
        }
        else
        {
            enemyCore = GameObject.Find("RedCore");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckForTarget();
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
            if (target.CompareTag("TowerRed") || target.CompareTag("TowerBlue"))
            {
                // put stopping distance to 10 for towers and put attack range to 5
                agent.stoppingDistance = 15f;
                if (isCaster)
                {
                    attackRange = 6f;
                }
                else
                {
                    attackRange = 3f;
                }
            }
            else
            {
                // put stopping distance to 0 for minions and put attack range to default for melee and caster
                agent.stoppingDistance = 0f;
                if (isCaster)
                {
                    attackRange = 5f;
                }
                else
                {
                    attackRange = 2f;
                }
            }
            if (isInRangeAttack())
            {
                Attack();
            }
            else
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, path))
                {
                    agent.SetDestination(target.transform.position);
                }
                else
                {
                    //Debug.LogWarning("Target is unreachable!");
                    // Manual movement as a fallback if the target is unreachable
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    transform.position += direction * Time.deltaTime * agent.speed;
                }
            }
        }
    }

    private void Attack()
    {
        if (lifeSystem != null && lifeSystem.CanAttack())  // Check if minion can attack
        {
            if (target != null)
            {
                LifeSystem targetLifeSystem = target.GetComponent<LifeSystem>();

                if (targetLifeSystem != null)
                {
                    targetLifeSystem.TakeDamage(lifeSystem.GetAttackDamage());
                }
            }
        }
        else
        {
            // Debug.Log("Attack on cooldown.");
        }

        // Stop movement after attacking
        agent.SetDestination(transform.position);
    }

    private void CheckForTarget()
    {
        previousTarget = target;
        // Clear the current target
        target = null;

        // List of potential targets sorted by priority
        List<GameObject> minions = new List<GameObject>();
        List<GameObject> towers = new List<GameObject>();
        List<GameObject> players = new List<GameObject>();

        // Detect all colliders within the detect range
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRange);

        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            // Check for opposing team tags
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

        // Set target based on priority
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
            target = enemyCore;
        }

        if (previousTarget != target)
        {
            agent.SetDestination(transform.position);
        }
    }

    // Helper method to find the closest target from a list
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

    // Uncomment this function to visualize the ranges in the Scene view
    private void OnDrawGizmos()
    {
        // Draw detect range in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Draw attack range in red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
