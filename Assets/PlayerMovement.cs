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

            //if ray hit object with tag "map"
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

            //if ray hit object with tag "enemy"
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Tower"))
            {
                target = hit.collider.gameObject;
                if (activeSelectEffect != null)
                {
                    Destroy(activeSelectEffect);
                }
                activeSelectEffect = Instantiate(selectTowerEffectPrefab, target.transform.position, Quaternion.identity);
            }

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Minions"))
            {
                target = hit.collider.gameObject;
                if (activeSelectEffect != null)
                {
                    Destroy(activeSelectEffect);
                }
                activeSelectEffect = Instantiate(selectMinionEffectPrefab, target.transform.position, Quaternion.identity);
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
        return Vector3.Distance(transform.position, target.transform.position) < range;
    }

    private void Attack()
    {
        Debug.Log("Attack");
        agent.SetDestination(transform.position);
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
