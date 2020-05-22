using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArvoreDeHabilidades : MonoBehaviour
{
    [SerializeField]
    private Skill[] skillsIovelik;
    [SerializeField]
    private Skill[] skillsTyva;
    [SerializeField]
    private static Text textoInfo;
    [SerializeField]
    private Text textoPerk;
    [SerializeField]
    private Button ativarHabilidade;

    [SerializeField]
    private Button[] botoesTyva;
    [SerializeField]
    private Button[] botoesIovelik;

    [SerializeField]
    private GameObject painelTyva, painelIovelik;

    private static int qntPerk = 0;

    int indiceAtual, qntAtivas;

    private void Start()
    {
        SetHabilidadesAtivas();
        AtivaArvorePlayerEscolhido();
    }

    public static void IncrementaPerk()
    {
        qntPerk++;
        textoInfo.text = qntPerk.ToString();
    }

    private static void DecrementaPerk()
    {
        qntPerk--;
        textoInfo.text = qntPerk.ToString();
    }

    void AtivaArvorePlayerEscolhido()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            painelIovelik.SetActive(true);
        }

        else
        {
            painelTyva.SetActive(true);
        }
    }

    private void SetHabilidadesAtivas()
    {
        Skill[] skillsAtivar;
        Button[] buttonAtivar;
        if(GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            skillsAtivar = skillsIovelik;
            buttonAtivar = botoesIovelik;
        }
        else
        {
            skillsAtivar = skillsTyva;
            buttonAtivar = botoesTyva;
        }

        for(int i = 0; i<skillsAtivar.Length; i++)
        {
            if(i!=skillsAtivar.Length-1 && i%2 == 0)
            {
                skillsAtivar[i].podeAtivar = true;
                buttonAtivar[i].enabled = true;
                continue;
            }

            bool valor;
            if(i != skillsAtivar.Length-1 && i%2 == 1)
            {
                valor = skillsAtivar[i - 1].IsActive();
            }
            else
            {
                valor = (qntAtivas >= 6) ? true : false;
            }

            skillsAtivar[i].podeAtivar = valor;
        }

    }

    public void AtivaCondicaoIovelik()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            skillsIovelik[indiceAtual].AtivaSkill();
            qntAtivas++;
            SetHabilidadesAtivas();
            DecrementaPerk();
        }
    }

    public void AtivaCondicaoTyva()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            skillsTyva[indiceAtual].AtivaSkill();
            qntAtivas++;
            SetHabilidadesAtivas();
            DecrementaPerk();
        }
    }

    public void ShowInfoIovelikHabilidade(int index)
    {
        indiceAtual = index;
        textoInfo.text = skillsIovelik[index].descricaoHabilidade;
        if (skillsIovelik[index].podeAtivar)
        {
            ativarHabilidade.gameObject.SetActive(true);
        }
        else
        {
            ativarHabilidade.gameObject.SetActive(false);
        }
    }

    public void ShowInfoTyvaHabilidade(int index)
    {
        indiceAtual = index;
        textoInfo.text = skillsTyva[index].descricaoHabilidade;
        if (skillsIovelik[index].podeAtivar)
        {
            ativarHabilidade.gameObject.SetActive(true);
        }
        else
        {
            ativarHabilidade.gameObject.SetActive(false);
        }
    }
}
