using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D enemyRb;
    [SerializeField] private float moveSpeed = 10f;

    private Vector3 direction;
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction = (Player.Instance.transform.position - transform.position).normalized;

        enemyRb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
    }
}
