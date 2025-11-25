using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    private void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
        exitButton.onClick.AddListener(OnClickExit);    }
    private void OnClickStart()
    {
        SceneManager.LoadScene("Gameplay");
    }
    private void OnClickExit()
    {
        Application.Quit();
    }
}
