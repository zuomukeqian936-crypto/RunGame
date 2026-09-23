using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [Header("プール管理")]
    [SerializeField] private ObjectPoolManager poolManager;

    [Header("プレハブのリスト")]
    [SerializeField] private List<GameObject> stagePrefabs;

    [Header("生成設定")]
    [SerializeField] private int maxPartsCount = 5;      // 画面上に維持するパーツの数
    [SerializeField] private float partLength = 60f;     // 洞窟パーツ1個のZ方向の長さ（実際の長さに合わせて調整）
    [SerializeField] private float recycleZ = -40f;      // このZ座標より手前に来たら、奥へワープさせるライン
    [SerializeField] private int initialPoolSize = 5;

    // 現在アクティブなパーツの管理用キュー
    private Queue<ActivePartInfo> activeParts = new Queue<ActivePartInfo>();

    private struct ActivePartInfo
    {
        public GameObject gameObject;
        public GameObject originalPrefab;
    }

    private void Start()
    {
        if (poolManager == null)
        {
            poolManager = gameObject.AddComponent<ObjectPoolManager>();
        }

        poolManager.InitializePool(stagePrefabs, initialPoolSize);

        // 初期パーツを奥に向かって順番に並べる（例: Z = 0, 60, 120...）
        for (int i = 0; i < maxPartsCount; i++)
        {
            SpawnPartAt(i * partLength);
        }
    }

    private void Update()
    {
        if (activeParts.Count == 0) return;

        // 一番手前にあるパーツをチェック
        GameObject oldestPart = activeParts.Peek().gameObject;

        // そのパーツが手前のライン（recycleZ）より後ろ（マイナス側）に行ったら、一番奥に移動させる
        if (oldestPart.transform.position.z < recycleZ)
        {
            RecycleAndRepositionOldestPart();
        }
    }

    private void SpawnPartAt(float zPosition)
    {
        if (stagePrefabs == null || stagePrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, stagePrefabs.Count);
        GameObject selectedPrefab = stagePrefabs[randomIndex];

        GameObject partObj = poolManager.GetToPool(selectedPrefab);
        partObj.transform.position = new Vector3(0, 0, zPosition);
        partObj.transform.rotation = Quaternion.identity;

        activeParts.Enqueue(new ActivePartInfo
        {
            gameObject = partObj,
            originalPrefab = selectedPrefab
        });
    }

    private void RecycleAndRepositionOldestPart()
    {
        // キューから一番古いパーツを取り出す
        ActivePartInfo oldPart = activeParts.Dequeue();

        // 現在一番奥にあるパーツのZ座標を見つける
        float maxZ = float.MinValue;
        foreach (var part in activeParts)
        {
            if (part.gameObject.transform.position.z > maxZ)
            {
                maxZ = part.gameObject.transform.position.z;
            }
        }

        // 一番奥のパーツのさらに「パーツ1個分先」に配置する
        float nextZ = maxZ + partLength;
        oldPart.gameObject.transform.position = new Vector3(0, 0, nextZ);

        // キューの最後尾（一番新しい扱い）に戻す
        activeParts.Enqueue(oldPart);
    }
}