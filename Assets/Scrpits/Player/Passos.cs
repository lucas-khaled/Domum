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
    
    private Player playerRef;

    public static Passos passos;
    public bool caminhando;

    private void Start()
    {
        passos = this;
        audioSource = Player.player.audioSource;
        playerRef = Player.player;
        playerRef.audioNovo = "Terra";
    }
    public void Passo()
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
