using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class DialogueSystem:MonoBehaviour
{

    public static DialogueSystem sistemaDialogo;

    private Dialogo dialogo;

    [SerializeField]
    private GameObject caixaTexto;
    [SerializeField]
    private GameObject setaProximo;

    [SerializeField]
    private Text dialogueText;

    private float letterDelay = 0.1f;
    private float letterMultiplier = 0.5f;

    private KeyCode DialogueInput = KeyCode.E;

    public string Names;

    private bool letterIsMultiplied = false;
    private bool dialogueActive = false;
    private bool dialogueEnded = false;
    private bool outOfRange = true;

    public AudioClip audioClip;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueText.text = ""; 
    }
    private void Awake()
    {
        sistemaDialogo = this;
    }
    public void NPCName(Dialogo dialogo)
    {
        this.dialogo = dialogo;
        outOfRange = false;
        if (!dialogueActive)
        {
            caixaTexto.SetActive(true);
            dialogueActive = true;
            StartCoroutine(StartDialogue());
        }
    }
    private IEnumerator StartDialogue()
    {
        Player.player.estadoPlayer = EstadoPlayer.INTERAGINDO;
        CameraController.cameraInstance.Trava = true;
        Player.player.SetPlayerOnIdle();

        int dialogueLength = dialogo.dialogueLines.Length;
        int currentDialogueIndex = 0;

        while (currentDialogueIndex < dialogueLength || !letterIsMultiplied)
        {
            if (!letterIsMultiplied)
            {
                letterIsMultiplied = true;
                caixaTexto.SetActive(true);
                StartCoroutine(DisplayString(dialogo.dialogueLines[currentDialogueIndex++]));

                if (currentDialogueIndex >= dialogueLength)
                {
                    dialogueEnded = true;
                }
            }
                
            yield return 0;
        }

        while (true)
        {
            if (Input.GetButton("Interact") && dialogueEnded == false)
            {
                break;
            }
            yield return 0;
        }
        dialogueEnded = false;
        dialogueActive = false;

        CameraController.cameraInstance.Trava = false;
        Player.player.estadoPlayer = EstadoPlayer.NORMAL;

        EventsController.onDialogoTerminado.Invoke(dialogo);

    }

    private IEnumerator DisplayString(string stringToDisplay)
    {
        if (outOfRange == false)
        {
            int stringLength = stringToDisplay.Length;
            int currentCharacterIndex = 0;

            dialogueText.text = "";

            while (currentCharacterIndex < stringLength)
            {
                dialogueText.text += stringToDisplay[currentCharacterIndex];
                currentCharacterIndex++;

                if (currentCharacterIndex < stringLength)
                {
                    if (Input.GetKey(DialogueInput))
                    {
                        yield return new WaitForSeconds(letterDelay * letterMultiplier);

                        if (audioClip) audioSource.PlayOneShot(audioClip, 0.5F);
                    }
                    else
                    {
                        yield return new WaitForSeconds(letterDelay);

                        if (audioClip) audioSource.PlayOneShot(audioClip, 0.5F);
                    }
                }
                else
                {
                    dialogueEnded = false;
                    break;
                }
            }

            setaProximo.SetActive(true);

                while (true)
                {
                    if (Input.GetKeyDown(DialogueInput))
                    {
                        setaProximo.SetActive(false);
                        caixaTexto.SetActive(false);
                        break;
                    }
                    yield return 0;
            }
            dialogueEnded = false;
            letterIsMultiplied = false;
            dialogueText.text = "";
        }
    }
}
