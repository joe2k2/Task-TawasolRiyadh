using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Dissolve Settings")]
    [SerializeField] private float dissolveDuration = 1f;

    private Material mat;
    private bool hasBeenHit = false;

    private string dissolveProperty = "_Value";

    void OnEnable()
    {
        if (mat == null)
            mat = GetComponent<MeshRenderer>().material;

        if (mat.HasProperty(dissolveProperty))
            mat.SetFloat(dissolveProperty, 0f);

        hasBeenHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenHit)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasBeenHit = true;

        GameManager.Instance.PlayerHitObstacle();

        StartCoroutine(DissolveEffect());
    }

    System.Collections.IEnumerator DissolveEffect()
    {
        float t = 0f;

        while (t < dissolveDuration)
        {
            float v = t / dissolveDuration;
            mat.SetFloat(dissolveProperty, v);
            t += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat(dissolveProperty, 1f);
        gameObject.SetActive(false);
    }
}
