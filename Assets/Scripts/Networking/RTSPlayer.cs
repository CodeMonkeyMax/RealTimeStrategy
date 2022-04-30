using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    public List<Unit>     GetMyUnits()     => myUnits;
    public List<Building> GetMyBuildings() => myBuildings;

    #region Server

    public override void OnStartServer() { // "Server version of Start()," NOT "when server starts."
        Unit.ServerOnUnitSpawned   += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned   += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

    }

    public override void OnStopServer() {
        Unit.ServerOnUnitSpawned   -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned   -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit) {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit) {
        if ( unit.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingDespawned(Building building) {
        if ( building.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myBuildings.Add(building);
    }

    private void ServerHandleBuildingSpawned(Building building) {
        if ( building.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myBuildings.Remove(building);
    }
    #endregion

    #region Client

    public override void OnStartAuthority() {
        if( NetworkServer.active ) { return; }

        Unit.AuthorityOnUnitSpawned   += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned   += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient() {
        if ( !isClientOnly || !hasAuthority ) { return; }
        Unit.AuthorityOnUnitSpawned   -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned   -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    
    // These methods only called on Client's game
    private void AuthorityHandleUnitSpawned(Unit unit) { // You can totally just have one AuthorityHandleObjectSpawned() & Despawned with object-type-specific overloads
        myUnits.Add(unit);
    }
    private void AuthorityHandleUnitDespawned(Unit unit) {
        myUnits.Remove(unit);
    }
    private void AuthorityHandleBuildingSpawned(Building building) {
        myBuildings.Add(building);
    }
    private void AuthorityHandleBuildingDespawned(Building building) {
        myBuildings.Remove(building);
    }
    #endregion
}
