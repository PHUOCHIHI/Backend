using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private bool showLoginOnStart = true;

    private void Start()
    {
        if (showLoginOnStart)
        {
            ShowLogin();
        }
        else
        {
            HideAll();
        }
    }

    public void ShowLogin()
    {
        SetOnlyActive(loginPanel);
    }

    public void ShowRegister()
    {
        SetOnlyActive(registerPanel);
    }

    public void ShowShop()
    {
        SetOnlyActive(shopPanel);
    }

    public void HideAll()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    private void SetOnlyActive(GameObject target)
    {
        if (loginPanel != null) loginPanel.SetActive(target == loginPanel);
        if (registerPanel != null) registerPanel.SetActive(target == registerPanel);
        if (shopPanel != null) shopPanel.SetActive(target == shopPanel);
    }
}
