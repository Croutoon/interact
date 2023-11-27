using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{

    public float speed = 2f;
    public float expForce = 10f;
    public float expRadius = 3f;
    public GameObject Explosion;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * speed, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ground")
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, expRadius);
            Transform player = null;
            foreach (var col in hitColliders)
            {
                if (col.transform.tag == "Player")
                {
                    Debug.Log("Wyalla");
                    player = col.transform;
                }
            }
            if(player != null)
            {
                player.GetComponent<Rigidbody>().AddForce((player.position - transform.position).normalized * expForce, ForceMode.Impulse);
                Debug.Log("Wyalla");
            }
            Instantiate(Explosion, transform.position, Quaternion.identity);
            GameObject.Destroy(transform.gameObject);
        }
    }
}
