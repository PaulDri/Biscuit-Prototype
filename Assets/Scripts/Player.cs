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


    //[SerializeField] private Camera mainCamera;
    private Vector3 mousePosition;
    private Camera mainCam;


    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private bool canFire;
    private float timer;
    [SerializeField] private float timeBetweenFiring;


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

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        HandleMovement();
        //mousePosition = Input.mousePosition;
        //mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Vector2 mouseDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        //transform.up = mouseDirection;

        mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePosition - transform.position;

        float rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        if (!canFire) 
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring) 
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && canFire) 
        {
            canFire = false;
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
        }

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
