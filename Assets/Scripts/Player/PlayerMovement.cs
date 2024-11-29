using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private LifeSystem lifeSystem;

    private GameObject target;
    private float range = 5f;

    public GameObject clickEffectPrefab;
    public GameObject selectTowerEffectPrefab;
    public GameObject selectMinionEffectPrefab;
    private GameObject activeSelectEffect;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lifeSystem = GetComponent<LifeSystem>();
        anim = GetComponent<Animator>();

        if (lifeSystem != null)
        {
            lifeSystem.Initialize(1000, 30, 25, 50, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleClick();
        Move();
        MovingAnimation();
    }

    private void MovingAnimation()
    {
        if (agent.velocity.magnitude > 0.1f) // A small threshold to detect movement
        {
            if (anim.GetInteger("speed") != 1) // Only set if not already moving
            {
                // Debug.Log("Moving");
                anim.SetInteger("speed", 1);
            }
        }
        else
        {
            if (anim.GetInteger("speed") != 0) // Only reset if not already idle
            {
                anim.SetInteger("speed", 0);
            }
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hit an object with the "Map" tag
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Map"))
            {
                agent.SetDestination(hit.point);
                target = null;

                GameObject clickEffect = Instantiate(clickEffectPrefab, new Vector3(hit.point.x, 0.1f, hit.point.z), Quaternion.identity);
                Destroy(clickEffect, 0.1f);

                if (activeSelectEffect != null)
                {
                    Destroy(activeSelectEffect);
                    activeSelectEffect = null;
                }
            }

            // Check if the player's tag is "PlayerRed" and select appropriate targets
            if (CompareTag("PlayerRed"))
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("TowerBlue"))
                {
                    SelectTarget(hit.collider.gameObject, selectTowerEffectPrefab);
                }
                else if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("MinionBlue"))
                {
                    SelectTarget(hit.collider.gameObject, selectMinionEffectPrefab);
                }
            }

            // Check if the player's tag is "PlayerBlue" and select appropriate targets
            if (CompareTag("PlayerBlue"))
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("TowerRed"))
                {
                    SelectTarget(hit.collider.gameObject, selectTowerEffectPrefab);
                }
                else if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("MinionRed"))
                {
                    SelectTarget(hit.collider.gameObject, selectMinionEffectPrefab);
                }
            }
        }
    }

    private void Move()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;

            if (target.CompareTag("TowerBlue") || target.CompareTag("TowerRed"))
            {
                // Adjust target position for towers
                float towerRadius = 3f; // Adjust this value as needed
                Vector3 directionToTower = (target.transform.position - transform.position).normalized;
                targetPosition = target.transform.position - directionToTower * towerRadius;

                // Debug.Log("Adjusted target position for tower: " + targetPosition);
            }

            if (isInRange())
            {
                // Debug.Log("In range, attacking target.");
                Attack();
            }
            else
            {
                // Debug.Log("Moving toward target at: " + targetPosition);
                agent.SetDestination(targetPosition);
            }
        }
        else
        {
            // Debug.Log("No target selected.");
        }
    }


    private bool isInRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= range;
    }

    private void Attack()
    {
        if (lifeSystem != null && lifeSystem.CanAttack())  // Check if player can attack
        {
            // Attack logic
            LifeSystem targetLifeSystem = target.GetComponent<LifeSystem>();
            if (targetLifeSystem != null)
            {
                int randomAttack = Random.Range(0, 2);
                if (randomAttack == 0)
                {
                    anim.SetBool("isAttacking1", true);
                }
                else
                {
                    anim.SetBool("isAttacking2", true);
                }

                if (targetLifeSystem != null)
                {
                    targetLifeSystem.TakeDamage(lifeSystem.GetAttackDamage());
                }

                // Lance une coroutine pour attendre avant de désactiver l'animation
                StartCoroutine(ResetAttackAnimation());
            }
        }
        else
        {
            // Debug.Log("Attack on cooldown.");
        }

        // Stop movement after attacking
        agent.SetDestination(transform.position);
    }

    //coroutine reset attack animation
    IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isAttacking1", false);
        anim.SetBool("isAttacking2", false);
    }

    private void SelectTarget(GameObject newTarget, GameObject effectPrefab)
    {
        target = newTarget;

        // Destroy the previous effect if it exists
        if (activeSelectEffect != null)
        {
            Destroy(activeSelectEffect);
        }

        // Instantiate the effect at the target's position, but not parent it yet
        activeSelectEffect = Instantiate(effectPrefab, target.transform.position, Quaternion.identity);

        // Preserve the original scale of the prefab by using the prefab's original localScale
        Vector3 originalScale = effectPrefab.transform.localScale;

        // Set the target as its parent
        activeSelectEffect.transform.SetParent(target.transform);

        // Apply the original scale
        activeSelectEffect.transform.localScale = originalScale;

        // Set the effect's position to the target's position with Y as 0.1f
        activeSelectEffect.transform.position = new Vector3(target.transform.position.x, 0.1f, target.transform.position.z);
    }
}
