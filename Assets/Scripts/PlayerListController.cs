using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Newtonsoft.Json;

public class PlayerListController : MonoBehaviour
{
    public UIDocument uiDocument;
    [Tooltip("Set your API URL here")]
    public string serverUrl = "http://localhost:5185/api/User";
    
    private ListView playerListView;
    private List<PlayerRow> players = new List<PlayerRow>();

    void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("No UIDocument found on " + gameObject.name);
            return;
        }

        var root = uiDocument.rootVisualElement;
        playerListView = root.Q<ListView>("PlayerList");

        if (playerListView != null)
        {
            // Set up the ListView
            playerListView.makeItem = () => 
            {
                var container = new VisualElement();
                container.AddToClassList("player-row");

                var nameLabel = new Label();
                nameLabel.name = "NameLabel";
                nameLabel.AddToClassList("player-info");
                container.Add(nameLabel);

                var emailLabel = new Label();
                emailLabel.name = "EmailLabel";
                emailLabel.AddToClassList("player-info");
                container.Add(emailLabel);

                var regionLabel = new Label();
                regionLabel.name = "RegionLabel";
                regionLabel.AddToClassList("player-info");
                container.Add(regionLabel);

                return container;
            };

            playerListView.bindItem = (element, index) => 
            {
                if (index < players.Count)
                {
                    var player = players[index];
                    element.Q<Label>("NameLabel").text = player.name;
                    element.Q<Label>("EmailLabel").text = player.email;
                    element.Q<Label>("RegionLabel").text = player.regionName;
                }
            };

            playerListView.itemsSource = players;
            playerListView.fixedItemHeight = 40;
        }

        // Start loading
        StartCoroutine(FetchPlayers());
    }

    IEnumerator FetchPlayers()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl))
        {
            string token = PlayerPrefs.GetString("token", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Host: " + serverUrl);
                Debug.LogError("Error fetching players: " + webRequest.error);
                Debug.LogError("Body: " + webRequest.downloadHandler.text);
            }
            else
            {
                string json = webRequest.downloadHandler.text;
                
                try 
                {
                    var users = JsonConvert.DeserializeObject<List<UserDto>>(json);

                    players.Clear();

                    if (users != null)
                    {
                        foreach (var u in users)
                        {
                            players.Add(new PlayerRow
                            {
                                id = u.id,
                                name = u.name,
                                email = u.email,
                                regionName = u.regionName
                            });
                        }
                    }

                    if (playerListView != null)
                    {
                        playerListView.itemsSource = players;
                        playerListView.Rebuild();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
        }
    }

    [System.Serializable]
    private class PlayerRow
    {
        public string id;
        public string regionName;
        public string name;
        public string email;
    }

    [System.Serializable]
    private class UserDto
    {
        public string id;
        public string name;
        public string email;
        public string regionName;
    }
}
