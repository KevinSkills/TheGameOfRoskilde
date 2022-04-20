using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PMove : NetworkBehaviour
{

    public PCon connection;

    public float gravity;
    public float drag;
    public float rotDrag;
    public float rotDragMP;

    public float acc;
    public float lerpTargetAcc;
    float _acc;

    public float accLerpUp, accLerpDown, accLerpDownDirChange;
    public float rotAcc;

    public float wallPushForceFloor;
    public float wallPushForceCeiling;
    public float wallPushForceSide;
    public float wallBounciness;
    
    float rotVel;

    Vector2 estimatedPosition;

    dirs dir;

    public Rigidbody2D rb;
    public Shooter shooter;

    float angle;

    public enum dirs {
        f, l, r, n //forwards, left, right, no direction
    }

    private void Start() {
        _acc = acc;
    }

    void FixedUpdate()
    {
        if (hasAuthority) {
            GetInputs();
        }

        Physics(Time.fixedDeltaTime);

        if (hasAuthority) cmdSendMoveData(); //maybe not every frame?

        

        //Shooting
        //shooter.Updater(dir);
    }

    private void Physics(float deltaTime) {
        //If forwards
        if (dir == dirs.f) {
            bool changingDir = (Vector2.Angle(transform.right, rb.velocity) > 90);

            
            _acc = Mathf.Lerp(_acc, lerpTargetAcc, 1 - Mathf.Pow(1 - ((changingDir) ? accLerpDownDirChange : accLerpDown), 50 * deltaTime));
            rb.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * deltaTime * _acc;
        }

        //If not forwards
        else {
            _acc = Mathf.Lerp(_acc, acc, 1 - Mathf.Pow(1 - accLerpUp, 50 * deltaTime));
            //If nothing
            if (dir == dirs.n) {
            }
            //If turning
            else {
                rotVel += rotAcc * deltaTime * ((dir == dirs.l) ? 1 : -1);
            }
        }

        //Update velocities
        rb.velocity += new Vector2(0, -gravity * Time.deltaTime * ((dir == dirs.f) ? (Mathf.Sqrt((1 + Mathf.Cos(angle + Mathf.PI / 2)) / 2)) : 1));
        rb.velocity *= Mathf.Pow(drag, Time.deltaTime);
        rb.velocity = HandleWallCollisions(transform.position, rb.velocity, transform.localScale.x / 2);

        //Update rotational velocities
        if (!Input.GetKey(KeyCode.Space)) transform.Rotate(0, 0, rotVel);
        rotVel *= Mathf.Pow(rotDrag * ((dir == dirs.f || dir == dirs.n) ? rotDragMP : 1), Time.deltaTime);

        if (hasAuthority) estimatedPosition = transform.position;
        else estimatedPosition += rb.velocity;
    }

    public void GetInputs() {
        //Input collection
        bool leftIn, rightIn;
        leftIn = Input.GetButton("Left" + connection.pIndex);
        rightIn = Input.GetButton("Right" + connection.pIndex);

        //Input processing
        if (leftIn && rightIn) dir = dirs.f;
        else if (leftIn) dir = dirs.l;
        else if (rightIn) dir = dirs.r;
        else dir = dirs.n;

        angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }

    public Vector3 HandleWallCollisions(Vector2 inputPosition, Vector2 inputVelocity, float radius) {
        Vector2 vel = inputVelocity;
        if (inputPosition.y > Camera.main.orthographicSize - radius && inputVelocity.y > 0) { //Ceiling
            vel = new Vector2(vel.x, -vel.y * wallBounciness - wallPushForceCeiling);
        }
        if (inputPosition.y < -Camera.main.orthographicSize + radius && inputVelocity.y < 0) { //Floor
            vel = new Vector2(vel.x, -vel.y * wallBounciness + wallPushForceFloor);
        }
        if (inputPosition.x > Camera.main.aspect * Camera.main.orthographicSize - radius && inputVelocity.x > 0) { //Right Side
            vel = new Vector2(-vel.x * wallBounciness - wallPushForceSide, vel.y);
        }
        if (inputPosition.x < Camera.main.aspect * -Camera.main.orthographicSize + radius && inputVelocity.x < 0) { //Left Side
            vel = new Vector2(-vel.x * wallBounciness + wallPushForceSide, vel.y);
        }
        return vel;
    }

    public void cmdSendMoveData() {

    }
}