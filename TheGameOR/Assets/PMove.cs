using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PMove : NetworkBehaviour
{

    [SyncVar]
    public PCon connection;

    [SyncVar]
    public Color color;

    public GameObject tester;

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

    float timer;

    public enum dirs {
        f, l, r, n //forwards, left, right, no direction
    }

    private void Start() {
        timer = Time.time;
        GetComponent<SpriteRenderer>().color = color;
        _acc = acc;
        //if (hasAuthority) StartCoroutine("SendMoveData");
        tester = GameObject.Find("tester");
    }

    void FixedUpdate()
    {
        if (!hasAuthority) tester.transform.position = new Vector2(5, _acc/10);

        if (hasAuthority) {
            GetInputs();
        }
        else {
            //rb.bodyType = RigidbodyType2D.Static;
            transform.position = Vector3.Lerp(transform.position, estimatedPosition, 0.1f);
        }

        Physics();

        if (hasAuthority && Time.time > timer + 0.1f) {
            timer = Time.time;
            cmdSendMoveData((float)NetworkTime.rtt, dir, transform.position, rb.velocity, transform.rotation.eulerAngles.z, rotVel, _acc);
        }

        //Shooting
        //shooter.Updater(dir);
    }

    //This doesn't work consistently for different deltatimes
    private void Physics() {
        //If forwards
        if (dir == dirs.f) {
            bool changingDir = (Vector2.Angle(transform.right, rb.velocity) > 90);

            
            _acc = Mathf.Lerp(_acc, lerpTargetAcc, 1 - Mathf.Pow(1 - ((changingDir) ? accLerpDownDirChange : accLerpDown), 50 * Time.fixedDeltaTime));
            angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            rb.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Time.fixedDeltaTime * _acc;
        }

        //If not forwards
        else {
            _acc = Mathf.Lerp(_acc, acc, 1 - Mathf.Pow(1 - accLerpUp, 50 * Time.fixedDeltaTime));
            //If nothing
            if (dir == dirs.n) {
            }
            //If turning
            else {
                rotVel += rotAcc * Time.fixedDeltaTime * ((dir == dirs.l) ? 1 : -1);
            }
        }

        //Update velocities
        rb.velocity += new Vector2(0, -gravity * Time.fixedDeltaTime * ((dir == dirs.f) ? (Mathf.Sqrt((1 + Mathf.Cos(angle + Mathf.PI / 2)) / 2)) : 1));
        rb.velocity *= Mathf.Pow(drag, Time.fixedDeltaTime);
        rb.velocity = HandleWallCollisions(estimatedPosition, rb.velocity, transform.localScale.x / 2);

        //Update rotational velocities
        if (!Input.GetKey(KeyCode.Space)) transform.Rotate(0, 0, rotVel);
        rotVel *= Mathf.Pow(rotDrag * ((dir == dirs.f || dir == dirs.n) ? rotDragMP : 1), Time.fixedDeltaTime);

        if (hasAuthority) estimatedPosition = transform.position;
        else estimatedPosition += rb.velocity * Time.fixedDeltaTime;
    }

    public void GetInputs() {
        //Input collection
        bool leftIn, rightIn;
        leftIn = Input.GetButton("Left"); //use +connection.pIndex if you want different controls for the 2 players. Since we are online, we want the same
        rightIn = Input.GetButton("Right");

        //Input processing
        if (leftIn && rightIn) dir = dirs.f;
        else if (leftIn) dir = dirs.l;
        else if (rightIn) dir = dirs.r;
        else dir = dirs.n;
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
        if (vel != inputVelocity) print("bounce");
        return vel;
    }

    [Command]
    public void cmdSendMoveData(float rtt, dirs direction, Vector2 position, Vector2 velocity, float zRotation, float rotationalVelocity, float variableAccelaration) {

        rpcSendMoveData(rtt, direction, position, velocity, zRotation, rotationalVelocity, variableAccelaration); ;
    }

    [ClientRpc]
    public void rpcSendMoveData(float rtt, dirs direction, Vector2 position, Vector2 velocity, float zRotation, float rotationalVelocity, float variableAccelaration) {
        
        if (hasAuthority) return; //we already have the best data, so we don't care about what the server tells us;

        dir = direction;
        estimatedPosition = position; //only set the estimated position to avoid jumps in the actual position
        rb.velocity = velocity;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
        rotVel = rotationalVelocity;
        _acc = variableAccelaration;

        float timeToArrive = (float)NetworkTime.rtt / 2 + rtt/2;
        while (timeToArrive > Time.fixedDeltaTime) {
            timeToArrive -= Time.fixedDeltaTime;
            Physics();
        }
    }
}