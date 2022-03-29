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
    public float lerpTargetAcc;
    float _acc;

    public float accLerpUp, accLerpDown, accLerpDownDirChance;
    public float rotAcc;

    public float attackSpeed;
    
    float rotVel;

    dirs dir;

    public Rigidbody2D rb;
    public Shooter shooter;

    public KeyCode leftButton, rightButton;

    

    public enum dirs {
        f, l, r, n //forwards, left, right, no direction
    }

    private void Start() {
        _acc = acc;
    }

    void FixedUpdate()
    {

        //Input collection
        bool leftIn, rightIn;
        leftIn = Input.GetKey(leftButton);
        rightIn = Input.GetKey(rightButton);

        //Input processing
        if (leftIn && rightIn) dir = dirs.f;
        else if (leftIn) dir = dirs.l;
        else if (rightIn) dir = dirs.r;
        else dir = dirs.n;

        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        //If forwards
        if (dir == dirs.f) {
            bool changingDir = (Vector2.Angle(transform.right, rb.velocity) > 90);
            _acc = Mathf.Lerp(_acc, lerpTargetAcc, (changingDir) ? accLerpDownDirChance : accLerpDown);
            rb.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Time.fixedDeltaTime * _acc;
        }

        //If not forwards
        else {
            _acc = Mathf.Lerp(_acc, acc, accLerpUp);
            //If nothing
            if (dir == dirs.n) {
            }
            //If turning
            else {
                rotVel += rotAcc * Time.fixedDeltaTime * ((dir == dirs.l) ? 1 : -1);
            }
        }


        //Physics
        rb.velocity += new Vector2(0, -gravity * Time.fixedDeltaTime * ((dir == dirs.f) ? (Mathf.Sqrt((1 + Mathf.Cos(angle + Mathf.PI / 2)) / 2)) : 1));
        rb.velocity *= Mathf.Pow(drag, Time.fixedDeltaTime);

        //Rotation physics
        if (!Input.GetKey(KeyCode.Space)) transform.Rotate(0, 0, rotVel);
        rotVel *= Mathf.Pow(rotDrag * ((dir == dirs.f || dir == dirs.n) ? rotDragMP : 1), Time.fixedDeltaTime);

        //Shooting
        shooter.Updater(dir);
    }
}