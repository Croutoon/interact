using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    bool onSlope = false;

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

        }
    }
    
    void Jump()
    {
        if(Input.GetKey(KeyCode.Space) && mS == MoveState.SLOPE)
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
            Debug.Log(angle.ToString("0.0"));

            if (angle <= maxSlopeAngle)
            {
                mS = MoveState.SLOPE;
            }
        }
        else
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
}
