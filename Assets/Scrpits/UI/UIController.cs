﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialJoystick;
    [SerializeField]
    private GameObject tutorialTeclado;
    [SerializeField]
    private GameObject novoSelecionado;
    [SerializeField]
    private GameObject canvasAudio;

    [SerializeField]
    private GameObject pauseInicio;
    [SerializeField]
    private GameObject habilidadeIovelik;

    public Image questAceitaTerminada;
    public GameObject questLogHUD;
    public GameObject posicao;

    [SerializeField]
    private Image playerLifeBar;
    [SerializeField]
    private Image XpBar;
    [SerializeField]
    private Image Skill;

    private GameObject tituloAtual;
    private GameObject textoAtual;

    [SerializeField]
    private Transform ContentGlosa;

    [SerializeField]
    private Scrollbar scrollGlosa;

    [SerializeField]
    private GameObject Usavel;

    [SerializeField]
    private GameObject RenderTyva;
    [SerializeField]
    private GameObject RenderIovelik;

    //public Transform Inicio;

    private GameObject[] Bases;

    [SerializeField]
    private GameObject Pause, painelMorte, pauseMenu;

    [SerializeField]
    private GameObject painelUsáveis;
    [SerializeField]
    private GameObject painelQuestLog;

    [SerializeField]
    private GameObject[] objetos;
    private float auxPause;

    bool questLogAberto = false;
    private List<Transform> listinha = new List<Transform>();

    public bool isPaused { get; private set; }

    #region SINGLETON

    public static UIController uiController;

    private void Awake()
    {
        uiController = this;
    }

    #endregion
    private void Start()
    {
        InicializarPainelUsasveis();
        AtualizarPainelUsaveis();
        AtualizaRender();
        PauseOn(true);
        PauseOff();
    }

    protected virtual void Update() {        
        PauseOn();
        Voltar();

        if (canvasAudio.activeInHierarchy && Input.GetButtonDown("Return"))
       {
           VoltarSelecao(novoSelecionado, canvasAudio);
       }
    }

    private void Voltar()
    {
        auxPause -= 0.1f;
        if (auxPause < -9999)
            auxPause = 0;

        if (Input.GetButtonDown("Pause") && auxPause <= 0)
        {
            bool desativado = false;
            for (int i = 0; i < objetos.Length; i++)
            {
                if (objetos[i].activeInHierarchy)
                {
                    pauseMenu.SetActive(true);
                    objetos[i].SetActive(false);

                    desativado = true;
                }
            }

            if (!desativado)
            {

                if (LojaUI.lojaUi.ativo)
                {
                    LojaUI.lojaUi.FecharLoja();
                }
                else
                if (BauUI.bauUI.bauAberto)
                {
                    BauUI.bauUI.CloseBau();
                }
                else
                if (isPaused && !desativado)
                {
                    PauseOff();
                    desativado = false;
                }
            }

        }
    }

    #region WORLD CANVAS

    public void InitCBT(string text, GameObject CBTprefab, Transform hitCanvas)
    {
        GameObject temp = Instantiate(CBTprefab) as GameObject;

        RectTransform tempRect = temp.GetComponent<RectTransform>();
        temp.transform.SetParent(hitCanvas);
        tempRect.transform.localPosition = CBTprefab.transform.localPosition;
        tempRect.transform.localScale = CBTprefab.transform.localScale;
        tempRect.transform.localRotation = CBTprefab.transform.localRotation;
        temp.GetComponent<Animator>().SetTrigger("Hit");
        temp.GetComponent<Text>().text = text;

        Destroy(temp.gameObject, 2);
    }

    public void LifeBar(Image lifebar, float value)
    {
        lifebar.fillAmount = value;
    }
    #endregion

    public void SaveGame()
    {
        SaveSystem.Save();
    }

    public void LifeBar(float value)
    {
        playerLifeBar.fillAmount = value;
    }

    private void VoltarSelecao(GameObject novoSelecionado, GameObject atual)
    {
        if (Input.GetButtonDown("Return"))
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(novoSelecionado);

            atual.SetActive(false);
        }
    }

    public void AtualizarTextoGlossario(GameObject titulo, GameObject texto)
    {
        if(tituloAtual != null)
        {
            tituloAtual.SetActive(false);
        }
        if(textoAtual != null)
        {
            textoAtual.SetActive(false);
            textoAtual.transform.SetParent(ContentGlosa.parent.parent.parent);
            
        }
        
        tituloAtual = titulo;
        textoAtual = texto;
        tituloAtual.SetActive(true);
        textoAtual.SetActive(true);
        textoAtual.transform.SetParent(ContentGlosa);
        scrollGlosa.value = 1;
    }
    public void XPbar(float value)
    {
        XpBar.fillAmount = value;
    }
    public void SkillCD(float value)
    {
        Skill.fillAmount = value;

    }


    public void InicializarPainelUsasveis(){

        for(int i = 0; i < painelUsáveis.transform.childCount; i++){
            listinha.Add(painelUsáveis.transform.GetChild(i));
        }
        for(int i = 0; i < Player.player.status.maxColetavel; i++){
            listinha[i].gameObject.SetActive(true);
        }
    }

    public void AtualizarPainelUsaveis(){

        int numFilho = 1;

        if(GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            numFilho = 2;
        }

        for(int i = 0; i < Player.player.status.QntColetavel; i++){
            listinha[i].GetChild(numFilho).gameObject.SetActive(true);
        }
        for(int i = Player.player.status.maxColetavel-1; i > Player.player.status.QntColetavel-1; i--){
            listinha[i].GetChild(numFilho).gameObject.SetActive(false);
        }
    }

    void AtualizaRender()
    {
        if(GameController.gameController.GetPersonagemEscolhido() ==  TipoPlayer.TYVA)
        {
            RenderTyva.SetActive(true);
        }
        else if(GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            RenderIovelik.SetActive(true);
        }
    }


    public void AbrirQuestLog() {
        painelQuestLog.SetActive(true);
    }

    public void FecharQuestLog() {
        painelQuestLog.SetActive(false);
    }

    public void PauseOn(bool mandeiPausar = false){

        if (mandeiPausar || (Input.GetButtonDown("Pause") && Player.player.estadoPlayer != EstadoPlayer.MORTO))
        {
            if (!LojaUI.lojaUi.ativo)
            {
                if (!isPaused)
                {
                    isPaused = true;
                    auxPause = 0.5f;
                }

                if (GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK)
                {
                    tutorialJoystick.SetActive(true);
                    tutorialTeclado.SetActive(false);
                }
                else
                {
                    tutorialTeclado.SetActive(true);
                    tutorialJoystick.SetActive(false);
                }

                if (EventsController.onPausedGame != null)
                {
                    EventsController.onPausedGame.Invoke();
                }

                MudaBotaoSelecionado(pauseInicio);

                if (!mandeiPausar)
                    Pause.SetActive(true);

                CameraController.cameraInstance.Trava = true;

                if (GameController.gameController.QualOrigemInput() == OrigemInput.MOUSE)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                Time.timeScale = 0;
            }
        }
    }

    public void PauseOff()
    {
        if (isPaused)
        {
            isPaused = false;
            Pause.SetActive(false);
            Cursor.visible = false;
            CameraController.cameraInstance.Trava = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void PainelMorteOn()
    {
        painelMorte.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MudarCena(string cena)
    {
        GameController.gameController.ChangeScene(cena, true);
    }

    public void SelecionaHabilidades(GameObject habilidade)
    {
        if (GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK && GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(habilidade);
        }
        else if (GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(habilidadeIovelik);
        }
    }

    public void MudaBotaoSelecionado(GameObject selecionado)
    {
        if (GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selecionado);
        }
            
    }
}
