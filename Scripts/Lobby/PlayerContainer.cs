using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContainer : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject defaultPanel;
    [SerializeField] private GameObject playerDataPanel;

    [Header("Data")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Toggle isReadyToggle;

    public void updateDisplay(LobbyPlayerState lobbyPlayerState) {
        playerNameText.text = lobbyPlayerState.playerName.ToString();
        isReadyToggle.isOn = lobbyPlayerState.isReady;
        defaultPanel.SetActive(false);
        playerDataPanel.SetActive(true);
    }

    public void disableDisplay() {
        defaultPanel.SetActive(true);
        playerDataPanel.SetActive(false);
    }
}
