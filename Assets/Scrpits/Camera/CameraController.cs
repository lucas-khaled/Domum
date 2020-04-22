﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    private enum OrigemInput {MOUSE, JOYSTICK}

    private OrigemInput origem;

    public float Sensibilidade_cam = 1;
    public Transform Target;

    public Transform player;
    float mouseX, mouseY, controleX, controleY;
    float controleXEsq, grauRotacao;
    

    public Camera cam;

    /*public Transform posicaoMira;
    
    public Transform posicaoInicial;
    */
    
    #region SINGLETON
    public static CameraController cameraInstance;
     private void Awake()
     {
         cameraInstance = this;
         cam = GetComponent<Camera>();
         
         //EventsController.onTyvaMira += ChangeCameraPosition;
     }
    #endregion

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        CamControl(QualOrigemInput());
    }

    void CamControl(OrigemInput origemInput)
    {
        if (origemInput == OrigemInput.JOYSTICK)
        {
            controleX += Input.GetAxis("RightStickHorizontal") * Sensibilidade_cam;
            controleY -= Input.GetAxis("RightStickVertical") * Sensibilidade_cam;
            controleY = Mathf.Clamp(controleY, -35, 120);

            transform.LookAt(Target);
            Target.rotation = Quaternion.Euler(controleY, controleX, 0);
            player.rotation = Quaternion.Euler(0, controleX, 0);

        }

        else
        {
            mouseX += Input.GetAxis("Mouse X") * Sensibilidade_cam;
            mouseY -= Input.GetAxis("Mouse Y") * Sensibilidade_cam;
            mouseY = Mathf.Clamp(mouseY, -35, 60);

            transform.LookAt(Target);

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
            }
            else
            {
                Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
                player.rotation = Quaternion.Euler(0, mouseX, 0);
            }

        }
    }


    private OrigemInput QualOrigemInput()
    {
        OrigemInput origemAtual = OrigemInput.MOUSE;

        string[] temp = Input.GetJoystickNames();
        if (temp.Length > 0 && !string.IsNullOrEmpty(temp[0]))
            origemAtual = OrigemInput.JOYSTICK;

        return origemAtual;
    }

    /*void ChangeCameraPosition(bool ligado)
    {
        Debug.Log("Camera Listened" +  ligado);
        if (ligado)
            StartCoroutine(MoveCamera(posicaoMira));
        else
            StartCoroutine(MoveCamera(posicaoInicial));
    }

    IEnumerator MoveCamera(Transform target)
    {
        camFollow.IsChanging(true);
        float newX = transform.localPosition.x;
        float newY = transform.localPosition.y;
        float newZ = transform.localPosition.z;

        Vector3 diference = target.localPosition - transform.localPosition; 

        while (Vector3.Distance(transform.localPosition, target.localPosition) > 0) {

            newX = Mathf.Clamp(newX + diference.x * 0.1f, Mathf.Min(transform.localPosition.x, target.localPosition.x), Mathf.Max(transform.localPosition.x, target.localPosition.x));
            newY = Mathf.Clamp(newY + diference.y * 0.1f, Mathf.Min(transform.localPosition.y, target.localPosition.y), Mathf.Max(transform.localPosition.y, target.localPosition.y));
            newZ = Mathf.Clamp(newZ + diference.z * 0.1f, Mathf.Min(transform.localPosition.z, target.localPosition.z), Mathf.Max(transform.localPosition.z, target.localPosition.z));

            transform.localPosition = new Vector3(newX, newY, newZ);
            yield return new WaitForSeconds(0.2f);
        }

        camFollow.IsChanging(false);
        Debug.Log("Cheguei");
    }*/

}