using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D enemyRb;
    [SerializeField] private float enemyMoveSpeed = 1.0f;
    [SerializeField] private int enemyScore = 10; // Base score for killing this enemy

    public int GetEnemyScore() => enemyScore;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (enemyRb == null) enemyRb = GetComponent<Rigidbody2D>();
        float speedMultiplier = 1f;

        if (EnemySpawner.Instance != null) speedMultiplier = EnemySpawner.Instance.GetCurrentWaveDifficultyMultiplier();
        enemyRb.velocity = enemyMoveSpeed * speedMultiplier * Vector2.down;
    }

    void Update()
    {
        CheckScreenBounds();
    }

    private void CheckScreenBounds()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.y < -0.1f)  EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ExplosionPool.Instance.PlayExplosion(transform.position, transform.rotation);
            Player.Instance.TakeDamage(10);
            
            // Enemy is destroyed on collision with player (no health system for player collision)
            // EnemyPool.Instance.ReturnEnemy(gameObject);
        }
    }
}
