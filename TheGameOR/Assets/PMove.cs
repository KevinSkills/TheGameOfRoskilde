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

    public float attackSpeed;
    bool isShooting;
    
    float rotVel;

    dirs dir;

    public Rigidbody2D rb;
    public GameObject bullet;

    

    enum dirs {
        f, l, r, n //forwards, left, right, no direction
    }

    private void Start() {
        StartCoroutine(Shoot());
    }

    void FixedUpdate()
    {
        isShooting = false;

        //Input collection
        bool leftIn, rightIn;
        leftIn = Input.GetKey(KeyCode.LeftArrow);
        rightIn = Input.GetKey(KeyCode.RightArrow);

        //Input processing
        if (leftIn && rightIn) dir = dirs.f;
        else if (leftIn) dir = dirs.l;
        else if (rightIn) dir = dirs.r;
        else dir = dirs.n;

        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        //If forwards
        if (dir == dirs.f) {
            rb.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Time.fixedDeltaTime * acc;
        }
        //If nothing
        else if (dir == dirs.n) {
            isShooting = true;
        }
        //If turning
        else {
            isShooting = true;
            rotVel += rotAcc * Time.fixedDeltaTime * ((dir == dirs.l) ? 1 : -1);
        }

        //Physics
        rb.velocity += new Vector2(0, -gravity * Time.fixedDeltaTime * ((dir == dirs.f) ? (Mathf.Sqrt((1 + Mathf.Cos(angle + Mathf.PI / 2)) / 2)) : 1));
        rb.velocity *= Mathf.Pow(drag, Time.fixedDeltaTime);

        //Rotation physics
        transform.Rotate(0, 0, rotVel);
        
        rotVel *= Mathf.Pow(rotDrag * ((dir == dirs.f || dir == dirs.n) ? rotDragMP : 1), Time.fixedDeltaTime);


    }

    IEnumerator Shoot () {
        while (true) {
            print("ahhhs");
            if (isShooting) {
                Instantiate(bullet, transform.position + new Vector3(0, 0, 1) + transform.right / 2, transform.rotation);
            }
            yield return new WaitForSeconds(1 / attackSpeed);
        }
    }
}