using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    // Components and prefabs
    private PlayerInputAction playerInputAction;
    private Rigidbody2D playerRb;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletTransform;


    // Player properties
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private bool canFire;
    [SerializeField] private float timeBetweenFiring;
    private float timer;
    public float CheckScore;

    public void IncreaseMoveSpeed(float amount) => moveSpeed += amount;
    public void IncreaseFireSpeed(float amount) => timeBetweenFiring = Mathf.Max(0.1f, timeBetweenFiring - amount);
    public void IncreaseBulletSpeed(float amount) => bulletSpeed += amount;

    public float GetMoveSpeed() => moveSpeed;
    public float GetTimeBetweenFiring() => timeBetweenFiring;
    public float GetBulletSpeed() => bulletSpeed;
    public bool CanFire() => canFire;
    public float GetFireCooldownProgress() => canFire ? 1f : Mathf.Clamp01(timer / timeBetweenFiring);

    void Start()
    {
        if (Instance != null) Debug.LogError("There is more than one Player Instance");
        Instance = this;

        playerInputAction = new PlayerInputAction();
        playerInputAction.PlayerInput.Enable();
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
        Fire();
        CheckWaveThreshold();
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
            canFire = false;

            GameObject bulletObj = BulletPool.Instance.GetBullet();
            if (bulletObj != null) bulletObj.transform.SetPositionAndRotation(bulletTransform.position, Quaternion.identity);

        }
    }

    private void CheckWaveThreshold()
    {
        if (EnemySpawner.Instance != null && CheckScore >= EnemySpawner.Instance.GetCurrentWaveScoreThreshold())
        {
            LevelUpSystem.Instance.ShowLevelUpOptions();
            EnemySpawner.Instance.AdvanceToNextWave();
        }


    }
}
