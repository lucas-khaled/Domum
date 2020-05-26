using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private void LateUpdate()
    {

        transform.parent = Player.player.transform;
        Vector3 newPosition = Player.player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
