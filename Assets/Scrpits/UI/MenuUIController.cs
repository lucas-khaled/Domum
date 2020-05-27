using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;

    private void Start()
    {
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
    }

    public void CarregaJogo()
    {
        GameController.gameController.LoadGame();
    }

    public void MudaCena(string cena)
    {
        GameController.gameController.ChangeScene(cena);
    }

    public void SairJogo()
    {
        GameController.gameController.FecharJogo();
    }
}
