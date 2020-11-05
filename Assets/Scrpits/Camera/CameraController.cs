﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrigemInput { MOUSE, JOYSTICK }

[RequireComponent(typeof(Camera))] [RequireComponent(typeof(CameraCollision))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Values")]
    [SerializeField]
    private float sensibilidade_cam = 1;
    [SerializeField]
    private float maxVerticalMovement = 60;
    [SerializeField]
    private float minVerticalMovement = -25;

    [Header("Target Values")]
    [SerializeField]
    private float targetOffset;
    [SerializeField]
    private Transform Target;

    [Header("Zoom Values")]
    [SerializeField]
    [Range(0.05f, 3f)]
    private float zoomSpeed = 1f;
    [SerializeField]
    [Range(2f, 10f)]
    private float maxZoom = 8f;
    [SerializeField]
    [Range(2f, 10f)]
    private float minZoom = 2f;
    [SerializeField]
    private float zoomSmooth = 0.5f;

    float zoomAmount = 0;

    float mouseX, mouseY, controleX, controleY;
    float controleXEsq, grauRotacao;

    CameraCollision camCollision;

    [HideInInspector]
    public bool Trava { get; set; }
    

    public Camera cam;

    Transform obstrucao;
    
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
        obstrucao = Target;
        camCollision = GetComponent<CameraCollision>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Transform GetTarget()
    {
        return Target;
    }

    void LateUpdate()
    {
        if (Player.player.estadoPlayer != EstadoPlayer.MORTO && !Trava)
        {
            CamControl(GameController.gameController.QualOrigemInput());
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            Zoom();
        }

        CamFolow();
        //CamObstruction();
    }

    void Zoom()
    {
        if (zoomAmount == maxZoom && Input.mouseScrollDelta.y < 0)
            return;

        if (zoomAmount == minZoom && Input.mouseScrollDelta.y > 0)
            return;

        float zoomDirection = (Input.mouseScrollDelta.y > 0) ? -0.3f : 0.3f;

        zoomAmount = Mathf.Clamp(zoomAmount + transform.forward.magnitude * zoomSpeed * zoomDirection, minZoom, maxZoom);
        camCollision.maxDistance = zoomAmount;
        //transform.position = Vector3.Lerp(transform.position, transform.position + (transform.forward * zoomSpeed * zoomDirection), 1 / zoomSmooth);
    }

    void CamFolow()
    {
        float x = Mathf.Lerp(Target.position.x, Player.player.transform.position.x, sensibilidade_cam);
        float y = Mathf.Lerp(Target.position.y, Player.player.transform.position.y + targetOffset, sensibilidade_cam);
        float z = Mathf.Lerp(Target.position.z, Player.player.transform.position.z, sensibilidade_cam);

        Target.position = new Vector3(x,y,z);
    }

    void CamControl(OrigemInput origemInput)
    {
        if (origemInput == OrigemInput.JOYSTICK)
        {

            controleX += Input.GetAxis("RightStickHorizontal") * sensibilidade_cam;
            controleY += Input.GetAxisRaw("RightStickVertical") * sensibilidade_cam;
            controleY = Mathf.Clamp(controleY, minVerticalMovement, maxVerticalMovement);

            transform.LookAt(Target);
            Target.rotation = Quaternion.Euler(controleY, controleX, 0);
            Player.player.transform.rotation = Quaternion.Euler(0, controleX, 0);

        }

        else
        {
            mouseX += Input.GetAxis("Mouse X") * sensibilidade_cam;
            mouseY += Input.GetAxis("Mouse Y") * sensibilidade_cam;
            mouseY = Mathf.Clamp(mouseY, minVerticalMovement, maxVerticalMovement);

            transform.LookAt(Target);

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
            }
            else
            {
                Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
                //Player.player.transform.rotation = Quaternion.Euler(0, mouseX, 0);
            }

        }
    }


   

    MeshRenderer desliga = null;
    void CamObstruction()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Target.position - transform.position, out hit, Vector3.Distance(transform.position, Player.player.transform.position)))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                obstrucao = hit.transform;

                desliga = GetMeshRenderer(obstrucao);

                if(desliga != null)
                    desliga.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if(Vector3.Distance(transform.position, obstrucao.position) >= 6f && Vector3.Distance(transform.position, Target.position) >= 3.5f)
                {
                    transform.Translate(transform.forward * zoomSpeed * Time.deltaTime);
                }
            }
            else
            {
                if(desliga != null)
                    desliga.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

                if(Vector3.Distance(transform.position, Target.position) < 7f)
                {
                    transform.Translate(-transform.forward * zoomSpeed * Time.deltaTime);
                }
            }
        }
    }

    MeshRenderer GetMeshRenderer(Transform pai)
    {
        MeshRenderer volta = pai.gameObject.GetComponent<MeshRenderer>();
        if(volta != null)
        {
            Debug.Log("1 " + volta.name);
            return volta;
        }

        for(int i = 0; i < pai.childCount; i++)
        {
            volta = GetMeshRenderer(pai.GetChild(i));

            if (volta != null)
            {
                Debug.Log(volta.name);
                break;
            }
        }

        return volta;
            
    }

}
