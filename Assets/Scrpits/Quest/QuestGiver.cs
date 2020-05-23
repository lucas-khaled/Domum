using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : Interagivel
{
    [SerializeField]
    Quest[] quests;

    int questsAceitas = 0;

    public override void Interact()
    {
        base.Interact();

        if (questsAceitas != 0)
        {
            if (questsAceitas > quests.Length - 1 && !quests[questsAceitas - 1].IsRealizada())
            {
                return;
            }
        }

        DarQuest();
        //DialogueSystem.sistemaDialogo.NPCName(quests[questsAceitas].dialogo);
    }

    public void DarQuest()
    {
        QuestLog.questLog.AdicionarQuest(quests[questsAceitas]);
        questsAceitas++;
    }
}
