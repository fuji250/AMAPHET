using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    //カメラの親オブジェクト
    public Transform viewPoint;

    //カメラ
    private Camera cam;

    private Rigidbody rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //カメラ格納
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //攻撃入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Invoke(nameof(Attack), 2.0f);
        }
        //防御入力
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
