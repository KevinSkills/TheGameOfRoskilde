using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Text playerIDText;

    public GameObject registerPanel;

    public GameObject loadingPanel;

    public GameObject settingsPanel;


    private void Start()
    {

        

        playerIDText.text = ConnectToNoble.playerID;
    }



    // Update is called once per frame
    void Update()
    {
        playerIDText.text = ConnectToNoble.playerID;

        if (ConnectToNoble.playerID.Equals("")) registerPanel.SetActive(true);
        else registerPanel.SetActive(false);

        loadingPanel.SetActive(false);
        
        if(ConnectToNoble.instance.isHost || (ConnectToNoble.instance.isClient && ConnectToNoble.instance.networkManager.client != null)) 
            loadingPanel.SetActive(true);

    }



    public void toggleSettingsPanel()
    {
        print("hej");
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
