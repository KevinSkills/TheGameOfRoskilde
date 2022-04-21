using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shooter : NetworkBehaviour
{

    [SyncVar]
    public PCon connection;

    public GameObject bullet;

    const int indexNormal = 0, indexShotgun = 1;
    public int currentGun = 0;
    int lastGun;

    PMove.dirs lastDir = PMove.dirs.n;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Updater(PMove.dirs dir) {
        switch (currentGun) {

            case indexNormal:
                ShootNormal(dir);
                break;
            case indexShotgun:
                ShootShotgun(dir);
                break;
        }
        lastGun = currentGun;
        lastDir = dir;
    }


    float ammoTime = -0.2f;
    float releaseTime;
    void ShootNormal(PMove.dirs dir) {

        releaseTime += Time.fixedDeltaTime;
        if (dir != PMove.dirs.f && releaseTime > 0.2f) return;
        if (dir == PMove.dirs.f) releaseTime = Mathf.Min(0, releaseTime - Time.fixedDeltaTime);

        float firerate = 10, speed = 10, spread = 8, velFromPlayer = 0.3f;
        
        if (lastGun != currentGun) ammoTime = 1 / firerate; //shoot immediately

        ammoTime += Time.fixedDeltaTime;
        while (ammoTime > 1 / firerate) {
            ammoTime -= 1 / firerate;
            ShootBullet(1, speed, spread, velFromPlayer);
        }
    }

    float shotgunCharge;
    void ShootShotgun(PMove.dirs dir) {
        float bulletsPerCharge = 5, maxcharge = 2.5f, speed = 15, spread = 17, knockback = 5, velFromPlayer = 0.5f;


        if (lastGun != currentGun) shotgunCharge = 0;

        if (dir == PMove.dirs.n) {
            if (lastDir != PMove.dirs.n) {
                GetComponent<Rigidbody2D>().velocity -= (Vector2)transform.right * knockback * Mathf.Sqrt(shotgunCharge);
                ShootBullet((int) (shotgunCharge * bulletsPerCharge), speed, spread, velFromPlayer);
            }
            shotgunCharge = 0;
        }
        else if (shotgunCharge < maxcharge){
            shotgunCharge += Time.fixedDeltaTime;
        }
    }

    void ShootBullet(int amount, float speed, float spread, float velFromPlayer) {
        for (int i = 0; i < amount; i++) {
            Bullet b = PCon.localPCon.activateBullet(transform.position + new Vector3(0, 0, 1) + transform.right / 2, transform.rotation).GetComponent<Bullet>();
            b.origin = gameObject;
            b.gameObject.layer = 9 + connection.pIndex; //Bullet0 is 9, Bullet1 is 10

            b.transform.Rotate(0, 0, Random.Range(-spread, spread));

            Rigidbody2D bRB = b.GetComponent<Rigidbody2D>();
            bRB.velocity = (Vector2) b.transform.right * speed + GetComponent<Rigidbody2D>().velocity * velFromPlayer;

            cmdShootBullet((float)NetworkTime.rtt, b.transform.position, b.transform.rotation, b.origin, b.gameObject.layer, bRB.velocity);
        }
    }
    
    [Command]
    void cmdShootBullet(float rtt, Vector2 position, Quaternion rotation, GameObject origin, int layerIndex, Vector2 velocity) {
        rpcShootBullet(rtt, position, rotation, origin, layerIndex, velocity);
    }

    [ClientRpc]
    void rpcShootBullet(float rtt, Vector2 position, Quaternion rotation, GameObject origin, int layerIndex, Vector2 velocity) {
        if (hasAuthority) return; 

        Vector2 calculatedPosition = position + (float)(rtt / 2 + NetworkTime.rtt / 2) * velocity;
        Bullet b = PCon.localPCon.activateBullet(calculatedPosition, rotation).GetComponent<Bullet>();
        b.origin = origin;
        b.gameObject.layer = layerIndex;

        b.GetComponent<Rigidbody2D>().velocity = velocity;
    }
}
