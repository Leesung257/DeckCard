using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private string battleSceneName = "SampleScene";
    [SerializeField] private string titleSceneName = "TitleScene";

    void Start()
    {
        titleText.text = "GAME OVER";

        infoText.text = "도달 스테이지 : " + GameResultData.ReachedStage;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(battleSceneName);
    }

    public void GoToTitleScene()
    {
        SceneManager.LoadScene(titleSceneName);
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
