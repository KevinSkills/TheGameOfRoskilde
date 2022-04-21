using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PCon : NetworkBehaviour {

    public int pIndex;

    public GameObject playerPrefab;

    

    // Start is called before the first frame update
    void Start() {
        if (!isLocalPlayer) return;
        
        pIndex = GameObject.FindGameObjectsWithTag("PlayerConnection").Length - 1;
        cmdSpawnMyPlayer(pIndex);
    }

    [Command]
    void cmdSpawnMyPlayer(int index) {

        GameObject myUnit = Instantiate(playerPrefab, transform.position, transform.rotation);

        myUnit.GetComponent<PMove>().connection = this;
        myUnit.GetComponent<Shooter>().connection = this;

        if (index == 1) myUnit.GetComponent<PMove>().color = new Color32((byte)1,   (byte)206, (byte)255, (byte)255);
                   else myUnit.GetComponent<PMove>().color = new Color32((byte)254, (byte)49,  (byte)0,   (byte)255);





        NetworkServer.Spawn(myUnit, connectionToClient);

       
    }
}
