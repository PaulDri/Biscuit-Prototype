using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D enemyRb;
    [SerializeField] private float enemyMoveSpeed = 1.0f;
    
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (enemyRb == null) enemyRb = GetComponent<Rigidbody2D>();
        enemyRb.velocity = Vector2.down * enemyMoveSpeed;
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
    
    public void KillEnemy()
    {
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(10);
            EnemyPool.Instance.ReturnEnemy(gameObject);
        }
    }
}
