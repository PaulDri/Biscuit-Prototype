using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private Rigidbody2D bulletRb;
    [SerializeField] private float bulletSpeed;

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
        bulletRb.velocity = Vector2.up * bulletSpeed;
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
            Player.Instance.CheckScore++;
        }
    }
}
