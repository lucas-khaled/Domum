using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;

    [SerializeField]
    private UnityEngine.UI.Slider volumeGeral;
    [SerializeField]
    private UnityEngine.UI.Slider volumeEfeitos;
    [SerializeField]
    private UnityEngine.UI.Slider volumeMusica;
    [SerializeField]
    private UnityEngine.Audio.AudioMixer audioMixer;

    private bool rodou;

    public void SetVolumeAudio()
    {
        GameController.gameController.volumeGeral = volumeGeral.value;
        audioMixer.SetFloat("volumeMaster", volumeGeral.value);
        GameController.gameController.volumeEfeitos = volumeEfeitos.value;
        audioMixer.SetFloat("volumeEfeitos", volumeEfeitos.value);
        GameController.gameController.volumeMusica = volumeMusica.value;
        audioMixer.SetFloat("volumeMusica", volumeMusica.value);
    }

    public void GetVolumeAudio()
    {
        float valor;
        audioMixer.GetFloat("volumeMaster",out valor);
        volumeGeral.value = valor;
        audioMixer.GetFloat("volumeEfeitos", out valor);
        volumeEfeitos.value = valor;
        audioMixer.GetFloat("volumeMusica", out valor);
        volumeMusica.value = valor;

    }

    private void Start()
    {
        GetVolumeAudio();

        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);

        if(GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK)
        {
            Cursor.visible = false;
            CameraController.cameraInstance.Trava = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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

    public void selecionaBotaoEscolha(GameObject selecionado)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selecionado);
    }

    public void MudaBotaoSelecionado(GameObject selecionado)
    {
        if (GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selecionado);
        }

    }
}
