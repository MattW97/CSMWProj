using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;

    public GameObject midPoint;

    private float marginDistance;

    private Vector3 newCameraPos;
    private Vector3 middlePoint;
    private Quaternion targetRotation;
    private float distanceFromMiddlePoint;
    private float distanceBetweenPlayers;
    private float cameraDistance;
    private float aspectRatio;
    private float fov;
    private float tanFov;

    private float p1DistToMid;
    private float p2DistToMid;
    private float p3DistToMid;
    private float p4DistToMid;

    void Start()
    {

        aspectRatio = Screen.width / Screen.height;
        tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);
        newCameraPos = Camera.main.transform.position;
    }

    void LateUpdate()
    {

        p1DistToMid = Vector3.Distance(player1.position, midPoint.transform.position);
        p2DistToMid = Vector3.Distance(player2.position, midPoint.transform.position);
        p3DistToMid = Vector3.Distance(player3.position, midPoint.transform.position);
        p4DistToMid = Vector3.Distance(player4.position, midPoint.transform.position);

        // Find the middle point between players.
        //IF 2 PLAYERS
        Vector3 vectorBetweenPlayers = (player1.position / 2) + (player2.position / 2);
        //IF 3 PLAYERS
        //Vector3 vectorBetweenPlayers = (player1.position / 3) + (player2.position / 3) + (player3.position / 3);
        //IF 4 PLAYERS
        //Vector3 vectorBetweenPlayers = (player1.position / 4) + (player2.position / 4) + (player3.position / 4) + (player4.position / 4);
        middlePoint = vectorBetweenPlayers;

        // Calculate the new distance.
        //IF 2 PLAYERS
        distanceBetweenPlayers = (p1DistToMid + p2DistToMid) / 2;
        //IF 3 PLAYERS
        //distanceBetweenPlayers = (p1DistToMid + p2DistToMid + p3DistToMid) / 3;
        //IF 4 PLAYERS
        //distanceBetweenPlayers = (p1DistToMid + p2DistToMid + p3DistToMid + p4DistToMid) / 4;

        marginDistance = 1 - (1 / distanceBetweenPlayers);
        newCameraPos.y = (1 - (1 / distanceBetweenPlayers)) * 25.0f;
        newCameraPos.z = midPoint.transform.position.z + (-1 * (newCameraPos.y * 0.466f));

        //Debug.Log(distanceBetweenPlayers);

        Camera.main.transform.position = new Vector3(midPoint.transform.position.x, newCameraPos.y, (0.75f + newCameraPos.z));
    }

    void FixedUpdate()
    {

        midPoint.transform.position = middlePoint + new Vector3(0, 1.25f, 0);
    }
}

