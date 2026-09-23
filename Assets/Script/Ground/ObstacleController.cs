using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField, Tooltip("洞窟と同期する移動スピード（MainGameManagerから自動取得する場合は0でも可）")]
    private float moveSpeed = 5f;
    [SerializeField, Tooltip("MainGameManagerのスピードと同期するかどうか")]
    private bool syncWithManager = true;

    private void Update()
    {
        // スピードの決定（マネージャーと同期、または手動設定）
        float currentSpeed = moveSpeed;
        if (syncWithManager && MainGameManager.Instance != null)
        {
            // ※MainGameManager側で scrollSpeed を public またはプロパティ公開しておく必要があります
            // 例: public float ScrollSpeed => scrollSpeed;
            // ここでは仮に直接参照できる形として記述しています
        }

        // 洞窟と同じ方向へ移動させる（例: 手前に流れてくる場合は -Z方向 や -Y方向）
        // ※ゲームの仕様（縦スクロール、横スクロール、奥に進むなど）に合わせて軸（Translate）を変更してください
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) // 3Dの場合は OnTriggerEnter(Collider collision)
    {
        // プレイヤー（蛇）に触れたかどうかの判定
        if (collision.CompareTag("Player"))
        {
            if (MainGameManager.Instance != null)
            {
                // MainGameManagerの障害物ヒット処理を呼び出す
                MainGameManager.Instance.OnObstacleHit();
            }

            // 必要に応じて、当たった後にこの障害物オブジェクトを消す場合
            // Destroy(gameObject);
        }
    }
}