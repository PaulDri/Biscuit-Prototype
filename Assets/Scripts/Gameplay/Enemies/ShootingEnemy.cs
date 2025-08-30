using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    [SerializeField] private float shootingInterval = 2f;
    private float shootingTimer;

    void Start()
    {
        shootingTimer = shootingInterval;
    }

    void Update()
    {
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0)
        {
            Shoot();
            shootingTimer = shootingInterval;
        }
    }

    private void Shoot()
    {
        GameObject bullet = BulletPool.Instance.GetEnemyBullet();
        if (bullet != null) bullet.transform.position = transform.position;
    }
}
