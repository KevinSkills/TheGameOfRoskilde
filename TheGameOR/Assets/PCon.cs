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
        cmdSpawnMyPlayer();
    }

    [Command]
    void cmdSpawnMyPlayer() {

        GameObject myUnit = Instantiate(playerPrefab, transform.position, transform.rotation);

        myUnit.GetComponent<PMove>().connection = this;
        myUnit.GetComponent<Shooter>().connection = this;

        
        

        NetworkServer.Spawn(myUnit, connectionToClient);

       
    }
}
