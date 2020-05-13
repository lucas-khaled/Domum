using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Holder_Quest : Button, ISelectHandler
{
    public Quest referenciaQuest;
    public Text referenciaTitulo;

    private List<Condicoes> condicoesQuest = new List<Condicoes>();
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        referenciaTitulo.text = referenciaQuest.nome;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        QuestLogUI.questLogUI.AtualizarDescricao(referenciaQuest);      
    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);
        QuestLogUI.questLogUI.LimparDescricoes();
    }

}
