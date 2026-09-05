using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;

public class GameClearSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text detailText;

    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject gameClaerPanel;
    [SerializeField] private TMP_Text rankingText;

    [SerializeField] private string battleSceneName = "SampleScene";
    [SerializeField] private string titleSceneName = "TitleScene";

    private ServerRankApiClient serverRankApiClient;

    void Start()
    {
        InitailizeServerRankApiClient();

        titleText.text = "GAME CLEAR";

        finalScoreText.text = "최종 점수 : " + GameResultData.FinalScore;

        detailText.text = "클리어 스테이지 : " + GameResultData.ReachedStage + "\n"
            + "남은 HP : " + GameResultData.PlayerHp + "\n"
            + "보유 골드 : " + GameResultData.Gold + "\n"
            + "전체 카드 수 : " + GameResultData.CardCount;

        ShowGameClearPanel();
        HideRankingPanel();
    }

    private void InitailizeServerRankApiClient()
    {
        serverRankApiClient = GetComponent<ServerRankApiClient>();

        if (serverRankApiClient == null)
        {
            serverRankApiClient = gameObject.AddComponent<ServerRankApiClient>();
        }
    }

    private void ShowGameClearPanel()
    {
        if(gameClaerPanel!= null)
        {
            gameClaerPanel.SetActive(true);
        }
    }

    private void HideGameClearPanel()
    {
        if(gameClaerPanel!= null)
        {
            gameClaerPanel.SetActive(false);
        }
    }

    public void ShowTop10Ranking()
    {
        HideGameClearPanel();
        ShowRankingPanel();

        rankingText.text = "Top 10 랭킹 불러오는 중...";

        serverRankApiClient.GetTop10Rankings(
            (success, rankings) =>
            {
                if (success == false)
                {
                    rankingText.text = "Top 10 랭킹 조회 실패";
                    return;
                }

                rankingText.text = FormatTop10Ranking(rankings);
            });
    }

    public void ShowMyRanking()
    {
        HideGameClearPanel();
        ShowRankingPanel();

        if (AccountSession.IsLoggedIn == false)
        {
            rankingText.text = "로그인 후 내 랭킹을 확인할 수 있습니다";
            return;
        }

        rankingText.text = AccountSession.Username + "님의 랭킹을 불러오는 중...";

        serverRankApiClient.GetUserRankings(
            AccountSession.Username,
            (success, response) =>
            {
                if (success == false)
                {
                    rankingText.text = "해당 사용자의 랭킹 기록이 없습니다";
                    return;
                }

                rankingText.text = FormatMyRanking(response);
            });
    }

    private string FormatTop10Ranking(RankingResponse[] rankings)
    {
        if(rankings==null|| rankings.Length == 0)
        {
            return "Top 10 랭킹 기록이 없습니다";
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("Top 10 랭킹");
        builder.AppendLine();

        for(int i = 0; i < rankings.Length; i++)
        {
            RankingResponse ranking = rankings[i];

            builder.AppendLine(
                (i + 1) + ". "
                + ranking.username
                + " | "
                + ranking.score + "점"
                + " | Stage " + ranking.stage);
        }

        return builder.ToString();
    }

    private string FormatMyRanking(UserRankingResponse response)
    {
        if (response == null || response.rankings == null || response.rankings.Length == 0)
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
                + ranking.score + "점"
                + " | Stage " + ranking.stage);
        }

        return builder.ToString();
    }

    public void CloseRanking()
    {
        HideRankingPanel();
        ShowGameClearPanel();
    }

    private void ShowRankingPanel()
    {
        if(rankingPanel != null)
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
