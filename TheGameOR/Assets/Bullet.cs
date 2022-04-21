using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public Rigidbody2D rb;

    public GameObject origin;

    // Start is called before the first frame update
    void Start() {
    }

    void Update() {
        if (Mathf.Abs(transform.position.x) > Camera.main.orthographicSize * Camera.main.aspect + 2
            || Mathf.Abs(transform.position.y) > Camera.main.orthographicSize + 2) 
        {
            if(GetComponent<SpriteRenderer>().enabled)PCon.localPCon.resetBullet(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isServer) return;
        if (collision.gameObject.tag == "Player") {
            if (collision.gameObject == origin) return;
            print("øhhh?");
            GM.instance.damage(collision.gameObject);
            rpcResetBulletAfterHit(gameObject);
        }
    }

    [ClientRpc]
    void rpcResetBulletAfterHit(GameObject bullet) {
        PCon.localPCon.resetBullet(bullet);
    }

    public static void resetAllBullets() {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Bullet")) {
            PCon.localPCon.resetBullet(g);
        }
    }

}
