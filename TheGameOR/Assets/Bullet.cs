using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float vel;
    public float spreadAngle;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0, 0, Random.Range(-spreadAngle, spreadAngle));
        rb.velocity = transform.right * vel;
    }

}
