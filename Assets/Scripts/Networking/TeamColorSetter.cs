using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamColor = new Color();

    #region Server

    public override void OnStartServer() { // This script is placed on objects. Upon spawning, such object will determine its player owner, and find that player's color
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        teamColor = player.GetTeamColor(); // Triggers syncvar
    }
    #endregion

    #region Client

    private void HandleTeamColorUpdated(Color oldColor, Color newColor) {
        foreach(Renderer renderer in colorRenderers ) {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }
    #endregion
}
