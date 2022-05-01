using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject buildingPreview;
    [SerializeField] private Sprite     icon = null;
    [SerializeField] private int        id = -1;
    [SerializeField] private int        price = 100;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned; 
    
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject GetBuildingPreview() => buildingPreview;
    public Sprite GetIcon() => icon;
    public int GetId()      => id;
    public int GetPrice()   => price;

    #region Server

    public override void OnStartServer() {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer() {
        ServerOnBuildingDespawned?.Invoke(this);
    }
    #endregion

    #region Client

    public override void OnStartAuthority() {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }
    public override void OnStopAuthority() {
        AuthorityOnBuildingDespawned?.Invoke(this);

    }
    #endregion
}