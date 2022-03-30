using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public int index;

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

        float firerate = 10, speed = 10, spread = 8, lifetime = 10, bulletMass = 0.1f, attractionRadius = 20, attractionStrength = 3;
        
        if (lastGun != currentGun) ammoTime = 1 / firerate; //shoot immediately
        ammoTime += Time.fixedDeltaTime;

        while (ammoTime > 1 / firerate) {
            ammoTime -= 1 / firerate;
            SpawnBullets(1, speed, spread, lifetime, bulletMass, attractionRadius, attractionStrength);
        }
    }

    float shotgunCharge;
    void ShootShotgun(PMove.dirs dir) {
        float bulletsPerCharge = 5, maxcharge = 2.5f, speed = 15, spread = 17,
            lifetime = 0.3f, bulletMass = 0.15f, knockback = 5, attractionRadius = 2, attractionStrength = 3;


        if (lastGun != currentGun) shotgunCharge = 0;

        if (dir == PMove.dirs.n) {
            if (lastDir != PMove.dirs.n) {
                GetComponent<Rigidbody2D>().velocity -= (Vector2)transform.right * knockback * Mathf.Sqrt(shotgunCharge);
                SpawnBullets((int) (shotgunCharge * bulletsPerCharge), speed, spread, lifetime, bulletMass, attractionRadius, attractionStrength);
            }
            shotgunCharge = 0;
        }
        else if (shotgunCharge < maxcharge){
            shotgunCharge += Time.fixedDeltaTime;
        }

        

    }

    void SpawnBullets(int amount, float speed, float spread, float lifetime, float bulletMass, float attractionRadius, float attractionStrength) {
        for (int i = 0; i < amount; i++) {
            Bullet b = Instantiate(bullet, transform.position + new Vector3(0, 0, 1) + transform.right / 2, transform.rotation).GetComponent<Bullet>();
            b.origin = gameObject;
            b.vel = speed;
            b.spreadAngle = spread;
            b.gameObject.layer = 9 + index; //Bullet0 is 9, Bullet1 is 10
            b.GetComponent<Rigidbody2D>().mass = bulletMass;
            b.attractionStrength = attractionStrength;
            foreach (CircleCollider2D col in b.GetComponents<CircleCollider2D>()) {
                if (col.isTrigger == true) {
                    col.radius = attractionRadius;
                    break;
                }
            }
            Destroy(b.gameObject, lifetime);
        }

    }
}
