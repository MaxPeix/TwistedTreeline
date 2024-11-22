using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;

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
    }

    // Update is called once per frame
    void Update()
    {
        HandleClick();
        Move();
        UpdateEffect();
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
        Debug.Log("Attack");
        agent.SetDestination(transform.position);
    }

    private void SelectTarget(GameObject newTarget, GameObject effectPrefab)
    {
        target = newTarget;

        if (activeSelectEffect != null)
        {
            Destroy(activeSelectEffect);
        }

        activeSelectEffect = Instantiate(effectPrefab, target.transform.position, Quaternion.identity);
    }

    private void UpdateEffect()
    {
        if (activeSelectEffect != null)
        {
            //update effect position with Y = 0.1f
            if (activeSelectEffect != null)
            {
                activeSelectEffect.transform.position = new Vector3(target.transform.position.x, 0.1f, target.transform.position.z);
            }
        }
    }
}
