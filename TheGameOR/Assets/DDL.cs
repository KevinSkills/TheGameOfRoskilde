using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDL : MonoBehaviour
{



    public static string playerID;


    private void Awake()
    {
        PlayerPrefs.GetString("playerID", "");

        DontDestroyOnLoad(this.gameObject);
    }

    public void resetPlayerID()
    {

        PlayerPrefs.DeleteKey("playerID");
        playerID = "";
    }

    public static void setPlayerID(string pID)
    {
        DDL.playerID= pID;
        PlayerPrefs.SetString("playerID", pID);
    }

    
}
