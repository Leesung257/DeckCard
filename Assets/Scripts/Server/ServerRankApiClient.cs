using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ServerRankApiClient : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "http://localhost:5122";

    public void SaveRankingToServer(
        string username,
        int score,
        int stage,
        Action<bool, string> onComplete)
    {
        StartCoroutine(SaveRankingToServerCourtine(
            username,
            score,
            stage,
            onComplete));
    }

    public void GetTop10Rankings(Action<bool, RankingResponse[]> onComplete)
    {
        StartCoroutine(GetTop10RankingsCoroutine(onComplete));
    }

    private IEnumerator SaveRankingToServerCourtine(
        string username,
        int score,
        int stage,
        Action<bool, string> onComplete)
    {
        string url = baseUrl + "/api/Rank";

        SaveRankingServerRequest requestData = new SaveRankingServerRequest
        {
            username = username,
            score = score,
            stage = stage,
        };

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler=new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if(request.result==UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 랭킹 저장 성공");
            Debug.Log(request.downloadHandler.text);

            onComplete?.Invoke(true, request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("서버 랭킹 저장 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);

            onComplete?.Invoke(false, request.downloadHandler.text);
        }
    }

    private IEnumerator GetTop10RankingsCoroutine(Action<bool, RankingResponse[]> onComplete)
    {
        string url = baseUrl + "/api/Rank/top10";

        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if(request.result==UnityWebRequest.Result.Success)
        {
            Debug.Log("Top10 랭킹 조회 성공");
            Debug.Log(request.downloadHandler.text);

            string wrappedJson = "{\"rankings\":" + request.downloadHandler.text + "}";

            RankingListResponse response = JsonUtility.FromJson<RankingListResponse>(wrappedJson);

            if (response == null || response.rankings == null)
            {
                onComplete?.Invoke(false, null);
                yield break;
            }

            onComplete?.Invoke(true, response.rankings);
        }
        else
        {
            Debug.LogError("Top10 랭킹 조회 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);

            onComplete?.Invoke(false, null);
        }
    }

    public void GetUserRankings(
        string username,
        Action<bool, UserRankingResponse> onComplete)
    {
        StartCoroutine(GetUserRankingsCoroutine(username, onComplete));
    }

    public IEnumerator GetUserRankingsCoroutine(
        string username,
        Action<bool, UserRankingResponse> onComplete)
    {
        string encodeUsername = UnityWebRequest.EscapeURL(username);
        string url = baseUrl + "/api/Rank/user/" + encodeUsername;

        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("유저 랭킹 조회 성공");
            Debug.Log(request.downloadHandler.text);

            UserRankingResponse response = JsonUtility.FromJson<UserRankingResponse>(request.downloadHandler.text);

            onComplete?.Invoke(response != null, response);
        }
        else
        {
            Debug.LogError("유저 랭킹 조회 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);

            onComplete?.Invoke(false, null);
        }
    }
}

[Serializable]
public class SaveRankingServerRequest
{
    public string username;
    public int score;
    public int stage;
}

[Serializable]
public class RankingResponse
{
    public int id;
    public string username;
    public int score;
    public int stage;
    public string createdAt;
}

[Serializable]
public class RankingListResponse
{
    public RankingResponse[] rankings;
}

[Serializable]
public class UserRankingResponse
{
    public string username;
    public int recordCount;
    public int bestScore;
    public int bestStage;
    public RankingResponse[] rankings;
}