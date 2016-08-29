using LevelGenerator2D;
using UnityEngine;

public class HealthPotionRune : RedRune
{
    /// <summary>
    /// Listens for enemy deaths so it knows when to spawn potions
    /// </summary>
    /// <param name="equipment">Equipment this rune is attached to</param>
    public override void Activate(Equipment equipment)
    {
        Enemy.OnDeath += OnEnemyDeath;
    }

    /// <summary>
    /// Stops listening for enemy deaths
    /// </summary>
    /// <param name="equipment">Equipment this rune is detached from</param>
    public override void Deactivate(Equipment equipment)
    {
        Enemy.OnDeath -= OnEnemyDeath;
    }

    /// <summary>
    /// Adds a health potion spawner to the enemy
    /// </summary>
    /// <param name="enemy">Enemy to add the spawner to</param>
    private void OnEnemyDeath(Enemy enemy)
    {
        var healthPotionPrefab = Resources.Load<HealthPotion>(FilePaths.HealthPotion) as HealthPotion;
        var spawner = enemy.gameObject.AddComponent<Spawner>();
        spawner.AddSpawnable(healthPotionPrefab.gameObject);
        spawner.SetSpawnChance(GetSpecialValue());
        Spawner.OnSpawn += OnPotionSpawn;
    }

    /// <summary>
    /// Adjusts the potion's effectiveness
    /// </summary>
    /// <param name="spawns"></param>
    private void OnPotionSpawn(GameObject[] spawns)
    {
        foreach (var spawn in spawns)
        {
            var healthPotion = spawn.GetComponent<HealthPotion>();
            if (healthPotion)
            {
                healthPotion.SetHealthRestore(healthPotion.GetHealthRestore() + GetBaseValue());
            }
        }
    }
}