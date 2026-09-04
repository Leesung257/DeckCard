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
}

[Serializable]
public class SaveRankingServerRequest
{
    public string username;
    public int score;
    public int stage;
}
