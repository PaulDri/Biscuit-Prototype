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
    public int health = 100; 
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private bool canFire;
    [SerializeField] private float timeBetweenFiring;
    private float timer;
    public float CheckScore;

    // Timer properties tracks survival time
    private float survivalTime = 0f;
    private bool isGameOver = false;

    // Damage flicker and invulnerability properties
    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1.5f;
    private float invulnerabilityTimer = 0f;
    private float flickerInterval = 0.1f;
    private float flickerTimer = 0f;
    private SpriteRenderer playerSprite;

    public void IncreaseMoveSpeed(float amount) => moveSpeed += amount;
    public void IncreaseFireSpeed(float amount) => timeBetweenFiring = Mathf.Max(0.1f, timeBetweenFiring - amount);
    public void IncreaseBulletSpeed(float amount) => bulletSpeed += amount;
    public void IncreaseInvulnerability(float amount) => invulnerabilityDuration += amount;

    public float GetMoveSpeed() => moveSpeed;
    public float GetTimeBetweenFiring() => timeBetweenFiring;
    public float GetBulletSpeed() => bulletSpeed;
    public bool CanFire() => canFire;
    public float GetFireCooldownProgress() => canFire ? 1f : Mathf.Clamp01(timer / timeBetweenFiring);
    public float GetSurvivalTime () => survivalTime;

    void Start()
    {
        if (Instance != null) Debug.LogError("There is more than one Player Instance");
        Instance = this;

        playerInputAction = new PlayerInputAction();
        playerInputAction.PlayerInput.Enable();
        playerRb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!isGameOver) 
        {
            HandleInvulnerability();
            HandleFlickerEffect();
            HandleMovement();
            Fire();
            UpdateSurvivalTimer(); 
        }
    }

    private void HandleInvulnerability()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0)
            {
                isInvulnerable = false;
                playerSprite.enabled = true;
            }
        }
    }

    private void HandleFlickerEffect()
    {
        if (isInvulnerable)
        {
            flickerTimer -= Time.deltaTime;
            if (flickerTimer <= 0)
            {
                playerSprite.enabled = !playerSprite.enabled; // Toggle visibility
                flickerTimer = flickerInterval;
            }
        }
    }

    private void UpdateSurvivalTimer()
    {
        survivalTime += Time.deltaTime;
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

            GameObject bulletObj = BulletPool.Instance.GetPlayerBullet();
            if (bulletObj != null) bulletObj.transform.SetPositionAndRotation(bulletTransform.position, Quaternion.identity);
            PlayerUI.Instance.PlayerShootSFX();
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        PlayerUI.Instance.DamagePlayerSFX();
        health -= damage;
        
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        flickerTimer = flickerInterval;
        playerSprite.enabled = false;
        
        if (health <= 0) Die();
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    private void Die()
    {
        Debug.Log($"Player survived for {survivalTime:F2} seconds");
        PlayerUI.Instance.ShowGameOverPanel(survivalTime);
        isGameOver = true;
    }
}
