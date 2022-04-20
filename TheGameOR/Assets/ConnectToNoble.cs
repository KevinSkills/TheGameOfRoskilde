using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NobleConnect.Mirror;
using Mirror;

public class ConnectToNoble : MonoBehaviour
{
    
    NobleNetworkManager networkManager;

    public Text status;

    public InputField nfcID;

    public InputField nfcIDToJoin;

    public GameObject disconnectButton;

    public string ip, port;

    // Used to determine which GUI to display
    bool isHost, isClient;

    public void Start()
    {
        // Cast from Unity's NetworkManager to a NobleNetworkManager.
        networkManager = (NobleNetworkManager)NetworkManager.singleton;
    }


    public void startHost()
    {
        isHost = true;
        isClient = false;

        networkManager.StartHost();

    }

    public void startClient()
    {
        //Initialize client
        networkManager.InitClient();
        isHost = false;
        isClient = true;

        //Get ip and port from database
        networkManager.networkAddress = "getIpFromDatabase()";
        networkManager.networkPort = ushort.Parse("getPortFromDatabase()");
        
        //start client
        networkManager.StartClient();
    }

    private void Update()
    {
        disconnectButton.SetActive(true);
        if (isHost)
        {
            //Host stuff
            if (networkManager.HostEndPoint != null)
            {
                ip = networkManager.HostEndPoint.Address.ToString();
                port = networkManager.HostEndPoint.Port.ToString();
                status.text = "Connected :D - ip: " + ip + ":" + port;
                //database tilføj her
            }

            

            if (!NobleServer.active) isHost = false;

        }
        else if (isClient && networkManager.client != null)
        {
            //Client stuff
            
            


        }
        else
        {
            disconnectButton.SetActive(false);
        }


    }

    public void Disconnect()
    {
        if (isHost)
        {
            networkManager.StopHost();
            isHost = false;
        }else if (isClient)
        {
            if (networkManager.client.isConnected)
            {
                // If we are already connected it is best to quit gracefully by sending
                // a disconnect message to the host.
                networkManager.client.Disconnect();
            }
            else
            {
                // If the connection is still in progress StopClient will cancel it
                networkManager.StopClient();
            }
            isClient = false;
        }
    }




}
