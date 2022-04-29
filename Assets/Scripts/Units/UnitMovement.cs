using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    [ServerCallback]
    private void Update() {
        Targetable target = targeter.GetTarget();

        if(target != null ) {
            if ( ( target.transform.position - transform.position ).sqrMagnitude > chaseRange * chaseRange ) {
                //chase
                agent.SetDestination(target.transform.position);
            }
            else if( agent.hasPath ){
                //stop chase
                agent.ResetPath();
            }
            return;
        }

        if ( !agent.hasPath ) { return; }
        if ( agent.remainingDistance > agent.stoppingDistance ) { return; }
        agent.ResetPath();
    }
    [Command] 
    public void CmdMove(Vector3 position) {
        targeter.ClearTarget();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
            return;
        }
        agent.SetDestination(hit.position);
    }
    #endregion

}
