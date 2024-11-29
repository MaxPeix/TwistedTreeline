using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSpellMinotaur : MonoBehaviour
{
    private float cooldown = 12f;
    private float lastAttackTime = 0f;
    private Animator anim;
    private LifeSystem lifeSystem;
    public GameObject healEffectPrefab;

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
        if (Input.GetKeyDown(KeyCode.W))
        {
            CastWSpell();
        }
    }

    // If the player presses the W key, the Minotaur heals and plays the healing effect
    public void CastWSpell()
    {
        if (Time.time - lastAttackTime >= cooldown)
        {
            // Heal the Minotaur for 30% max HP
            lifeSystem.SetHP(lifeSystem.GetHP() + (lifeSystem.MaxHP * 0.3f));

            // Instantiate heal effect at the Minotaur's position
            if (healEffectPrefab != null)
            {
                GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
                healEffect.transform.SetParent(transform); // Parent the effect to the Minotaur
            }
            else
            {
                Debug.LogWarning("Heal Effect Prefab is not assigned!");
            }

            // Update the last attack time
            lastAttackTime = Time.time;
        }
    }
}
