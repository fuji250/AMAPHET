using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    //�J�����̐e�I�u�W�F�N�g
    public Transform viewPoint;

    //�J����
    private Camera cam;

    private Rigidbody rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //�J�����i�[
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Invoke(nameof(Attack), 2.0f);
        }
        //�h�����
        StartCoroutine("Defend");
        //Defend();
        /*
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Invoke(nameof(Defend), 2.0f);
        }*/

        animator.SetFloat("Speed",rb.velocity.magnitude);

    }
    void Attack()
    {
        animator.SetTrigger("Attack");
    }

    /*
    void Defend()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("defend", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("defend", false);
        }
    }
    /*/

    IEnumerator Defend()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            yield return new WaitForSeconds(2.0f);
            animator.SetBool("defend", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            yield return new WaitForSeconds(2.0f);
            animator.SetBool("defend", false);
        }
    }
}
