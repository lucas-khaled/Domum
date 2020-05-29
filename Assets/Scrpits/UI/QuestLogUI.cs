﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class QuestLogUI : MonoBehaviour
{
    private GameObject bandeiraAtiva;

    [Header("Texts")]
    [SerializeField]
    private Text tituloQuest;
    [SerializeField]
    private Text descricaoQuestAuxiliar;
    [SerializeField]
    private Text descricaoQuest;
    [SerializeField]
    private Text famaMissaoText;
    [SerializeField]
    private Text dinheiroMissaoText;
    [SerializeField]
    private Text xpMissaoText;

    private List<GameObject> slotQuests =  new List<GameObject>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject slot;

    [Header("Contents")]
    [SerializeField]
    private Transform contentAceitas;
    [SerializeField]
    private Transform contentFeitas;

    [Header("HUD")]
    [SerializeField]
    private Text titulo;
    [SerializeField]
    private Transform contentDescricao;
    [SerializeField]
    private GameObject topicosCondicoes;

    [Header("Icons")]
    [SerializeField]
    private Sprite iconeMissao;
    [SerializeField]
    private Sprite iconeInimigoMissao;

    public static QuestLogUI questLogUI;
    private Quest questSelecionada;
    private SpriteRenderer iconeSpriteRenderer;

    void Awake() {
        EventsController.onQuestLogChange += AtualizarQuestLog;
        EventsController.onCondicaoTerminada += TerminaCondicao;
        questLogUI = this;
    }

   private void Start()
   {
        GameObject go = GameObject.Find("IconeMissão");
        iconeSpriteRenderer = go.GetComponent<SpriteRenderer>();
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.Euler(90, 180, 0);
        go.transform.localScale = Vector3.one * 3f;
   }

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

        if (condAtual != null)
        {
            if (quest == questSelecionada) {
                StartCoroutine(AtualizarQuestHUD(quest, bandeiraAtiva));
                AtualizaIconesMissao();
            }
        }
        else
        {
            tituloQuest.transform.parent.gameObject.SetActive(false);
            iconeSpriteRenderer.gameObject.SetActive(false);
        }
    }

    void AtualizaIconesMissao()
    {
        if (questSelecionada.getCondicaoAtual().tipoCondicao == Condicoes.TipoCondicao.COMBATE)
        {
            iconeSpriteRenderer.sprite = iconeInimigoMissao;
            iconeSpriteRenderer.gameObject.GetComponent<LineRenderer>().enabled = false;
        }
        else if (questSelecionada.getCondicaoAtual().tipoCondicao == Condicoes.TipoCondicao.IDA)
        {
            iconeSpriteRenderer.sprite = null;
            DrawIdaCircle();
        }
        else
        {
            iconeSpriteRenderer.sprite = iconeMissao;
            iconeSpriteRenderer.gameObject.GetComponent<LineRenderer>().enabled = false;
        }

        Vector3 pos = questSelecionada.getCondicaoAtual().local;
        pos.y = 25;
        iconeSpriteRenderer.gameObject.transform.position = pos;
    }

    void DrawIdaCircle()
    {
        LineRenderer line = iconeSpriteRenderer.gameObject.GetComponent<LineRenderer>();
        line.enabled = true;
        float radius = questSelecionada.getCondicaoAtual().distanciaChegada / iconeSpriteRenderer.gameObject.transform.localScale.x;

        float segments = 30;
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < segments; i++)
        {
            x = (Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }

    }
    public IEnumerator AtualizarQuestHUD(Quest quest, GameObject bandeiraAtiva)
    {
        /*if (this.bandeiraAtiva != null)
            this.bandeiraAtiva.SetActive(false);

        bandeiraAtiva.SetActive(true);
        this.bandeiraAtiva = bandeiraAtiva;

        tituloQuest.transform.parent.gameObject.SetActive(true);

        tituloQuest.text = quest.nome;
        descricaoQuest.text = quest.getCondicaoAtual().descricao;

        questSelecionada = quest;
        AtualizaIconesMissao();*/

        if (this.bandeiraAtiva != null)
            this.bandeiraAtiva.SetActive(false);

        bandeiraAtiva.SetActive(true);
        this.bandeiraAtiva = bandeiraAtiva;

        tituloQuest.transform.parent.gameObject.SetActive(true);

        if (questSelecionada == null)
        {
            tituloQuest.text = quest.nome;
            descricaoQuest.text = quest.getCondicaoAtual().descricao;
            questSelecionada = quest;
        }
        else
        {
            descricaoQuestAuxiliar.text = descricaoQuest.text;
            var descricaoAuxiliar = Instantiate(descricaoQuestAuxiliar, descricaoQuest.transform.position, Quaternion.identity);
            descricaoAuxiliar.transform.parent = descricaoQuest.transform;
            descricaoAuxiliar.transform.localScale = Vector3.one;
            descricaoQuest.GetComponent<Text>().enabled = false;

            yield return new WaitForSeconds(0.8f);
            Destroy(descricaoAuxiliar);

            descricaoQuest.text = quest.getCondicaoAtual().descricao;
            descricaoQuest.canvasRenderer.SetAlpha(0.0f);
            descricaoQuest.GetComponent<Text>().enabled = true;
            descricaoQuest.CrossFadeAlpha(1, 1.5f, false);

            AtualizaIconesMissao();
        }
    }
}
