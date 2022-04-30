using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer() {
        currentHealth = maxHealth;
        //UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }
    public override void OnStopServer() {
        //UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    public void DealDamage(int damageAmount) {
        if (currentHealth == 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        Debug.Log($"{gameObject.ToString()}: Hit! My health is now {currentHealth}.");

        if ( currentHealth != 0 ) { return; }

        Debug.Log($"{gameObject.ToString()}: My health has reached 0.");
        ServerOnDie?.Invoke();
        Debug.Log("I Died");
    }

    [Server]
    private void ServerHandlePlayerDie() {
        //if(!NetworkManager.singleton.gameObject.GetComponent<RTSNetworkManager>().playerGameOver[connectionToClient.connectionId]) { return; } // if client ID's game is not over according to NetworkManager, return.

        DealDamage(currentHealth);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth) {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
