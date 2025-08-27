using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefab;
    private float spawnRangeX = 9f;
    private float spawnPositionY = 16f;
    private float startDelay = 2f;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private float spawnCooldown = 1f;
    private float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = spawnCooldown;
        InvokeRepeating("SpawnEnemy", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTime > 0) 
        {
            spawnTime -= Time.deltaTime;
        }

        if (spawnTime <= 0) 
        {
            SpawnEnemy();
            spawnTime = spawnCooldown;
        }
    }

    private void SpawnEnemy() 
    {
        Vector2 spawnPosition = new Vector2(Random.Range(spawnRangeX, -spawnRangeX), spawnPositionY);
        int enemyIndex = Random.Range(0, enemyPrefab.Length);
        Instantiate(enemyPrefab[enemyIndex], spawnPosition, enemyPrefab[enemyIndex].transform.rotation);
    }
}
