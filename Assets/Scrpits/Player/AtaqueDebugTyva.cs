using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueDebugTyva : StateMachineBehaviour
{
    private float auxSom;
    private GameObject[] adagas;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        //Mudar quando colocar som
        /*if (stateInfo.IsName("Attack1"))
        {
            auxSom = 0.5f;
            Debug.Log("1");

        }
        else if (stateInfo.IsName("Attack2"))
        {
            auxSom = 0.4f;
            Debug.Log("2");
        }
        else if (stateInfo.IsName("Attack3"))
        {
            auxSom = 0.7f;
            Debug.Log("3");
        }*/


        //aa
        adagas = GameObject.FindGameObjectsWithTag("Adaga");
        adagas[0].GetComponent<BoxCollider>().enabled = true;
        adagas[1].GetComponent<BoxCollider>().enabled = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (auxSom > -100)
            auxSom -= Time.deltaTime;

        if (auxSom <= 0 && auxSom != -999)
        {
            auxSom = -999;
            animator.gameObject.GetComponent<Player>().audioSource.PlayOneShot(Player.player.ataque);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
