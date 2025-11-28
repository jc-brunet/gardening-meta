using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class SpaceWarpManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OVRManager.SetSpaceWarp(true);
        
    }

    public void ActivateSpaceWarp()
    {
        OVRManager.SetSpaceWarp(true);
    }

    public void DeactivateSpaceWarp()
    {
        OVRManager.SetSpaceWarp(false);
    }

    // Call this from any client to deactivate SpaceWarp
    public void RequestDisableSpaceWarp()
    {
        if (IsClient)
        {
            DisableSpaceWarpServerRpc();
        }
    }

    // Call this from any client to activate SpaceWarp
    public void RequestEnableSpaceWarp()
    {
        if (IsClient)
        {
            EnableSpaceWarpServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableSpaceWarpServerRpc(ServerRpcParams rpcParams = default)
    {
        DisableSpaceWarpClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableSpaceWarpServerRpc(ServerRpcParams rpcParams = default)
    {
        EnableSpaceWarpClientRpc();
    }

    [ClientRpc]
    private void DisableSpaceWarpClientRpc(ClientRpcParams rpcParams = default)
    {
        OVRManager.SetSpaceWarp(false);
    }

    [ClientRpc]
    private void EnableSpaceWarpClientRpc(ClientRpcParams rpcParams = default)
    {
        OVRManager.SetSpaceWarp(true);
    }
}
