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
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float timeBetweenFiring;
    [SerializeField] private bool canShoot = true;
    public float CheckScore;

    private bool canFire = true;
    private float timer;

    // Timer properties tracks survival time
    private float survivalTime = 0f;
    public bool recordTime = false;
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
    public void IncreaseBulletDamage(int amount) => bulletDamage += amount;
    public void IncreaseInvulnerability(float amount) => invulnerabilityDuration += amount;

    public float GetMoveSpeed() => moveSpeed;
    public float GetTimeBetweenFiring() => timeBetweenFiring;
    public float GetBulletSpeed() => bulletSpeed;
    public int GetBulletDamage() => bulletDamage;
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
        if (recordTime) UpdateSurvivalTimer();

        if (!isGameOver) 
        {
            HandleInvulnerability();
            HandleFlickerEffect();
            HandleMovement();

            if (!canShoot) return;
            Fire();
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

        if (canFire && canShoot)
        {
            canFire = false;

            GameObject bulletObj = BulletPool.Instance.GetPlayerBullet();

            if (bulletObj != null) 
                bulletObj.transform.SetPositionAndRotation(bulletTransform.position, Quaternion.identity);

            PlayerUI.Instance.PlayerShootSFX();
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        PlayerUI.Instance.DamagePlayerSFX();
        health -= damage;
        PlayerUI.Instance.UpdateHealthBar();
        
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        flickerTimer = flickerInterval;
        playerSprite.enabled = false;
        
        if (health <= 0) Die();
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, 100); // Cap at 100 health
        Debug.Log($"Player healed for {amount} health. Current health: {health}");
        PlayerUI.Instance.UpdateHealthBar();
    }

    private void Die()
    {
        Debug.Log($"Player survived for {survivalTime:F2} seconds");
        PlayerUI.Instance.ShowGameOverPanel(survivalTime);
        isGameOver = true;
    }

    public void EnableShooting()
    {
        canShoot = true;
        Debug.Log("Player shooting enabled");
    }

    public void DisableShooting()
    {
        canShoot = false;
        Debug.Log("Player shooting disabled");
    }

    public void ToggleShooting()
    {
        canShoot = !canShoot;
        Debug.Log($"Player shooting {(canShoot ? "enabled" : "disabled")}");
    }
}
