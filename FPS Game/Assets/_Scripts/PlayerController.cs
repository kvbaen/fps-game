using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    public bool IsMovingOrJumping => currentInput != Vector2.zero || !characterController.isGrounded;
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKey(crouchKey);

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] public KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] public KeyCode dropKey = KeyCode.G;
    [SerializeField] public KeyCode actionKey = KeyCode.E;
    [SerializeField] public KeyCode reloadKey = KeyCode.R;
    [SerializeField] public KeyCode switchToFirstGunKey = KeyCode.Alpha1;
    [SerializeField] public KeyCode switchToSecondGunKey = KeyCode.Alpha2;
    [SerializeField] public KeyCode switchToKnifeKey = KeyCode.Alpha3;
    [SerializeField] public KeyCode switchToGranadeKey = KeyCode.Alpha4;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 90.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 90.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouching Parameters")]
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new(0, 0, 0);
    private float velocity = 3.0f;
    private Vector3 vectorVelocity = new(3.0f, 0 ,0);
    public Vector3 gunRotation = Vector3.zero;
    [SerializeField]
    private Transform cameraHolder;
    public Transform gunHolder;
    private CharacterController characterController;
    [SerializeField]
    private Camera _mainCamera;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
                HandleJump();
            if (canCrouch)
                HandleCrouch();

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        float moveSpeed = IsSprinting && !ShouldCrouch ? sprintSpeed : walkSpeed;
        currentInput = new Vector2(moveSpeed * Input.GetAxis("Vertical"), moveSpeed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        if(gunRotation != Vector3.zero && !IsMovingOrJumping)
        {
            cameraHolder.transform.localRotation = Quaternion.Euler(
                rotationX + gunRotation.x / 1.2f,
                gunRotation.y / 1.2f,
                gunRotation.z / 1.2f
                );
        }
        else
        {
            cameraHolder.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    private void HandleCrouch()
    {
        if (Physics.Raycast(cameraHolder.transform.position, Vector3.up, 1f)) 
            return;

        float targetHeight = ShouldCrouch ? crouchingHeight : standingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = ShouldCrouch ? crouchingCenter : standingCenter;
        Vector3 currentCenter = characterController.center;

        characterController.height = Mathf.SmoothDamp(currentHeight, targetHeight, ref velocity, timeToCrouch);
        characterController.center = Vector3.SmoothDamp(currentCenter, targetCenter, ref vectorVelocity, timeToCrouch);

    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    public void SetGunRotation(Vector3 _gunRotation)
    {
        gunRotation = _gunRotation;
        gunHolder.localRotation = Quaternion.Euler(gunRotation);
    }
}

