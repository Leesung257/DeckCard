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
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_Text loginStatusText;

    private ServerRankApiClient serverRankApiClient;
    private ServerAuthApiClient serverAuthApiClient;

    private void Start()
    {
        InitializeServerRankApiClient();
        InitailizeServerAuthApiClient();

        if(loginStatusText!=null )
        {
            loginStatusText.text = "로그인이 필요합니다";
        }

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

    private void InitailizeServerAuthApiClient()
    {
        serverAuthApiClient = GetComponent<ServerAuthApiClient>();

        if(serverAuthApiClient == null)
        {
            serverAuthApiClient = gameObject.AddComponent<ServerAuthApiClient>();
        }
    }

    public void RegisterAccount()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        
        if(string.IsNullOrWhiteSpace(username)||string.IsNullOrWhiteSpace(password))
        {
            loginStatusText.text = "아이디와 비밀번호를 입력해주세요";
            return;
        }

        loginStatusText.text = "회원가입 요청 중...";

        serverAuthApiClient.Register(
            username,
            password,
            (success, response) =>
            {
                if (success)
                {
                    AccountSession.Login(username, password);
                    loginStatusText.text = "회원가입 성공 / 로그인 완료";
                }
                else
                {
                    loginStatusText.text = "회원가입 실패";
                }
            });
    }

    public void LoginAccount()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            loginStatusText.text = "아이디와 비밀번호를 입력해주세요";
            return;
        }

        loginStatusText.text = "로그인 요청 중...";

        serverAuthApiClient.Login(
            username,
            password,
            (success, response) =>
            {
                if (success)
                {
                    AccountSession.Login(username, password);
                    loginStatusText.text = "로그인 성공";
                }
                else
                {
                    loginStatusText.text = "로그인 실패";
                }
            });
    }

    public void StartGame()
    {
        if (AccountSession.IsLoggedIn == false)
        {
            loginStatusText.text = "로그인 후 게임을 시작해주세요";
            return;
        }

        SceneManager.LoadScene(battleSceneName);
    }

    public void ShowRanking()
    {
        HideTitleMenu();
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

    public void ShowMyRanking()
    {
        if (AccountSession.IsLoggedIn == false)
        {
            if (loginStatusText != null)
            {
                loginStatusText.text = "로그인 후 내 랭킹을 확인할 수 있습니다";
            }

            return;
        }

        string username = AccountSession.Username;

        HideTitleMenu();
        ShowRankingPanel();

        rankingText.text = username + "님의 랭킹 불러오는 중...";

        serverRankApiClient.GetUserRankings(
            username,
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
