using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestData
{
    public Quest[] questsAceitas;
    public Quest[] questsFinalizadas;

    public QuestGiverSave[] questGiversSave;

    public QuestData(QuestLog questLog)
    {
        questsAceitas = questLog.getQuestAceitas().ToArray();
        questsFinalizadas = questLog.getQuestFinalizadas().ToArray();

        QuestGiver[] questsGivers = GameObject.FindObjectsOfType<QuestGiver>();
        RelacionaGivers(questsGivers);
    }

    void RelacionaGivers(QuestGiver[] questGiver)
    {
        questGiversSave = new QuestGiverSave[questGiver.Length];

        for(int i = 0; i<questGiver.Length; i++)
        {
            questGiversSave[i] = new QuestGiverSave(questGiver[i].name, questGiver[i].GetQntQuestAceitas(), questGiver[i].GetQuestOnGiver());
            Debug.Log("Salvei: " + questGiver[i].name + " - " + questGiver[i].GetQntQuestAceitas() + " - " + questGiver[i].GetQuestOnGiver().Length);
        }
    }

    [Serializable]
    public struct QuestGiverSave
    {
        public string giverName;
        public int giverQntAceitas;
        public Quest[] questsOnGiver;

        public QuestGiverSave(string giverName, int giverQntAceitas, Quest[] questsOnGiver)
        {
            this.giverName = giverName;
            this.giverQntAceitas = giverQntAceitas;
            this.questsOnGiver = questsOnGiver;
        }
    }
}
