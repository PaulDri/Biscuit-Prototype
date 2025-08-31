using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField, Range(0, 1.0f)] private float healthDropChance = 0.25f;
    private int currentHealth;
    
    // Health bar UI
    private Image healthBarImage;

    private void Awake()
    {
        healthBarImage = transform.Find("Image").GetComponentInChildren<Image>();
    }
    
    private void OnEnable()
    {
        // Reset health when enemy is spawned/reused
        currentHealth = maxHealth;

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

        // Award score based on enemy's score property and wave difficulty
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
        int baseScore = enemyMovement != null ? enemyMovement.GetEnemyScore() : 10;

        int scoreIncrement = baseScore;
        if (EnemySpawner.Instance != null)
        {
            float multiplier = EnemySpawner.Instance.GetCurrentWaveDifficultyMultiplier();
            int waveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();
            float exponentialFactor = Mathf.Pow(1.05f, waveIndex);
            scoreIncrement = Mathf.RoundToInt(baseScore * multiplier * exponentialFactor);
        }

        Player.Instance.CheckScore += scoreIncrement;

        // Drop health pickup with chance
        if (Random.value < healthDropChance)
        {
            GameObject healthPickup = HealthPickupPool.Instance.GetHealthPickup();
            if (healthPickup != null)
            {
                healthPickup.transform.position = transform.position;
                healthPickup.SetActive(true);
            }
        }

        // Return enemy to pool
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
    
    // Update health bar fill amount
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.DOFillAmount((float)currentHealth / maxHealth, 0.3f)
                .SetEase(Ease.OutQuint);
        }
    }
}
