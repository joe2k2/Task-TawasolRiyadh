using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private float rotationSpeed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Collected! +{scoreValue} points");
            Destroy(gameObject);
        }
    }
}
