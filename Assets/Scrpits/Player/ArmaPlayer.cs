using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaPlayer : MonoBehaviour
{
    [SerializeField]
    private int Dano;
    private BoxCollider colisor;


    public SkinnedMeshRenderer mesh;
    [SerializeField]
    private ParticleSystem particleHit;
    public static ArmaPlayer armaPlayer;

    private void Awake()
    {
        armaPlayer = this;
    }

    private void Start()
    {      
        colisor = this.GetComponent<BoxCollider>();
        if (gameObject.activeSelf)
            Inventario.inventario.armaMesh = mesh;
    }

    public int CalculaDano()
    {
        return Dano + Inventario.inventario.armaEquipada.dano + Random.Range(-5, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        bool doParticle = false;
        if (other.gameObject.tag == "Inimigo" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
        {
            other.gameObject.GetComponent<Inimigo>().ReceberDano(CalculaDano());
            colisor.enabled = false;
            doParticle = true;
        }
        if (other.gameObject.tag == "Girafa" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
        {
            other.gameObject.GetComponent<Girafa>().ReceberDano(CalculaDano());
            colisor.enabled = false;
            doParticle = true;
        }
        if (other.gameObject.tag == "Tigre" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
        {
            other.gameObject.GetComponent<Tigre>().ReceberDano(CalculaDano());
            colisor.enabled = false;
            doParticle = true;
        }

        if (particleHit != null && doParticle)
        {
            ParticleSystem particula = Instantiate(particleHit.gameObject, transform.position, particleHit.transform.rotation).GetComponent<ParticleSystem>();
            particula.Play();
            Destroy(particula.gameObject, 5);
        }
    }
}
