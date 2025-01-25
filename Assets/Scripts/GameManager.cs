using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float difficultyScaling = 1.1f;

    private List<Character> characters = new List<Character>();
    private List<Enemy> activeEnemies = new List<Enemy>();
    private int score;
    private float gameTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeCharacters();
        StartGame();
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        UpdateDifficulty();
    }

    private void InitializeCharacters()
    {
        var warrior = FindFirstObjectByType<Warrior>();
        var mage = FindFirstObjectByType<Mage>();

        if (warrior != null) AddCharacter(warrior);
        if (mage != null) AddCharacter(mage);
    }

    public void AddCharacter(Character character)
    {
        if (character != null && !characters.Contains(character))
        {
            characters.Add(character);
        }
    }

    public void RemoveCharacter(Character character)
    {
        characters.Remove(character);
        CheckGameOver();
    }

    public void AddEnemy(Enemy enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

    private void StartGame()
    {
        score = 0;
        gameTime = 0;

        foreach (var character in characters)
        {
            if (character != null && spawnPoint != null)
            {
                character.transform.position = spawnPoint.position;
            }
        }
    }

    private void UpdateDifficulty()
    {
        // Increase difficulty based on game time
        float difficulty = 1 + (gameTime / 60f) * difficultyScaling;

        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                // Apply difficulty scaling to enemies
                enemy.SetDifficultyMultiplier(difficulty);
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    private void CheckGameOver()
    {
        if (characters.Count == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log($"Game Over! Score: {score}, Time Survived: {Mathf.Floor(gameTime)}s");
        // Add game over logic here
    }
}