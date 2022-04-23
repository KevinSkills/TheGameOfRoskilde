using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GM : NetworkBehaviour
{

    public GameObject redWin, blueWin;

    public Quaternion rot0, rot1;
    public Text text0, text1;
    public static GM instance;
    public bool isReady = false;

    [SyncVar]
    int hp0 = 15, hp1 = 15;




    // Start is called before the first frame update
    void Start()
    {
        instance = this;


    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady && GameObject.FindGameObjectsWithTag("Player").Length > 1) isReady = true;

        text0.text = hp0.ToString();
        text1.text = hp1.ToString();
        if (!isServer) return;
        if (hp0 < 1 ) rpcResetGame(1);
        else if (hp1 < 1) rpcResetGame(0);
    }

    public void damage(GameObject player) {
        if (player.layer == 6) hp0--; //if it's on the player0 layer
        else hp1--;
    }
    
    [ClientRpc]
    private void rpcResetGame(int indexWin) {
        if (indexWin >= 0) Instantiate((indexWin == 0) ? redWin : blueWin);
        Bullet.resetAllBullets();
        hp0 = 15;
        hp1 = hp0;

        
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            PMove pm = player.GetComponent<PMove>();
            player.transform.position = pm.startPos;
            player.transform.rotation = pm.startRot;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        }
    }
}

