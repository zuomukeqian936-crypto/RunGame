using System.Collections.Generic;
using UnityEngine;

public class MaterialStrengthController : MonoBehaviour
{
    [Header("操作したいマテリアルをここに直接ドラッグ＆ドロップしてください")]
    [SerializeField] private List<Material> targetMaterials = new List<Material>();

    [Header("上：Sideways_Strength の設定")]
    [SerializeField] private float sidewaysMin = -1f;
    [SerializeField] private float sidewaysMax = 1f;
    [SerializeField] private float sidewaysChangeInterval = 2f;
    [SerializeField] private float sidewaysChangeSpeed = 2f;
    private float currentSideways;
    private float targetSideways;
    private float sidewaysTimer;

    [Header("下：Backwards_Strength の設定")]
    [SerializeField] private float backwardsMin = -1f;
    [SerializeField] private float backwardsMax = 1f;
    [SerializeField] private float backwardsChangeInterval = 2f;
    [SerializeField] private float backwardsChangeSpeed = 2f;
    private float currentBackwards;
    private float targetBackwards;
    private float backwardsTimer;

    private void Start()
    {
        if (targetMaterials == null || targetMaterials.Count == 0)
        {
            Debug.LogWarning("操作するマテリアルが設定されていません。", this);
            enabled = false;
            return;
        }

        targetSideways = Random.Range(sidewaysMin, sidewaysMax);
        currentSideways = targetSideways;

        targetBackwards = Random.Range(backwardsMin, backwardsMax);
        currentBackwards = targetBackwards;

        ApplyShaderValues();
    }

    private void Update()
    {
        if (targetMaterials.Count == 0) return;

        sidewaysTimer += Time.deltaTime;
        if (sidewaysTimer >= sidewaysChangeInterval)
        {
            sidewaysTimer = 0f;
            targetSideways = Random.Range(sidewaysMin, sidewaysMax);
        }

        // 【重要】MoveTowards ではなく、Lerp や SmoothDamp を使うと、目標値が変わった瞬間も急にカクッとならず滑らかになります
        // 第3引数の値を小さくする（例: 2f * Time.deltaTime など）と、よりゆっくりジワッと変化します
        currentSideways = Mathf.Lerp(currentSideways, targetSideways, sidewaysChangeSpeed * Time.deltaTime);

        backwardsTimer += Time.deltaTime;
        if (backwardsTimer >= backwardsChangeInterval)
        {
            backwardsTimer = 0f;
            targetBackwards = Random.Range(backwardsMin, backwardsMax);
        }
        currentBackwards = Mathf.Lerp(currentBackwards, targetBackwards, backwardsChangeSpeed * Time.deltaTime);

        ApplyShaderValues();
    }

    private void ApplyShaderValues()
    {
        foreach (var mat in targetMaterials)
        {
            if (mat != null)
            {
                mat.SetFloat("Sideways_Strength", currentSideways);
                mat.SetFloat("Backwards_Strength", currentBackwards);
            }
        }
    }
}

