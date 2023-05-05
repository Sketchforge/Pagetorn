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


    [Header("Miscelaneous")]
    [SerializeField] AudioClip loseClip;
    public LayerMask enemyMask;

    public Vector3 velocity;
    public bool isGrounded;
    public bool isOnEnemy;

    [Header("Data Storage")]
    [SerializeField] private Vector3 lastPos;
    [SerializeField] private Vector3 currentPos;

    [Header("Headbob and FootSteps")]
    [SerializeField] private float _walkBobSpeed = 14f;
    [SerializeField] private float _walkBobAmount = 0.05f;
    [SerializeField] private float _sprintBobSpeed = 18f;
    [SerializeField] private float _sprintBobAmount = .11f;
    [SerializeField] private float _crouchBobSpeed = 8f;
    [SerializeField] private float _crouchBobAmount = 0.02f;
    private float _defaultYpos = 0;
    private float _headbobTimer;


    private bool gamePaused;
    private bool isDead;

    private Vector2 _moveDir;
    private bool _isSprinting;
    private bool _isCrouching;
    private bool _willJumpThisFrame;

    private void Awake()
    {
        _defaultYpos = cam.transform.localPosition.y;
    }

    private void OnEnable()
    {
        GameManager.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManager.OnPause -= OnPause;
    }

    private void Start()
    {
        _defaultMoveSpeed = _moveSpeed;
        lastPos = transform.position;
        currentPos = transform.position;
    }

    private void Update()
    {
        if (gamePaused || isDead) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask); //checks for collision with floor using a small invisible sphere; returns true/false
        isOnEnemy = Physics.CheckSphere(groundCheck.position, _groundDistance, enemyMask);
        //isHurt = Physics.CheckSphere(groundCheck.position, _groundDistance, hazardMask);


    }

    private void FixedUpdate()
    {
        SimulateGravity();

        ResetVelocity();

        Move();
    }

    public void SetMoveDir(Vector2 moveDir) => _moveDir = moveDir;
    public void SetToJump() => _willJumpThisFrame = true;
    public void SetSprinting(bool sprinting) => _isSprinting = sprinting;
    public void SetCrouching(bool crouching) => _isCrouching = crouching;

    private void OnPause(bool paused)
    {
        gamePaused = paused;
    }

    public void OnPlayerDeath()
    {
        isDead = true;
    }

    public void OnPlayerRespawn()
    {
        // Note: Respawn position is handled by the Player Manager
        isDead = false;
        // TODO: Ensure there is no stored velocity from before death
    }

    void Move()
    {
        if ((_moveDir != new Vector2(0,0)) || !_isSprinting || !_isCrouching)
        {
            GameManager.Data.AmountTimeStoodStill += Time.deltaTime;
            
        }
        
        //If Sprint button is pressed, sprint!
        if (_isSprinting && !_isCrouching)
        {
            _moveSpeed = _sprintSpeed;
            PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hydration, 0.1f);

        }

        //If Crouch button is pressed, crouch down
        else if (_isCrouching && !_isSprinting)
        {
            _moveSpeed = _slowedSpeed * 3;
            controller.height = 1;
        }

        //If neither are pressed, reset.
        else if (!_isCrouching && !this._isSprinting)
        {
            _moveSpeed = _defaultMoveSpeed;
            controller.height = 2;
        }

        Vector3 _viewDirection = new Vector3(_moveDir.x, 0f, _moveDir.y).normalized;
        if (_viewDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_viewDirection.x, _viewDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * _moveSpeed * Time.deltaTime);
        }


 

        //update DataManager
        currentPos = transform.position;
        GameManager.Data.NumberDistanceWalked += Vector3.Distance(currentPos, lastPos);
        lastPos = currentPos;


        //Vector3 _moveDirection = transform.right * x + transform.forward * z; //find the move direction based on axis buttons pressed times their respective transforms

        //Footstep code
        if (Mathf.Abs(_moveDir.x) > 0.1f || Mathf.Abs(_moveDir.y) > 0.1f)
        {
            if (isGrounded)
            {
                //HEADBOB
                _headbobTimer += Time.deltaTime * (_isCrouching ? _crouchBobSpeed : _isSprinting ? _sprintBobSpeed : _walkBobSpeed);
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, _defaultYpos + Mathf.Sin(_headbobTimer) *
                    (_isCrouching ? _crouchBobAmount : _isSprinting ? _sprintBobAmount : _walkBobAmount), cam.transform.localPosition.z);

                if (slowSteps != null)
                {
                    slowSteps.volume = Random.Range(0.8f, 1);
                    slowSteps.pitch = Random.Range(0.8f, 1);
                    slowSteps.Play();
                }
            }
            else
            {
                cam.transform.localPosition = new Vector3(cam.localPosition.x, Mathf.Lerp(cam.localPosition.y, _defaultYpos, 8 * Time.deltaTime), cam.localPosition.z);
                _headbobTimer = 0;
            }
        }
        else
        {
            cam.transform.localPosition = new Vector3(cam.localPosition.x, Mathf.Lerp(cam.localPosition.y, _defaultYpos, 8 * Time.deltaTime), cam.localPosition.z);
            _headbobTimer = 0;
        }
    }

    void SimulateGravity()
    {
        velocity.y += _gravity * Time.deltaTime; //need a velocity variable to simulate real gravity
        controller.Move(velocity * Time.deltaTime); //multiply times deltaTime twice, as is shown on velocity equation
        if (_willJumpThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity); //force required to jump according to physics. (Square root of jump height x (-2) x gravity)
            _moveSpeed = _sprintSpeed;
        }
        _willJumpThisFrame = false;
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