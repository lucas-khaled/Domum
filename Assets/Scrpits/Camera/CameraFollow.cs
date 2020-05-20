using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private void LateUpdate()
    {
        Vector3 newPosition = Player.player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
