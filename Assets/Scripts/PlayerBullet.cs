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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BulletPool.Instance.ReturnBullet(gameObject);
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            Debug.Log("Hit!");
            BulletPool.Instance.ReturnBullet(gameObject);
            BulletPool.Instance.ReturnBullet(collision.gameObject);
        } 
    }
}
