using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image playerLifeBar;
    public Image XpBar;
    public Image Skill;
    public GameObject Usavel;
    //public Transform Inicio;
    private GameObject[] Bases;
    public GameObject Pause;
    public InventarioUI Inventario;

    public GameObject Painel;
    public GameObject QuestLog;
    public bool questLogAberto = false;
    private List<Transform> listinha = new List<Transform>();


    #region SINGLETON

    public static UIController uiController;

    private void Awake()
    {
        uiController = this;
    }

    #endregion
    private void Start()
    {
        UseTest();
        Usandinho();
        
    }

    protected virtual void Update() {        
       PauseOn();

        /*if (Input.GetKey("J")) { //trocar para GetButtonDown("QuestLog") e setar o botão
            // Pausar jogo
            AbrirQuestLog();
            questLogAberto = true;            
        }

        if (Input.GetKey("J") && questLogAberto == true) { //trocar para GetButtonDown("QuestLog") e setar o botão
                FecharQuestLog();
                questLogAberto = false;
                // Retomar jogo
            }*/
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

    public void LifeBar(float value)
    {
        playerLifeBar.fillAmount = value;
    }

    public void XPbar(float value)
    {
        XpBar.fillAmount = value;
    }
    public void SkillCD(float value)
    {
        Skill.fillAmount = value;

    }


    public void UseTest(){

        for(int i = 0; i < Painel.transform.childCount; i++){
            listinha.Add(Painel.transform.GetChild(i));
        }
        for(int i = 0; i < Player.player.status.maxColetavel; i++){
            listinha[i].gameObject.SetActive(true);
        }
    }
    public void Usandinho(){
        for(int i = 0; i < Player.player.status.QntColetavel; i++){
            listinha[i].GetChild(1).gameObject.SetActive(true);
        }
        for(int i = Player.player.status.maxColetavel-1; i > Player.player.status.QntColetavel-1; i--){
            listinha[i].GetChild(1).gameObject.SetActive(false);
        }
    }


    public void AbrirQuestLog() {
        QuestLog.SetActive(true);
    }

    public void FecharQuestLog() {
        QuestLog.SetActive(false);
    }

    public void PauseOn(){

        if(Input.GetKey(KeyCode.Escape)){

        Pause.SetActive(true);
        CameraController.cameraInstance.Trava = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        }

    }
    public void PauseOff(){

        Pause.SetActive(false);
        Cursor.visible = false;
        CameraController.cameraInstance.Trava = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

}
