using UnityEngine;
using Unity.Netcode;
using Unity.Services.Vivox;
using System;
using Oculus.Avatar2;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class VivoxVoiceInitializer : NetworkBehaviour
{

    [SerializeField] string channelName;
    private Channel3DProperties _channel3DProperties;

    async void Start()
    {
        _channel3DProperties = new Channel3DProperties(50, 5, 0.250f, AudioFadeModel.ExponentialByDistance);
        //await UnityServices.InitializeAsync();
        //if (!AuthenticationService.Instance.IsSignedIn) { await AuthenticationService.Instance.SignInAnonymouslyAsync(); }
        //await VivoxService.Instance.InitializeAsync();
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("correctly signed in vivox");
            await VivoxService.Instance.LoginAsync();
            await VivoxService.Instance.JoinPositionalChannelAsync(channelName, ChatCapability.AudioOnly, _channel3DProperties);
        }
        else { Debug.Log("signedin failed"); }
    }

    override public void OnNetworkDespawn()
    {
        VivoxService.Instance.LeaveAllChannelsAsync();
        LogoutOfVivoxAsync();
    }


    public async void LogoutOfVivoxAsync()
    {
        await VivoxService.Instance.LogoutAsync();
    }


}
