using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPush : MonoBehaviour
{

    public float force;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * force);
        }
    }
}
