using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI movesText;
    public GameObject levelCompletePanel;
    public GameObject levelFailPanel;

    private float timeRemaining;
    private int movesRemaining;
    private bool gameActive = false;
    private string constraint;

    void Awake()
    {
        Instance = this;
    }

    public void StartLevel(LevelData levelData)
    {
        constraint = levelData.constraint;
        gameActive = true;

        if (constraint == "time")
        {
            timeRemaining = levelData.time_limit;
            if (movesText) movesText.gameObject.SetActive(false);
            if (timerText) timerText.gameObject.SetActive(true);
            UpdateTimerUI();
        }
        else
        {
            movesRemaining = levelData.move_limit;
            if (timerText) timerText.gameObject.SetActive(false);
            if (movesText) movesText.gameObject.SetActive(true);
            UpdateMovesUI();
        }

        if (levelCompletePanel) levelCompletePanel.SetActive(false);
        if (levelFailPanel) levelFailPanel.SetActive(false);
    }

    void Update()
    {
        if (!gameActive || constraint != "time") return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            gameActive = false;
            LevelFailed();
        }
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    void UpdateTimerUI()
    {
        if (timerText)
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
    }

    void UpdateMovesUI()
    {
        if (movesText)
            movesText.text = "Moves: " + movesRemaining.ToString();
    }

    public void UseMove()
    {
        if (constraint != "moves") return;
        movesRemaining--;
        UpdateMovesUI();
        if (movesRemaining <= 0)
            LevelFailed();
    }

    public void LevelComplete()
    {
        gameActive = false;
        if (levelCompletePanel) levelCompletePanel.SetActive(true);
    }
    public void NextLevel()
    {
        GameBoard board = FindObjectOfType<GameBoard>();
        int nextLevel = board.currentLevel.level_id + 1;
        if (nextLevel <= 20)
            board.LoadLevel(nextLevel);
        else
            Debug.Log("Game Complete! All levels done.");
    }

    public void LevelFailed()
    {
        gameActive = false;
        if (levelFailPanel) levelFailPanel.SetActive(true);
    }

    public void RetryLevel()
    {
        GameBoard board = FindObjectOfType<GameBoard>();
        board.BuildBoard();
        StartLevel(board.currentLevel);
    }
}