using EosSharp.Core.Api.v1;
using System.Collections;
using System.Collections.Generic;
using AnchorLinkSharp;
using Assets.Packages.AnchorLinkTransportSharp.Src;
using Assets.Packages.AnchorLinkTransportSharp.Src.StorageProviders;
using Assets.Packages.AnchorLinkTransportSharp.Src.Transports.UiToolkit;
using Assets.Packages.UniversalAuthenticatorLibrary.Src.UiToolkit;
using UnityEngine;

public class MultiUiExampleScript : MonoBehaviour
{
    private UnityUAL _unityUal;
    private User _user;

    public UnityCanvasUAL UnityCanvasUAL;
    public UnityUiToolkitUAL UnityUiToolkitUAL;

    public bool UseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        if (UseCanvas)
            _unityUal = UnityCanvasUAL;
        else
            _unityUal = UnityUiToolkitUAL;

        _unityUal.OnUserLogin += OnLoggedIn;
        _unityUal.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void OnLoggedIn(User user)
    {
        _user = user;
        await _user.SignTransaction(new Action[] {
            new Action()
                {
                    account = "eosio.token",
                    name = "transfer",
                    authorization = new List<PermissionLevel>() {  }, // TODO
                    data = new Dictionary<string, object>()
                    {
                        { "from", await _user.GetAccountName() },
                        { "to", "test3.liq" },
                        { "quantity", "1 WAX" },
                        { "memo", "Anchor is the best! Thank you <3" }
                    }
                }
        });
    }

}
