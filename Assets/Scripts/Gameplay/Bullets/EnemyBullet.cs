using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D bulletRb;
    [SerializeField] private float bulletSpeed = 10f;
    public Vector2 direction = Vector2.down;

    void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        direction = Vector2.down;
    }

    void Update()
    {
        BulletSpawn();
        CheckScreenBounds();
    }

    private void BulletSpawn()
    {
        bulletRb.velocity = direction * bulletSpeed;
    }


    private void CheckScreenBounds()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.y > 1.1f || screenPoint.y < -0.1f || screenPoint.x > 1.1f || screenPoint.x < -0.1f)
            BulletPool.Instance.ReturnBullet(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ExplosionPool.Instance.PlayExplosion(transform.position, transform.rotation);
            BulletPool.Instance.ReturnBullet(gameObject);
            Player.Instance.TakeDamage(10);
        }
    }
}
