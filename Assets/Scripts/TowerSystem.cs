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

    // Start is called before the first frame update
    void Start()
    {
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
            LifeSystem targetLifeSystem = currentTarget.GetComponent<LifeSystem>();
            if (targetLifeSystem != null)
            {
                // If the target is a player, apply progressively increasing damage
                if (currentTarget.CompareTag("PlayerRed") || currentTarget.CompareTag("PlayerBlue"))
                {
                    ApplyPlayerDamage(targetLifeSystem);
                }
                else if (currentTarget.CompareTag("MinionRed") || currentTarget.CompareTag("MinionBlue"))
                {
                    float damage = 0.4f; // Ranged minions take 90%, melee minions take 40%
                    if (targetLifeSystem.AttackRange < 5f)
                    {
                        damage = 0.4f;
                    }
                    else
                    {
                        damage = 0.9f;
                    }
                    targetLifeSystem.TakeDamage(targetLifeSystem.MaxHP * damage);
                }
                else
                {
                    // Deal standard damage to others
                    targetLifeSystem.TakeDamage(damagePerHit);
                }

                // Apply the effect on the target
                ApplyEffectOnTarget(currentTarget);
            }
        }
    }

    // Apply progressive damage to players
    void ApplyPlayerDamage(LifeSystem targetLifeSystem)
    {
        float damage = damagePerHit;

        // If this is the second shot, apply 80 damage
        if (playerShotCount == 1)
        {
            damage = damagePerHit + 40f;
        }
        // If this is the third shot, apply 120 damage
        else if (playerShotCount == 2)
        {
            damage = damagePerHit + 80f;
        }
        // For all subsequent shots, apply 200 damage
        else if (playerShotCount >= 3)
        {
            damage = damagePerHit + 160f;
        }

        // Deal the calculated damage to the player
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
