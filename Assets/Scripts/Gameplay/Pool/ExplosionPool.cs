using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ExplosionPool : MonoBehaviour
{
    public static ExplosionPool Instance;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxPoolSize = 20;

    private ObjectPool<GameObject> explosionPool;

    private void Awake()
    {
        Instance = this;

        explosionPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(explosionPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: poolSize,
            maxSize: maxPoolSize
        );
    }

    public void PlayExplosion(Vector3 position, Quaternion rotation)
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = explosionPool.Get();
            explosion.transform.SetPositionAndRotation(position, rotation);
            StartCoroutine(ReturnToPoolAfterDelay(explosion, 1f));
        }

        PlayerUI.Instance.EnemyDieSFX();
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionPool.Release(obj);
    }
}
