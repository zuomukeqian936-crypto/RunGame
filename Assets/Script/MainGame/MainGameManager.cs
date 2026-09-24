using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween用

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    [Header("距離・スコア関連")]
    [SerializeField, Tooltip("ステージ（地面）の移動スピード")]
    private float scrollSpeed = 5f;
    [SerializeField] int score = 0;

    public int CurrentScore => score;

    [Header("ゲーム状態")]
    [SerializeField] bool isGameOver = false;
    [SerializeField] bool isGameStarted = false; // カウントダウンが終わりゲームが開始したか

    // 外部のスクリプトからゲームが開始しているか確認できるようにするためのプロパティ
    public bool IsGameStarted => isGameStarted;

    [Header("障害物ヒット設定")]
    [SerializeField, Tooltip("障害物に当たれる許容回数（2回目でゲームオーバー）")]
    private int maxObstacleHits = 2;
    private int currentObstacleHits = 0;
    [SerializeField, Tooltip("1回当たったときに戻る距離（または位置の調整値）")]
    private float rewindDistance = 2.0f;
    [SerializeField, Tooltip("プレイヤー（蛇）のTransform")]
    private Transform playerTransform;

    [Header("カウントダウンUI設定")]
    [SerializeField, Tooltip("画面中央などに配置するカウントダウン表示用のText")]
    private Text countdownText;

    [Header("フェード設定（DOTween用）")]
    [SerializeField, Tooltip("画面フェード用のUI Image（黒幕など）")]
    private Image fadePanel;
    [SerializeField, Tooltip("フェードアウト/インにかかる時間（秒）")]
    private float fadeDuration = 1.0f;

    [Header("シーン遷移設定")]
    [SerializeField, Tooltip("シーン遷移用コントローラー")]
    private GameSceneController gameSceneController;

    private float elapsedTime = 0f;

    public event Action OnGameOverEvent;
    public event Action<int> OnScoreChangedEvent;

    private void Awake()
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

    private void Start()
    {
        // シーン開始時にフェードインとカウントダウンのシーケンスを開始
        StartCoroutine(GameStartSequence());
    }

    /// <summary>
    /// ゲーム開始時のフェードイン ＆ 3秒間カウントダウン演出
    /// </summary>
    private IEnumerator GameStartSequence()
    {
        // 最初はゲームが始まっていない状態を明示
        isGameStarted = false;

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;

            yield return fadePanel.DOFade(0f, fadeDuration).WaitForCompletion();
            fadePanel.gameObject.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            countdownText.text = "3";
            yield return new WaitForSeconds(1f);

            countdownText.text = "2";
            yield return new WaitForSeconds(1f);

            countdownText.text = "1";
            yield return new WaitForSeconds(1f);

            countdownText.text = "START!";
            yield return new WaitForSeconds(0.5f);

            countdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        // カウントダウン終了後にゲーム開始フラグをオンにする
        isGameStarted = true;
    }

    private void Update()
    {
        // カウントダウン中（isGameStartedがfalse）またはゲームオーバー時は処理を進めない
        if (!isGameStarted || isGameOver) return;

        elapsedTime += Time.deltaTime;
        int calculatedScore = Mathf.FloorToInt(elapsedTime * scrollSpeed);

        if (calculatedScore > score)
        {
            score = calculatedScore;
            OnScoreChangedEvent?.Invoke(score);
        }
    }

    /// <summary>
    /// 障害物に当たったときに呼び出すメソッド
    /// </summary>
    public void OnObstacleHit()
    {
        if (isGameOver || !isGameStarted) return;

        currentObstacleHits++;
        Debug.Log($"障害物にヒット！ 現在の被弾回数: {currentObstacleHits} / {maxObstacleHits}");

        if (currentObstacleHits < maxObstacleHits)
        {
            RewindPlayerPosition();
        }
        else
        {
            OnSnakeHit();
        }
    }

    /// <summary>
    /// プレイヤー（蛇）の座標を一段階前に戻す
    /// </summary>
    private void RewindPlayerPosition()
    {
        if (playerTransform != null)
        {
            Vector3 pos = playerTransform.position;
            pos.z -= rewindDistance;
            playerTransform.position = pos;

            Debug.Log("蛇の座標を一段階前に戻しました。");
        }
        else
        {
            Debug.LogWarning("Player Transform がアタッチされていません。");
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void OnSnakeHit()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("ゲームオーバー！フェードアウト演出開始");

        StartCoroutine(GameOverFadeCoroutine());
    }

    private IEnumerator GameOverFadeCoroutine()
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

        OnGameOverEvent?.Invoke();

        if (GameUIEventChannel.Instance != null)
        {
            GameUIEventChannel.Instance.RaiseShowGameOverUI();
        }

        // --- 変更: JSON形式でデータを保存する ---
        GameData data = new GameData();
        data.finalScore = score;

        string jsonString = JsonUtility.ToJson(data);
        string filePath = Application.persistentDataPath + "/gamedata.json";

        System.IO.File.WriteAllText(filePath, jsonString);
        Debug.Log($"JSONでスコアを保存しました: {filePath}");

        // リザルトシーンへの遷移
        if (gameSceneController != null)
        {
            gameSceneController.ChangeScene(GameType.Result);
        }
        else
        {
            GameSceneController controller = FindObjectOfType<GameSceneController>();
            if (controller != null)
            {
                controller.ChangeScene(GameType.Result);
            }
            else
            {
                Debug.LogError("GameSceneController がシーン内に見つかりません。");
            }
        }
    }

    public void TriggerGameOver()
    {
        OnSnakeHit();
    }
}