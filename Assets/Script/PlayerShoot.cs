using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public Transform spawnPos;
    public GameObject bullet;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameObject rocket = Instantiate(bullet, spawnPos.position, spawnPos.rotation);
            rocket.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        }
    }
}
