using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsController
{
    public delegate void OnMorteInimigo(int xp);
    public static OnMorteInimigo onMorteInimigoCallback;
}

