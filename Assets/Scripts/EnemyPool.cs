using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    [SerializeField] private GameObject[] enemyPrefabs;
    private IObjectPool<GameObject>[] enemyPools;

    private void Awake()
    {
        Instance = this;
        enemyPools = new IObjectPool<GameObject>[enemyPrefabs.Length];

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int index = i;
            enemyPools[i] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(enemyPrefabs[index]),
                actionOnGet: enemy => enemy.SetActive(true),
                actionOnRelease: enemy => enemy.SetActive(false),
                actionOnDestroy: enemy => Destroy(enemy),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 500
            );
        }
    }

    public GameObject GetEnemy(Vector2 position)
    {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = enemyPools[enemyIndex].Get();
        enemy.transform.SetPositionAndRotation(position, enemyPrefabs[enemyIndex].transform.rotation);
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemy.name.StartsWith(enemyPrefabs[i].name))
            {
                enemyPools[i].Release(enemy);
                return;
            }
        }     
    }
}
