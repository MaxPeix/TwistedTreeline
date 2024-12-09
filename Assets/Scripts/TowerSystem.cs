using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSystem : MonoBehaviour
{
    public float damagePerHit = 40f;
    private float lastAttackTime = 0f;
    private float attackCooldown = 1f;

    private GameObject currentTarget;
    private int playerShotCount = 0;

    private LifeSystem lifeSystem;

    // Effect to apply on target when attacked
    public GameObject hitEffectPrefab; // Reference to the effect prefab
    private GameObject currentEffect; // Track the current effect
    public LineRenderer lineRenderer; // Assign the LineRenderer in the Inspector or dynamically
    public GameObject bulletPrefab;  // Assign the bullet prefab in the Inspector
    public Transform firePoint;      // A point on the tower where the bullet is fired



    // Start is called before the first frame update
    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.enabled = false; // Initially, disable the line
        lifeSystem = GetComponent<LifeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Tower HP: " + lifeSystem.GetHP());
        if (lifeSystem.IsAlive())
        {
            // Priority targeting system
            FindAndTargetEnemies();

            // If the tower has a target and can attack, attack the target
            if (currentTarget != null && CanAttack())
            {
                AttackTarget();
            }

            // If the target is no longer in range, destroy the effect
            if (currentEffect != null && !IsTargetInRange())
            {
                Destroy(currentEffect);
            }
        }
    }

    // Method to find and target minions first, then players
    void FindAndTargetEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f); // Search for targets in range (adjust range as needed)
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            GameObject potentialTarget = collider.gameObject;

            // If the target is a minion (priority) and different team, set it as the target
            if (potentialTarget.CompareTag("MinionRed") && gameObject.CompareTag("TowerBlue") ||
                potentialTarget.CompareTag("MinionBlue") && gameObject.CompareTag("TowerRed"))
            {
                float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = potentialTarget;
                }
            }
            // If no minions are found, target players from other team
            else if ((potentialTarget.CompareTag("PlayerRed") && gameObject.CompareTag("TowerBlue")) ||
                     (potentialTarget.CompareTag("PlayerBlue") && gameObject.CompareTag("TowerRed")))
            {
                if (closestTarget == null) // Only target player if no minion is available
                {
                    float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = potentialTarget;
                    }
                }
            }
        }

        // Check if the target has changed
        if (currentTarget != closestTarget)
        {
            ResetPlayerShotCount();  // Reset the shot count when the target changes
            // Destroy the current effect if the target is changed
            if (currentEffect != null)
            {
                Destroy(currentEffect);
                currentEffect = null;
            }
        }

        // Assign the closest valid target
        currentTarget = closestTarget;
        // Draw the line if a target is found
        if (currentTarget != null)
        {
            DrawLineToTarget(currentTarget.transform.position);
        }
        else
        {
            DisableLine(); // Disable the line if no target
        }
    }

    void DrawLineToTarget(Vector3 targetPosition)
    {
        lineRenderer.enabled = true; // Enable the line
        lineRenderer.SetPosition(0, transform.position); // Start point (tower position)
        lineRenderer.SetPosition(1, targetPosition);     // End point (target position)
    }

    void DisableLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Disable the line when no target
        }
    }
    // Check if the tower can attack based on cooldown
    bool CanAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            return true;
        }
        return false;
    }

    // Attack the current target
    void AttackTarget()
    {
        if (currentTarget != null)
        {
            // Instantiate a bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // Configure the bullet's target and damage
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetTarget(currentTarget, damagePerHit, ApplyPlayerDamage, playerShotCount);
            }
        }
    }

    // Apply progressive damage to players
    void ApplyPlayerDamage(LifeSystem targetLifeSystem)
    {
        float damage = damagePerHit;

        if (playerShotCount == 1)
        {
            damage += 40f;
        }
        if (playerShotCount == 2)
        {
            damage += 80f;
        }
        else if (playerShotCount >= 3)
        {
            damage += 160f;
        }
        targetLifeSystem.TakeDamage(damage);
        playerShotCount++;
    }

    // Reset player shot count when the target changes
    void ResetPlayerShotCount()
    {
        playerShotCount = 0;
    }

    // Apply a visual or other effect on the target when attacked
    void ApplyEffectOnTarget(GameObject target)
    {
        if (hitEffectPrefab != null)
        {
            // If the effect already exists, destroy it first
            if (currentEffect != null)
            {
                Destroy(currentEffect);
            }

            // Instantiate the effect at the target's position
            currentEffect = Instantiate(hitEffectPrefab, target.transform.position, Quaternion.identity);

            currentEffect.transform.SetParent(target.transform);

            // Offset to prevent overlap with the target's collider
            currentEffect.transform.position = new Vector3(target.transform.position.x, 0.1f, target.transform.position.z);
        }
    }

    // Check if the target is in range (this helps to determine if the effect should persist)
    bool IsTargetInRange()
    {
        if (currentTarget == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distanceToTarget <= 11f; // Check if target is within the effective range
    }
}
