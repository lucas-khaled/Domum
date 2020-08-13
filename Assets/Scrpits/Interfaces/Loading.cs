using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    public void Carregar()
    {
        StartCoroutine(Carregamento());
    }

    private IEnumerator Carregamento()
    {
        yield return new WaitForSeconds(1f);
        AsyncOperation level = SceneManager.LoadSceneAsync("Level");
        while (level.progress < 1)
        {
            yield return new WaitForEndOfFrame();
        }

    }
}
