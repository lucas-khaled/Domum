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

    private List<GameObject> slotQuests;

    [SerializeField]
    private GameObject slot;

    [SerializeField]
    private Transform content;

    [SerializeField]
    private Text titulo;

    [SerializeField]
    private Transform contentDescricao;

    [SerializeField]
    private GameObject topicosCondicoes;

    public static QuestLogUI questLogUI;

    void Awake() {
        EventsController.onQuestLogChange += AtualizarQuestLog;
        //EventsController.onQuestConditionChange += AtualizarDescricao;
        questLogUI = this;
    }
    
    void AtualizarQuestLog(Quest quest) {

        GameObject obj = slot;
        obj.GetComponent<Holder_Quest>().referenciaQuest = quest;

        GameObject questInstanciada = (GameObject)Instantiate(obj);
        slotQuests.Add(questInstanciada);
        
        questInstanciada.transform.SetParent(content);        
        questInstanciada.transform.localScale = Vector3.one;
    }

    public void AtualizarDescricao(Quest quest) {
        
        // limpar descriçoes quando mudar de quest
        LimparDescricoes();

        List<Condicoes> condicoesSoFar = new List<Condicoes>();

        titulo.text = quest.nome;
        foreach (Condicoes condicoes in quest.condicoes) {
            condicoesSoFar.Add(condicoes);
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

    public void AtualizarQuestHUD(Quest quest, GameObject bandeiraAtiva)
    {
        if (this.bandeiraAtiva != null)
        this.bandeiraAtiva.SetActive(false);

        bandeiraAtiva.SetActive(true);
        this.bandeiraAtiva = bandeiraAtiva;

        tituloQuest.text = quest.nome;
        descricaoQuest.text = quest.getCondicaoAtual().descricao;
    }
}
