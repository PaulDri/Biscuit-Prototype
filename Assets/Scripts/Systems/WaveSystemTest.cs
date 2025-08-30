using UnityEngine;

public class WaveSystemTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool enableTesting = true;
    [SerializeField] private float testScoreIncrement = 5f;
    [SerializeField] private float testInterval = 2f;
    [SerializeField] private bool showWaveInfo = true;
    
    private float testTimer;
    private int lastWaveIndex = 0;

    void Update()
    {
        if (!enableTesting) return;
        
        testTimer += Time.deltaTime;
        if (testTimer >= testInterval)
        {
            testTimer = 0;
            TestScoreIncrease();
        }

        // Show wave info when it changes
        if (showWaveInfo && EnemySpawner.Instance != null && EnemySpawner.Instance.GetCurrentWaveIndex() != lastWaveIndex)
        {
            lastWaveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();
            Debug.Log($"Wave changed to: {lastWaveIndex + 1}");
        }
    }

    private void TestScoreIncrease()
    {
        if (Player.Instance != null)
        {
            Player.Instance.CheckScore += testScoreIncrement;
            Debug.Log($"Test: Increased score to {Player.Instance.CheckScore}");
        }
    }
}
