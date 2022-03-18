using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        form.AddField("username", "Quang");
        form.AddField("password", "Quang1234");

        WWW www = new WWW("http://localhost/register.php", form);

        

        yield return www;

        if(www.text.Equals("0"))
        {
            print("hej");
        }else
        {
            print("ERROR: " + www.text);
        }

    
    
    }

}
