using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSpellMinotaur : MonoBehaviour
{
    private int damage = 150;
    private float range = 3f;
    private float cooldown = 6f;
    private float lastAttackTime = 0f;
    private Animator anim;
    private LifeSystem lifeSystem;
    public GameObject kickEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lifeSystem = GetComponent<LifeSystem>();
        lastAttackTime = Time.time - cooldown; // Set the last attack time to the past to allow the first attack
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CastQSpell();
        }
    }

    //if player clicks on the Q key with his mouse on either a minion or a enemy player, the Minotaur will deal the damage to the target and project the target in a straight line
    public void CastQSpell()
    {
        if (Time.time - lastAttackTime >= cooldown)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Vector3.Distance(transform.position, hit.point) <= range)
            {
                // Check if the target is a valid enemy or minion
                if (hit.collider.CompareTag("PlayerBlue") && this.CompareTag("PlayerRed")
                    || hit.collider.CompareTag("PlayerRed") && this.CompareTag("PlayerBlue")
                    || hit.collider.CompareTag("MinionBlue") && this.CompareTag("PlayerRed")
                    || hit.collider.CompareTag("MinionRed") && this.CompareTag("PlayerBlue"))
                {
                    // Debug.Log("Attacking: " + hit.collider.gameObject.name);
                    GameObject target = hit.collider.gameObject;
                    LifeSystem targetLifeSystem = target.GetComponent<LifeSystem>();

                    anim.SetBool("isKicking", true);
                    if (kickEffectPrefab != null)
                    {
                        GameObject kickEffect = Instantiate(kickEffectPrefab, transform.position, Quaternion.identity);
                        kickEffect.transform.SetParent(hit.collider.transform); // Parent the effect to the target
                    }
                    else
                    {
                        Debug.LogWarning("Kick Effect Prefab is not assigned!");
                    }

                    if (targetLifeSystem != null)
                    {
                        // Deal damage
                        targetLifeSystem.TakeDamage(damage + (int)(lifeSystem.GetAttackDamage() * 0.5f));
                        lastAttackTime = Time.time;

                        // Apply knockback
                        StartCoroutine(KnockbackTarget(target, 3.5f, 0.5f)); // Knockback with 2 units over 0.5 seconds
                    }

                    StartCoroutine(ResetAnimation());
                }
            }
        }
    }

    // Knockback Coroutine
    IEnumerator KnockbackTarget(GameObject target, float knockbackDistance, float duration)
    {
        if (target == null)
        {
            yield break; // Exit if the target is null
        }

        Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;
        Vector3 startPosition = target.transform.position;
        Vector3 targetPosition = startPosition + knockbackDirection * knockbackDistance;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Check if the target still exists
            if (target == null)
            {
                yield break; // Stop the coroutine if the target is destroyed
            }

            target.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (target != null)
        {
            target.transform.position = targetPosition; // Ensure final position if the target still exists
        }
    }


    IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isKicking", false);
    }
}
