using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreTextBG;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreTextBG;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    private int score = 0;

    [SerializeField] private Material material1;
    [SerializeField] private Material material2;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Optional: Keep across scenes
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateScoreText();
    }

    private void Update()
    {
        material1.SetFloat("_UnscaledTime", Time.unscaledTime);
        material2.SetFloat("_UnscaledTime", Time.unscaledTime);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void AddScore(int amount = 1)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreTextBG.text = $"Score: <color=red>{score.ToString()}</color>"; 
            scoreText.text = $"Score: {score.ToString()}";
            finalScoreTextBG.text = $"Score: <color=red>{score.ToString()}</color>";
            finalScoreText.text = $"Score: {score.ToString()}";
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            if (isPaused)
                pauseMenu.GetComponent<Popup>().AnimateToYOffscreen();
            else
                pauseMenu.GetComponent<Popup>().AnimateToYZero();
        }
    }

    public void PauseGame()
    {
        if (pauseMenu != null)
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0; // Pause the game
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1; // Resume the game
        }
    }
}
