using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [Header("プール管理")]
    [SerializeField] private ObjectPoolManager poolManager; // プール用クラスの参照

    [Header("プレハブのリスト")]
    [SerializeField] private List<GameObject> groundPrefabs; // 地面のプレハブリスト
    [SerializeField] private List<GameObject> tunnelPrefabs; // トンネルのプレハブリスト

    [Header("生成設定")]
    [SerializeField] private Transform playerTransform; // プレイヤーのtransform
    [SerializeField] private float spawnZ = 0f;          // 次に生成するZ座標の位置
    [SerializeField] private int maxPartsCount = 5;      // 画面上に常に維持するパーツの数
    [SerializeField] private float spawnDistanceCheck = 50f; // プレイヤーがこの距離まで来たら先を生成
    [SerializeField] private int initialPoolSize = 3;    // 各プレハブの初期プール数

    // 画面上でアクティブなパーツの情報を保持する構造体
    private struct ActivePartInfo
    {
        public GameObject gameObject;
        public GameObject originalPrefab;
    }

    // 現在アクティブなパーツのキュー
    private Queue<ActivePartInfo> activeParts = new Queue<ActivePartInfo>();

    private void Start()
    {
        if (poolManager == null)
        {
            poolManager = gameObject.AddComponent<ObjectPoolManager>();
        }

        // プールの事前準備
        poolManager.InitializePool(groundPrefabs, initialPoolSize);
        poolManager.InitializePool(tunnelPrefabs, initialPoolSize);

        // 初期パーツを生成して並べる
        for (int i = 0; i < maxPartsCount; i++)
        {
            SpawnRandomPart();
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // 一定距離進んだら新しいパーツを生成し、古いものをプールに戻す
        if (spawnZ - playerTransform.position.z < spawnDistanceCheck)
        {
            SpawnRandomPart();
            ReturnOldPartToPool();
        }
    }

    /// <summary>
    ///ランダムにパーツを選び、プールから取り出して配置する
    /// </summary>
    private void SpawnRandomPart()
    {
        bool isTunnel = (Random.value < 0.5f) && (tunnelPrefabs != null && tunnelPrefabs.Count > 0);
        List<GameObject> targetList = isTunnel ? tunnelPrefabs : groundPrefabs;

        if (targetList == null || targetList.Count == 0) return;

        int randomIndex = Random.Range(0, targetList.Count);
        GameObject selectedPrefab = targetList[randomIndex];

        float partLength = GetPartLength(selectedPrefab);
        Vector3 spawnPosition = new Vector3(0, 0, spawnZ + (partLength / 2f));

        // プールから取得
        GameObject partObj = poolManager.Get(selectedPrefab);
        partObj.transform.position = spawnPosition;
        partObj.transform.rotation = Quaternion.identity;

        // 管理キューに追加
        activeParts.Enqueue(new ActivePartInfo
        {
            gameObject = partObj,
            originalPrefab = selectedPrefab
        });

        spawnZ += partLength;
    }

    /// <summary>
    ///古いパーツをプールに返却する
    /// </summary>
    private void ReturnOldPartToPool()
    {
        if (activeParts.Count > maxPartsCount)
        {
            ActivePartInfo oldPartInfo = activeParts.Dequeue();
            poolManager.ReturnToPool(oldPartInfo.originalPrefab, oldPartInfo.gameObject);
        }
    }

    /// <summary>
    ///プレハブの長さ（Z軸）を取得
    /// </summary>
    private float GetPartLength(GameObject prefab)
    {
        BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
        if (boxCollider != null) return boxCollider.size.z * prefab.transform.localScale.z;

        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (renderer != null) return renderer.bounds.size.z;

        return 10f;
    }
}
