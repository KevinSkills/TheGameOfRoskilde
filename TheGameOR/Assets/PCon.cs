using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PCon : NetworkBehaviour {

    public static PCon localPCon;

    public int pIndex;

    public GameObject playerPrefab;

    //Bullet pool stuff
    public int poolSize;
    public GameObject bulletPrefab;
    Queue<GameObject> bulletQueue = new Queue<GameObject>();

    




    // Start is called before the first frame update
    void Start() {

        if (!isLocalPlayer) return;
        localPCon = this;

        pIndex = GameObject.FindGameObjectsWithTag("PlayerConnection").Length - 1;
        cmdSpawnMyPlayer(pIndex);

        if (pIndex == 0) cmdInitiateBullets();
        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Bullet")) {
            bulletQueue.Enqueue(b);
            EnableComponents(b, false);
        }
    }

    [Command]
    void cmdSpawnMyPlayer(int index) {

        GameObject myUnit = Instantiate(playerPrefab, transform.position, transform.rotation);

        myUnit.GetComponent<PMove>().connection = this;
        myUnit.GetComponent<Shooter>().connection = this;

        if (index == 1) myUnit.GetComponent<PMove>().color = new Color32((byte)1,   (byte)206, (byte)255, (byte)255);
                   else myUnit.GetComponent<PMove>().color = new Color32((byte)254, (byte)49,  (byte)0,   (byte)255);

        NetworkServer.Spawn(myUnit, connectionToClient);

        GM.instance.SetPlayer(myUnit);
    }

    [Command]
    void cmdInitiateBullets () {
        for (int i = 0; i < poolSize; i++) {
            GameObject b = Instantiate(bulletPrefab);
            NetworkServer.Spawn(b);
        }
    }

    void EnableComponents(GameObject g, bool b) {
        g.GetComponent<Rigidbody2D>().bodyType = (b) ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        g.GetComponent<SpriteRenderer>().enabled = b;
        g.GetComponent<Collider2D>().enabled = b;
    }


    public GameObject activateBullet(Vector3 position, Quaternion rotation) {
        if (bulletQueue.Count < 1) Debug.LogWarning("Unable to spawn bullet");

        //put in the back of the queue 
        GameObject b = bulletQueue.Dequeue();

        //conf
        b.transform.position = position;
        b.transform.rotation = rotation;
        //endconf
        EnableComponents(b, true);
        return b;
    }

    public void resetBullet(GameObject b) {
        EnableComponents(b, false);
        bulletQueue.Enqueue(b);
    }
}
