using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveFileName = "/save.json";

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
}