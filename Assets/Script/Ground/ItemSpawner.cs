using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("プレハブ設定")]
    [SerializeField, Tooltip("障害物のプレハブリスト")]
    private List<GameObject> obstaclePrefabs = new List<GameObject>();

    [SerializeField, Tooltip("取得用チーズ（ポイント）のプレハブ")]
    private GameObject cheesePrefab;

    [Header("3つのレーンのX座標設定")]
    [SerializeField, Tooltip("プレイヤーのレーン位置と合わせる")]
    private float[] laneXPositions = { -8f, -5f, -2f };

    [Header("生成タイミング・位置設定")]
    [SerializeField] private float spawnInterval = 2.5f;   // 生成間隔（秒）
    [SerializeField] private float spawnZPosition = 60f;   // 生成される奥のZ座標
    [SerializeField] private float normalCheeseY = 0.5f;   // 通常の高さ（チーズ）
    [SerializeField] private float highCheeseY = 3.0f;     // 障害物がある場合の高い位置（チーズ）
    [SerializeField] private float obstacleY = 0f;         // 障害物の高さ

    private float timer = 0f;

    void Update()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0 || cheesePrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnObstaclesAndCheese();
        }
    }

    /// <summary>
    /// 3つのレーンを考慮して障害物とチーズをバランスよく生成する
    /// </summary>
    private void SpawnObstaclesAndCheese()
    {
        // 1. 各レーンに障害物を置くかどうかを決定
        bool[] hasObstacle = new bool[laneXPositions.Length];
        int obstacleCount = 0;

        // 「完全な障害物（3つ全てが塞がる状態）」を防ぐため、最大でも「レーン数 - 1」個までにする
        int maxObstacles = laneXPositions.Length - 1;

        for (int i = 0; i < laneXPositions.Length; i++)
        {
            // 50%の確率で障害物を配置、かつ最大数に達していない場合
            if (Random.value > 0.4f && obstacleCount < maxObstacles)
            {
                hasObstacle[i] = true;
                obstacleCount++;
            }
        }

        // 万が一、一つも障害物が選ばれなかった場合は、ランダムで1つだけ確実に置く
        if (obstacleCount == 0)
        {
            int safeIndex = Random.Range(0, laneXPositions.Length);
            hasObstacle[safeIndex] = true;
        }

        // 2. 各レーンの判定結果に基づいて、障害物とチーズを生成
        for (int i = 0; i < laneXPositions.Length; i++)
        {
            float targetX = laneXPositions[i];

            if (hasObstacle[i])
            {
                // 【障害物があるレーン】
                // 障害物を生成
                int randomObsIndex = Random.Range(0, obstaclePrefabs.Count);
                Vector3 obstaclePos = new Vector3(targetX, obstacleY, spawnZPosition);
                Instantiate(obstaclePrefabs[randomObsIndex], obstaclePos, Quaternion.identity);

                // 障害物がある場所は「少し高い場所」にチーズを配置（ジャンプして取る）
                Vector3 cheesePos = new Vector3(targetX, highCheeseY, spawnZPosition);
                Instantiate(cheesePrefab, cheesePos, Quaternion.identity);
            }
            else
            {
                // 【障害物がない安全なレーン】
                // 一定の確率で、通常の高さにチーズを配置する
                if (Random.value > 0.3f)
                {
                    Vector3 cheesePos = new Vector3(targetX, normalCheeseY, spawnZPosition);
                    Instantiate(cheesePrefab, cheesePos, Quaternion.identity);
                }
            }
        }
    }
}
