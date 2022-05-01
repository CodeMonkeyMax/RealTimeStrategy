using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text resourcesText = null;
    private RTSPlayer player;
    
    // Update is called once per frame
    void Update()
    {
        if ( player == null ) {
            try {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();// connection.identity is connection's "NetworkIdentity" component, present on Player GameObject.
                if ( player != null ) {
                    ClientHandleResourcesUpdated(player.GetResources());
                    player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                }

            } catch {
                Debug.Log("Player null! :(");
            }
        }
    }

    private void OnDestroy() {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources) {
        resourcesText.text = $"Resources: {resources}";
    }
}
