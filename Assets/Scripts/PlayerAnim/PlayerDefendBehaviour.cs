using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefendBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<PlayerManager>() != null)
        {
            animator.GetComponent<PlayerManager>().moveSpeed = 0;
            animator.GetComponent<PlayerManager>().barrierChecker.SetActive(true);
            animator.GetComponent<PlayerManager>().barrier.SetActive(true);
        }
        
        if (animator.GetComponent<GhostManager>() != null)
        {
            animator.GetComponent<GhostManager>().barrierChecker.SetActive(true);
            animator.GetComponent<GhostManager>().barrier.SetActive(true);


        }
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<PlayerManager>() != null)
        {
            animator.GetComponent<PlayerManager>().moveSpeed = 3.0f;
            animator.GetComponent<PlayerManager>().barrierChecker.SetActive(false);
            animator.GetComponent<PlayerManager>().barrier.SetActive(false);


        }
        if (animator.GetComponent<GhostManager>() != null)
        {
            animator.GetComponent<GhostManager>().barrierChecker.SetActive(false);
            animator.GetComponent<GhostManager>().barrier.SetActive(false);

        }
    }

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
