using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] Health health = null;

    public static event Action ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawned; 
    public static event Action<UnitBase> ServerOnBaseDespawned; 

    #region Server

    public override void OnStartServer() {
        health.ServerOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }
    public override void OnStopServer() {
        ServerOnBaseDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie() {
        Debug.Log("UnitBase: I have been destroyed.");
        int connId = connectionToClient.connectionId;
        Debug.Log("UnitBase: I belong to player " + connId);
        //NetworkManager.singleton.gameObject.GetComponent<RTSNetworkManager>().playerGameOver[connId] = true;
        ServerOnPlayerDie?.Invoke(); // passes the ID of the player who owns this base
        Debug.Log("ServerOnPlayerDie invoked.");
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client


    #endregion

}
