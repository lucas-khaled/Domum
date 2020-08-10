using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsController
{
    public delegate void OnMorteInimigo(int xp);
    public static OnMorteInimigo onMorteInimigoCallback;

    public delegate void OnPlayerChangeState(EstadoPlayer estadoPlayer);
    public static OnPlayerChangeState onPlayerStateChanged;

    public delegate void OnInteracao(Interagivel interagivel);
    public static OnInteracao onInteracao;

    public delegate void OnItemPego(Item item);
    public static OnItemPego onItemPego;

    public delegate void OnInventarioChange(Item item, bool mudanca);
    public static OnInventarioChange onInventarioChange; 

    public delegate void OnQuestLogChange(Quest quest, bool endQuest = false);
    public static OnQuestLogChange onQuestLogChange;

    public delegate void OnCondicaoTerminada(Quest quest);
    public static OnCondicaoTerminada onCondicaoTerminada;

    public delegate void OnDialogoTerminado(Dialogo dialogo);
    public static OnDialogoTerminado onDialogoTerminado;

    public delegate void OnLinhaTerminada(Dialogo dialogo);
    public static OnLinhaTerminada onLinhaTerminada;

    public delegate void OnPausedGame();
    public static OnPausedGame onPausedGame;

    public static void ClearAll()
    {
        if (onPlayerStateChanged != null)
        {
            foreach (Delegate d in onMorteInimigoCallback.GetInvocationList())
            {
                onMorteInimigoCallback -= (OnMorteInimigo)d;
            }
        }

        if (onPlayerStateChanged != null)
        {
            foreach (Delegate d in onPlayerStateChanged.GetInvocationList())
            {
                onPlayerStateChanged -= (OnPlayerChangeState)d;
            }
        }

        if (onInteracao != null)
        {
            foreach (Delegate d in onInteracao.GetInvocationList())
            {
                onInteracao -= (OnInteracao)d;
            }
        }

        if (onItemPego != null)
        {
            foreach (Delegate d in onItemPego.GetInvocationList())
            {
                onItemPego -= (OnItemPego)d;
            }
        }

        if (onInventarioChange != null)
        {
            foreach (Delegate d in onInventarioChange.GetInvocationList())
            {
                onInventarioChange -= (OnInventarioChange)d;
            }
        }

        if (onQuestLogChange != null)
        {
            foreach (Delegate d in onQuestLogChange.GetInvocationList())
            {
                onQuestLogChange -= (OnQuestLogChange)d;
            }
        }

        if (onCondicaoTerminada != null)
        {
            foreach (Delegate d in onCondicaoTerminada.GetInvocationList())
            {
                onCondicaoTerminada -= (OnCondicaoTerminada)d;
            }
        }

        if (onDialogoTerminado != null)
        {
            foreach (Delegate d in onDialogoTerminado.GetInvocationList())
            {
                onDialogoTerminado -= (OnDialogoTerminado)d;
            }
        }

        if (onLinhaTerminada != null)
        {
            foreach (Delegate d in onLinhaTerminada.GetInvocationList())
            {
                onLinhaTerminada -= (OnLinhaTerminada)d;
            }
        }

        if (onPausedGame != null)
        {
            foreach (Delegate d in onPausedGame.GetInvocationList())
            {
                onPausedGame -= (OnPausedGame)d;
            }
        }
    }
}

