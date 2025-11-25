using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private float rotationSpeed = 100f;

    private bool hasBeenCollected = false;

    void OnEnable()
    {
        hasBeenCollected = false;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenCollected)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasBeenCollected = true;

        GameManager.Instance.AddScore(scoreValue);

        gameObject.SetActive(false);
    }
}
