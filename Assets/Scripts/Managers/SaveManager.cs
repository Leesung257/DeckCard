using System.IO;
using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    private const string SaveFileName = "/save.json";

    [SerializeField] private ServerSaveApiClient serverSaveApiClient;

    public bool SaveLocal(SaveData saveData)
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            string path = GetSavePath();

            Debug.Log(json);
            Debug.Log("저장 경로: " + path);

            File.WriteAllText(path, json);

            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError("게임 저장 실패: " + e.Message);
            return false;
        }
    }

    public string GetSavePath()
    {
        return Application.persistentDataPath + SaveFileName;
    }

    public bool TryLoadLocal(out SaveData saveData)
    {
        saveData = null;

        string path=GetSavePath();

        if (File.Exists(path) == false)
        {
            Debug.LogWarning("저장파일이 없습니다: " + path);
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(json);

            return saveData != null;
        }
        catch(System.Exception e)
        {
            Debug.LogError("게임 불러오기 실패: " + e.Message);
            return false;
        }
    }

    private void Awake()
    {
        serverSaveApiClient = GetComponent<ServerSaveApiClient>();

        if (serverSaveApiClient == null)
        {
            serverSaveApiClient = gameObject.AddComponent<ServerSaveApiClient>();
        }
    }

    public void SaveServer(
        SaveData saveData,
        string username,
        string password,
        Action<bool, string> onComplete)
    {
        string saveJson = JsonUtility.ToJson(saveData, true);

        serverSaveApiClient.SaveGameToServer(
            username,
            password, 
            saveJson, 
            onComplete);
    }

    public void LoadServer(
        string username,
        string password,
        Action<bool, SaveData> onComplete)
    {
        serverSaveApiClient.LoadGameFromServer(
            username,
            password,
            (success, responseJson) =>
            {
                if (success == false)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                LoadGameServerResponse response = JsonUtility.FromJson<LoadGameServerResponse>(responseJson);

                if (response == null || string.IsNullOrWhiteSpace(response.saveJson))
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                SaveData saveData = JsonUtility.FromJson<SaveData>(response.saveJson);

                onComplete?.Invoke(saveData != null, saveData);
            });
    }
}