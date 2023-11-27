using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shake : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Cam; // set this via inspector
    public Rigidbody rb;
    public float decreaseFactor = 1.0f;
    public float startShake = 1.0f;

    float shakeAmmount;

    void Update()
    {
        if (rb.velocity.y < startShake)
        {
            shakeAmmount = (rb.velocity.y - startShake) * decreaseFactor;
            Cam.localPosition = Random.insideUnitSphere * shakeAmmount;
        }
    }
}

