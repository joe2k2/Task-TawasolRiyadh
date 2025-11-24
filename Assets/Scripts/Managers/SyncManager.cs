using UnityEngine;

public class SyncManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private PlayerController rightPlayer;
    [SerializeField] private GhostPlayerController leftPlayer;

    void Start()
    {
        if (rightPlayer != null && leftPlayer != null)
        {
            rightPlayer.OnStateChanged += leftPlayer.ReceiveState;
        }
        else
        {
            Debug.LogError("SyncManager: Player references not assigned!");
        }
    }

    void OnDestroy()
    {
        if (rightPlayer != null && leftPlayer != null)
        {
            rightPlayer.OnStateChanged -= leftPlayer.ReceiveState;
        }
    }
}
