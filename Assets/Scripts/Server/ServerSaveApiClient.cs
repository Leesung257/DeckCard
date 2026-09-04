using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ServerSaveApiClient : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "http://localhost:5122";

    public void SaveGameToServer(
        string username,
        string password,
        string saveJson,
        Action<bool, string> onComplete)
    {
        StartCoroutine(SaveGameToServerCoroutine(
            username,
            password,
            saveJson,
            onComplete));
    }

    private IEnumerator SaveGameToServerCoroutine(
        string username,
        string password,
        string saveJson,
        Action<bool, string> onComplete)
    {
        string url = baseUrl + "/api/Save";

        SaveGameServerRequest requestData = new SaveGameServerRequest
        {
            username = username,
            password = password,
            saveJson = saveJson
        };

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 저장 성공");
            Debug.Log(request.downloadHandler.text);

            onComplete?.Invoke(true, request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("서버 저장 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);

            onComplete?.Invoke(false, request.downloadHandler.text);
        }
    }

    public void LoadGameFromServer(
        string username,
        string password,
        Action<bool, string> onComplete)
    {
        StartCoroutine(LoadGameFromServerCoroutine(
            username,
            password, 
            onComplete));
    }

    private IEnumerator LoadGameFromServerCoroutine(
        string username,
        string password,
        Action<bool, string> onComplete)
    {
        string url = baseUrl + "/api/Save/load";

        LoadGameServerRequest requestData = new LoadGameServerRequest
        {
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 불러오기 성공");
            Debug.Log(request.downloadHandler.text);

            onComplete?.Invoke(true, request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("서버 불러오기 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);

            onComplete?.Invoke(false, request.downloadHandler.text);
        }
    }
 
}

[Serializable]
public class SaveGameServerRequest
{
    public string username;
    public string password;
    public string saveJson;
}

[SerializeField]
public class LoadGameServerRequest
{
    public string username;
    public string password;
}

[SerializeField]
public class LoadGameServerResponse
{
    public string message;
    public string username;
    public string saveJson;
    public string updateAt;
}
