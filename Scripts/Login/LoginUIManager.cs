using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private InputField nameInputField;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.GetString("PlayerName");

        // debug
        // PlayerPrefs.SetString("PlayerName", nameInputField.text);
        // HostManager.Instance.startHost();
    }

    public void onHostClick() {
        PlayerPrefs.SetString("PlayerName", nameInputField.text);
        HostManager.Instance.startHost();
    }

    public void onClientClicked() {
        PlayerPrefs.SetString("PlayerName", nameInputField.text);
        ClientManager.Instance.startClient();
    }

}
