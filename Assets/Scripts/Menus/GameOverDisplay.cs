using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text   winnerNameText = null;
    
    private void Start() {
        Debug.Log("GameOverDisplay Instantiated! Subscribing...");
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }
    private void OnDestroy() {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame() {
        if (NetworkServer.active && NetworkClient.isConnected ) {
            // Stop Hosting
            NetworkManager.singleton.StopHost();
        } else {
            // Stop Clienting
            NetworkManager.singleton.StopClient();
        }
    }
    private void ClientHandleGameOver(string winner) {
        Debug.Log("Game Over.");
        winnerNameText.text = $"{winner} has WON!";
        gameOverDisplayParent.gameObject.SetActive(true);
    }

}
