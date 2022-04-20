using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTest : MonoBehaviour
{
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rb.velocity + (Vector2) transform.parent.position;
    }
}
