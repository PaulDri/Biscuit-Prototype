using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    
    // Health bar UI
    [SerializeField] private Image healthBarImage;
    
    // Optional: Different health values for different enemy types
    [SerializeField] private bool isShootingEnemy = false;
    
    private void OnEnable()
    {
        // Reset health when enemy is spawned/reused
        
        // Optional: Adjust health based on wave difficulty only once per wave
        if (EnemySpawner.Instance != null)
        {
            int currentWaveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();
            if (currentWaveIndex != EnemySpawner.Instance.lastWaveIndex)
            {
                float difficultyMultiplier = EnemySpawner.Instance.GetCurrentWaveDifficultyMultiplier();
                maxHealth = Mathf.RoundToInt(maxHealth * difficultyMultiplier);
            }
            currentHealth = maxHealth;
        }
        
        // Update health bar
        UpdateHealthBar();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Update health bar
        UpdateHealthBar();
        
        ExplosionPool.Instance.PlayExplosion(transform.position, transform.rotation);

        // Optional: Play damage sound/effect here
        // AudioManager.Instance.PlaySFX(enemyHitSFX);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // Play explosion effect
        ExplosionPool.Instance.PlayExplosion(transform.position, transform.rotation);
        
        // Play death sound
        PlayerUI.Instance.EnemyDieSFX();
        
        // Award score based on enemy type and wave difficulty
        int scoreIncrement = isShootingEnemy ? 2 : 1;
        
        if (EnemySpawner.Instance != null)
        {
            float multiplier = EnemySpawner.Instance.GetCurrentWaveDifficultyMultiplier();
            int waveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();
            float exponentialFactor = Mathf.Pow(1.05f, waveIndex);
            scoreIncrement = Mathf.RoundToInt(scoreIncrement * multiplier * exponentialFactor);
        }
        
        Player.Instance.CheckScore += scoreIncrement;
        
        // Return enemy to pool
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
    
    // Optional: Getter for current health (useful for UI or debugging)
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    // Optional: Getter for max health
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    // Optional: Method to set enemy type (shooting vs regular)
    // public void SetEnemyType(bool shootingEnemy)
    // {
    //     isShootingEnemy = shootingEnemy;
    //     // Adjust health based on enemy type if needed
    //     if (isShootingEnemy)
    //     {
    //         maxHealth = 15; // Shooting enemies have more health
    //     }
    // }
    
    // Update health bar fill amount
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            healthBarImage.fillAmount = healthPercentage;
            
            // Optional: Change color based on health
            if (healthPercentage > 0.6f)
                healthBarImage.color = Color.green;
            else if (healthPercentage > 0.3f)
                healthBarImage.color = Color.yellow;
            else
                healthBarImage.color = Color.red;
        }
    }
}
