using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsController
{
    public delegate void OnMorteInimigo(int xp);
    public static OnMorteInimigo onMorteInimigoCallback;

    public delegate void OnPlayerChangeState(EstadoPlayer estadoPlayer);
    public static OnPlayerChangeState onPlayerStateChanged;

    public delegate void OnTyvaMira(bool ligado);
    public static OnTyvaMira onTyvaMira;
}

