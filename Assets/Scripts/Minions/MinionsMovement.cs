using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionsMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    private GameObject enemyCore;

    private float attackRange = 3f;
    public float detectRange = 8f;

    [SerializeField] private bool isCaster;

    // Start is called before the first frame update
   void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(20, 80); // Assign random priority to avoid collisions
        agent.radius = 0.2f; // Adjust this based on your scene
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

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

        // Debug.Log("Enemy Core Target: " + enemyCore); // Debugging line
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckForTarget();
        // Debug.Log(target);
    }

    private bool isInRangeAttack()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    private void Move()
    {
        if (target != null)
        {
            if (isInRangeAttack())
            {
                Attack();
            }
            else
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Attack");
        agent.SetDestination(transform.position);
    }

    private void CheckForTarget()
    {
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
