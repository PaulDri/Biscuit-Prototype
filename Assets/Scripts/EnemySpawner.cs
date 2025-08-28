using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    private float spawnRangeX = 9f;
    private float spawnPositionY = 16f;

    [SerializeField] private List<Wave> waves;
    private int currentWaveIndex = 0;
    private float spawnTime;
    private bool isWaveActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        if (waves == null || waves.Count == 0)
        {
            Debug.LogError("No waves configured in EnemySpawner!");
            return;
        }

        StartWave();
    }

    void Update()
    {
        if (!isWaveActive) return;

        if (spawnTime > 0) spawnTime -= Time.deltaTime;

        if (spawnTime <= 0) 
        {
            SpawnEnemy();
            spawnTime = waves[currentWaveIndex].spawnInterval;
        }
    }

    private void StartWave()
    {
        // If we've reached the end of predefined waves, generate a new wave
        if (currentWaveIndex >= waves.Count)
        {
            GenerateNewWave();
        }

        isWaveActive = true;
        spawnTime = waves[currentWaveIndex].spawnCooldown;
        
        Debug.Log($"Starting Wave {currentWaveIndex + 1} - Score Threshold: {waves[currentWaveIndex].scoreThreshold}, " +
                 $"Spawn Interval: {waves[currentWaveIndex].spawnInterval:F2}, " +
                 $"Difficulty: {waves[currentWaveIndex].difficultyMultiplier:F2}");
    }

    private void GenerateNewWave()
    {
        // Get the last wave to base the new wave on
        Wave lastWave = waves[waves.Count - 1];
        
        // Create a new wave with increased difficulty
        Wave newWave = new Wave
        {
            // TODO: Subject to change
            scoreThreshold = Mathf.RoundToInt((lastWave.scoreThreshold + currentWaveIndex * 5) * Mathf.Pow(1.1f, currentWaveIndex)),
            spawnInterval = Mathf.Max(lastWave.spawnInterval * 0.9f, 0.5f), // 10% faster, min 0.5s
            spawnCooldown = Mathf.Max(lastWave.spawnCooldown * 0.9f, 1.0f), // 10% faster, min 1.0s
            difficultyMultiplier = lastWave.difficultyMultiplier * 1.1f // 10% more difficult
        };
        
        waves.Add(newWave);
        Debug.Log($"Generated new wave {waves.Count} with increased difficulty");
    }

    private void SpawnEnemy() 
    {
        Vector2 spawnPosition = new(Random.Range(-spawnRangeX, spawnRangeX), spawnPositionY);
        EnemyPool.Instance.GetEnemy(spawnPosition);
    }

    public void AdvanceToNextWave()
    {
        currentWaveIndex++;
        StartWave();
    }

    public int GetPrevWaveScoreThreshold () => currentWaveIndex == 0 ? 0 : waves[currentWaveIndex - 1].scoreThreshold; 
    public int GetCurrentWaveScoreThreshold() => waves[currentWaveIndex].scoreThreshold;
    public int GetCurrentWaveIndex() => currentWaveIndex;
}
