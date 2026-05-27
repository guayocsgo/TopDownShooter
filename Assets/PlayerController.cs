using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 5f;
    private float gravity = -9.81f;
    private float rotationSpeed = 20f;

    // Cuanto más alto, más rápido sigue el crosshair. Bajalo si sigue girando raro (prueba entre 5 y 15)
    [SerializeField] private float lookSmoothing = 10f;

    private Camera mainCamera;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 lookTarget;
    private Vector3 rawLookTarget;
    private float verticalVelocity;
    private bool isJumping;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        lookTarget = transform.position + transform.forward * 2f;
        rawLookTarget = lookTarget;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            isJumping = true;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mouseScreenPosition = context.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        // Plano a la altura real del jugador
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (groundPlane.Raycast(ray, out float enter))
        {
            rawLookTarget = ray.GetPoint(enter);
        }
    }

    private void Update()
    {
        ApplyGravity();
        MovePlayer();
        SmoothLookTarget();
        RotateTowardsMouse();
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            if (!isJumping) verticalVelocity = -1f;
            isJumping = false;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
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

        Vector3 movement = (camForward * moveInput.y + camRight * moveInput.x) * moveSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }

    // Suaviza el salto brusco del punto en el plano cuando la cámara Cinemachine rota
    private void SmoothLookTarget()
    {
        lookTarget = Vector3.Lerp(lookTarget, rawLookTarget, lookSmoothing * Time.deltaTime);
    }

    private void RotateTowardsMouse()
    {
        Vector3 lookDirection = lookTarget - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}   