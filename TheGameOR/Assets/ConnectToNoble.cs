using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NobleConnect.Mirror;
using Mirror;
using UnityEngine.Networking;

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

    bool connected;

    public void Start()
    {
        // Cast from Unity's NetworkManager to a NobleNetworkManager.
        networkManager = (NobleNetworkManager)NetworkManager.singleton;
    }


    public void startHost()
    {
        connected = false;
        isHost = true;
        isClient = false;

        networkManager.StartHost();


    }

    public void startClient()
    {
        //if no adress given, then get it 
        StartCoroutine(IE_startClientFromDB());

    }

    IEnumerator IE_startClientFromDB()
    {
        //Get ip and port from database (do this with webrequest)
        WWWForm form = new WWWForm();

        form.AddField("playerID", nfcIDToJoin.text);

        UnityWebRequest uwr = UnityWebRequest.Post("https://thegameor.000webhostapp.com/get.php", form);

        yield return uwr.SendWebRequest();
        string dataText = uwr.downloadHandler.text;

        if (dataText[0].Equals('0'))
        {

            //if it worked, then startclient with data:

            Debug.Log(dataText);

            string[] splitData = dataText.Remove(0).Split(':'); //array with ip on 0 and port on 1

            startClient(splitData[0], ushort.Parse(splitData[0]));
        }
        else
        {
            print("ERROR: " + dataText);
        }
    }


    IEnumerator sendAdressToDB(string givenIp, string givenPort)
    {
        
        WWWForm form = new WWWForm();

        form.AddField("playerID", nfcID.text);
        form.AddField("ip", givenIp);
        form.AddField("port", givenPort);

        UnityWebRequest uwr = UnityWebRequest.Post("https://thegameor.000webhostapp.com/set.php", form);

        yield return uwr.SendWebRequest();
        string dataText = uwr.downloadHandler.text;

        if (dataText[0].Equals('0'))
        {

            //if it worked, then startclient with data:

            Debug.Log("worked");


        }
        else
        {
            print("ERROR: " + dataText);
            Disconnect();
        }
    }

    public void startClient(string givenIp, int givenPort)
    {
        //Initialize client
        networkManager.InitClient();
        isHost = false;
        isClient = true;



        //set the ip and port in the network manager
        networkManager.networkAddress = givenIp;
        networkManager.networkPort = givenPort;


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
                
                if (!connected)
                {
                    connected = true;
                    StartCoroutine(sendAdressToDB(ip, port));

                }
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
