using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NobleConnect.Mirror;
using Mirror;
using UnityEngine.Networking;

public class ConnectToNoble : MonoBehaviour 
{

    [HideInInspector]
    public NobleNetworkManager networkManager;



       

    public InputField manualInputHost;

    public InputField manualInputClient;

    public GameObject disconnectButton;

    public static ConnectToNoble instance;

    



    public string ip, port;

    public static string playerID;

    // Used to determine which GUI to display
    [HideInInspector]
    public bool isHost, isClient;

    bool connected;

    private void Awake()
    {
        instance = this;
        playerID = PlayerPrefs.GetString("playerID", "");

        
    }

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
        StartCoroutine(IE_startClientFromDB(manualInputClient.text));

    }

    public void startClient(string id)
    {
        
        StartCoroutine(IE_startClientFromDB(id));

    }


    IEnumerator IE_startClientFromDB(string idToJoin)
    {
        //Get ip and port from database (do this with webrequest)
        WWWForm form = new WWWForm();

        form.AddField("playerID", idToJoin);

        UnityWebRequest uwr = UnityWebRequest.Post("https://thegameor.000webhostapp.com/get.php", form);

        yield return uwr.SendWebRequest();
        string dataText = uwr.downloadHandler.text;

        if (dataText[0].Equals('0'))
        {

            //if it worked, then startclient with data:

            Debug.Log(dataText);

            string[] splitData = dataText.Remove(0, 1).Split(':'); //array with ip on 0 and port on 1

            print(splitData[0] + "    " + splitData[1]);

            startClient(splitData[0], ushort.Parse(splitData[1]));
        }
        else
        {
            print("ERROR: " + dataText);
        }
    }


    IEnumerator sendAdressToDB(string givenIp, string givenPort)
    {
        string id;
        if (manualInputHost.text.Equals("")) id = playerID;
        else
        {
            print("vivp");
            id = manualInputHost.text;
        }

        print("bool: " + manualInputHost.text);

        WWWForm form = new WWWForm();
        



        form.AddField("playerID", id);
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






    

    




    public void resetPlayerID()
    {
        print("eh");
        PlayerPrefs.DeleteKey("playerID");
        playerID = "";
        
    }

    public void setPlayerID(string pID)
    {
        playerID = pID;
        PlayerPrefs.SetString("playerID", pID);
        
    }
}
