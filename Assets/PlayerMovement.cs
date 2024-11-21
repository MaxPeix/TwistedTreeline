using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    private GameObject target;
    private float range = 5f;

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
            }

            //if ray hit object with tag "enemy"
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Tower"))
            {
                target = hit.collider.gameObject;
            }

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Minions"))
            {
                target = hit.collider.gameObject;
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
}
