using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private string battleSceneName = "SampleScene";

    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private TMP_Text rankingText;
    [SerializeField] private GameObject titleMenuPanel;

    private ServerRankApiClient serverRankApiClient;

    private void Start()
    {
        InitializeServerRankApiClient();

        ShowTitleMenu();
        HideRankingPanel();
    }

    private void InitializeServerRankApiClient()
    {
        serverRankApiClient = GetComponent<ServerRankApiClient>();

        if (serverRankApiClient == null)
        {
            serverRankApiClient = gameObject.AddComponent<ServerRankApiClient>();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(battleSceneName);
    }

    public void ShowRanking()
    {
        HideTitleMenu();
        ShowRankingPanel();

        rankingText.text = "랭킹 불러오는 중...";

        serverRankApiClient.GetTop10Rankings(
            (success, rankings) =>
            {
                if (success == false)
                {
                    rankingText.text = "랭킹 조회 실패";
                    return;
                }

                rankingText.text = FormatTop10Ranking(rankings);
            });
    }

    private string FormatTop10Ranking(RankingResponse[] rankings)
    {
        if(rankings==null||rankings.Length == 0)
        {
            return "랭킹 기록이 없습니다";
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("Top 10 랭킹");
        builder.AppendLine();

        for(int i=0;i<rankings.Length;i++)
        {
            RankingResponse ranking = rankings[i];

            builder.AppendLine(
                (i + 1) + ". "
                + ranking.username
                + " / Score: " + ranking.score
                + " / Stage: " + ranking.stage);
        }

        return builder.ToString();
    }

    public void CloseRanking()
    {
        HideRankingPanel();
        ShowTitleMenu();
    }

    private void ShowTitleMenu()
    {
        if (titleMenuPanel != null)
        {
            titleMenuPanel.SetActive(true);
        }
    }

    private void HideTitleMenu()
    {
        if(titleMenuPanel != null)
        {
            titleMenuPanel.SetActive(false);
        }
    }

    private void ShowRankingPanel()
    {
        if (rankingPanel != null)
        {
            rankingPanel.SetActive(true);
        }
    }

    private void HideRankingPanel()
    {
        if (rankingPanel != null)
        {
            rankingPanel.SetActive(false);
        }
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
