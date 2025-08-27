using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private PlayerInputAction playerInputAction;
    private Rigidbody2D playerRb;
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletTransform;

    // TODO: canFire not updating properly
    [SerializeField] private bool canFire;
    [SerializeField] private float timeBetweenFiring;
    private float timer;
    public float CheckScore;
    
    public float GetMoveSpeed() => moveSpeed;
    public float GetTimeBetweenFiring() => timeBetweenFiring;
    public bool CanFire() => canFire;
    public float GetFireCooldownProgress() => canFire ? 1f : Mathf.Clamp01(timer / timeBetweenFiring);

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) 
        {
            Debug.LogError("There is more than one Player Instance");
        }
        Instance = this;

        playerInputAction = new PlayerInputAction();
        playerInputAction.PlayerInput.Enable();
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        Fire();
    }

    private void HandleMovement() 
    {
        Vector2 inputVector = GetMovementNormalized();

        Vector2 moveDirection = new Vector2(inputVector.x, 0);

        playerRb.velocity = new Vector2(moveDirection.x * moveSpeed, 0);
    }

    private Vector2 GetMovementNormalized() 
    {
        Vector2 inputVector = playerInputAction.PlayerInput.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    private void Fire() 
    {
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring) 
            {
                canFire = true;
                timer = 0;
            }
        }

        if (canFire) 
        {
            if (CheckScore == 10) 
            {
                timeBetweenFiring -= 1f;
            }
            canFire = false;
            GameObject bulletObj = BulletPool.Instance.GetBullet();
            if (bulletObj != null)
            {
                bulletObj.transform.position = bulletTransform.position;
                bulletObj.transform.rotation = Quaternion.identity;
            }
        }
    }
}
