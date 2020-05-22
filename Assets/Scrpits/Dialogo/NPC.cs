using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class NPC : Interagivel {

    private DialogueSystem npc;

    public string Name;

    public Quest[] quests;
    private Quest proximaQuest;

    [TextArea(5, 10)]
    public string[] dialogosFamaBaixa;
    [TextArea(5, 10)]
    public string[] dialogosFamaMedia;
    [TextArea(5, 10)]
    public string[] dialogosFamaAlta;

    void Start () {
        npc = FindObjectOfType<DialogueSystem>();
    }

    private void getProximaQuest()
    {
        proximaQuest = null;
        for (int i = 0; i < quests.Length; i++)
        {
            if (!quests[i].IsRealizada())
            {
                proximaQuest = quests[i];
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponent<NPC>().enabled = true;
        getProximaQuest();

    }
    public void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Player") && Input.GetKeyDown(KeyCode.E))
        {
            this.gameObject.GetComponent<NPC>().enabled = true;
            npc.Names = Name;
        }
    }

    public void OnTriggerExit()
    {
        npc.OutOfRange();
        this.gameObject.GetComponent<NPC>().enabled = false;
        npc.SairDialogo();
    }
}

