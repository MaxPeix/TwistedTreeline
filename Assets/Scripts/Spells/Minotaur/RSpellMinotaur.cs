using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSpellMinotaur : MonoBehaviour
{
    private float cooldown = 50f;
    private float lastAttackTime = 0f;
    private Animator anim;
    private LifeSystem lifeSystem;
    public GameObject summonPrefab;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            CastRSpell();
        }
    }

    public void CastRSpell()
    {
        if (Time.time - lastAttackTime >= cooldown)
        {
            lastAttackTime = Time.time; // Update the last attack time

            // Get the mouse position in world space
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.transform.position.y - transform.position.y; // Adjust z to match the plane of the game
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Calculate the direction from the Minotaur to the mouse position
            Vector3 direction = (worldMousePosition - transform.position).normalized;

            // Set the spawn position to be in front of the Minotaur, towards the mouse
            Vector3 spawnPosition = transform.position + direction * 8;

            // Adjust Y position to 0
            spawnPosition.y = 0;

            if (summonPrefab != null)
            {
                // Calculate the Y rotation angle based on the direction
                float angleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // Instantiate the summon prefab with the calculated rotation
                GameObject summon = Instantiate(
                    summonPrefab,
                    spawnPosition,
                    Quaternion.Euler(180, angleY, 0) // Adjust rotation to face the direction
                );

                // Destroy the summon after 3 seconds
                Destroy(summon, 3f);
            }
            else
            {
                Debug.LogWarning("Summon Prefab is not assigned!");
            }
        }
    }


}
