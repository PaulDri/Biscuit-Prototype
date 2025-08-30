using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D enemyRb;
    //[SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float enemyMoveSpeed = 1.0f;

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

    // Tanggalin ung mga ship/kalaban pag lumabas sa camera
    private void CheckScreenBounds()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.y < -0.1f)  EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ExplosionPool.Instance.PlayExplosion(transform.position, transform.rotation);
            Player.Instance.TakeDamage(10);
            EnemyPool.Instance.ReturnEnemy(gameObject);
        }
    }

    //public void PlayExplosion()
    //{

    //    if (explosionPrefab != null)
    //    {
    //        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
    //        Destroy(explosion, 1f);
    //    }

    //    PlayerUI.Instance.EnemyDieSFX();
    //}
}
