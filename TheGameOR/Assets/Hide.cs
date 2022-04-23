using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hide : MonoBehaviour
{

    public Behaviour[] beforeStartOnly;
    public Behaviour[] afterStartOnly;


    // Start is called before the first frame update
    void Start()
    {        

    }

    // Update is called once per frame
    void Update()
    {
        if (!GM.instance) return; //Vi er endnu ikke spawnet
        if (GM.instance.isReady) {
            foreach (Behaviour b in beforeStartOnly) {
                b.enabled = false;
            }
            foreach (Behaviour b in afterStartOnly) {
                b.enabled = true;
            }
        }
        else {
            foreach (Behaviour b in beforeStartOnly) {
                b.enabled = true;
            }
            foreach (Behaviour b in afterStartOnly) {
                b.enabled = false;
            }
        }

    }
}
