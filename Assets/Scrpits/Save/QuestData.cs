using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public Quest[] questsAceitas;
    public Quest[] questsFinalizadas;
    public QuestGiver[] questsGivers;

    public QuestData(QuestLog questLog)
    {
        questsAceitas = questLog.getQuestAceitas().ToArray();
        questsFinalizadas = questLog.getQuestFinalizadas().ToArray();

        questsGivers = GameObject.FindObjectsOfType<QuestGiver>();
    }
}
