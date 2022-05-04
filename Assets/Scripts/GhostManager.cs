using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GhostManager : MonoBehaviourPunCallbacks
{
    //カメラの親オブジェクト
    public Transform viewPoint;

    //カメラ
    private Camera cam;

    private Rigidbody rb;
    Animator animator;

    public Collider weaponCollider;

    public  int maxHp = 100;
    int hp;

    bool isDie;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        HideColliderWeapn();


        //カメラ格納
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //攻撃入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Invoke(nameof(Attack), 2.0f);
            //Invoke(nameof(photonView.RPC("Attack", RpcTarget.All)), 2.0f);
            //photonView.RPC("Invoke(nameof(Attack), 2.0f)",RpcTarget.All);
            photonView.RPC("Attack",RpcTarget.All);
            //StartCoroutine(photonView.RPC(("Attack", RpcTarget.All)));

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
    [PunRPC]
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(2.0f);

        animator.SetTrigger("Attack");
    }

    /*
    void Defend()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("Defend", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("Defend", false);
        }
    }
    /*/

    IEnumerator Defend()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            yield return new WaitForSeconds(2.0f);
            animator.SetBool("Defend", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            yield return new WaitForSeconds(2.0f);
            animator.SetBool("Defend", false);
        }
    }

    //武器の判定を有効にしたり・無効にしたりする関数
    public void HideColliderWeapn()
    {
        weaponCollider.enabled = false;
    }

    public void ShowColliderWeapn()
    {
        weaponCollider.enabled = true;
    }

    [PunRPC]
    void Hurt(int damage)
    {
        //yield return new WaitForSeconds(2.0f);

        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            isDie = true;
            animator.SetTrigger("Die");
            //gameOverText.SetActive(true);
            rb.velocity = Vector3.zero;
        }
        Debug.Log("残りHP" + hp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (hp <= 0)
        {
            return;
        }

        if (other.CompareTag("Weapon"))
        {
            //DamageManagerを持つコライダーにぶつかった際に攻撃を受ける
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                Debug.Log("敵にダメージを与えた2");

                if (animator.GetBool("Defend"))
                {
                    Debug.Log("敵の攻撃を防いだ！");
                    return;
                }
                //ダメージを与えるものにぶつかったら
                
                //Hurt(damageManager.damage);

                 photonView.RPC("Hurt",
                 RpcTarget.All,
                 damageManager.damage);
            }
        }
        
    }
}
