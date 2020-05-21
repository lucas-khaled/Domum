using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArvoreDeHabilidades : MonoBehaviour
{
    public Skill[] skillsIovelik;
    public Skill[] skillsTyva;

    public void AtivaCondicaoIovelik(int index)
    {
        skillsIovelik[index].AtivaSkill();
    }

    public void AtivaCondicaoTyva(int index)
    {
        skillsTyva[index].AtivaSkill();
    }
}
