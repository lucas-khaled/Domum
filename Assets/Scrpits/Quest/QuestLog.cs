using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{

    #region SINGLETON
    public static QuestLog questLog;

    private void Awake()
    {
        questLog = this;
    }

    #endregion

    List<Quest> quests;

    public void AdicionarQuest(Quest quest)
    {
        quests.Add(quest);
    }
}
