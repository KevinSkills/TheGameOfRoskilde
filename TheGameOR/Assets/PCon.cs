using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PCon : NetworkBehaviour {

    public int pIndex;

    public GameObject playerPrefab;

    GameObject myUnit;

    // Start is called before the first frame update
    void Start() {
        if (!isLocalPlayer) return;
        pIndex = GameObject.FindGameObjectsWithTag("PlayerConnection").Length - 1;
        cmdSpawnMyPlayer();
    }

    [Command]
    void cmdSpawnMyPlayer() {

        myUnit = Instantiate(playerPrefab, transform.position, transform.rotation);
        myUnit.GetComponent<PMove>().connection = this;
        myUnit.GetComponent<Shooter>().connection = this;

        NetworkServer.Spawn(myUnit);
        myUnit.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }
}
