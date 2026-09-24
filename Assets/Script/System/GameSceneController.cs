using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    public void ChangeScene(GameType sceneType)
    {
        switch (sceneType)
        {
            case GameType.Title:
                SceneManager.LoadScene("TitleScene");
                break;
            case GameType.Main:
                SceneManager.LoadScene("MainScene");
                break;
            case GameType.Result:
                SceneManager.LoadScene("ResultScene");
                break;
        }
    }
}
