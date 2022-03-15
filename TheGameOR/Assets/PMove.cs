using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMove : MonoBehaviour
{

    public float gravity;
    public float drag;
    public float rotDrag;
    public float rotDragMP;

    public float acc;
    public float rotAcc;

    float rotVel;

    dirs dir;

    public Rigidbody2D rb;

    

    enum dirs {
        f, l, r, n //forwards, left, right, no direction
    }

    void FixedUpdate()
    {
        //Input collection
        bool leftIn, rightIn;
        leftIn = Input.GetKey(KeyCode.LeftArrow);
        rightIn = Input.GetKey(KeyCode.RightArrow);

        //Input processing
        if (leftIn && rightIn) dir = dirs.f;
        else if (leftIn) dir = dirs.l;
        else if (rightIn) dir = dirs.r;
        else dir = dirs.n;

        //If forwards
        if (dir == dirs.f) {
            float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            rb.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Time.fixedDeltaTime * acc;
        }
        //If nothing
        else if (dir == dirs.n) { }
        //If turning
        else rotVel += rotAcc * Time.fixedDeltaTime * ((dir == dirs.l) ? 1 : -1);

        //Physics
        rb.velocity += new Vector2(0, -gravity * Time.fixedDeltaTime * ((dir == dirs.f) ? 0 : 1));
        rb.velocity *= Mathf.Pow(drag * ((dir == dirs.f) ? 0.01f : 1), Time.fixedDeltaTime);

        //Rotation physics
        transform.Rotate(0, 0, rotVel);
        
        rotVel *= Mathf.Pow(rotDrag * ((dir == dirs.f || dir == dirs.n) ? rotDragMP : 1), Time.fixedDeltaTime);


        print(transform.rotation.z);
    }
}