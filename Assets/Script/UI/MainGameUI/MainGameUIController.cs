using UnityEngine;
using UnityEngine.UI;

public class MainGameUIController : MonoBehaviour
{
    [Header("UI要素の参照")]
    [SerializeField, Tooltip("画面にスコアを表示するUI Text")]
    private Text scoreText;

    private void Start()
    {
        // MainGameManagerのイベントにリスナーを登録
        if (MainGameManager.Instance != null)
        {
            MainGameManager.Instance.OnScoreChangedEvent += UpdateScoreText;

            // 初期スコアを反映
            UpdateScoreText(MainGameManager.Instance.CurrentScore);
        }
    }

    private void OnDestroy()
    {
        // メモリリークやエラー防止のため、イベントの登録解除を行う
        if (MainGameManager.Instance != null)
        {
            MainGameManager.Instance.OnScoreChangedEvent -= UpdateScoreText;
        }
    }

    /// <summary>
    /// スコアテキストを更新する
    /// </summary>
    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score : {score:D6}";
        }
    }
}
