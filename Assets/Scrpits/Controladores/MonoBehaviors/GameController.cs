using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    private TipoPlayer qualPlayer;

    private void Awake()
    {
        gameController = this;
        DontDestroyOnLoad(this);
    }

    public void EscolherPersonagem(string personagem)
    {

        if(personagem == "Iovelik")
        {
            qualPlayer = TipoPlayer.IOVELIK;
        }
        else
        {
            qualPlayer = TipoPlayer.TYVA;
        }
    }

    public TipoPlayer GetPersonagemEscolhido()
    {
        return qualPlayer;
    }

    public void ChangeScene(string cena)
    {
        SceneManager.LoadScene(cena);
    }
}
