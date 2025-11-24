using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool destroyOnHit = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle!");

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
