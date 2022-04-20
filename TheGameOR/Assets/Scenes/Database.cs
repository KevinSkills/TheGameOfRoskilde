using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Database : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(register());
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    IEnumerator register() {
        WWWForm form = new WWWForm();
        form.AddField("port", "john");
        form.AddField("ip", "123");
        form.AddField("playerID", "121223");

        UnityWebRequest uwr = UnityWebRequest.Post("https://thegameor.000webhostapp.com/get.php", form);
        
        

        


        yield return uwr.SendWebRequest();
        string dataText = uwr.downloadHandler.text;

        if (dataText.Equals("0"))
        {
            print("hej");
        }else
        {
            print("ERROR: " + dataText);
        }

    
    
    }

}
