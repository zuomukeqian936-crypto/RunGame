using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("障害物プレハブのリスト")]
    [SerializeField, Tooltip("ランダムに出現させたい障害物のプレハブをここに登録します")]
    private List<GameObject> obstaclePrefabs = new List<GameObject>();

    [Header("3つの道のX座標設定")]
    [SerializeField, Tooltip("プレイヤーのレーン位置に合わせて調整してください")]
    private float[] laneXPositions = { -8f, -5f, -2f };

    [Header("生成タイミング設定")]
    [SerializeField, Tooltip("何秒ごとに障害物を生成するか")]
    private float spawnInterval = 2f;

    [SerializeField, Tooltip("画面の奥側（生成されるZ座標）")]
    private float spawnZPosition = 60f;

    [SerializeField, Tooltip("障害物の高さ（Y座標）")]
    private float spawnYPosition = 0f;

    private float timer = 0f;

    void Update()
    {
        // プレハブが登録されていなければ処理しない
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0) return;

        timer += Time.deltaTime;

        // 一定時間ごとに障害物を生成
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomObstacle();
        }
    }

    /// <summary>
    /// リストからランダムに障害物を選び、3つの道のどこかに生成する
    /// </summary>
    private void SpawnRandomObstacle()
    {
        // 1. 障害物リストからランダムに1つ選ぶ
        int randomObstacleIndex = Random.Range(0, obstaclePrefabs.Count);
        GameObject selectedPrefab = obstaclePrefabs[randomObstacleIndex];

        // 2. 3つの道（左・真ん中・右）の中からランダムに1つ選ぶ
        int randomLaneIndex = Random.Range(0, laneXPositions.Length);
        float targetX = laneXPositions[randomLaneIndex];

        // 3. 生成位置を決定（奥のZ座標から手前に流れてくる想定）
        Vector3 spawnPosition = new Vector3(targetX, spawnYPosition, spawnZPosition);

        // 4. 障害物を生成
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}
