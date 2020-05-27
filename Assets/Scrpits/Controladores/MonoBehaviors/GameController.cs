using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public Texture2D cursor;
    private TipoPlayer qualPlayer;

    private void Awake()
    {
        gameController = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
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
    public void FecharJogo()
    {
        Application.Quit();
    }
}
