using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private Button restartButton;

    [Header("Text Format")]
    [SerializeField] private string scoreFormat = "Score: {0}";
    [SerializeField] private string livesFormat = "Lives: {0}";
    [SerializeField] private string finalScoreFormat = "Final Score: {0}";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScore(0);
        UpdateLives(GameManager.Instance != null ? GameManager.Instance.maxLives : 3);

        gameOverPanel.SetActive(false);
        mainmenuButton.onClick.AddListener(OnClickMainMenu);
        restartButton.onClick.AddListener(OnClickRestart);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = string.Format(scoreFormat, score);
    }

    public void UpdateLives(int lives)
    {
       livesText.text = string.Format(livesFormat, lives);
    }

    public void ShowGameOver(int finalScore)
    {
        gameOverPanel.SetActive(true);
        infoPanel.SetActive(false);

        finalScoreText.text = string.Format(finalScoreFormat, finalScore);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnClickMainMenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }
    private void OnClickRestart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
