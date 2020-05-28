using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestLogUI : MonoBehaviour
{
    private GameObject bandeiraAtiva;

    [SerializeField]
    private Text tituloQuest;
    [SerializeField]
    private Text descricaoQuest;
    [SerializeField]
    private Text famaMissaoText;
    [SerializeField]
    private Text dinheiroMissaoText;
    [SerializeField]
    private Text xpMissaoText;

    private List<GameObject> slotQuests =  new List<GameObject>();

    [SerializeField]
    private GameObject slot;

    [SerializeField]
    private Transform contentAceitas;
    [SerializeField]
    private Transform contentFeitas;

    [SerializeField]
    private Text titulo;

    [SerializeField]
    private Transform contentDescricao;

    [SerializeField]
    private GameObject topicosCondicoes;

    public static QuestLogUI questLogUI;

    private Quest questSelecionada;

    void Awake() {
        EventsController.onQuestLogChange += AtualizarQuestLog;
        EventsController.onCondicaoTerminada += TerminaCondicao;
        questLogUI = this;
    }

   /*private void Start()
    {
        if (GameController.gameController.isLoadedGame)
        {
            LoadQuestLogUI();
        }
    }

    void LoadQuestLogUI()
    {
        foreach(Quest qa in QuestLog.questLog.getQuestAceitas())
        {
            AtualizarQuestLog(qa);
        }
        foreach(Quest qf in QuestLog.questLog.getQuestFinalizadas())
        {
            AtualizarQuestLog(qf, true);
        }
    }*/

    void AtualizarQuestLog(Quest quest, bool endQuest = false) {

        if (endQuest)
        {
            GameObject goExists = slotQuests.Find(x => x.GetComponent<Holder_Quest>().referenciaQuest == quest);

            if (goExists != null)
            {
                goExists.transform.SetParent(contentFeitas);
                goExists.transform.GetChild(0).GetComponent<Text>().color = Color.gray;
                goExists.transform.GetChild(1).gameObject.SetActive(false);
                return;
            }
        }

        GameObject obj = slot;
        obj.GetComponent<Holder_Quest>().referenciaQuest = quest;

        GameObject questInstanciada = (GameObject)Instantiate(obj);
        slotQuests.Add(questInstanciada);

        if (endQuest)
        {
            questInstanciada.transform.SetParent(contentFeitas);
            questInstanciada.transform.GetChild(0).GetComponent<Text>().color = Color.gray;
            questInstanciada.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
            questInstanciada.transform.SetParent(contentAceitas);
        
        questInstanciada.transform.localScale = Vector3.one;
    }

    public void AtualizarDescricao(Quest quest) {
        
        // limpar descriçoes quando mudar de quest
        LimparDescricoes();

        xpMissaoText.text = quest.getRecompensaAtual().GetXP().ToString();
        dinheiroMissaoText.text = quest.getRecompensaAtual().GetDinheiro().ToString();
        famaMissaoText.text = quest.getRecompensaAtual().GetFama().ToString();

        List<Condicoes> condicoesSoFar = new List<Condicoes>();

        titulo.text = quest.nome;
        foreach (Condicoes condicoes in quest.condicoes) {
            condicoesSoFar.Add(condicoes);
            Debug.Log(condicoes.descricao);
            if (condicoes == quest.getCondicaoAtual()) {
                break;
            }
        }

        foreach (Condicoes condicoes in condicoesSoFar) {
            GameObject condicao = (GameObject)Instantiate(topicosCondicoes);        
            condicao.transform.SetParent(contentDescricao); 
            condicao.transform.localScale = Vector3.one;
            condicao.GetComponent<Text>().text = condicoes.descricao;

            if (condicoes == condicoesSoFar[condicoesSoFar.Count - 1] && quest.getCondicaoAtual() != null) {
                condicao.GetComponent<Text>().color = Color.white;
            }
        }
    }

    public void LimparDescricoes() {
        
        for (int i = 0; i < contentDescricao.childCount; i++) {
            Destroy(contentDescricao.GetChild(i).gameObject);            
        }
        titulo.text = string.Empty;
    }

    private void TerminaCondicao(Quest quest)
    {
        StartCoroutine(WaitToLoadQuest(quest));
    }

    IEnumerator WaitToLoadQuest(Quest quest)
    {
        yield return new WaitForSeconds(0.1f);
        Condicoes condAtual = quest.getCondicaoAtual();

        if (condAtual != null && quest == questSelecionada)
        {
            tituloQuest.text = quest.nome;
            descricaoQuest.text = quest.getCondicaoAtual().descricao;
        }
        else
        {
            tituloQuest.text = "Selecione uma quest no QuestLog";
            descricaoQuest.text = string.Empty;
        }
    }

    public void AtualizarQuestHUD(Quest quest, GameObject bandeiraAtiva)
    {
        if (this.bandeiraAtiva != null)
        this.bandeiraAtiva.SetActive(false);

        bandeiraAtiva.SetActive(true);
        this.bandeiraAtiva = bandeiraAtiva;

        tituloQuest.text = quest.nome;
        descricaoQuest.text = quest.getCondicaoAtual().descricao;

        questSelecionada = quest;
    }
}
