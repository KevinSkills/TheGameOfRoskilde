using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletPooler : NetworkBehaviour
{
    public int poolSize;

    public Transform bulletPool;

    public GameObject bullet;

    public static BulletPooler instance;

    Queue<GameObject> bulletQueue = new Queue<GameObject>();

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Debug.LogError("Multiple BulletPools");
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cmdSpawnTest();
        if (isServer) {
            for (int i = 0; i < poolSize; i++) {
                GameObject b = Instantiate(bullet, bulletPool);
                b.SetActive(true);
                bulletQueue.Enqueue(b);
                NetworkServer.Spawn(b);

            }
        }
        else {
            foreach(Transform t in bulletPool) {
                bulletQueue.Enqueue(t.gameObject);
            }
        }
    }

    [Command]
    void cmdSpawnTest() {
        NetworkServer.Spawn(Instantiate(bullet));
    }
  


}
