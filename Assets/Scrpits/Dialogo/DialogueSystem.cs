﻿using System.Collections;
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
    private GameObject proximoJoystick;
    [SerializeField]
    private GameObject proximoMouse;

    [SerializeField]
    private Text dialogueText;

    private float letterDelay = 0.05f;
    private float letterMultiplier = 0.0001f;

    public string Names;

    private bool letterIsMultiplied = false;
    private bool dialogueActive = false;
    private bool dialogueEnded = false;
    private bool outOfRange = true;

    public AudioClip audioClip;
    AudioSource audioSource;

    public bool IsDialogEnded()
    {
        return dialogueEnded;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueText.text = ""; 
    }
    private void Awake()
    {
        sistemaDialogo = this;
    }
    public void IniciaDialogo(Dialogo dialogo)
    {
        this.dialogo = dialogo;
        this.dialogo.whosDialog = dialogo.whosDialog;
        outOfRange = false;
        if (!dialogueActive)
        {
            caixaTexto.SetActive(true);
            dialogueActive = true;
            StartCoroutine(StartDialogue());
        }
    }
    public void SairDialogo()
    {
        Player.player.estadoPlayer = EstadoPlayer.NORMAL;
        caixaTexto.SetActive(false);
    }
    private IEnumerator StartDialogue()
    {
        Player.player.estadoPlayer = EstadoPlayer.INTERAGINDO;
        CameraController.cameraInstance.Trava = true;
        Player.player.SetPlayerOnIdle();

        if (!outOfRange)
        {

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
        else
        {
            SairDialogo();
        }
    }

    private IEnumerator DisplayString(string stringToDisplay)
    {
        if (outOfRange == false)
        {
            EventsController.onLinhaTerminada.Invoke(dialogo);

            int stringLength = stringToDisplay.Length;
            int currentCharacterIndex = 0;

            dialogueText.text = "";

            while (currentCharacterIndex < stringLength)
            {
                dialogueText.text += stringToDisplay[currentCharacterIndex];
                currentCharacterIndex++;

                if (currentCharacterIndex < stringLength)
                {
                    if (Input.GetButtonDown("Interact") && currentCharacterIndex >= 2)
                    {
                        yield return new WaitForSeconds(letterDelay * letterMultiplier);
                        if (caixaTexto.gameObject.activeInHierarchy)
                        {
                            while (currentCharacterIndex < stringLength)
                            {
                                dialogueText.text += stringToDisplay[currentCharacterIndex];
                                currentCharacterIndex++;
                            }
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(letterDelay);
                    }
                }
                else
                {
                    dialogueEnded = true;
                    break;
                }
            }

            if (GameController.gameController.QualOrigemInput() == OrigemInput.JOYSTICK)
                proximoJoystick.SetActive(true);
            else
                proximoMouse.SetActive(true);

            while (true)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    proximoJoystick.SetActive(false);
                    proximoMouse.SetActive(false);
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
