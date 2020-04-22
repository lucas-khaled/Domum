﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ArmaPlayer : MonoBehaviour
{
    [SerializeField]
    private int Dano;
    private Inimigo inimigo;

    private void Start()
    {
        if (gameObject.activeSelf)
            Inventario.inventario.armaMesh = GetComponent<SkinnedMeshRenderer>();
    }

    private int CalculaDano()
    {
        return Dano + Inventario.inventario.armaEquipada.dano + Random.Range(-5, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Inimigo" && Player.player.estadoPlayer == EstadoPlayer.ATACANDO)
        {
            inimigo.ReceberDano(CalculaDano());
        }
    }
}