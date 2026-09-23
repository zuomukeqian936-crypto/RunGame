using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween用

public class GameOverController : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject gameOverPanel; // ゲームオーバー時に表示するUIパネル
    [SerializeField] private Text scoreText;           // 今回のスコア表示用
    [SerializeField] private Text rankingText;         // ランキング表示用（1位〜5位）

    [Header("フェード設定（DOTween用）")]
    [SerializeField, Tooltip("画面フェード用のUI Image（黒幕など）")]
    private Image fadePanel;
    [SerializeField, Tooltip("フェードアウト/インにかかる時間（秒）")]
    private float fadeDuration = 1.0f;

    [Header("シーン制御への参照")]
    [SerializeField] private GameSceneController sceneController;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // リザルト画面が開いたときに、もし黒幕があればパッと明るくする（フェードイン）
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
            fadePanel.DOFade(0f, fadeDuration).OnComplete(() => {
                fadePanel.gameObject.SetActive(false);
            });
        }
    }

    /// <summary>
    /// MainGameManagerから直接呼び出されるメソッド
    /// </summary>
    public void ShowGameOver()
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

        // 今回のスコアを表示（6桁ゼロ埋め指定）
        if (scoreText != null)
        {
            scoreText.text = $"Score : {finalScore:D6}";
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
            rankingStr += $"{i + 1}位 : {scores[i]:D6}\n";
        }
        // 5件に満たない場合の表示補正
        for (int i = scores.Count; i < 5; i++)
        {
            rankingStr += $"{i + 1}位 : ------\n";
        }

        rankingText.text = rankingStr;
    }

    public void OnClickRetry()
    {
        Time.timeScale = 1f;
        // フェードアウトを挟んでからメインシーンへ
        StartCoroutine(FadeAndChangeScene(GameType.Main));
    }

    public void OnClickTitle()
    {
        Time.timeScale = 1f;
        // フェードアウトを挟んでからタイトルシーンへ
        StartCoroutine(FadeAndChangeScene(GameType.Title));
    }

    /// <summary>
    /// 画面を暗くしてからシーンを切り替えるコルーチン
    /// </summary>
    private IEnumerator FadeAndChangeScene(GameType nextScene)
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            yield return fadePanel.DOFade(1f, fadeDuration).WaitForCompletion();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (sceneController != null)
        {
            sceneController.ChangeScene(nextScene);
        }
    }
}