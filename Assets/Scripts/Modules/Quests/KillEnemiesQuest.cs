using UnityEngine;

public class KillEnemiesQuest : Quest
{
    [Tooltip("Enemy type required to be killed")]
    [SerializeField]
    private Enemy enemyType;

    [Tooltip("Number of enemies that must be killed to complete quest")]
    [SerializeField]
    [Range(1, 10)]
    private int enemyKillCount = 1;

    void Update()
    {
        if (IsComplete())
        {
            return;
        }
        if (enemyKillCount <= 0)
        {
            Complete();
        }
    }

    void OnEnable()
    {
        Enemy.OnDeath += OnEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnDeath -= OnEnemyDeath;
    }

    public override bool IsQualified()
    {
        return true;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        if (enemy.GetType() == enemyType.GetType())
        {
            enemyKillCount--;
        }
    }
}