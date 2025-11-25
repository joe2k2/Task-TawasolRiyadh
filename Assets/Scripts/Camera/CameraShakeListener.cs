using System.Collections;
using UnityEngine;

public class CameraShakeListener : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.OnPlayerHitObstacle += Shake;
    }

    void OnDisable()
    {
        GameManager.OnPlayerHitObstacle -= Shake;
    }

    void Shake()
    {
        StartCoroutine(ScreenShake());
    }

    IEnumerator ScreenShake()
    {
        Camera cam = GameManager.Instance.playerCamera;
        Vector3 originalPos = cam.transform.localPosition;

        float duration = 0.3f;
        float strength = 0.25f;
        float time = 0f;

        while (time < duration)
        {
            cam.transform.localPosition = originalPos + Random.insideUnitSphere * strength;
            time += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }
}
