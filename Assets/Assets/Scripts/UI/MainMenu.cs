using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    [SerializeField] private float blinkInterval = 0.8f;
    private bool gameStarted = false;
    private float nextBlinkTime;

    private void Start()
    {
        Time.timeScale = 0f; // Pause the game at start
        nextBlinkTime = Time.unscaledTime + blinkInterval;
    }

    private void Update()
    {
        if (!gameStarted)
        {
            // Handle text blinking
            if (Time.unscaledTime >= nextBlinkTime)
            {
                pressAnyKeyText.enabled = !pressAnyKeyText.enabled;
                nextBlinkTime = Time.unscaledTime + blinkInterval;
            }

            // Check for any key press
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        pressAnyKeyText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
} 