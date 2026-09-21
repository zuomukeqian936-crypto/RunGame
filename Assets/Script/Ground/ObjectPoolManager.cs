using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // プレハブごとのオブジェクトプールを保持する辞書
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    /// <summary>
    ///プールの初期化（あらかじめインスタンスを作っておく）
    /// </summary>
    public void InitializePool(List<GameObject> prefabs, int initialSize)
    {
        if (prefabs == null) return;

        foreach (var prefab in prefabs)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                poolDictionary.Add(prefab, new Queue<GameObject>());

                for (int i = 0; i < initialSize; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    poolDictionary[prefab].Enqueue(obj);
                }
            }
        }
    }

    /// <summary>
    ///プールからオブジェクトを取得する（足りなければ新しく生成する）
    /// </summary>
    public GameObject Get(GameObject prefab)
    {
        if (poolDictionary.ContainsKey(prefab) && poolDictionary[prefab].Count > 0)
        {
            GameObject obj = poolDictionary[prefab].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // 足りない場合は新規生成
        return Instantiate(prefab);
    }

    /// <summary>
    ///オブジェクトをプールに返却する
    /// </summary>
    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
