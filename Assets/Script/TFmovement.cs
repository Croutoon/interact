using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TFmovement : MonoBehaviour
{

    [Header("References:")]
    public Transform orientation;
    public Transform cameraAxis;
    public Camera cam;

    [Space(10)]
    [Header("Ground Movement:")]
    public float groundSpeed;
    public float groundDrag;

    [Space(10)]
    [Header("Air Movement:")]
    public float airSpeed;
    public float airDrag;

    [Space(10)]
    [Header("Wall Movement:")]
    public float wallDrag;
    public float wallSpeed;
    public float wallRunTime;
    public float counterGravityForce;

    float currentWRF;
    float currentCGF;
    float currentWRT;
    bool wallFall = false;

    [Space(10)]
    [Header("Jump:")]
    public float jumpForce;
    public float doubleJumpForce;
    public float wallJumpForce;
    public float wallLeapForce;

    [Space(10)]
    [Header("Ground sphere check:")]
    public LayerMask whatIsGround;
    public Transform spherePosition;
    public float sphereRadius;
    public float maxSlopeAngle;

    [Space(10)]
    [Header("Camera:")]
    public float tiltSpeed = 1f;
    public float tilt = 10f;

    //Components
    Rigidbody rb;

    //Variables
    Vector3 direction;
    Vector3 wallDirection;
    Vector3 wallRunNormal;

    public enum MoveState
    {
        GROUND,
        SLOPE,
        AIR,
        WALLRUNNING
    }

    [Space(10)]
    public MoveState mS;

    RaycastHit slopeHit;
    bool canWallRun = true;
    bool canDoubleJump = true;

    float targetTilt = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        direction = GetDirection(orientation).normalized;
        Jump();
    }

    void FixedUpdate()
    {
        UpdateState();
        UpdatePhysics();
        Move();
    }

    void LateUpdate()
    {
        if(mS == MoveState.WALLRUNNING)
        {
            targetTilt = getCameraTilt() * tilt;
        }
        switch (mS)
        {
            case MoveState.WALLRUNNING:
                targetTilt = getCameraTilt() * tilt;
                break;
            default:
                targetTilt = 0;
                break;
        }

        cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, Quaternion.Euler(0, 0, targetTilt), Time.deltaTime * tiltSpeed);
    }

    void Move()
    {
        switch (mS)
        {
            case MoveState.GROUND:
                rb.AddForce(direction * groundSpeed, ForceMode.Force);
                break;
            case MoveState.SLOPE:
                rb.AddForce(GetSlopeDireciton() * groundSpeed, ForceMode.Force);
                break;
            case MoveState.AIR:
                rb.AddForce(direction * airSpeed, ForceMode.Force);
                break;
            case MoveState.WALLRUNNING:
                rb.AddForce(-wallRunNormal * 50, ForceMode.Force);

                Vector3 horizontal = cameraAxis.right * Input.GetAxis("Horizontal");
                Vector3 vertical = cameraAxis.forward * Input.GetAxis("Vertical");
                Vector3 camDir = horizontal + vertical;
                wallDirection = Vector3.ProjectOnPlane(camDir, wallRunNormal);

                currentWRT += 0.1f;

                if(currentWRT > wallRunTime)
                {
                    currentWRT = 0f;
                    wallFall = true;
                }

                if (wallFall)
                {
                    if (currentCGF > 0)
                    {
                        currentCGF = currentCGF - currentWRT * counterGravityForce;
                    }
                    else
                    {
                        currentCGF = 0;
                    }
                }

                rb.AddForce(wallDirection * currentWRF, ForceMode.Force);
                rb.AddForce(Vector3.up * currentCGF, ForceMode.Force);
                break;

        }
    }
    
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mS == MoveState.SLOPE)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            if(mS == MoveState.WALLRUNNING)
            {
                rb.AddForce(Vector3.up * wallJumpForce, ForceMode.Impulse);
                rb.AddForce(wallLeapForce * wallRunNormal, ForceMode.Impulse);
                rb.AddForce(wallLeapForce * orientation.forward, ForceMode.Impulse);
            }
            if(mS == MoveState.AIR && canDoubleJump)
            {

                Vector3 velH = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                Vector3 newVelH = GetDirection(orientation) * velH.magnitude;

                rb.velocity = new Vector3(newVelH.x, rb.velocity.y, newVelH.z);
                rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);

                canDoubleJump = false;
            }
        }
        
    }

    void UpdateState()
    {
        if (Physics.CheckSphere(spherePosition.position, sphereRadius, whatIsGround))
        {
            mS = MoveState.GROUND;
            canDoubleJump = true;

            Physics.Raycast(spherePosition.position, -Vector3.up, out slopeHit, Mathf.Infinity, whatIsGround);
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            if (angle <= maxSlopeAngle)
            {
                mS = MoveState.SLOPE;
            }
        }
        else if (!Physics.CheckSphere(spherePosition.position, sphereRadius * 1.1f, whatIsGround))
        {
            mS = MoveState.AIR;
        }

    }

    void UpdatePhysics()
    {
        switch (mS)
        {
            case MoveState.GROUND:
                rb.drag = groundDrag;
                rb.useGravity = false;
                
                break;
            case MoveState.AIR:
                rb.drag = airDrag;
                rb.useGravity = true;
                break;
            case MoveState.SLOPE:
                rb.drag = groundDrag;
                rb.useGravity = false;
                break;
            case MoveState.WALLRUNNING:
                rb.drag = wallDrag;
                rb.useGravity = true;
                break;
        }
    }

    float getCameraTilt()
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(wallRunNormal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = Vector3.SignedAngle(Vector3.up, wallRunNormal, Quaternion.AngleAxis(90f, rotDir) * wallRunNormal);
        angle -= 90;
        angle /= 180;
        Vector3 playerDir = orientation.forward;
        Vector3 normal = new Vector3(wallRunNormal.x, 0, wallRunNormal.z);
        return Vector3.Cross(playerDir, normal).y * angle;
    }

    Vector3 GetDirection(Transform dir)
    {
        return dir.forward * Input.GetAxis("Vertical") + dir.right * Input.GetAxis("Horizontal");
    }

    Vector3 GetSlopeDireciton()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }
    
    Vector3 GetWallRunningDirection()
    {
        return Vector3.ProjectOnPlane(wallDirection, wallRunNormal);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            Vector3 wallNormal = collision.contacts[0].normal;
            float angle = Vector3.Angle(wallNormal, Vector3.up);
            if(angle > 45)
            {
                wallRunNormal = wallNormal;
                if(canWallRun)
                {
                    mS = MoveState.WALLRUNNING;
                    currentWRF = wallSpeed;
                    currentCGF = counterGravityForce;
                    currentWRT = 0;
                    wallFall = false;
                    canDoubleJump = true;
                }
            }
        }
    }
}
