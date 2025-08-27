using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D enemyRb;
    [SerializeField] private float enemyMoveSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();

        enemyRb.velocity = Vector2.down * enemyMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
