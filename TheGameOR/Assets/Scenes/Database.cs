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
        form.AddField("username", "john");
        form.AddField("password", "weeb");

        UnityWebRequest uwr = UnityWebRequest.Post("http://localhost/register.php", form);
        
        

        


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
