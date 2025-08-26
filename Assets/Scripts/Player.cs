using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpPower = 7f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask biscuitLayer;
    private PlayerInputAction playerInputAction;

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

        Vector2 moveDirection = new Vector2(inputVector.x, 0);
        //float playerMovement = moveDirection * moveSpeed;

        //transform.position += moveDirection * moveDistance;
        playerRb.velocity = new Vector2(moveDirection.x * moveSpeed, playerRb.velocity.y);
        float rayCastDistance = 2f;

        //For checking if the player touched the ground
        bool canJump1 = Physics2D.Raycast(groundCheck.position, Vector2.down, rayCastDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down, Color.red);

        if (Input.GetKeyDown(KeyCode.Space) && canJump1) 
        {
            playerRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        //Changes the character face
        float rotationSpeed = 10f;
        transform.right = Vector2.Lerp(transform.right, moveDirection, Time.deltaTime * rotationSpeed);

    }

    //private void PlayerInteract() 
    //{
    //    float rayCastDistance1 = 0.5f;
    //    bool interact = Physics2D.Raycast(groundCheck.position, Vector2.right, rayCastDistance1, biscuitLayer);
    //    Debug.DrawRay(groundCheck.position, Vector2.right, Color.black);

    //    if (interact)
    //    {
    //        Debug.Log("Interacted!");
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Biscuit")) 
        {
            //ShowScore.Instance.sc
            Destroy(collision.gameObject);
            Debug.Log("You get the biscuit");
        }
    }

    private Vector2 GetMovementVectorNormalized() 
    {
        Vector2 inputVector = playerInputAction.PlayerInput.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
