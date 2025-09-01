using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossMovement : MonoBehaviour
{
    [Header("Boss Properties")]
    [SerializeField] private State currentState = State.Moving;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool isInvulnerable = false;
    private Vector3 startPosition;
    private float stateTimer;
    private bool hasReachedPosition = false;
    private float startStrafeTime;
    private int currentHealth;
    
    [Header("Movement Settings")]
    [SerializeField] private float bossMovement = 2f;
    [SerializeField] private float stopY = 3f;
    [SerializeField] private float strafeSpeed = 2f;
    [SerializeField] private float strafeAmplitude = 3f;
    [SerializeField] private float leftBoundary = -10f;
    [SerializeField] private float rightBoundary = 11f;
    
    [Header("State Timing")]
    [SerializeField] private float minStateTime = 1f;
    [SerializeField] private float maxStateTime = 3f;
    [SerializeField] private float shootingTime = 0.5f;
    
    [Header("Random State Settings")]
    [SerializeField] private State[] availableStates = { State.Shooting, State.Strafing, State.Stopped };
    [SerializeField] private float[] stateWeights = { 30f, 40f, 30f }; // Shooting, Strafing, Stopped
    public enum State { Moving, Stopped, Shooting, Strafing }

    [Header("UI Reference")]
    [SerializeField] private Image healthBar;
    [SerializeField] private CanvasGroup healthCanvasGroup;

    void Start()
    {
        startPosition = transform.position;
        currentHealth = maxHealth;
        isInvulnerable = true; // Boss is invulnerable from the start

        if (healthBar != null) healthBar.fillAmount = 1f;
        if (healthCanvasGroup != null) healthCanvasGroup.alpha = 0f;
    }
    
    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                HandleMoving();
                break;
            case State.Stopped:
                HandleStopped();
                break;
            case State.Shooting:
                HandleShooting();
                break;
            case State.Strafing:
                HandleStrafing();
                break;
        }
    }
    
    #region State Handlers
    
    private void HandleMoving()
    {
        transform.Translate(Vector2.down * bossMovement * Time.deltaTime);
        if (transform.position.y <= stopY)
        {
            hasReachedPosition = true;
            isInvulnerable = false;

            healthCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuint);

            startPosition = transform.position;
            SwitchToRandomState();
        }
    }
    
    private void HandleStopped()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SwitchToRandomState();
        }
    }
    
    private void HandleShooting()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            Shoot();
            SwitchToRandomState();
        }
    }
    
    private void HandleStrafing()
    {
        // Strafe movement
        float x = startPosition.x + Mathf.Sin((Time.time - startStrafeTime) * strafeSpeed) * strafeAmplitude;
        x = Mathf.Clamp(x, leftBoundary, rightBoundary);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SwitchToRandomState();
        }
    }
    
    #endregion
    
    #region Random State Selection
    
    private void SwitchToRandomState()
    {
        if (!hasReachedPosition)
        {
            currentState = State.Moving;
            return;
        }
        
        State newState = GetRandomState();
        
        // Prevent switching to the same state (optional)
        int attempts = 0;
        while (newState == currentState && attempts < 10)
        {
            newState = GetRandomState();
            attempts++;
        }
        
        currentState = newState;
        SetStateTimer();

        if (currentState == State.Strafing)
        {
            startStrafeTime = Time.time;
            startPosition = transform.position;
            startPosition.x = Mathf.Clamp(startPosition.x, leftBoundary, rightBoundary);
        }

        Debug.Log($"Boss switched to: {currentState}");
    }
    
    private State GetRandomState()
    {
        // Method 1: Simple random selection
        // return availableStates[Random.Range(0, availableStates.Length)];
        
        // Method 2: Weighted random selection (uncomment to use)
        float totalWeight = 0f;
        foreach (float weight in stateWeights)
            totalWeight += weight;
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        for (int i = 0; i < availableStates.Length; i++)
        {
            currentWeight += stateWeights[i];
            if (randomValue <= currentWeight)
                return availableStates[i];
        }
        
        return availableStates[0]; // Fallback
    }
    
    private void SetStateTimer()
    {
        switch (currentState)
        {
            case State.Shooting:
                stateTimer = shootingTime;
                break;
            case State.Stopped:
            case State.Strafing:
                stateTimer = Random.Range(minStateTime, maxStateTime);
                break;
        }
    }
    
    #endregion
    
    #region Combat
    
    private void Shoot()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject bullet = BulletPool.Instance.GetEnemyBullet();
            if (bullet != null)
            {
                bullet.transform.position = transform.position + new Vector3((i - 1.5f) * 0.5f, 0, 0);
                float angle = -15f + i * 10f;
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.down;
                bullet.GetComponent<EnemyBullet>().direction = direction;
            }
        }
    }
    
    #endregion

    #region Health System

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        if (healthBar != null) healthBar.fillAmount = (float)currentHealth / maxHealth;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        // Handle boss death - can be expanded with explosion effects, score updates, etc.
        Debug.Log("Boss defeated!");
        EnemySpawner.Instance.OnBossDefeated();
        PlayerUI.Instance.DamagePlayerSFX();
        Destroy(gameObject);
    }

    #endregion

    #region Advanced Random Patterns (Optional)
    
    // Method 3: Pattern-based random states
    // [System.Serializable]
    // public class StatePattern
    // {
    //     public State[] pattern;
    //     public float weight = 1f;
    // }
    
    // [Header("Advanced Patterns")]
    // [SerializeField] private StatePattern[] statePatterns = {
    //     new StatePattern { pattern = new State[] { State.Shooting, State.Strafing }, weight = 1f },
    //     new StatePattern { pattern = new State[] { State.Strafing, State.Shooting, State.Shooting }, weight = 0.7f },
    //     new StatePattern { pattern = new State[] { State.Stopped, State.Shooting }, weight = 0.5f }
    // };
    
    // private int currentPatternIndex = 0;
    // private int patternStep = 0;
    
    // private void SwitchToPatternState()
    // {
    //     if (statePatterns.Length == 0) return;
        
    //     // Get current pattern
    //     StatePattern currentPattern = statePatterns[currentPatternIndex];
        
    //     // Get next state in pattern
    //     currentState = currentPattern.pattern[patternStep];
        
    //     // Advance pattern
    //     patternStep++;
    //     if (patternStep >= currentPattern.pattern.Length)
    //     {
    //         patternStep = 0;
    //         // Choose new random pattern
    //         currentPatternIndex = Random.Range(0, statePatterns.Length);
    //     }
        
    //     SetStateTimer();
    // }
    
    #endregion
}
