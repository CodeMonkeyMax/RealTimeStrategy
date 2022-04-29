using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server
    [Command]
    private void CmdSpawnUnit() {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);
        NetworkServer.Spawn(unitInstance, connectionToClient); // inherits from Spawner's connectionToClient 
    }
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData) {
        if(eventData.button != PointerEventData.InputButton.Left ) { return; }
        if ( !hasAuthority ) { return; }
        CmdSpawnUnit();
    }
    #endregion
}