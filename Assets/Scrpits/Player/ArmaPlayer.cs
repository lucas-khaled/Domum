using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaPlayer : MonoBehaviour
{
    [SerializeField]
    private int Dano;

    public SkinnedMeshRenderer mesh;

    public int danoAux;
    public static ArmaPlayer armaPlayer;

    private void Awake()
    {
        if (gameObject.activeSelf)
            Inventario.inventario.armaMesh = mesh;
    }

    private void Start()
    {
        armaPlayer = this;
        danoAux = 0;
    }

    private int CalculaDano()
    {
        return Dano + Inventario.inventario.armaEquipada.dano + Random.Range(-5, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (danoAux > 0)
        {
            if (other.gameObject.tag == "Inimigo" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
            {

                Debug.Log("UIUI inimigo");
                other.gameObject.GetComponent<Inimigo>().ReceberDano(CalculaDano());

            }
            if (other.gameObject.tag == "Girafa" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
            {

                Debug.Log("UIUI girafa");
                other.gameObject.GetComponent<Girafa>().ReceberDano(CalculaDano());

            }
            if (other.gameObject.tag == "Tigre" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
            {

                Debug.Log("UIUI tigron");
                other.gameObject.GetComponent<Tigre>().ReceberDano(CalculaDano());

            }
            danoAux--;
        }
    }
}
