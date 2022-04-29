using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public UnitMovement GetUnitMovement() => unitMovement;
    public Targeter GetTargeter() => targeter;

    #region Server

    public override void OnStartServer() { // Method called when Unit is spawned, only on server/host 
        //base.OnStartServer(); I don't need this??
        ServerOnUnitSpawned?.Invoke(this); // Invokes an action, defined in this class. Listened for on RTSPlayer class
    }
    public override void OnStopServer() {
        //base.OnStopServer(); I don't need this??
        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    //[Client]
    public override void OnStartClient() {
        if ( !isClientOnly || !hasAuthority ) { return; }
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    //[Client]
    public override void OnStopClient() {
        if ( !isClientOnly || !hasAuthority ) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select() {
        if ( !hasAuthority ) { return; }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect() {
        if ( !hasAuthority ) { return; }
        onDeselected?.Invoke();
    }
    #endregion
}
