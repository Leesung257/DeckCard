using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameClearSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text detailText;

    [SerializeField] private string battleSceneName = "SampleScene";
    [SerializeField] private string titleSceneName = "TitleScene";

    void Start()
    {
        titleText.text = "GAME CLEAR";

        finalScoreText.text = "최종 점수 : " + GameResultData.FinalScore;

        detailText.text = "클리어 스테이지 : " + GameResultData.ReachedStage + "\n"
            + "남은 HP : " + GameResultData.PlayerHp + "\n"
            + "보유 골드 : " + GameResultData.Gold + "\n"
            + "전체 카드 수 : " + GameResultData.CardCount;
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
