using System.Collections.Generic;
using UnityEngine;

public class ScoreRankingManager : MonoBehaviour
{
    public static ScoreRankingManager Instance { get; private set; }

    private const int MaxRankingCount = 5;
    private const string ScoreKeyPrefix = "RankingScore_";

    private List<int> highScores = new List<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// スコアをランキングに登録し、上位5件を維持する（5位より低い場合は自動で除外される）
    /// </summary>
    public void RegisterScore(int newScore)
    {
        highScores.Add(newScore);
        // 降順（高い順）にソート
        highScores.Sort((a, b) => b.CompareTo(a));

        // 5位より多くなった分（6位以降）を自動削除
        if (highScores.Count > MaxRankingCount)
        {
            highScores.RemoveRange(MaxRankingCount, highScores.Count - MaxRankingCount);
        }

        SaveScores();
    }

    /// <summary>
    /// 現在のランキングリストを取得する
    /// </summary>
    public List<int> GetHighScores()
    {
        return new List<int>(highScores);
    }

    private void SaveScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt(ScoreKeyPrefix + i, highScores[i]);
        }
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        highScores.Clear();
        for (int i = 0; i < MaxRankingCount; i++)
        {
            string key = ScoreKeyPrefix + i;
            if (PlayerPrefs.HasKey(key))
            {
                highScores.Add(PlayerPrefs.GetInt(key));
            }
        }
        highScores.Sort((a, b) => b.CompareTo(a));
    }
}
