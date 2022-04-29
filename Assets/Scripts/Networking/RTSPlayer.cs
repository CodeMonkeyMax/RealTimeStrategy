using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();

    public List<Unit> GetMyUnits() => myUnits;

    #region Server

    public override void OnStartServer() { // "Server version of Start()," NOT "when server starts."
        Unit.ServerOnUnitSpawned   += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    public override void OnStopServer() {
        Unit.ServerOnUnitSpawned   -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit) {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit) {
        if ( unit.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myUnits.Remove(unit);
    }
    #endregion

    #region Client

    public override void OnStartClient() {
        if( !isClientOnly ) { return; }
        Unit.AuthorityOnUnitSpawned   += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }
    public override void OnStopClient() {
        if ( !isClientOnly ) { return; }
        Unit.AuthorityOnUnitSpawned   -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }
    
    // These methods only called on Client's game
    private void AuthorityHandleUnitSpawned(Unit unit) {
        if ( !hasAuthority ) { return; } // Stops unit from being added to other players' Unit lists in Client game
        myUnits.Add(unit);
    }
    private void AuthorityHandleUnitDespawned(Unit unit) {
        if ( !hasAuthority ) { return; }
        myUnits.Remove(unit);
    }
    #endregion
}
