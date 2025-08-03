using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreTextBG;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreTextBG;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private int score = 0;

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
}
