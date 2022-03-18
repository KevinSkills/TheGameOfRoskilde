using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float vel;
    public float spreadAngle;
    public Rigidbody2D rb;

    public GameObject origin;


    // Start is called before the first frame update
    void Start() {
        transform.Rotate(0, 0, Random.Range(-spreadAngle, spreadAngle));
        rb.velocity = transform.right * vel;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            if (collision.gameObject == origin) return;

            Destroy(gameObject);
        }
    }

}
