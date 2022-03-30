using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float vel;
    public float spreadAngle;
    public Rigidbody2D rb;

    public GameObject origin;

    public float attractionStrength;

    // Start is called before the first frame update
    void Start() {
        transform.Rotate(0, 0, Random.Range(-spreadAngle, spreadAngle));
        rb.velocity = transform.right * vel;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            if (collision.gameObject == origin) return;
            GM.instance.damage(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "PlayerCol") {
            if (collision.gameObject == origin) return;
            rb.velocity += (Vector2)(collision.transform.position - transform.position) * attractionStrength * Time.fixedDeltaTime;
        }
    }

    public static void destroyAllBullets() {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Bullet")) {
            Destroy(g);
        }

    }

}
