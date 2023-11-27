using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public Transform player;
    public GameObject wallPrefab;
    public int wallAhead;
    float playerX;
    public float distance;
    public float distanceAdd;

    void Start()
    {
        for (int i = 0; i < wallAhead; i++)
        {
            playerX += distance;
            Instantiate(wallPrefab, new Vector3(0, 0, playerX), Quaternion.identity);
        }

    }

    void Update()
    {
        if (player.position.z - playerX + (distance * wallAhead) >= 0)
        {
            playerX += distance;
            distance += distanceAdd;
            Instantiate(wallPrefab, new Vector3(0, 0, playerX), Quaternion.identity);
        }
    }
}
