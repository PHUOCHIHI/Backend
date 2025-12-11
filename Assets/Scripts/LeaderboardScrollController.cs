using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class LeaderboardScrollController : MonoBehaviour
{
    [Header("UI References")]
    public Transform content;           // Content của Scroll View
    public GameObject rowPrefab;        // Prefab Row chứa các Text (TMP)

    [Header("Server")] 
    public string serverUrl = "http://localhost:5185/api/User";

    private readonly List<GameObject> spawnedRows = new List<GameObject>();

    private void OnEnable()
    {
        StartCoroutine(FetchAndDisplayUsers());
    }

    private IEnumerator FetchAndDisplayUsers()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching users: " + webRequest.error);
                Debug.LogError("Body: " + webRequest.downloadHandler.text);
                yield break;
            }

            string json = webRequest.downloadHandler.text;
            Debug.Log("Leaderboard users JSON: " + json);

            List<User> users = null;
            try
            {
                // API của bạn đang trả về dạng object có isSuccess, data, ...
                // ví dụ: { "isSuccess": true, "notification": "", "data": [ ... ] }
                var response = JsonConvert.DeserializeObject<ResponseUsers>(json);
                if (response != null && response.isSuccess && response.data != null)
                {
                    users = response.data;
                }
                else
                {
                    Debug.LogError("Leaderboard: response null hoặc isSuccess = false");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing users JSON: " + e.Message);
            }

            if (users == null)
            {
                yield break;
            }

            // Clear old rows
            for (int i = 0; i < spawnedRows.Count; i++)
            {
                if (spawnedRows[i] != null)
                {
                    Destroy(spawnedRows[i]);
                }
            }
            spawnedRows.Clear();

            // Optionally: sort users by something, ví dụ userId tăng dần
            // users.Sort((a, b) => a.userId.CompareTo(b.userId));

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];

                GameObject row = Instantiate(rowPrefab, content);
                spawnedRows.Add(row);

                // Giả định trong prefab Row có 4 TextMeshProUGUI lần lượt là:
                // Rank, Username, Region, Role (hoặc tuỳ bạn đặt)
                var texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
                if (texts.Length > 0)
                {
                    // Rank: 1,2,3,...
                    texts[0].text = (i + 1).ToString();
                }
                if (texts.Length > 1)
                {
                    // Username: ưu tiên username, fallback userId nếu rỗng
                    var name = string.IsNullOrEmpty(user.username)
                        ? $"User {user.userId}"
                        : user.username;
                    texts[1].text = name;
                }
                if (texts.Length > 2)
                {
                    texts[2].text = user.region != null ? user.region.name : string.Empty;
                }
                if (texts.Length > 3)
                {
                    texts[3].text = user.role != null ? user.role.name : string.Empty;
                }
            }
        }
    }

    [System.Serializable]
    private class ResponseUsers
    {
        public bool isSuccess;
        public string notification;
        public List<User> data;
    }
}
