using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D bulletRb;

    void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        BulletSpawn();
        CheckScreenBounds();
    }

    private void BulletSpawn() 
    {
        bulletRb.velocity = Vector2.up * Player.Instance.GetBulletSpeed();
    }

    private void CheckScreenBounds()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.y > 1.1f || screenPoint.y < -0.1f || screenPoint.x > 1.1f || screenPoint.x < -0.1f)
            BulletPool.Instance.ReturnBullet(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BulletPool.Instance.ReturnBullet(gameObject);
            
            // Deal damage to enemy instead of immediately destroying it
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Player.Instance.GetBulletDamage()); // Use player's bullet damage
            }
            else
            {
                // Fallback: if enemy doesn't have health component, destroy it immediately
                EnemyPool.Instance.ReturnEnemy(collision.gameObject);
                
                int scoreIncrement = 1;
                if (EnemySpawner.Instance != null)
                {
                    float multiplier = EnemySpawner.Instance.GetCurrentWaveDifficultyMultiplier();
                    int waveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();
                    float exponentialFactor = Mathf.Pow(1.05f, waveIndex);
                    scoreIncrement = Mathf.RoundToInt(multiplier * exponentialFactor);
                }

                Player.Instance.CheckScore += scoreIncrement;
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            ExplosionPool.Instance.PlayExplosion(transform.position, transform.rotation);
            BulletPool.Instance.ReturnBullet(gameObject);
            BulletPool.Instance.ReturnBullet(collision.gameObject);
        } 
    }
}
