using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mapa : MonoBehaviour
{
    [SerializeField]
    Vector2 terrainDimesions;
    [SerializeField]
    private Sprite marcador;
    [SerializeField]
    private GameObject marcadorCenaPrefab;
    [SerializeField]
    private GameObject telaMapa;

    RectTransform rectTransform;
    GameObject marcadorAtual = null;

    GameObject marcadorCenaAtual = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && gameObject.activeInHierarchy)
        {
            PutMarker();
        }
    }

    void PutMarker()
    {
        Vector3 pos = Input.mousePosition;
        GameObject marcador = new GameObject("Marcador", typeof(RectTransform), typeof(Image));

        RectTransform rectMarcador = marcador.GetComponent<RectTransform>();
        Image imageMarcador = marcador.GetComponent<Image>();

        rectMarcador.pivot = Vector2.zero;
        rectMarcador.anchorMax = Vector2.zero;
        rectMarcador.anchorMin = Vector2.zero;

        rectMarcador.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 75);
        rectMarcador.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 75);


        imageMarcador.sprite = this.marcador;
        imageMarcador.preserveAspect = true;


        rectMarcador.position = pos;

        marcador.transform.SetParent(gameObject.transform, true);

        rectMarcador.localScale = Vector3.one;

        Debug.Log("Pos on 2D: " + rectMarcador.anchoredPosition);

        if (rectMarcador.anchoredPosition.x < rectTransform.rect.width && rectMarcador.anchoredPosition.y < rectTransform.rect.height)
        {
            if (marcadorAtual != null)
            {
                Destroy(marcadorAtual);
                marcadorAtual = null;
            }
            marcadorAtual = marcador;

            Vector3 posCena = new Vector3(rectMarcador.anchoredPosition.x * (terrainDimesions.x / rectTransform.rect.width), 30f,
                rectMarcador.anchoredPosition.y * (terrainDimesions.y / rectTransform.rect.height));

            Debug.Log("Pos on 3D" + posCena);

            if (marcadorCenaAtual != null)
                marcadorCenaAtual.transform.position = posCena;
            else
                marcadorCenaAtual = Instantiate(marcadorCenaPrefab, posCena, marcadorCenaPrefab.transform.rotation);
        }
        else
        {
            Destroy(marcador);
        }
    }

    Ray rayMan = new Ray(Vector3.zero, Vector3.zero);
    Vector3 hitPos;

    public void FastTravel(RectTransform buttonTransform)
    {
        Vector2 correctPosition = new Vector2(buttonTransform.anchoredPosition.x + buttonTransform.rect.width/2, buttonTransform.anchoredPosition.y + buttonTransform.rect.height/2);

        Vector3 pos = new Vector3(correctPosition.x * (terrainDimesions.x / rectTransform.rect.width), 100,
            correctPosition.y * (terrainDimesions.y / rectTransform.rect.height));

        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit hit;       

        Physics.Raycast(ray, out hit, 100,LayerMask.GetMask("Ground"));

        rayMan = ray;
        hitPos = hit.point;

        Player.player.transform.position = hit.point;
        telaMapa.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if(rayMan.origin != Vector3.zero)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawRay(rayMan);
            Gizmos.DrawWireSphere(hitPos, 1);
            Debug.Log(rayMan.origin);
        }
    }

}
