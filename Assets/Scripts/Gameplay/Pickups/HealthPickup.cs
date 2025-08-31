using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float lifetime = 10f; // How long the pickup lasts before disappearing
    [SerializeField] private float moveSpeed = 2f; // Speed at which pickup moves toward player
    [SerializeField] private float pickupRange = 1.5f; // Distance at which pickup is automatically collected
    [SerializeField] private float healthSpeed = 1f;

    private float spawnTime;
    // private bool isAttracted = false;

    private void OnEnable()
    {
        spawnTime = Time.time;

        // Add initial downward velocity like EnemyMovement
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            float speedMultiplier = 1f;
            if (EnemySpawner.Instance != null) speedMultiplier = EnemySpawner.Instance.GetCurrentWaveDifficultyMultiplier();
            rb.velocity = healthSpeed * speedMultiplier * Vector2.down;
        }
    }

    private void Update()
    {
        // Auto-destroy after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            HealthPickupPool.Instance.ReturnHealthPickup(gameObject);
            return;
        }

        // Check if player is close enough to attract pickup
        if (Player.Instance != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

            if (distanceToPlayer <= pickupRange)
            {
                // Attract pickup to player
                Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
                transform.position += moveSpeed * Time.deltaTime * direction;

                // If very close, collect the pickup
                if (distanceToPlayer <= 0.5f)
                {
                    CollectPickup();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectPickup();
        }
    }

    private void CollectPickup()
    {
        // Heal the player
        Player.Instance.Heal(healAmount);

        // Play pickup sound effect
        PlayerUI.Instance.HealthPickupSFX();

        // Return to pool
        HealthPickupPool.Instance.ReturnHealthPickup(gameObject);
    }

    // Optional: Visual feedback methods
    // public void SetHealAmount(int amount)
    // {
    //     healAmount = amount;
    // }
}
