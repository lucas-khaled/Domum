using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAmbient : MonoBehaviour
{
    [SerializeField]
    private GameObject[] audios;
    [SerializeField]
    private GameObject audioAtual;
    [SerializeField]
    private string audioPassos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(TrocaAudio());
        }
        Player.player.audioNovo = audioPassos;
    }

    private IEnumerator TrocaAudio()
    {
        for (int i = 0; i < audios.Length; i++)
        {
            AudioSource volume = audios[i].GetComponent<AudioSource>();
            while (volume.volume > 0)
            {
                yield return new WaitForEndOfFrame();
                volume.volume -= 0.05f;
            }
        }
        AudioSource volumeAtual = audioAtual.GetComponent<AudioSource>();
        while (volumeAtual.volume < 1)
        {
            yield return new WaitForEndOfFrame();
            volumeAtual.volume += 0.05f;
        }
    }
}
