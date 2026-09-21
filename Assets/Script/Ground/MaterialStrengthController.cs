using UnityEngine;

public class MaterialStrengthController : MonoBehaviour
{
    [Header("対象のオブジェクト（Renderer）")]
    [SerializeField] private Renderer targetRenderer;
    private Material targetMaterial;

    [Header("上：Sideways_Strength の設定")]
    [SerializeField] private float sidewaysMin = -0.0005f;
    [SerializeField] private float sidewaysMax = 0.0005f;
    [SerializeField] private float sidewaysChangeInterval = 2f; // 新しい目標値に切り替わるまでの時間（秒）
    [SerializeField] private float sidewaysChangeSpeed = 2f;   // 変化の追従スピード
    private float currentSideways;
    private float targetSideways;
    private float sidewaysTimer;

    [Header("下：Backwards_Strength の設定")]
    [SerializeField] private float backwardsMin = -0.002f;
    [SerializeField] private float backwardsMax = 0.002f;
    [SerializeField] private float backwardsChangeInterval = 2f; // 新しい目標値に切り替わるまでの時間（秒）
    [SerializeField] private float backwardsChangeSpeed = 2f;   // 変化の追従スピード
    private float currentBackwards;
    private float targetBackwards;
    private float backwardsTimer;

    private void Start()
    {
        // 自分自身（または子孫）の Renderer を自動で取得する
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }
        }

        if (targetRenderer != null)
        {
            // プレハブごとに固有のマテリアルインスタンスを作成して操作する
            targetMaterial = targetRenderer.material;
        }
        else
        {
            Debug.LogWarning("Renderer が見つかりませんでした。", this);
            enabled = false;
            return;
        }

        // 初期値をランダムな目標値で初期化
        targetSideways = Random.Range(sidewaysMin, sidewaysMax);
        currentSideways = targetSideways;

        targetBackwards = Random.Range(backwardsMin, backwardsMax);
        currentBackwards = targetBackwards;

        ApplyShaderValues();
    }

    private void Update()
    {
        if (targetMaterial == null) return;

        // --- 上 (Sideways_Strength) のランダム変化処理 ---
        sidewaysTimer += Time.deltaTime;
        if (sidewaysTimer >= sidewaysChangeInterval)
        {
            sidewaysTimer = 0f;
            // 指定された範囲（-0.0005 ～ 0.0005）からランダムに次の目標値を決める
            targetSideways = Random.Range(sidewaysMin, sidewaysMax);
        }
        // 現在の値から目標値へ滑らかに近づける
        currentSideways = Mathf.MoveTowards(currentSideways, targetSideways, sidewaysChangeSpeed * Time.deltaTime);


        // --- 下 (Backwards_Strength) のランダム変化処理 ---
        backwardsTimer += Time.deltaTime;
        if (backwardsTimer >= backwardsChangeInterval)
        {
            backwardsTimer = 0f;
            // 指定された範囲（-0.002 ～ 0.002）からランダムに次の目標値を決める
            targetBackwards = Random.Range(backwardsMin, backwardsMax);
        }
        // 現在の値から目標値へ滑らかに近づける
        currentBackwards = Mathf.MoveTowards(currentBackwards, targetBackwards, backwardsChangeSpeed * Time.deltaTime);


        // シェーダーに値を適用
        ApplyShaderValues();
    }

    /// <summary>
    ///シェーダーのプロパティに現在の値をセットする
    /// </summary>
    private void ApplyShaderValues()
    {
        targetMaterial.SetFloat("Sideways_Strength", currentSideways);
        targetMaterial.SetFloat("Backwards_Strength", currentBackwards);
    }
}
