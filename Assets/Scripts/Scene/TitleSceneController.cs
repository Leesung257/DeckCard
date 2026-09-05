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
    [SerializeField] private TMP_InputField usernameInputField;

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
        string username = usernameInputField.text;

        if (string.IsNullOrWhiteSpace(username))
        {
            HideTitleMenu();
            ShowRankingPanel();

            rankingText.text = "아이디를 입력해주세요";
            return;
        }

        HideTitleMenu();
        ShowRankingPanel();

        rankingText.text = "랭킹 불러오는 중...";

        serverRankApiClient.GetUserRankings(
            username,
            (success, response) =>
            {
                if (success == false)
                {
                    rankingText.text = "해당 사용자의 랭킹 기록이 없습니다.";
                    return;
                }

                rankingText.text = FormatMyRanking(response);
            });

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

    private string FormatMyRanking(UserRankingResponse response)
    {
        if(response == null || response.rankings == null || response.rankings.Length == 0)
        {
            return "랭킹 기록이 없습니다";
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine(response.username + "님의 랭킹 기록");
        builder.AppendLine();
        builder.AppendLine("기록 수 : " + response.recordCount);
        builder.AppendLine("최고 점수 : " + response.bestScore);
        builder.AppendLine("최고 스테이지 : " + response.bestStage);
        builder.AppendLine();

        for(int i = 0; i < response.rankings.Length; i++)
        {
            RankingResponse ranking = response.rankings[i];

            builder.AppendLine(
                (i + 1) + ". "
                + "Score: " + ranking.score
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
