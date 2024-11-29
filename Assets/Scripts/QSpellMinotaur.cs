using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSpellMinotaur : MonoBehaviour
{
    private int damage = 150;
    private float range = 2f;
    private float cooldown = 6f;
    private float lastAttackTime = 0f;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lastAttackTime = Time.time - cooldown; // Set the last attack time to the past to allow the first attack
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("CD Time: " + (Time.time - lastAttackTime));
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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //check if im PlayerRed i can only attack PlayerBlue and minionBlue and vice versa
                if (hit.collider.CompareTag("PlayerBlue") && this.CompareTag("PlayerRed")
                || hit.collider.CompareTag("PlayerRed") && this.CompareTag("PlayerBlue")
                || hit.collider.CompareTag("MinionBlue") && this.CompareTag("PlayerRed")
                || hit.collider.CompareTag("MinionRed") && this.CompareTag("PlayerBlue"))
                {
                    Debug.Log("Attacking: " + hit.collider.gameObject.name);
                    GameObject target = hit.collider.gameObject;
                    LifeSystem targetLifeSystem = target.GetComponent<LifeSystem>();

                    anim.SetBool("isKicking", true);

                    if (targetLifeSystem != null)
                    {
                        //damage is equal to damage + 50% of player AD from lifesystem
                        targetLifeSystem.TakeDamage(damage + (int)(targetLifeSystem.GetAttackDamage() * 0.5f));
                        //add knockback effect
                        target.GetComponent<Rigidbody>().AddForce(transform.forward * 500f);
                        lastAttackTime = Time.time;
                    }
                    StartCoroutine(ResetAnimation());
                }
            }
        }
    }

    IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isKicking", false);
    }
}
