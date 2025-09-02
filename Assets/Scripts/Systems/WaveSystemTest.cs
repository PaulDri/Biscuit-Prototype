using UnityEngine;

public class WaveSystemTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool enableTesting = true;
    [SerializeField] private bool showWaveInfo = true;
    [SerializeField] private KeyCode clearEnemiesKey = KeyCode.C;
    [SerializeField] private KeyCode skipWaveKey = KeyCode.S;

    private int lastWaveIndex = 0;

    void Update()
    {
        if (!enableTesting) return;

        if (showWaveInfo && EnemySpawner.Instance != null && EnemySpawner.Instance.GetCurrentWaveIndex() != lastWaveIndex)
        {
            lastWaveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();
            Debug.Log($"Wave changed to: {lastWaveIndex + 1}");
        }

        if (Input.GetKeyDown(clearEnemiesKey)) ClearAllEnemies();
        if (Input.GetKeyDown(skipWaveKey)) SkipWave();
    }

    private void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) Destroy(enemy);
        Debug.Log($"Test: Cleared {enemies.Length} enemies");
    }

    private void SkipWave()
    {
        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.AdvanceToNextWave();
            Debug.Log("Test: Skipped to next wave");
        }
    }
}
