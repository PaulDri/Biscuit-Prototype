using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;
    [SerializeField] private GameObject playerBulletPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;
    
    private IObjectPool<GameObject> playerBulletPool;
    private IObjectPool<GameObject> enemyBulletPool;

    private void Awake()
    {
        Instance = this;
        
        // Player bullet pool
        playerBulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(playerBulletPrefab),
            actionOnGet: bullet => bullet.SetActive(true),
            actionOnRelease: bullet => bullet.SetActive(false),
            actionOnDestroy: bullet => Destroy(bullet),
            collectionCheck: false,
            defaultCapacity: 50,
            maxSize: 100
        );
        
        // Enemy bullet pool
        enemyBulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(enemyBulletPrefab),
            actionOnGet: bullet => bullet.SetActive(true),
            actionOnRelease: bullet => bullet.SetActive(false),
            actionOnDestroy: bullet => Destroy(bullet),
            collectionCheck: false,
            defaultCapacity: 50,
            maxSize: 100
        );
    }

    public GameObject GetPlayerBullet() => playerBulletPool.Get();
    
    public GameObject GetEnemyBullet() => enemyBulletPool.Get();

    public void ReturnBullet(GameObject bullet)
    {
        if (bullet.GetComponent<PlayerBullet>() != null) playerBulletPool.Release(bullet);
        else if (bullet.GetComponent<EnemyBullet>() != null) enemyBulletPool.Release(bullet);
    }
}
