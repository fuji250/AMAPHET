using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private float x;
    private float z;
    public float moveSpeed = 1.0f;
    private Vector3 latestPos;

    private Rigidbody rb;
    private Animator animator;

    public  float smooth = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();   //アニメーションを取得する
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Playerの位置座標を毎フレーム最後に取得する
        latestPos = transform.position;　　　　　　　　　　　　　//Palyerの位置座標を更新する　

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;　　　　　　　//歩く速度　　
        animator.SetFloat("Walk", rb.velocity.magnitude);        //歩くアニメーションに切り替える

        if (diff.magnitude > 0.01f)
        {
            //transform.rotation = Quaternion.LookRotation(diff);  //キーを押した方向を向くようにする

            //キーを押し方向転換
            Quaternion rotation = Quaternion.LookRotation(diff);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");    //マウスクリックで攻撃モーション
        }
    }
}