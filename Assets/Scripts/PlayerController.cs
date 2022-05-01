using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //カメラの親オブジェクト
    public Transform viewPoint;

    //カメラ
    private Camera cam;

    private float x;
    private float z;
    public float Speed = 1.0f;
    private Vector3 latestPos;

    //回転速度
    public  float smooth = 150f;
    //入力された値格納
    private Vector3 moveDir;

    //レイを飛ばすオブジェクトの位置
    public Transform groundCheckPoint;

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
        //移動関数を呼ぶ
        PlayerMove();

        //攻撃入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        //防御入力
        Defend();
        
        animator.SetFloat("Speed",rb.velocity.magnitude);

        //アニメーター遷移
        //AnimatorSet();
    }
    public void PlayerMove()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Playerの位置座標を毎フレーム最後に取得する
        latestPos = transform.position;　　　　　　　　　　　　　//Palyerの位置座標を更新する　

        rb.velocity = new Vector3(x, 0, z) * Speed;　　　　　　　//歩く速度　　
        //animator.SetFloat("Walk", rb.velocity.magnitude);        //歩くアニメーションに切り替える

        if (diff.magnitude > 0.01f)
        {
            //キーを押し方向転換
            Quaternion rotation = Quaternion.LookRotation(diff);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);
        }
    }
    void Attack()
    {
        animator.SetTrigger("Attack");
    }

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

    private void AnimatorSet()
    {
        //Walk判定
        if (moveDir != Vector3.zero)
        {
            animator.SetBool("Walk", true);

        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }
}
