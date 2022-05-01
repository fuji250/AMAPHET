using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //カメラの親オブジェクト
    public Transform viewPoint;

    //カメラ
    private Camera cam;

    //歩き速度
    public float walkSpeed = 4f;
    //走りスピード
    public float runSpeed = 8f;
    //入力された値格納
    private Vector3 moveDir;
    //進方向格納
    private Vector3 movement;
    //実際の移動速度
    private float activeMoveSpeed = 4f;
    //ジャンプ力
    public Vector3 jumpForce = new Vector3(0,6,0);
    //視点移動の速度
    public float mouseSensitivity = 1f;
    //ユーザーのマウス入力格納
    private Vector2 mouseInpt;
    //y軸の回転格納
    private float verticalMouseInput;

    //レイを飛ばすオブジェクトの位置
    public Transform groundCheckPoint;

    //地面レイヤー
    public LayerMask groundLayers;

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

        //視点移動関数の呼び出し
        PlayerRotate();

        //地面に足がついていれば
        if (IsGround())
        {
            //走り関数呼び出し
            Run();

            //ジャンプ関数を呼ぶ
            Jump();
        }
        //攻撃入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        animator.SetFloat("Speed",rb.velocity.magnitude);

        //アニメーター遷移
        AnimatorSet();

    }

    private void LateUpdate()
    {
        //カメラの位置調節
        cam.transform.position = viewPoint.position;
        //回転
        cam.transform.rotation = viewPoint.rotation;
    }

    public void PlayerMove()
    {
        //移動キーの入力検知して値を格納する
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0,
            Input.GetAxisRaw("Vertical"));

        //進方向を出して変数に格納
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        //現在位置に反映していく
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    public void PlayerRotate()
    {
        //変数にユーザーのマウスの動きを格納
        mouseInpt = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        //マウスのｘ軸の動きを反映
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInpt.x,
            transform.eulerAngles.z);

        //ｙ軸の値に現在の値を足す
        verticalMouseInput += mouseInpt.y;

        //数値を丸める
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f,60f);

        //viewpointに丸めた数値を反映
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput,
            viewPoint.transform.rotation.eulerAngles.y,
            viewPoint.transform.rotation.eulerAngles.z);
    }

    public bool IsGround()
    {
        //判定してbool値を返す
        return Physics.Raycast(groundCheckPoint.position,Vector3.down,0.25f,groundLayers);
    }

    public void Jump()
    {
        //地面についていて、スペースキーが押されたとき
        if (IsGround()&&Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce,ForceMode.Impulse);
        }
    }

    public void Run()
    {
        //シフト押されている間スピード切り替える
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = walkSpeed;
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
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

        /*
        //run判定
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }*/

    }
}
