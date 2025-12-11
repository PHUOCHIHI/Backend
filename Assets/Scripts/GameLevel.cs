using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using ModelGameLevel;
using TMPro;

public class GameLevel : MonoBehaviour
{
    public GameObject PrefabGameLevel;
    public RectTransform ParentGameLevel;

    private void Start()
    {
        StartCoroutine(GetRequestAPIGameLevel());
    }

    private IEnumerator GetRequestAPIGameLevel()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:5185/api/APIGame/GetAllGameLevel"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("API call failed: " + www.error);
            }
            else
            {
                try
                {
                    ResponseAPI response = JsonConvert.DeserializeObject<ResponseAPI>(www.downloadHandler.text);
                    HandleGetResponseLevel(response);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Exception during JSON parsing: " + ex.Message);
                }
            }
        }
    }

    private void HandleGetResponseLevel(ResponseAPI response)
    {
        if (response != null && response.isSuccess)
        {
            if (response.data != null)
            {
                foreach (var level in response.data)
                {
                    GameObject game = Instantiate(PrefabGameLevel, ParentGameLevel);
                    game.GetComponent<LevelGameData>().levelId = level.levelId;
                    game.GetComponent<LevelGameData>().title = level.title;
                    game.GetComponent<LevelGameData>().description = level.description;
                    game.GetComponentInChildren<TMP_Text>().text = level.title;
                }
            }
        }
    }
}
