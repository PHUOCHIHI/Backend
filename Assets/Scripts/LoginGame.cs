using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using Login;

public class LoginGame : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text notification;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject listPanel;
    [SerializeField] private bool showLoginPanelOnAwake = true;
    private string baseUrl = "http://localhost:5000";
    private string loginEndpoint = "/api/auth/login";
    private string avatarPath = "/uploads/avatars/";

    private void Awake()
    {
        if (!showLoginPanelOnAwake)
        {
            return;
        }

        if (loginPanel != null)
        {
            loginPanel.SetActive(true);
        }

        if (registerPanel != null)
        {
            registerPanel.SetActive(false);
        }

        if (listPanel != null)
        {
            listPanel.SetActive(false);
        }
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        RequestLoginData loginData = new RequestLoginData(email, password);
        string body = JsonConvert.SerializeObject(loginData);
        Debug.Log("Unity Login email: " + email);
        Debug.Log("Unity Login password length: " + password.Length);
        Debug.Log("Unity Login request body: " + body);

        using (UnityWebRequest www = new UnityWebRequest(baseUrl + loginEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("URL: " + www.url);
                Debug.Log("HTTP Code: " + www.responseCode);
                Debug.Log("Error: " + www.error);
                Debug.Log("Body: " + www.downloadHandler.text);
                notification.text = "Network/API error";
            }
            else
            {
                Debug.Log("Login HTTP Code: " + www.responseCode);
                Debug.Log("Login Body: " + www.downloadHandler.text);
                Login.ResponseLogin response = JsonConvert.DeserializeObject<Login.ResponseLogin>(www.downloadHandler.text);
                if (response != null && response.isSuccess)
                {
                    PlayerPrefs.SetString("token", response.data.token);
                    PlayerPrefs.SetString("userId", response.data.user.id);
                    PlayerPrefs.SetString("email", response.data.user.email);
                    PlayerPrefs.SetString("name", response.data.user.name);
                    PlayerPrefs.SetString("linkAvatar", baseUrl + avatarPath + response.data.user.avatar);
                    PlayerPrefs.SetInt("regionId", response.data.user.regionId);

                    notification.text = "Login success";

                    // Hiện panel danh sách, ẩn các panel khác
                    if (listPanel != null)
                    {
                        listPanel.SetActive(true);
                    }
                    if (loginPanel != null)
                    {
                        loginPanel.SetActive(false);
                    }
                    if (registerPanel != null)
                    {
                        registerPanel.SetActive(false);
                    }
                }
                else if (response != null)
                {
                    notification.text = response.notification;

                    // Hiện panel đăng ký, ẩn các panel khác
                    if (registerPanel != null)
                    {
                        registerPanel.SetActive(true);
                    }
                    if (loginPanel != null)
                    {
                        loginPanel.SetActive(false);
                    }
                    if (listPanel != null)
                    {
                        listPanel.SetActive(false);
                    }
                }
            }
        }
    }
}
