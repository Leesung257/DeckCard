using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private string battleSceneName = "SampleScene";

    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject titleMenuPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject userInfoPanel;
    [SerializeField] private GameObject deleteAccountPanel;
    [SerializeField] private TMP_Text rankingText;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField deletePasswordInputField;
    [SerializeField] private TMP_Text loginStatusText;
    [SerializeField] private TMP_Text currentUserText;

    private ServerRankApiClient serverRankApiClient;
    private ServerAuthApiClient serverAuthApiClient;

    private void Start()
    {
        InitializeServerRankApiClient();
        InitailizeServerAuthApiClient();

        ShowTitleMenu();
        HideRankingPanel();
        HideDeleteAccountPanel();

        RefreshLoginUI();
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

    private void RefreshLoginUI()
    {
        if (AccountSession.IsLoggedIn)
        {
            if (loginPanel != null)
            {
                loginPanel.SetActive(false);
            }

            if(userInfoPanel!=null)
            {
                userInfoPanel.SetActive(true);
            }

            if (currentUserText != null)
            {
                currentUserText.text = AccountSession.Username + "님 로그인 중";
            }

            if (loginStatusText != null)
            {
                loginStatusText.text = "로그인 상태입니다";
            }
        }
        else
        {
            if(loginPanel != null)
            {
                loginPanel.SetActive(true);
            }

            if (userInfoPanel != null)
            {
                userInfoPanel.SetActive(false);
            }

            if (currentUserText != null)
            {
                currentUserText.text = "";
            }

            if (loginStatusText != null)
            {
                loginStatusText.text = "로그인이 필요합니다.";
            }
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
                    loginStatusText.text = "회원가입 성공";

                    usernameInputField.text = "";
                    passwordInputField.text = "";

                    RefreshLoginUI();
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

                    usernameInputField.text = "";
                    passwordInputField.text = "";

                    RefreshLoginUI();
                }
                else
                {
                    loginStatusText.text = "로그인 실패";
                }
            });
    }

    public void LogoutAccount()
    {
        if (AccountSession.IsLoggedIn == false)
        {
            if (loginStatusText != null)
            {
                loginStatusText.text = "현재 로그인된 계정이 없습니다";
            }

            return;
        }

        AccountSession.Logout();

        if (usernameInputField != null)
        {
            usernameInputField.text = "";
        }

        if(passwordInputField != null)
        {
            passwordInputField.text = "";
        }

        RefreshLoginUI() ;

        if(loginStatusText != null)
        {
            loginStatusText.text = "로그아웃 되었습니다";
        }
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

    public void OpenDeleteAccountPanel()
    {
        if (AccountSession.IsLoggedIn == false)
        {
            if (loginStatusText != null)
            {
                loginStatusText.text = "로그인 후 회원 탈퇴를 할 수 있습니다";
            }

            return;
        }

        HideTitleMenu();

        if(deletePasswordInputField != null)
        {
            deletePasswordInputField.text = "";
        }

        if(deleteAccountPanel != null)
        {
            deleteAccountPanel.SetActive(true);
        }
    }

    public void CancelDeleteAccount()
    {
        HideDeleteAccountPanel();
        ShowTitleMenu();
    }

    public void DeleteAccount()
    {
        if(AccountSession.IsLoggedIn == false)
        {
            HideDeleteAccountPanel();
            ShowTitleMenu();

            if (loginStatusText != null)
            {
                loginStatusText.text = "현재 로그인된 계정이 없습니다";
            }

            return;
        }

        string password = deletePasswordInputField.text;

        if (string.IsNullOrWhiteSpace(password))
        {
            if (loginStatusText != null)
            {
                loginStatusText.text = "비밀번호를 입력해주세요";
            }

            return;
        }

        if(loginStatusText!= null)
        {
            loginStatusText.text = "회원 탈퇴 요청 중...";
        }

        serverAuthApiClient.DeleteAccount(
            AccountSession.Username,
            password,
            (success, response) =>
            {
                if (success)
                {
                    AccountSession.Logout();

                    HideDeleteAccountPanel();
                    ShowTitleMenu();
                    RefreshLoginUI();

                    if (loginStatusText != null)
                    {
                        loginStatusText.text = "회원 탈퇴가 완료되었습니다";
                    }
                }
                else
                {
                    if (loginStatusText != null)
                    {
                        loginStatusText.text = "회원 탈퇴 실패";
                    }
                }
            });
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

    private void HideDeleteAccountPanel()
    {
        if(deleteAccountPanel != null)
        {
            deleteAccountPanel.SetActive(false);
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
