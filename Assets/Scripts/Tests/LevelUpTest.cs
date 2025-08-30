using UnityEngine;

public class LevelUpTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool enableTest = true;
    [SerializeField] private float testInterval = 5f;
    [SerializeField] private float scoreIncrement = 10f;
    
    private float timer;

    void Update()
    {
        if (!enableTest) return;
        
        timer += Time.deltaTime;
        if (timer >= testInterval)
        {
            timer = 0;
            TestLevelUp();
        }
    }

    private void TestLevelUp()
    {
        if (Player.Instance != null)
        {
            Player.Instance.CheckScore += scoreIncrement;
            Debug.Log($"Test: Increased score to {Player.Instance.CheckScore}");
            
            // Check if we should trigger level up
            if (EnemySpawner.Instance != null && 
                Player.Instance.CheckScore >= EnemySpawner.Instance.GetCurrentWaveScoreThreshold())
            {
                Debug.Log("Test: Score threshold reached, should trigger level up!");
            }
        }
    }
}
