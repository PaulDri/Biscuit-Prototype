[System.Serializable]
public class Wave
{
    public int scoreThreshold;
    public float spawnInterval;
    public float spawnCooldown;
    public float difficultyMultiplier = 1.0f;
    public float shootingEnemySpawnRate = 0.1f; // Chance to spawn shooting enemy (0.0 to 1.0)
}
