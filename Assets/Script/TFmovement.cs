using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TFmovement : MonoBehaviour
{

    [Header("References:")]
    public Transform orientation;

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

    [Space(10)]
    [Header("Jump:")]
    public float jumpForce;

    [Space(10)]
    [Header("Ground sphere check:")]
    public LayerMask whatIsGround;
    public Transform spherePosition;
    public float sphereRadius;
    public float maxSlopeAngle;

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

                //Vector3 horizontal = Vector3.ProjectOnPlane(orientation.forward, wallRunNormal) * .5f;
                //Vector3 vertical = Vector3.ProjectOnPlane(Camera.main.transform.forward, wallRunNormal);

                Vector3 horizontal = Camera.main.transform.right * Input.GetAxis("Horizontal");
                Vector3 vertical = Camera.main.transform.forward * Input.GetAxis("Vertical");
                Vector3 camDir = horizontal + vertical;
                wallDirection = Vector3.ProjectOnPlane(camDir, wallRunNormal);

                rb.AddForce(wallDirection * wallSpeed, ForceMode.Force);
                break;

        }
    }
    
    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && mS == MoveState.SLOPE)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void UpdateState()
    {
        if (Physics.CheckSphere(spherePosition.position, sphereRadius, whatIsGround))
        {
            mS = MoveState.GROUND;

            Physics.Raycast(spherePosition.position, -Vector3.up, out slopeHit, Mathf.Infinity, whatIsGround);
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            if (angle <= maxSlopeAngle)
            {
                mS = MoveState.SLOPE;
            }
        }
        else if (!Physics.CheckSphere(spherePosition.position, sphereRadius * 1.2f, whatIsGround))
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
                rb.useGravity = false;
                break;
        }
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
                }
            }
        }
    }
}
