using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Text Formats")]
    [SerializeField] private string victoryText = "VICTORY!";
    [SerializeField] private string defeatText = "GAME OVER";
    [SerializeField] private string scoreFormat = "Final Score: {0}";
    [SerializeField] private string waveFormat = "Wave Reached: {0}";
    [SerializeField] private string timeFormat = "Time: {0:D2}:{1:D2}";

    private void Start()
    {
        gameOverPanel.SetActive(false);
        
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void ShowGameOver(bool victory)
    {
        Time.timeScale = 0f; // Pause the game
        
        // Set header text based on victory/defeat
        gameOverText.text = victory ? victoryText : defeatText;
        
        // Get final stats
        int finalScore = ScoreManager.Instance.CurrentScore;
        float finalTime = ScoreManager.Instance.PlayTime;
        int finalWave = FindObjectOfType<WaveManager>().CurrentWave;
        
        // Update UI texts
        scoreText.text = string.Format(scoreFormat, finalScore);
        waveText.text = string.Format(waveFormat, finalWave);
        
        int minutes = Mathf.FloorToInt(finalTime / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);
        timeText.text = string.Format(timeFormat, minutes, seconds);
        
        gameOverPanel.SetActive(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 