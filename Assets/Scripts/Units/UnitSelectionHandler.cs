using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    
    private Camera mainCamera;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start() {

        mainCamera = Camera.main;
    }

    private void Update() {

        if ( Mouse.current.leftButton.wasPressedThisFrame ) {
            foreach ( Unit selectedUnit in SelectedUnits ) {
                selectedUnit.Deselect();
            }
            SelectedUnits.Clear();
        } else if( Mouse.current.leftButton.wasReleasedThisFrame ) {
            ClearSelectionAre();
        }
    }

    private void ClearSelectionAre() {
        
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if ( !Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask) ) { return; }
        if(!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; } // This test also returns reference to Unit. Handy dandy as FUCK!
        if ( !unit.hasAuthority ) { return; }
        SelectedUnits.Add(unit);
        foreach(Unit selectedUnit in SelectedUnits ) {
            selectedUnit.Select();
        }
    }
}
