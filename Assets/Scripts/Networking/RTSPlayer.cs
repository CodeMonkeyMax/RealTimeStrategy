using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

   
    public event Action<int> ClientOnResourcesUpdated;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private Color          teamColor = new Color();
    private List<Unit>     myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    // Getters & Setters // # // # //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Getters & Setters

    public bool           GetIsPartyOwner()    => isPartyOwner;
    public string         GetDisplayName()     => displayName;
    public Transform      GetCameraTransform() => cameraTransform;
    public Color          GetTeamColor()       => teamColor;
    public List<Unit>     GetMyUnits()         => myUnits;
    public List<Building> GetMyBuildings()     => myBuildings;
    public int            GetResources()       => resources;

    #endregion

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point) {
        
        // Check if building placement area is clear
        if ( Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer) ) {
            return false;
        }

        // Check if building is within range of other player buildings
        foreach ( Building building in myBuildings ) {
            if ( ( point - building.transform.position ).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit ) {
                return true;
            }
        }
        return false;
    }

    // Server Side // # // # //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Server

    public override void OnStartServer() { // "Server version of Start()," NOT "when server starts."
        Unit.ServerOnUnitSpawned   += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned   += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer() {
        Unit.ServerOnUnitSpawned   -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned   -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    public void SetDisplayName(string displayName) {
        this.displayName = displayName;
    }

    [Server]
    public void SetPartyOwner(bool state) {
        isPartyOwner = state;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor) => teamColor = newTeamColor;
    [Server]
    public void SetResources(int resources)      => this.resources = resources;

    [Command]
    public void CmdStartGame() {
        if ( !isPartyOwner ) { return; }

        ( (RTSNetworkManager)NetworkManager.singleton ).StartGame();
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point) {
        Building buildingToPlace = null;
        foreach(Building building in buildings ) {
            if(building.GetId() == buildingId ) {
                buildingToPlace = building;
                break;
            }
        }

        // Return if no building to place
        if ( buildingToPlace == null ) { return; }
        // Return if not enough money
        if(resources < buildingToPlace.GetPrice() ) { return; }
        // Do I still need this guy?
        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        if(!CanPlaceBuilding(buildingCollider, point)) { return; }

        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        SetResources(resources - buildingToPlace.GetPrice());
    }

    private void ServerHandleUnitSpawned(Unit unit) {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit) {
        if ( unit.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building) {
        if ( building.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myBuildings.Add(building);
    }
    private void ServerHandleBuildingDespawned(Building building) {
        if ( building.connectionToClient.connectionId != connectionToClient.connectionId ) { return; }
        myBuildings.Remove(building);
    }


    #endregion

    // Client Side // # // # //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Client

    public override void OnStartAuthority() {
        if( NetworkServer.active ) { return; }

        Unit.AuthorityOnUnitSpawned   += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned   += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStartClient() {
        DontDestroyOnLoad(gameObject);
        if ( NetworkServer.active ) { return; }
        ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient() {
        ClientOnInfoUpdated?.Invoke();

        if ( !isClientOnly || !hasAuthority ) { return; }

        ( (RTSNetworkManager)NetworkManager.singleton ).Players.Remove(this);

        if ( !hasAuthority ) { return;
        }
        Unit.AuthorityOnUnitSpawned   -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned   -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    private void ClientHandleResourcesUpdated(int oldBalance, int newBalance) {
        ClientOnResourcesUpdated?.Invoke(newBalance);
    }
    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName) {
        ClientOnInfoUpdated?.Invoke();
    }

    // These methods only called on Client's game
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState) {
        if ( !hasAuthority ) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }
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
