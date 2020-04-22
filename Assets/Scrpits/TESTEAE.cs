using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTEAE : MonoBehaviour
{
    void Update()
    {
        if (Input.GetAxis("RightStickVertical") != 0)
        {
            Debug.Log("DISGRAÇA");
        }
    }
}
