using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 mousePosition;
    private Camera mainCam;
    private Rigidbody2D bulletRb;
    [SerializeField] private float force;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        bulletRb = GetComponent<Rigidbody2D>();

        mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        Vector3 rotation = transform.position - mousePosition;
        bulletRb.velocity = new Vector2(direction.x, direction.y ).normalized * force;
        float Bulletrotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, Bulletrotation + 90);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
