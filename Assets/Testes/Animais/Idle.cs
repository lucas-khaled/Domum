using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : StateMachineBehaviour
{
    private double aux = 0;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aux <= 0)
        {
            int random = Random.Range(1, 4);
            if (random == 1)
            {
                animator.ResetTrigger("Deitar");
                animator.SetBool("Caminhando", true);
                animator.SetBool("Deitado", false);
                //comer ou andar
            }
            if (random == 2)
            {
                random = Random.Range(1, 3);
                if (random == 1)
                {
                    animator.SetBool("Caminhando",false);
                    animator.SetTrigger("Deitar");
                    //ficar parado ou deitar
                }
            }
            aux = 10;
        }
        else
        {
            aux = aux - Time.deltaTime;
        }
    }
}
