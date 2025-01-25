using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 3f;
    [SerializeField] private float difficultyIncreaseRate = 30f;
    [SerializeField] private float spawnRadius = 10f;

    private float nextSpawnTime;
    private float nextDifficultyIncrease;
    private Transform player;
    private bool isGameActive = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            isGameActive = false;
            return;
        }
        nextSpawnTime = Time.time + spawnRate;
        nextDifficultyIncrease = Time.time + difficultyIncreaseRate;
    }

    private void Update()
    {
        if (!isGameActive || player == null)
        {
            isGameActive = false;
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
        }

        if (Time.time >= nextDifficultyIncrease)
        {
            IncreaseDifficulty();
        }
    }

    private void SpawnEnemy()
    {
        if (player == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + randomDirection * spawnRadius;

        ObjectPool.Instance.SpawnFromPool("Enemy", spawnPosition, Quaternion.identity);
        nextSpawnTime = Time.time + spawnRate;
    }

    private void IncreaseDifficulty()
    {
        spawnRate = Mathf.Max(spawnRate * 0.9f, 0.5f);
        nextDifficultyIncrease = Time.time + difficultyIncreaseRate;
    }
}