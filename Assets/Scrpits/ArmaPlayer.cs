using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaPlayer : MonoBehaviour
{
    [SerializeField]
    private int Dano;
    private Inimigo inimigo;
    private int CalculaDano()
    {
        return Dano + Random.Range(-5, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Inimigo" && Player.player.estadoPlayer == EstadoPlayer.ATACANDO)
        {
            inimigo.ReceberDano(CalculaDano());
        }
    }
}
