using System.Collections;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    // Basic stats
    public float HP { get; private set; }
    public float MaxHP { get; private set; }
    public float Armor { get; private set; }
    public float MagicArmor { get; private set; }
    public float AttackDamage { get; private set; }
    public float MagicDamage { get; private set; }

    public float regenHp = 0f;  // Health regeneration per second
    private float regenInterval = 0.1f;  // Regeneration interval (0.1f seconds)

    private float attackCooldown = 1f;
    private float lastAttackTime;

    // Initialize stats (can be called from other scripts)
    public void Initialize(float maxHP, float armor, float magicArmor, float attackDamage, float magicDamage)
    {
        MaxHP = maxHP;
        HP = maxHP;
        Armor = armor;
        MagicArmor = magicArmor;
        AttackDamage = attackDamage;
        MagicDamage = magicDamage;
        lastAttackTime = Time.time;

        // Start health regeneration
        StartCoroutine(RegenerateHealth());
    }

    // Getter and Setter for HP
    public void SetHP(float value)
    {
        HP = Mathf.Clamp(value, 0, MaxHP);
        if (HP <= 0)
        {
            DestroyObject(); // Destroy the object when HP is zero or less
        }
    }

    public float GetHP()
    {
        return HP;
    }

    // Getter and Setter for Armor
    public void SetArmor(float value)
    {
        Armor = Mathf.Max(0, value);
    }

    public float GetArmor()
    {
        return Armor;
    }

    // Getter and Setter for Magic Armor
    public void SetMagicArmor(float value)
    {
        MagicArmor = Mathf.Max(0, value);
    }

    public float GetMagicArmor()
    {
        return MagicArmor;
    }

    // Getter and Setter for Attack Damage
    public void SetAttackDamage(float value)
    {
        AttackDamage = Mathf.Max(0, value);
    }

    public float GetAttackDamage()
    {
        return AttackDamage;
    }

    // Getter and Setter for Magic Damage
    public void SetMagicDamage(float value)
    {
        MagicDamage = Mathf.Max(0, value);
    }

    public float GetMagicDamage()
    {
        return MagicDamage;
    }

    public bool CanAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            return true;
        }
        return false;
    }

    // Take physical damage
    public void TakeDamage(float damage)
    {
        // Calculate effective damage after armor reduction but armor reduces damage by a percentage
        float effectiveDamage = Mathf.Max(0, damage - (damage * Armor / 100));
        SetHP(HP - effectiveDamage);
    }

    // Take magical damage
    public void TakeMagicDamage(float damage)
    {
        // Calculate effective damage after magic armor reduction but magic armor reduces damage by a percentage
        float effectiveDamage = Mathf.Max(0, damage - (damage * MagicArmor / 100));
        SetHP(HP - effectiveDamage);
    }

    // Check if the entity is alive
    public bool IsAlive()
    {
        return HP > 0;
    }

    // Destroy the GameObject
    private void DestroyObject()
    {
        Destroy(gameObject); // Destroys the GameObject when HP is 0 or less
    }

    // Coroutine to handle health regeneration
    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            if (HP < MaxHP)
            {
                // Regenerate health
                SetHP(HP + regenHp * regenInterval);
            }
            yield return new WaitForSeconds(regenInterval);
        }
    }
}
