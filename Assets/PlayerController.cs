using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Mouse Look")]
    [SerializeField] private float lookSmoothing = 10f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;

    private Camera mainCamera;
    private CharacterController characterController;
    private Animator animator;

    private Vector2 moveInput;

    private Vector3 lookTarget;
    private Vector3 rawLookTarget;

    private float verticalVelocity;
    private bool isJumping;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        lookTarget = transform.position + transform.forward * 2f;
        rawLookTarget = lookTarget;
    }

    private void Update()
    {
        ApplyGravity();
        MovePlayer();
        SmoothLookTarget();
        RotateTowardsMouse();
        UpdateAnimations();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Shoot");

            if (bulletPrefab != null && shootPoint != null)
            {
                Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            }
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Reload");
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mouseScreenPosition = context.ReadValue<Vector2>();

        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(0f, transform.position.y, 0f)
        );

        if (groundPlane.Raycast(ray, out float enter))
        {
            rawLookTarget = ray.GetPoint(enter);
        }
    }

    private void MovePlayer()
    {
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 movement =
            (camForward * moveInput.y + camRight * moveInput.x) * moveSpeed;

        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;

            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void SmoothLookTarget()
    {
        lookTarget = Vector3.Lerp(
            lookTarget,
            rawLookTarget,
            lookSmoothing * Time.deltaTime
        );
    }

    private void RotateTowardsMouse()
    {
        Vector3 lookDirection = lookTarget - transform.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimations()
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.magnitude > 0.1f;

        animator.SetBool("IsMoving", isMoving);

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
    }
}