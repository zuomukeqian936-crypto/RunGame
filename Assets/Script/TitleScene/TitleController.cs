using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween用

public class TitleController : MonoBehaviour
{
    [Header("フェード設定")]
    [SerializeField, Tooltip("画面フェード用の黒幕UI Image")]
    private Image fadePanel;
    [SerializeField, Tooltip("フェードアウトにかかる時間（秒）")]
    private float fadeDuration = 1.0f;

    [Header("シーン制御への参照")]
    [SerializeField] private GameSceneController sceneController;

    private void Start()
    {
        // タイトルシーン開始時にフェードパネルがあれば、明るくする（フェードイン）
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;

            fadePanel.DOFade(0f, fadeDuration).OnComplete(() => {
                fadePanel.gameObject.SetActive(false);
            });
        }
    }

    /// <summary>
    /// タイトルの「ゲームスタート」ボタンから呼び出すメソッド
    /// </summary>
    public void OnClickGameStart()
    {
        StartCoroutine(StartGameFadeCoroutine());
    }

    private System.Collections.IEnumerator StartGameFadeCoroutine()
    {
        // 画面を暗くする（フェードアウト）
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            yield return fadePanel.DOFade(1f, fadeDuration).WaitForCompletion();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // メインゲームシーンへ移動
        if (sceneController != null)
        {
            sceneController.ChangeScene(GameType.Main);
        }
    }
}