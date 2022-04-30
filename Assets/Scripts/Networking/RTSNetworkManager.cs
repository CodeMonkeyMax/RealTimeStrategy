using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
    public bool[] playerGameOver;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate(
            unitSpawnerPrefab, 
            conn.identity.transform.position, 
            conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnerInstance, conn);

        //// Max workaround
        //bool[] newGOArray = new bool[singleton.numPlayers];
        //for ( int i = 0; i < playerGameOver.Length; i++ ) {
        //    newGOArray[i] = playerGameOver[i];
        //}
        //playerGameOver = newGOArray;
    }
    /// <summary>
    /// Spawn in "Game Over Handler" object
    /// </summary>
    /// <param name="sceneName">Parameter fed by default. Unused.</param>
    public override void OnServerSceneChanged(string sceneName) {
        if ( SceneManager.GetActiveScene().name.StartsWith("Scene_Map") ) {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }

        //// Max workaround
        //playerGameOver = new bool[singleton.numPlayers];
        //for ( int i = 0; i < playerGameOver.Length; i++ ) {
        //    playerGameOver[i] = false;
        //}
    }
}
