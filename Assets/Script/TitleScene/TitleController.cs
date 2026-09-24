using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween用

public class TitleController : MonoBehaviour
{
    [Header("フェード設定")]
    [SerializeField, Tooltip("画面フェード用の黒幕UI Image")]
    private Image _fadePanel;
    [SerializeField, Tooltip("フェードアウトにかかる時間（秒）")]
    private float _fadeDuration = 1.0f;

    [Header("シーン制御への参照")]
    [SerializeField] private GameSceneController _sceneController;

    [Header("UI要素")]
    [SerializeField, Tooltip("操作用テキスト")]
    private Text _instructionText;
    [SerializeField, Tooltip("最初に表示する操作説明ボタン（またはスタート前のメニューボタン）")]
    private Button _instructionButton;
    [SerializeField, Tooltip("ゲームスタートボタン")]
    private Button _gameStartButton;
    [SerializeField, Tooltip("キャンセルボタン")]
    private Button _cancelButton;

    private void Start()
    {
        // タイトルシーン開始時にフェードパネルがあれば、明るくする（フェードイン）
        if (_fadePanel != null)
        {
            _fadePanel.gameObject.SetActive(true);
            Color c = _fadePanel.color;
            c.a = 1f;
            _fadePanel.color = c;

            _fadePanel.DOFade(0f, _fadeDuration).OnComplete(() => {
                _fadePanel.gameObject.SetActive(false);
            });
        }

        // 初期状態：_instructionButtonを表示し、操作テキスト・ゲームスタート・キャンセルは非表示にする
        SetInitialState();
    }

    /// <summary>
    /// 初期状態にする（最初のボタンだけ表示）
    /// </summary>
    private void SetInitialState()
    {
        if (_instructionButton != null) _instructionButton.gameObject.SetActive(true);
        if (_instructionText != null) _instructionText.gameObject.SetActive(false);
        if (_gameStartButton != null) _gameStartButton.gameObject.SetActive(false);
        if (_cancelButton != null) _cancelButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 最初に _instructionButton を押したときの処理
    /// </summary>
    public void OnClickInstructionButton()
    {
        // _instructionButtonを非表示にする
        if (_instructionButton != null)
        {
            _instructionButton.gameObject.SetActive(false);
        }

        // _instructionText, _gameStartButton, _cancelButton を表示する
        if (_instructionText != null) _instructionText.gameObject.SetActive(true);
        if (_gameStartButton != null) _gameStartButton.gameObject.SetActive(true);
        if (_cancelButton != null) _cancelButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// _gameStartButton が押されたとき（実際にゲームへ移行）
    /// </summary>
    public void OnClickGameStart()
    {
        StartCoroutine(StartGameFadeCoroutine());
    }

    /// <summary>
    /// _cancelButton が押されたとき（初期状態に戻す）
    /// </summary>
    public void OnClickCancel()
    {
        SetInitialState();
    }

    private IEnumerator StartGameFadeCoroutine()
    {
        // 画面を暗くする（フェードアウト）
        if (_fadePanel != null)
        {
            _fadePanel.gameObject.SetActive(true);
            yield return _fadePanel.DOFade(1f, _fadeDuration).WaitForCompletion();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // メインゲームシーンへ移動
        if (_sceneController != null)
        {
            _sceneController.ChangeScene(GameType.Main);
        }
    }
}