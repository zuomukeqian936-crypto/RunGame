using System;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    [Header("スコア関連")]
    [SerializeField, Tooltip("走る前のスタート地点のZ座標")] float startZ;
    [SerializeField] int score = 0;

    [Header("ゲーム状態")]
    [SerializeField, Tooltip("ゲームオーバーかどうかの判定")] bool isGameOver = false;

    // ゲームオーバー時に発火するイベント
    public event Action OnGameOverEvent;

    // ポイントが加算された時に現在のスコアを通知するイベント (引数にスコアを渡す)
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
        // スタート時のZ座標を記録しておく
        startZ = transform.position.z;
    }

    private void Update()
    {
        if (isGameOver) return;

        // --- Z座標に応じたポイント加算の計算 ---
        // プレイヤーの現在のZ座標から進んだ距離を計算し、それをスコアにする
        int calculatedScore = Mathf.FloorToInt(transform.position.z - startZ);

        if (calculatedScore > score)
        {
            score = calculatedScore;
          
            OnScoreChangedEvent?.Invoke(score);
        }
    }

    /// <summary>
    /// ゲームオーバー処理をトリガーにイベント発火処理
    /// </summary>
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("ゲームオーバー！");

        OnGameOverEvent?.Invoke();
    }
}
