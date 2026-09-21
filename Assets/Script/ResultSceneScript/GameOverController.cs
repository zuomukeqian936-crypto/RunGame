using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject gameOverPanel; // ゲームオーバー時に表示するUIパネル
    [SerializeField] private Text scoreText;           // 今回のスコア表示用
    [SerializeField] private Text rankingText;         // ランキング表示用（1位〜5位）

    [Header("シーン制御への参照")]
    [SerializeField] private GameSceneController sceneController;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (MainGameManager.Instance != null)
        {
            MainGameManager.Instance.OnGameOverEvent += HandleGameOver;
        }
    }

    private void OnDestroy()
    {
        if (MainGameManager.Instance != null)
        {
            MainGameManager.Instance.OnGameOverEvent -= HandleGameOver;
        }
    }

    private void HandleGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // MainGameManagerから最終スコアを取得
        int finalScore = 0;
        if (MainGameManager.Instance != null)
        {
            finalScore = MainGameManager.Instance.CurrentScore;
        }

        // 今回のスコアを表示
        if (scoreText != null)
        {
            scoreText.text = "Score: " + finalScore;
        }

        // ランキングに登録して表示を更新
        if (ScoreRankingManager.Instance != null)
        {
            ScoreRankingManager.Instance.RegisterScore(finalScore);
            UpdateRankingDisplay();
        }
    }

    private void UpdateRankingDisplay()
    {
        if (rankingText == null || ScoreRankingManager.Instance == null) return;

        List<int> scores = ScoreRankingManager.Instance.GetHighScores();
        string rankingStr = "--- RANKING (Top 5) ---\n";

        for (int i = 0; i < scores.Count; i++)
        {
            rankingStr += $"{i + 1}位: {scores[i]}\n";
        }
        // 5件に満たない場合の表示補正
        for (int i = scores.Count; i < 5; i++)
        {
            rankingStr += $"{i + 1}位: ---\n";
        }

        rankingText.text = rankingStr;
    }

    public void OnClickRetry()
    {
        Time.timeScale = 1f;
        if (sceneController != null)
        {
            sceneController.ChangeScene(GameType.Main);
        }
    }

    public void OnClickTitle()
    {
        Time.timeScale = 1f;
        if (sceneController != null)
        {
            sceneController.ChangeScene(GameType.Title);
        }
    }
}
