using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Format Settings")]
    [SerializeField] private string scoreFormat = "SCORE: {0:D8}";
    [SerializeField] private string timeFormat = "TIME: {0:D2}:{1:D2}";
    [SerializeField] private string waveFormat = "WAVE {0}";

    private WaveManager waveManager;
    private ScoreManager scoreManager;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        scoreManager = ScoreManager.Instance;
        
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found!");
        }
    }

    private void Update()
    {
        UpdateScore();
        UpdateTime();
        UpdateWave();
    }

    private void UpdateScore()
    {
        if (scoreText != null && scoreManager != null)
        {
            scoreText.text = string.Format(scoreFormat, scoreManager.CurrentScore);
        }
    }

    private void UpdateTime()
    {
        if (timeText != null && scoreManager != null)
        {
            int minutes = Mathf.FloorToInt(scoreManager.PlayTime / 60);
            int seconds = Mathf.FloorToInt(scoreManager.PlayTime % 60);
            timeText.text = string.Format(timeFormat, minutes, seconds);
        }
    }

    private void UpdateWave()
    {
        if (waveText != null && waveManager != null)
        {
            waveText.text = string.Format(waveFormat, waveManager.CurrentWave);
        }
    }
} 