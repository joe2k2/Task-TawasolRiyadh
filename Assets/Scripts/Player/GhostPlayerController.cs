using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GhostPlayerController : MonoBehaviour
{
    [Header("Sync Settings")]
    [SerializeField] private float networkDelay = 0.1f;
    [SerializeField] private float interpolationSpeed = 10f;

    [Header("Visual Settings")]
    [SerializeField] private float ghostAlpha = 0.7f;

    private StateBuffer<PlayerState> stateBuffer;
    private Rigidbody rb;
    private Vector3 targetPosition;
    private Vector3 targetVelocity;
    private const int bufferCapacity = 120;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        stateBuffer = new StateBuffer<PlayerState>(bufferCapacity);
    }
    public void ReceiveState(PlayerState state)
    {
        stateBuffer.Add(state);
    }

    void Update()
    {
        ProcessStateBuffer();
        InterpolateToTarget();
    }

    void ProcessStateBuffer()
    {
        float targetTime = Time.time - networkDelay;

        if (stateBuffer.TryDequeue(out PlayerState state))
        {
            targetPosition = state.position;
            targetVelocity = state.velocity;
        }
    }

    void InterpolateToTarget()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * interpolationSpeed
        );
    }

    public void ResetGhost()
    {
        stateBuffer.Clear();
        transform.position = Vector3.zero;
    }
}
