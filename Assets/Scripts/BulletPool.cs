using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;
    [SerializeField] private GameObject bulletPrefab;
    private IObjectPool<GameObject> bulletPool;

    private void Awake()
    {
        Instance = this;
        bulletPool = new ObjectPool<GameObject>(
        createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: bullet => bullet.SetActive(true),
            actionOnRelease: bullet => bullet.SetActive(false),
            actionOnDestroy: bullet => Destroy(bullet),
            collectionCheck: false,
            defaultCapacity: 50,
            maxSize: 100
        );
    }

    public GameObject GetBullet()
    {
        return bulletPool.Get();
    }

    public void ReturnBullet(GameObject bullet)
    {
        bulletPool.Release(bullet);
    }
}
