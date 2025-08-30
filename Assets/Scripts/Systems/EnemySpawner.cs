using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    private float spawnRangeX = 9f;
    private float spawnPositionY = 16f;

    [SerializeField] private List<Wave> waves;
    private int currentWaveIndex = 0;
    public int lastWaveIndex = -1;
    private float spawnTime;
    private bool isWaveActive = false;

    // Wave generation parameters
    [Header("Wave Generation Settings")]
    [SerializeField, Range(1.0f, 2.0f), Tooltip("Exponential growth factor for score threshold calculation")] private float scoreThresholdBaseMultiplier = 1.1f;
    [SerializeField, Range(0, 10), Tooltip("Linear growth added to score threshold per wave")] private int scoreThresholdLinearGrowth = 4;
    [SerializeField, Range(0.5f, 1.0f), Tooltip("Multiplier for spawn interval (lower = faster spawning)")] private float spawnIntervalMultiplier = 0.8f;
    [SerializeField, Range(0.5f, 1.0f), Tooltip("Multiplier for spawn cooldown (lower = faster initial spawn)")] private float spawnCooldownMultiplier = 0.8f;
    [SerializeField, Range(1.0f, 2.0f), Tooltip("Difficulty multiplier applied to enemies each wave")] private float difficultyMultiplier = 1.2f;
    [SerializeField, Range(0.0f, 0.1f), Tooltip("Percentage increase in shooting enemy spawn rate per wave")] private float shootingEnemySpawnRateIncrease = 0.05f;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("Maximum shooting enemy spawn rate (as percentage)")] private float maxShootingEnemySpawnRate = 0.5f;
    [SerializeField, Range(0.1f, 1.0f), Tooltip("Minimum time between enemy spawns")] private float minSpawnInterval = 0.25f;
    [SerializeField, Range(0.1f, 2.0f), Tooltip("Minimum initial spawn delay for new waves")] private float minSpawnCooldown = 0.5f;

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
        
        CheckWaveThreshold();
    }
    
    
    private void CheckWaveThreshold()
    {
        if ( Player.Instance.CheckScore >= GetCurrentWaveScoreThreshold())
        {
            LevelUpSystem.Instance.ShowLevelUpOptions();
            AdvanceToNextWave();
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
                // Calculate exponentially increasing score threshold: base + linear growth, scaled by multiplier per wave
                scoreThreshold = Mathf.RoundToInt((lastWave.scoreThreshold + currentWaveIndex * scoreThresholdLinearGrowth) * Mathf.Pow(scoreThresholdBaseMultiplier, currentWaveIndex)),

                // Faster spawn interval, with minimum limit
                spawnInterval = Mathf.Max(lastWave.spawnInterval * spawnIntervalMultiplier, minSpawnInterval),

                // Faster spawn cooldown, with minimum limit
                spawnCooldown = Mathf.Max(lastWave.spawnCooldown * spawnCooldownMultiplier, minSpawnCooldown),

                // More difficult enemies
                difficultyMultiplier = lastWave.difficultyMultiplier * difficultyMultiplier,

                // Increase shooting enemy spawn rate, capped at maximum
                shootingEnemySpawnRate = Mathf.Min(lastWave.shootingEnemySpawnRate + shootingEnemySpawnRateIncrease, maxShootingEnemySpawnRate)
            };

            waves.Add(newWave);
            Debug.Log($"Generated new wave {waves.Count} with increased difficulty");
        }

    private void SpawnEnemy()
    {
        float currentSpawnRate = waves[currentWaveIndex].shootingEnemySpawnRate;
        bool isShootingEnemy = Random.value < currentSpawnRate;
        Vector2 spawnPosition = new(Random.Range(-spawnRangeX, spawnRangeX), spawnPositionY);

        if (isShootingEnemy) EnemyPool.Instance.GetShootingEnemy(spawnPosition);
        else EnemyPool.Instance.GetEnemy(spawnPosition);
    }

    public void AdvanceToNextWave()
    {
        lastWaveIndex = currentWaveIndex;
        currentWaveIndex++;
        StartWave();
    }

    public int GetPrevWaveScoreThreshold () => currentWaveIndex == 0 ? 0 : waves[currentWaveIndex - 1].scoreThreshold; 
    public int GetCurrentWaveScoreThreshold() => waves[currentWaveIndex].scoreThreshold;
    public int GetCurrentWaveIndex() => currentWaveIndex;

    public float GetCurrentWaveDifficultyMultiplier()
    {
        if (waves == null || waves.Count == 0) return 1f;
        if (currentWaveIndex >= waves.Count) return waves[waves.Count - 1].difficultyMultiplier;
        return waves[currentWaveIndex].difficultyMultiplier;
    }
}
