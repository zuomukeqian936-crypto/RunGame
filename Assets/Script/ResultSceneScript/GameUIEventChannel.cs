using System;
using UnityEngine;

public class GameUIEventChannel : MonoBehaviour
{
    public static GameUIEventChannel Instance { get; private set; }

    // UI表示用のゲームオーバーイベント
    public event Action OnShowGameOverUIEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// UI用のゲームオーバーイベントを発火する
    /// </summary>
    public void RaiseShowGameOverUI()
    {
        OnShowGameOverUIEvent?.Invoke();
    }
}