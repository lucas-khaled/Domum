using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passos : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip passos;
    [SerializeField]
    private LayerMask layerChao;
    private void Start()
    {
        audioSource = Player.player.audioSource;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerChao)
        {
            audioSource.volume = Random.Range(0.8f, 1);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            audioSource.PlayOneShot(passos);
        }
    }
}
