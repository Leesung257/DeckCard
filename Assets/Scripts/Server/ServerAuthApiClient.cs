using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ServerAuthApiClient : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "http://localhost:5122";

    public void Register(
        string username,
        string password,
        Action<bool,string> onComplete)
    {
        StartCoroutine(SendAuthRequest(
            baseUrl+"/api/Auth/register",
            username,
            password,
            onComplete));
    }

    public void Login(
        string username,
        string password,
        Action<bool, string> onComplete)
    {
        StartCoroutine(SendAuthRequest(
            baseUrl + "/api/Auth/login",
            username,
            password,
            onComplete));
    }

    private IEnumerator SendAuthRequest(
        string url,
        string username,
        string paswword,
        Action<bool,string> onComplete)
    {
        AuthRequest requestData = new AuthRequest
        {
            username = username,
            password = paswword
        };

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();

        bool success = request.result == UnityWebRequest.Result.Success;

        if(success)
        {
            Debug.Log("Auth 인증 성공");
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Auth 요청 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
            
            onComplete?.Invoke(false, "서버 연결 실패: " + request.error);
            yield break;
        }

        onComplete?.Invoke(success, request.downloadHandler.text);
    }

    public void DeleteAccount(
        string username,
        string password,
        Action<bool, string> onComplete)
    {
        StartCoroutine(SendDeleteAccountRequest(
            baseUrl + "/api/Auth/delete",
            username,
            password,
            onComplete));
    }

    private IEnumerator SendDeleteAccountRequest(
        string url,
        string username,
        string password,
        Action<bool, string> onComplete)
    {
        AuthRequest requestData = new AuthRequest
        {
            username = username,
            password = password
        };

        string json=JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "DELETE");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        bool success = request.result == UnityWebRequest.Result.Success;

        if(success)
        {
            Debug.Log("회원 탈퇴 요청 성공");
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("회원 탈퇴 요청 실패");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }

        onComplete?.Invoke(success, request.downloadHandler.text);
    }
}

[Serializable]
public class AuthRequest
{
    public string username;
    public string password;
}
