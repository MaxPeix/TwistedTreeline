using System.Collections.Generic;
using UnityEngine;

public class ExpSystem : MonoBehaviour
{
    // Experience tracking
    public int blueTeamExp = 0;
    public int redTeamExp = 0;

    // Levels
    public int blueTeamLevel = 1;
    public int redTeamLevel = 1;

    // Experience thresholds for leveling up
    public int[] levelThresholds = { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };

    private void Update()
    {
        // For testing purposes: press keys to simulate gaining experience
        // if (Input.GetKeyDown(KeyCode.B)) GainExp("BlueTeam", 500); // Simulate PlayerRed for BlueTeam
        // if (Input.GetKeyDown(KeyCode.N)) GainExp("RedTeam", 500);  // Simulate PlayerRed for RedTeam
    }

    // Gain experience for a team
    public void GainExp(string team, int amount)
    {
        if (team == "BlueTeam")
        {
            blueTeamExp += amount;
            CheckLevelUp("BlueTeam");
        }
        else if (team == "RedTeam")
        {
            redTeamExp += amount;
            CheckLevelUp("RedTeam");
        }
    }

    // Check if the team levels up
    private void CheckLevelUp(string team)
    {
        if (team == "BlueTeam" && blueTeamLevel < levelThresholds.Length &&
            blueTeamExp >= levelThresholds[blueTeamLevel - 1])
        {
            blueTeamLevel++;
            ApplyLevelUp("BlueTeam");
        }
        else if (team == "RedTeam" && redTeamLevel < levelThresholds.Length &&
                 redTeamExp >= levelThresholds[redTeamLevel - 1])
        {
            redTeamLevel++;
            ApplyLevelUp("RedTeam");
        }
    }

    // Apply level-up effects to all entities in a team
    private void ApplyLevelUp(string team)
    {
        // Determine relevant tags for the team
        string[] tags = team == "BlueTeam" ? new string[] { "MinionBlue", "PlayerBlue" } : new string[] { "MinionRed", "PlayerRed" };

        // Find all relevant entities for the team
        List<GameObject> teamEntities = new List<GameObject>();
        foreach (string tag in tags)
        {
            teamEntities.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        // Apply level-up effects to all entities
        foreach (var entity in teamEntities)
        {
            LifeSystem lifeSystem = entity.GetComponent<LifeSystem>();
            if (lifeSystem != null)
            {
                // Boost stats based on the entity's name
                switch (lifeSystem.EntityName)
                {
                    case "Minion":
                        lifeSystem.MaxHP += lifeSystem.MaxHP * 0.1f;
                        lifeSystem.SetHP(lifeSystem.GetHP() + (lifeSystem.MaxHP * 0.1f)); // Heal for 10% max HP
                        lifeSystem.AttackDamage += lifeSystem.AttackDamage * 0.05f;
                        break;
                    case "Minotaur":
                        lifeSystem.MaxHP += lifeSystem.MaxHP * 0.15f;
                        lifeSystem.SetHP(lifeSystem.GetHP() + (lifeSystem.MaxHP * 0.15f)); // Heal for 15% max HP
                        lifeSystem.AttackDamage += lifeSystem.AttackDamage * 0.1f;
                        lifeSystem.respawnTime += 3f;
                        break;
                    default:
                        break;
                }
            }
        }

        Debug.Log(team + " has leveled up to level " + (team == "BlueTeam" ? blueTeamLevel : redTeamLevel));
    }
}
