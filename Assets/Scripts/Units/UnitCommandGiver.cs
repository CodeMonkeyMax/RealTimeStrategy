using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; } // check for RMB press
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()); // create ray from mousedown
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask) ) { return; } // check if ray hits anything
        TryMove(hit.point); // order move to ray hit
    }

    private void TryMove(Vector3 point) {
        foreach(Unit unit in unitSelectionHandler.SelectedUnits ) {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
