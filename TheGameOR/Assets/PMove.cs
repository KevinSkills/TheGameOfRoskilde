using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PMove : NetworkBehaviour
{

    [SyncVar]
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

        Physics();

        if (hasAuthority) cmdSendMoveData(dir, transform.position, rb.velocity, transform.rotation.eulerAngles.z, rotVel, _acc); //maybe not every frame?

        else {
            rb.bodyType = RigidbodyType2D.Static;
            transform.position = Vector3.Lerp(transform.position, estimatedPosition, 1f);
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

    [Command]
    public void cmdSendMoveData(dirs direction, Vector2 position, Vector2 velocity, float zRotation, float rotationalVelocity, float variableAccelaration) {
        //Maybe check if the movement is valid?
        dir = direction;
        transform.position = position;
        rb.velocity = velocity;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
        rotVel = rotationalVelocity;
        _acc = variableAccelaration;

        float timeToArrive = (float)NetworkTime.rtt / 2;
        while (timeToArrive > Time.fixedDeltaTime) {
            timeToArrive -= Time.fixedDeltaTime;
            Physics();
        }

        rpcSendMoveData(dir, transform.position, rb.velocity, transform.rotation.eulerAngles.z, rotVel, _acc); ;
    }

    [ClientRpc]
    public void rpcSendMoveData(dirs direction, Vector2 position, Vector2 velocity, float zRotation, float rotationalVelocity, float variableAccelaration) {

        if (hasAuthority) return; //we already have the best data, so we don't care about what the server tells us;

        dir = direction;
        estimatedPosition = position; //only set the estimated position to avoid jumps in the actual position
        rb.velocity = velocity;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
        rotVel = rotationalVelocity;
        _acc = variableAccelaration;

        float timeToArrive = (float)NetworkTime.rtt / 2;
        while (timeToArrive > Time.fixedDeltaTime) {
            timeToArrive -= Time.fixedDeltaTime;
            Physics();
        }
    }
}