using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogo
{
    public string[] dialogueLines;
    public string whosDialog { get; set; }

    public Dialogo DefinePlayerDialog()
    {
        Dialogo retorno = new Dialogo();
        retorno.whosDialog = this.whosDialog;
        List<string> returnDialog = new List<string>();

        for(int i = 0; i<dialogueLines.Length; i++)
        {
            int walked = 1;
            int walkedBefore = 0;
            bool firstWalk = true;
            string correctLine = string.Empty;
            for(int k = 0; k<dialogueLines[i].Length; k++)
            {
                
                char carac = dialogueLines[i][k];
                if(carac == '|')
                {

                    int j = k-1;
                    int sentidoIteracao = (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA) ? 1 : -1;

                    while(true)
                    {
                        
                        if (dialogueLines[i][j] == '#')
                            break;
                        j += sentidoIteracao;
                        walked++;
                    }

                    if (sentidoIteracao > 0)
                        k = j;
                    else
                    {
                        
                        int realJ = j;
                        if (firstWalk)
                            firstWalk = false;
                        else
                            realJ = j - walkedBefore;

                        walkedBefore += walked+2;
                        walked = 1;
                        correctLine = correctLine.Remove(realJ);
                    }
                }
                else
                {
                    if (carac != '#')
                        correctLine += carac;
                }
            }

            returnDialog.Add(correctLine);
        }

        retorno.dialogueLines = returnDialog.ToArray();
        return retorno;
    }
}
