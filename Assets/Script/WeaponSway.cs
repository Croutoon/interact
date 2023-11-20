using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public Transform targetRot;
    public Transform targetPos;
    public float rotSpeed;
    public float posSpeed;
    public float yOffset;
    public float tuff;

    // Update is called once per frame
    void LateUpdate()
    {
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot.rotation, Time.deltaTime * rotSpeed);


        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetPos.position.y + yOffset, transform.position.z), Time.deltaTime * (posSpeed + (transform.position.y - targetPos.position.y) * tuff));
        transform.position = new Vector3(targetPos.position.x, transform.position.y, targetPos.position.z);

    }
}
