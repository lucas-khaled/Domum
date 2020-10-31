using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passos : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip passosTerra;
    [SerializeField]
    private AudioClip passosAreia;
    [SerializeField]
    private AudioClip passosCascalho;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float tamanhoRayCast;
    public bool podeTocar;
    [SerializeField]
    private GameObject outroPe;
    private Player playerRef;
    private float delay;
    [SerializeField]
    private float delayTempo;

    public static Passos passos;
    public bool caminhando;

    private void Start()
    {
        delayTempo = delay;
        passos = this;
        audioSource = Player.player.audioSource;
        playerRef = Player.player;
        playerRef.audioNovo = "Terra";
    }
    private void Update()
    {
        if (podeTocar)
        delay -= Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, tamanhoRayCast, layerMask) && caminhando && podeTocar && delay <= 0)
        {
            Debug.Log("A");
            delay = delayTempo;

            outroPe.GetComponent<Passos>().podeTocar = true;
            podeTocar = false;

            audioSource.volume = Random.Range(0.8f, 1);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            if (playerRef.audioNovo == "Terra")
                audioSource.PlayOneShot(passosTerra);
            else if (playerRef.audioNovo == "Areia")
                audioSource.PlayOneShot(passosAreia);
            else
                audioSource.PlayOneShot(passosCascalho);
        }
    }
    public void Passo()
    {
        //PeAlto = true;
    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Chao" && Player.player.estadoPlayer != EstadoPlayer.ATACANDO && caminhando)
        {
            audioSource.volume = Random.Range(0.8f, 1);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            if (playerRef.audioNovo == "Terra")
                audioSource.PlayOneShot(passosTerra);
            else if (playerRef.audioNovo == "Areia")
                audioSource.PlayOneShot(passosAreia);
            else
                audioSource.PlayOneShot(passosCascalho);
        }
    }*/
}
