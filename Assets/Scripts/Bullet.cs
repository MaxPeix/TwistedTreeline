using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject target;
    private float speed = 20f;
    private float damage;
    private System.Action<LifeSystem> customDamageLogic; // Delegate for custom player damage logic
    private int shotCount;

    public void SetTarget(GameObject newTarget, float newDamage, System.Action<LifeSystem> damageLogic, int playerShotCount)
    {
        target = newTarget;
        damage = newDamage;
        customDamageLogic = damageLogic;
        shotCount = playerShotCount;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroy bullet if target is null
            return;
        }

        // Move the bullet toward the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if the bullet reaches the target
        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        LifeSystem targetLifeSystem = target.GetComponent<LifeSystem>();
        if (targetLifeSystem != null)
        {
            // Apply custom player damage logic if available
            if (target.CompareTag("PlayerRed") || target.CompareTag("PlayerBlue"))
            {
                customDamageLogic?.Invoke(targetLifeSystem);
            }
            if (target.CompareTag("MinionRed") || target.CompareTag("MinionBlue"))
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
                targetLifeSystem.TakeDamage(40f);
            }
        }

        Destroy(gameObject); // Destroy the bullet after hitting the target
    }
}
