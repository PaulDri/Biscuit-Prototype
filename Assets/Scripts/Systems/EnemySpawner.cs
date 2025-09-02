using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    private float spawnRangeX = 9f;
    private float spawnPositionY = 20f;

    [SerializeField] private List<Wave> waves;
    private int currentWaveIndex = 0;
    public int lastWaveIndex = -1;
    private float spawnTime;
    private bool isWaveActive = false;
    private bool isBossWave = false;
    [SerializeField] private GameObject bossPrefab;
    private GameObject currentBoss;

    // Wave generation parameters
    [Header("Wave Generation Settings")]
    [SerializeField, Range(0.5f, 1.0f), Tooltip("Multiplier for spawn interval (lower = faster spawning)")] private float spawnIntervalMultiplier = 0.8f;
    [SerializeField, Range(0.5f, 1.0f), Tooltip("Multiplier for spawn cooldown (lower = faster initial spawn)")] private float spawnCooldownMultiplier = 0.8f;
    [SerializeField, Range(1.0f, 2.0f), Tooltip("Difficulty multiplier applied to enemies each wave")] private float difficultyMultiplier = 1.05f;
    [SerializeField, Range(0.0f, 0.1f), Tooltip("Percentage increase in shooting enemy spawn rate per wave")] private float shootingEnemySpawnRateIncrease = 0.05f;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("Maximum shooting enemy spawn rate (as percentage)")] private float maxShootingEnemySpawnRate = 0.5f;
    [SerializeField, Range(0.1f, 1.0f), Tooltip("Minimum time between enemy spawns")] private float minSpawnInterval = 0.75f;
    [SerializeField, Range(0.1f, 2.0f), Tooltip("Minimum initial spawn delay for new waves")] private float minSpawnCooldown = 0.75f;

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

    private float waveTimer = 60f;
    private bool waveTimerExpired = false;

    void Update()
    {
        if (isBossWave) return; // Don't spawn enemies during boss wave

        if (!isWaveActive) return;

        if (!waveTimerExpired)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                waveTimerExpired = true;
                spawnTime = 0f;
            }
        }

        if (spawnTime > 0 && !waveTimerExpired) spawnTime -= Time.deltaTime;

        if (spawnTime <= 0 && !waveTimerExpired)
        {
            SpawnEnemy();
            spawnTime = waves[currentWaveIndex].spawnInterval;
        }

        CheckWaveThreshold();
    }


    private void CheckWaveThreshold()
    {
        if (isBossWave) return; // Don't check during boss wave

        if (waveTimerExpired)
        {
            // Check if all enemies are cleared
            if (AreAllEnemiesCleared())
            {
                LevelUpSystem.Instance.ShowLevelUpOptions();
                AdvanceToNextWave();
            }
        }
    }


    private void StartWave()
    {
        if (currentWaveIndex >= waves.Count) GenerateNewWave();

        waveTimer = 60f;
        waveTimerExpired = false;

        // Check if this is a boss wave (every 15 waves: 15, 30, 45, etc.)
        isBossWave = (currentWaveIndex + 1) % 10 == 0;

        if (isBossWave)
        {
            SpawnBoss();
            isWaveActive = false; // Disable regular enemy spawning
            Debug.Log($"Starting Boss Wave {currentWaveIndex + 1}");
        }
        else
        {
            isWaveActive = true;
            spawnTime = waves[currentWaveIndex].spawnCooldown;
            Debug.Log($"Starting Wave {currentWaveIndex + 1} - Score Threshold: {waves[currentWaveIndex].scoreThreshold}, " +
                     $"Spawn Interval: {waves[currentWaveIndex].spawnInterval:F2}, " +
                     $"Difficulty: {waves[currentWaveIndex].difficultyMultiplier:F2}");
        }
    }

        private void GenerateNewWave()
        {
            // Get the last wave to base the new wave on
            Wave lastWave = waves[waves.Count - 1];

            // Create a new wave with increased difficulty
            Wave newWave = new Wave
            {
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

    private void SpawnBoss()
    {
        if (bossPrefab != null)
        {
            AudioManager.Instance.PlayMusic(PlayerUI.Instance.bossBGM);
            currentBoss = Instantiate(bossPrefab, new Vector2(0, spawnPositionY), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Boss prefab not assigned in EnemySpawner!");
        }
    }

    public void OnBossDefeated()
    {
        if (isBossWave)
        {
            AudioManager.Instance.PlayMusic(PlayerUI.Instance.bossDie);
            isBossWave = false;
            LevelUpSystem.Instance.ShowLevelUpOptions();
            AdvanceToNextWave();
            AudioManager.Instance.PlayMusic(PlayerUI.Instance.gameBG);
        }
    }

    public void AdvanceToNextWave()
    {
        lastWaveIndex = currentWaveIndex;
        currentWaveIndex++;
        StartWave();
        PlayerUI.Instance.UpdateWaveDisplay();
    }

    private bool AreAllEnemiesCleared()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
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
