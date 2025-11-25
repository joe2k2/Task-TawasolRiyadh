using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Physics Settings")]
    [SerializeField] private float gravityMultiplier = 1f;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private bool isGrounded;
    private bool canJump = true;

    public bool IsGrounded => isGrounded;
    public Vector3 Position => transform.position;
    public Vector3 Velocity => rb.linearVelocity;
    public float JumpForce => jumpForce;

    public event System.Action<PlayerState> OnStateChanged;

    void Awake()
    {
        InitializeRigidbody();
        InitializeInput();
    }

    void FixedUpdate()
    {
        BroadcastState();
    }

    void BroadcastState()
    {
        PlayerState state = new PlayerState(
            Time.time,
            transform.localPosition,
            rb.linearVelocity,
            isGrounded,
            !isGrounded && rb.linearVelocity.y > 0
        );

        OnStateChanged?.Invoke(state);
    }

    void OnEnable()
    {
        inputActions?.Enable();
    }

    void OnDisable()
    {
        inputActions?.Disable();
    }

    void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void InitializeInput()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Jump.performed += OnJumpPerformed;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (isGrounded && canJump)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void SetJumpEnabled(bool enabled)
    {
        canJump = enabled;
    }

    public void ResetPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        isGrounded = false;
    }

    void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Jump.performed -= OnJumpPerformed;
            inputActions.Dispose();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
