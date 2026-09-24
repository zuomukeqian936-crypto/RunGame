using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM設定")]
    [SerializeField, Tooltip("BGM再生用のAudioSource")]
    private AudioSource bgmAudioSource;

    [Header("SEプール設定")]
    [SerializeField, Tooltip("同時に鳴らせるSEの最大数（プールするAudioSourceの数）")]
    private int sePoolSize = 5;
    private List<AudioSource> seAudioSourcePool = new List<AudioSource>();

    [Header("オーディオクリップ（音源）")]
    [SerializeField, Tooltip("ボタン音など")]
    private AudioClip buttonSeClip;
    [SerializeField, Tooltip("障害物ヒット時の音")]
    private AudioClip hitSeClip;
    [SerializeField, Tooltip("ゲームオーバー時の音")]
    private AudioClip gameOverSeClip;
    [SerializeField, Tooltip("宝石獲得時の音")]
    private AudioClip gemGetSeClip; // ★追加：宝石獲得用SE
    [SerializeField, Tooltip("メインゲームのBGM")]
    private AudioClip mainBgmClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 起動時に指定された数だけSE用のAudioSourceを自動生成してプールに格納する
        for (int i = 0; i < sePoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            seAudioSourcePool.Add(source);
        }
    }

    /// <summary>
    /// SE（効果音）をプールから空いているAudioSourceを探して再生する
    /// </summary>
    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource targetSource = null;

        // 再生中でない（空いている）AudioSourceを探す
        foreach (var source in seAudioSourcePool)
        {
            if (!source.isPlaying)
            {
                targetSource = source;
                break;
            }
        }

        // すべて使用中の場合は、プール内の最初（または一番古いもの）を強制的に再利用する
        if (targetSource == null && seAudioSourcePool.Count > 0)
        {
            targetSource = seAudioSourcePool[0];
        }

        // 音を再生
        if (targetSource != null)
        {
            targetSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// BGMを再生するメソッド
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (clip != null && bgmAudioSource != null)
        {
            if (bgmAudioSource.clip == clip && bgmAudioSource.isPlaying) return;
            bgmAudioSource.clip = clip;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }
    }

    // --- 個別の呼び出し用ショートカットメソッド ---

    public void PlayButtonSE()
    {
        PlaySE(buttonSeClip);
    }

    public void PlayHitSE()
    {
        PlaySE(hitSeClip);
    }

    public void PlayGameOverSE()
    {
        PlaySE(gameOverSeClip);
    }

    public void PlayGemGetSE() // ★追加：宝石獲得SEを呼び出すメソッド
    {
        PlaySE(gemGetSeClip);
    }

    public void PlayMainBGM()
    {
        PlayBGM(mainBgmClip);
    }
}