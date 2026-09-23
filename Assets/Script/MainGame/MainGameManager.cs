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

        isGameStarted = true;
    }

    private void Update()
    {
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
            // 1回目のヒット：蛇の座標を一段階前に戻す処理
            RewindPlayerPosition();
        }
        else
        {
            // 2回目のヒット：ゲームオーバーへ繋げる
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
            // 例として、進行方向の逆（手前）に少し座標を戻す（ゲームの仕様に合わせて軸を調整してください）
            // 縦スクロールや奥に進むゲームの場合は Z や Y を調整します
            Vector3 pos = playerTransform.position;
            pos.z -= rewindDistance; // 例：Z軸を後ろに戻す場合
            playerTransform.position = pos;

            Debug.Log("蛇の座標を一段階前に戻しました。");
        }
        else
        {
            Debug.LogWarning("Player Transform がアタッチされていません。");
        }
    }

    /// <summary>
    /// 蛇に触れた（または2回目の障害物ヒット）ときなどに呼び出すゲームオーバーメソッド
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
    }

    public void TriggerGameOver()
    {
        OnSnakeHit();
    }
}