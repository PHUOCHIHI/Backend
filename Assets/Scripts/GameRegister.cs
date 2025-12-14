using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using Register;

public class GameRegister : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nameInput;
    public GameObject notification;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public static int selectedRegionId;

    public void OnClickRegister()
    {
        StartCoroutine(Register());
    }

    private IEnumerator Register()
    {
        selectedRegionId = GameRegion.selectedRegionId;
        RegisterRequestData requestData = new RegisterRequestData(
            emailInput.text,
            passwordInput.text,
            nameInput.text,
            "",
            selectedRegionId
        );

        string body = JsonUtility.ToJson(requestData);

        if (emailInput.text == "" || passwordInput.text == "" || nameInput.text == "")
        {
            notification.SetActive(true);
            notification.GetComponentsInChildren<TMP_Text>()[1].text = "Vui lòng nhập đầy đủ thông tin!";
            yield break;
        }

        using (UnityWebRequest www = new UnityWebRequest("http://localhost:5000/api/auth/register","POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");   
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                string responseJson = www.downloadHandler.text;
                Debug.Log(responseJson);

                ResponseUserError responseErr = JsonConvert.DeserializeObject<ResponseUserError>(responseJson);
                if (responseErr != null && responseErr.data != null && responseErr.data.Count > 0)
                {
                    notification.SetActive(true);
                    var textComponents = notification.GetComponentsInChildren<TMP_Text>();

                    string error = string.Join(", ", responseErr.data.Select(e => e.description));
                    if (textComponents.Length > 0)
                    {
                        textComponents[Mathf.Min(1, textComponents.Length - 1)].text = error;
                    }
                }
            }
            else
            {
                string json = www.downloadHandler.text;
                ResponseUserSuccess response = JsonConvert.DeserializeObject<ResponseUserSuccess>(json);
                var data = response.data;
                if (response.isSuccess)
                {
                    notification.SetActive(true);
                    var textComponents = notification.GetComponentsInChildren<TMP_Text>();
                    if (textComponents.Length > 0)
                    {
                        textComponents[Mathf.Min(1, textComponents.Length - 1)].text =
                            "Đăng ký thành công, vui lòng quay lại trang và đăng nhập! " + data.name;
                    }

                    if (registerPanel != null)
                    {
                        registerPanel.SetActive(false);
                    }
                    if (loginPanel != null)
                    {
                        loginPanel.SetActive(true);
                    }
                }
            }
        }
    }
}
