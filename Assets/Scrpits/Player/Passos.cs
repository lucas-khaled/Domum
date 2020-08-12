using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passos : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip passos;
    private void Start()
    {
        audioSource = Player.player.audioSource;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AEO");
        if (other.gameObject.tag == "Chao" && Player.player.estadoPlayer != EstadoPlayer.ATACANDO)
        {
            audioSource.volume = Random.Range(0.8f, 1);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            audioSource.PlayOneShot(passos);
            Debug.Log("AIO");
        }
    }
}
