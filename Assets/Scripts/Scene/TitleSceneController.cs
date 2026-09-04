using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private string battleSceneName = "SampleScene";
    public void StartGame()
    {
        SceneManager.LoadScene(battleSceneName);
    }

    public void ShowRanking()
    {
        Debug.Log("다음단계");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
