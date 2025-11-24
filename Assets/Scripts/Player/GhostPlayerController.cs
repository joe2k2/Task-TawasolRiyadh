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

        SetupGhostVisuals();
    }

    void SetupGhostVisuals()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material ghostMat = new Material(renderer.material);
            Color color = ghostMat.color;
            color.a = ghostAlpha;
            ghostMat.color = color;
            renderer.material = ghostMat;
        }
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
