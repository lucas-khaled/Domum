using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysVisibleOnMinimap : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;
    GameObject instanciaVista = null;

    [SerializeField]
    [Range(1, 30)]
    float radius = 1;
    [SerializeField]
    Camera miniMapCamera;
    [SerializeField]
    GameObject prefabDeInstancia;
    [SerializeField]
    bool cutOff = false;

    public void SetMinimapCameraAndIconPrefab(GameObject icon, Camera minimapCam)
    {
        miniMapCamera = minimapCam;
        prefabDeInstancia = icon;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();

        if(cutOff)
            StartCoroutine(SeDesliga());
    }

    IEnumerator SeDesliga()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        gameObject.SetActive(false);
        if (instanciaVista != null)
            Destroy(instanciaVista);

        Debug.Log("Desliguei memo");
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (spriteRenderer.enabled)
            {
                ShowIconInMinimap(spriteRenderer);
                return;
            }

            if (lineRenderer.enabled)
            {
                ShowIconInMinimap(lineRenderer);
                return;
            }
        }
    }

    void ShowIconInMinimap(Renderer renderer)
    {
        if (renderer.IsVisibleFrom(miniMapCamera))
        {
            if (instanciaVista != null)
            {
                Destroy(instanciaVista.gameObject);
                instanciaVista = null;
            }
        }
        else
        {
            if (instanciaVista != null)
                instanciaVista.transform.position = RendererExtensions.GetMinimimapPosition(miniMapCamera, transform.position,radius);
            else
            {
                instanciaVista = Instantiate(prefabDeInstancia, RendererExtensions.GetMinimimapPosition(miniMapCamera, transform.position,radius), transform.rotation);
                instanciaVista.transform.localScale = this.transform.localScale;
            }
        }
    }
}

public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public static Vector3 GetMinimimapPosition(Camera camera, Vector3 pos, float radius)
    {
        Vector3 finalPoint = Vector3.zero;

        Vector3 camPoint = camera.transform.position;
        camPoint.y = pos.y;

        Ray ray = new Ray(pos, camPoint - pos);

        finalPoint = ray.GetPoint(Vector3.Distance(camPoint, pos) - radius);

        return finalPoint;
    }
    
}
