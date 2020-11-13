using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private bool followPlayerRotation = true;
    private void LateUpdate()
    {
        Vector3 newPosition =  Player.player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        Vector3 euler = (followPlayerRotation) ? Player.player.transform.eulerAngles : CameraController.cameraInstance.GetTarget().eulerAngles;
        euler.x = 90;

        transform.rotation = Quaternion.Euler(euler);
    }
}
