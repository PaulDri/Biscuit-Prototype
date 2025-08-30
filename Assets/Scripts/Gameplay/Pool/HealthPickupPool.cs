using UnityEngine;
using UnityEngine.Pool;

public class HealthPickupPool : MonoBehaviour
{
    public static HealthPickupPool Instance;
    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxPoolSize = 20;

    private ObjectPool<GameObject> healthPickupPool;

    private void Awake()
    {
        Instance = this;

        healthPickupPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(healthPickupPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: poolSize,
            maxSize: maxPoolSize
        );
    }

    public GameObject GetHealthPickup()
    {
        return healthPickupPool.Get();
    }

    public void ReturnHealthPickup(GameObject pickup)
    {
        healthPickupPool.Release(pickup);
    }
}
