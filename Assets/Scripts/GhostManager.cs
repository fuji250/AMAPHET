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
    Animator matesAnimator;
    public PlayerManager playerManager;
    public Collider weaponCollider;

    int hp;

    float delayTime = 2.0f;

    bool isDie;
    bool isMateDie;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = PlayerManager.instance.maxHp;

        
        //matesAnimator = playerManager.animator;

        HideColliderWeapn();


        //カメラ格納
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || isDie)
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

        animator.SetFloat("Speed",rb.velocity.magnitude);
    }

    [PunRPC]
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(delayTime);
        animator.SetTrigger("Attack");
    }

    IEnumerator Defend()
    {
        /*
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            yield return new WaitForSeconds(delayTime);
            animator.SetBool("Defend", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            yield return new WaitForSeconds(delayTime);
            animator.SetBool("Defend", false);
        }
        */

        //プレイヤーがガードしていれば遅れてガード
        if (playerManager.animator.GetBool("Defend"))
        {
            yield return new WaitForSeconds(delayTime);
            animator.SetBool("Defend", true);
        }
        else
        {
            yield return new WaitForSeconds(delayTime);
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
        animator.SetTrigger("Hurt");

        hp -= damage;
        if (hp <= 0)
        {
            //死亡関数
            Death(damage);
        }
        //Debug.Log("残りHP" + hp);
    }

    public void Death(int damage)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //キルデスイベント呼び出し
        //gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        //gameManager.ScoreSet(actor,0,1);
    }

    public bool CheckDeath()
    {
        if (isMateDie)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine　|| hp <= 0)
        {
            return;
        }


        if (other.CompareTag("Weapon"))
        {
            //DamageManagerを持つコライダーにぶつかった際に攻撃を受ける
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                Debug.Log("敵にダメージを与えられた");

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
