using System.Collections;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    [Header("Basic Stats")]
    public float MaxHP = 1000f;           // Set default maximum HP
    public float HP = 1000f;             // Set default current HP
    public float Armor = 30f;            // Default physical armor
    public float MagicArmor = 25f;       // Default magic resistance
    public float AttackDamage = 50f;     // Default attack damage
    public float MagicDamage = 0f;       // Default magic damage
    public float Speed = 3.5f;           // Movement speed
    public float AttackRange = 2f;       // Attack range
    public float AttackCooldown = 1f;    // Attack cooldown in seconds

    [Header("Health Regeneration")]
    public float regenHp = 0f;           // Health regeneration per second
    private float regenInterval = 0.1f;  // Interval for regeneration

    private float lastAttackTime;        // Timestamp of the last attack
    public float respawnTime = 8f;           // Respawn time for the entity
    public string EntityName;          // Name of the entity
    private ExpSystem expSystem;        // Reference to the ExpSystem component

    public HealthBar healthBar;


    private void Start()
    {
        GameObject gameEngine = GameObject.Find("GameEngine");
        if (gameEngine != null)
        {
            expSystem = gameEngine.GetComponent<ExpSystem>();
        }

        // Clamp HP to ensure it does not exceed MaxHP or fall below 0
        HP = Mathf.Clamp(HP, 0, MaxHP);

        // Start health regeneration
        StartCoroutine(RegenerateHealth());
    }

    // Getter and Setter for HP
    public void SetHP(float value)
    {
        HP = Mathf.Clamp(value, 0, MaxHP);
        if (HP <= 0)
        {
            // Determine the amount of experience to award based on the entity's tag
            int expAmount = 0;
            string team = null;

            if (CompareTag("MinionBlue") || CompareTag("PlayerBlue") || CompareTag("TowerBlue"))
            {
                team = "RedTeam"; // The opposing team gains experience
            }
            else if (CompareTag("MinionRed") || CompareTag("PlayerRed") || CompareTag("TowerRed"))
            {
                team = "BlueTeam";
            }

            if (CompareTag("MinionBlue") || CompareTag("MinionRed"))
            {
                expAmount = 100;
            }
            else if (CompareTag("PlayerBlue") || CompareTag("PlayerRed"))
            {
                expAmount = 500;
            }
            else if (CompareTag("TowerBlue") || CompareTag("TowerRed"))
            {
                expAmount = 1000;
            }

            // Award experience to the opposing team
            if (expSystem != null && team != null && expAmount > 0)
            {
                // Debug.Log("Awarding " + expAmount + " experience to " + team);
                expSystem.GainExp(team, expAmount);
            }

            // Destroy the GameObject if its not a Player
            if (!CompareTag("PlayerBlue") && !CompareTag("PlayerRed"))
            {
                DestroyObject();
            }
            else
            {
                // Respawn the player
                StartRespawn();
            }
        }
    }

    private void StartRespawn()
    {
        if (GameEngine.Instance != null)
        {
            GameEngine.Instance.StartCoroutine(GameEngine.Instance.RespawnCoroutine(this, respawnTime));
        }
        else
        {
            Debug.LogError("GameEngine instance is missing!");
        }
        gameObject.SetActive(false); // Deactivate the GameObject
    }

    public void FinishRespawn()
    {
        // Reset HP and enable the GameObject
        HP = MaxHP;
        if (healthBar != null)
        {
            healthBar.UpdateHealth(1);
        }
        else
        {
            Debug.LogWarning("HealthBar reference is missing!");
        }

        gameObject.SetActive(true);

        // Set the position of the player to their spawn point
        if (CompareTag("PlayerBlue"))
        {
            transform.position = new Vector3(54.5f, 0f, 0f);
        }
        else if (CompareTag("PlayerRed"))
        {
            transform.position = new Vector3(-53f, 0f, 0f);
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

    // Getter and Setter for Speed
    public void SetSpeed(float value)
    {
        Speed = Mathf.Max(0, value);
    }

    public float GetSpeed()
    {
        return Speed;
    }

    // Getter and Setter for Attack Range
    public void SetAttackRange(float value)
    {
        AttackRange = Mathf.Max(0, value);
    }

    public float GetAttackRange()
    {
        return AttackRange;
    }

    // Getter and Setter for Attack Cooldown
    public void SetAttackCooldown(float value)
    {
        AttackCooldown = Mathf.Max(0, value);
    }

    public float GetAttackCooldown()
    {
        return AttackCooldown;
    }

    public bool CanAttack()
    {
        if (Time.time - lastAttackTime >= AttackCooldown)
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
        healthBar.UpdateHealth(HP / MaxHP);
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
