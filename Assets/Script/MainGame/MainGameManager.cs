using System;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    [Header("スコア関連")]
    [SerializeField, Tooltip("走る前のスタート地点のZ座標")] float startZ;
    [SerializeField] int score = 0;

    // 外部から現在のスコアを参照できるようにするプロパティ
    public int CurrentScore => score;

    [Header("ゲーム状態")]
    [SerializeField, Tooltip("ゲームオーバーかどうかの判定")] bool isGameOver = false;

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
        startZ = transform.position.z;
    }

    private void Update()
    {
        if (isGameOver) return;

        int calculatedScore = Mathf.FloorToInt(transform.position.z - startZ);

        if (calculatedScore > score)
        {
            score = calculatedScore;
            OnScoreChangedEvent?.Invoke(score);
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("ゲームオーバー！");

        OnGameOverEvent?.Invoke();
    }
}
