using UnityEngine;

public class GroundController : MonoBehaviour
{
    [Header("スピード設定")]
    [SerializeField, Tooltip("移動の基本方向（Z軸マイナス手前方向など）")]
    private Vector3 moveDirection = new Vector3(0f, 0f, -1f);

    [SerializeField, Tooltip("ゲーム開始時の初期スピード")]
    private float initialSpeed = 5f;

    [SerializeField, Tooltip("到達する最高スピード")]
    private float maxSpeed = 20f;

    [SerializeField, Tooltip("1秒間にどれくらいスピードが上がるか（加速度）")]
    private float acceleration = 0.5f;

    private float currentSpeed;

    void Start()
    {
        // 初期スピードをセット
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        // 最高スピードに達するまで、時間経過とともに徐々にスピードを上げる
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed); // maxSpeedを超えないように制限
        }

        // 現在のスピードを反映してマップを移動させる
        transform.position += moveDirection.normalized * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 外部から強制的にスピードを変更・リセットしたい場合に呼び出せるメソッド
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
}
