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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lifeSystem = GetComponent<LifeSystem>();

        if (lifeSystem != null)
        {
            lifeSystem.Initialize(1000, 30, 25, 50, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Player hp: " + lifeSystem.GetHP());
        HandleClick();
        Move();
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
            if (isInRange())
            {
                Attack();
            }
            else
            {
                agent.SetDestination(target.transform.position);
            }
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
                targetLifeSystem.TakeDamage(lifeSystem.GetAttackDamage());
                // Debug.Log("Attacking Target");
            }
        }
        else
        {
            // Debug.Log("Attack on cooldown.");
        }

        // Stop movement after attacking
        agent.SetDestination(transform.position);
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
