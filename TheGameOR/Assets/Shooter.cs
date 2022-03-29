using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
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


    float ammoTime;
    void ShootNormal(PMove.dirs dir) {
        if (dir == PMove.dirs.f) return;

        float firerate = 10, speed = 10, spread = 8, lifetime = 10;
        
        if (lastGun != currentGun) ammoTime = 1 / firerate; //shoot immediately
        ammoTime += Time.fixedDeltaTime;

        while (ammoTime > 1 / firerate) {
            ammoTime -= 1 / firerate;
            SpawnBullets(1, speed, spread, lifetime);
        }
    }

    float shotgunCharge;
    void ShootShotgun(PMove.dirs dir) {
        float bulletsPerCharge = 5, speed = 15, spread = 17, lifetime = 0.3f;


        if (lastGun != currentGun) shotgunCharge = 0;

        if (dir == PMove.dirs.n) {
            if (lastDir != PMove.dirs.n) {
                SpawnBullets((int) (shotgunCharge * bulletsPerCharge), speed, spread, lifetime);
            }
            shotgunCharge = 0;
        }
        else {
            shotgunCharge += Time.fixedDeltaTime;
        }

        

    }

    void SpawnBullets(int amount, float speed, float spread, float lifetime) {
        for (int i = 0; i < amount; i++) {
            Bullet b = Instantiate(bullet, transform.position + new Vector3(0, 0, 1) + transform.right / 2, transform.rotation).GetComponent<Bullet>();
            b.origin = gameObject;
            b.vel = speed;
            b.spreadAngle = spread;
            Destroy(b.gameObject, lifetime);
        }

    }
}
