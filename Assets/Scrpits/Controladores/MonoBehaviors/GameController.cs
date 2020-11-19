﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public float volumeGeral;
    public float volumeEfeitos;
    public float volumeMusica;

    public Sprite interagivelTeclado;
    public Sprite interagivelJoystick;

    private OrigemInput origem;

    private TipoPlayer qualPlayer;
    
    private bool isLoadedGame = false;

    public bool IsLoadedGame()
    {
        return isLoadedGame;
    }
    
    private void Awake()
    {
        if (gameController == null)
        {
            gameController = this;
            DontDestroyOnLoad(this);
        }
        else if (GameController.gameController != this)
        {
            Destroy(this);
        }   
    }

    public void EscolherPersonagem(string personagem)
    {
        if (personagem == "Iovelik")
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

    public AsyncOperation LoadPrincipalScene()
    {
        return SceneManager.LoadSceneAsync("Fase_PrincipalBLOQUING");
    }

    public void ChangeScene(string cena, bool clearDelegates = false)
    {
        if(clearDelegates)
            EventsController.ClearAll();

        SceneManager.LoadScene(cena);
    }

    public void SaveGame()
    {
        isLoadedGame = true;
        SaveSystem.Save();
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data != null)
        {
            isLoadedGame = true;
            qualPlayer = (TipoPlayer)data.playerData.qualPlayer;
            ChangeScene("Fase_PrincipalBLOQUING");
        }
    }

    public void FecharJogo()
    {
        Application.Quit();
    }

    public OrigemInput QualOrigemInput()
    {
        OrigemInput origemAtual = OrigemInput.MOUSE;

        string[] temp = Input.GetJoystickNames();
        if (temp.Length > 0 && !string.IsNullOrEmpty(temp[0]))
            origemAtual = OrigemInput.JOYSTICK;

        return origemAtual;
    }

}
