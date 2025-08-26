using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask biscuitLayer;
    private PlayerInputAction playerInputAction;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else 
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInputAction = new PlayerInputAction();
        playerInputAction.PlayerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        //PlayerInteract();
    }

    private void HandleMovement() 
    {

        Vector2 inputVector = GetMovementVectorNormalized();

        Vector2 moveDirection = new Vector2(inputVector.x, inputVector.y);

        playerRb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        //Changes the character face
        float rotationSpeed = 10f;
        groundCheck.right = Vector2.Lerp(groundCheck.right, moveDirection, Time.deltaTime * rotationSpeed);

    }

    private Vector2 GetMovementVectorNormalized() 
    {
        Vector2 inputVector = playerInputAction.PlayerInput.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
