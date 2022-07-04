using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("CameraSettings")]
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Movement Settings")]
    public CharacterController controller;
    public float _defaultMoveSpeed;
    public float _moveSpeed = 12f;
    public float _slowedSpeed = 2f;
    public float _sprintSpeed = 23f;
    public float _gravity = -9.81f;
    public float _jumpHeight = 3f;
    [SerializeField] AudioSource slowSteps;


    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float _groundDistance = -0.4f; //radius of sphere
    public LayerMask groundMask; //checks for collision with the floor specifically, in case it catches player collision first, which it will

    [Header("Respawn Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;
    public bool isDead;

    [Header("Miscelaneous")]
    [SerializeField] AudioClip loseClip;
    public LayerMask enemyMask;

    public Vector3 velocity;
    public bool isGrounded;
    bool isSprinting;
    bool isHurt;
    public bool isOnEnemy;
    


    private void Start()
    {
        _defaultMoveSpeed = _moveSpeed;
        //currentHealth = maxHealth;
        //healthBar.SetMaxHealth(Mathf.FloorToInt(maxHealth));
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask); //checks for collision with floor using a small invisible sphere; returns true/false
        isOnEnemy = Physics.CheckSphere(groundCheck.position, _groundDistance, enemyMask);
        //isHurt = Physics.CheckSphere(groundCheck.position, _groundDistance, hazardMask);

        SimulateGravity();

        ResetVelocity();

        Move();
    }





    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        bool _isCrouching = Input.GetButton("Crouch"); //LCtrl or C
        bool _isSprinting = Input.GetButton("Sprint"); //Shift/Z

        
        //If Sprint button is pressed, sprint!

        
        if (_isSprinting  && !_isCrouching)
        {
            _moveSpeed = _sprintSpeed;
            
        }

        //If Crouch button is pressed, crouch down

        else if (_isCrouching && !_isSprinting)
        {
            _moveSpeed = _slowedSpeed * 3;
            controller.height = 1;
        }

        //If neither are pressed, reset.
        else if (!_isCrouching && !isSprinting)
        {
            _moveSpeed = _defaultMoveSpeed;
            controller.height = 2;

        }

        Vector3 _viewDirection = new Vector3(x, 0f, z).normalized;
        if (_viewDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_viewDirection.x, _viewDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * _moveSpeed * Time.deltaTime);
        }

        


        //Vector3 _moveDirection = transform.right * x + transform.forward * z; //find the move direction based on axis buttons pressed times their respective transforms

        //Footstep code
        if (controller.isGrounded && controller.velocity.magnitude > 1f)
        {
            if (slowSteps != null)
            {

                slowSteps.volume = Random.Range(0.8f, 1);
                slowSteps.pitch = Random.Range(0.8f, 1);
                slowSteps.Play();
            }
        }


    }

    void SimulateGravity()
    {
        velocity.y += _gravity * Time.deltaTime; //need a velocity variable to simulate real gravity
        controller.Move(velocity * Time.deltaTime); //multiply times deltaTime twice, as is shown on velocity equation
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity); //force required to jump according to physics. (Square root of jump height x (-2) x gravity)
            _moveSpeed = _sprintSpeed;
        }
    }

    void ResetVelocity()
    {
        if (!isGrounded && velocity.y < 0)
            _moveSpeed -= 0.01f;

        if (isGrounded && velocity.y < 0) //when touching the ground, AND when velocity is at all greater than 0 (meaning player is being pushed by gravity), reset the velocity
        {
            velocity.y = -2f; //sticks player to ground   
            _moveSpeed = _defaultMoveSpeed;
        }
    }

}
   