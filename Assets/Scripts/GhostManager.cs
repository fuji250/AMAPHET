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
    GameManager gameManager;

    int hp;

    public  float delayTime = 1.5f;

    bool isDie;
    bool isMateDie;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //GameManager格納
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = PlayerManager.instance.maxHp;

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
            photonView.RPC("Attack",RpcTarget.All);
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
    public void Repelled()
    {
        animator.SetTrigger("Repelled");
    }

    [PunRPC]
    void Hurt(int damage, string name,int actor)
    {
        animator.SetTrigger("Hurt");

        hp -= damage;
        if (hp <= 0)
        {
            //死亡関数
            Death(name, actor);
        }
        //Debug.Log("残りHP" + hp);
    }

    public void Death(string name, int actor)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //キルデスイベント呼び出し
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        gameManager.ScoreGet(actor,0,1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine　|| hp <= 0)
        {
            return;
        }

        if (other.gameObject.tag == "Weapon")
        {
            if (other.transform.root.gameObject.GetPhotonView().Owner.NickName ==photonView.Owner.NickName)
            {
                return;
            }
            //DamageManagerを持つコライダーにぶつかった際に攻撃を受ける
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {

                if (animator.GetBool("Defend"))
                {
                    Debug.Log("敵の攻撃を防いだ！");
                    other.transform.root.gameObject.GetPhotonView().RPC("Repelled",RpcTarget.All);
                    return;
                }
                //Debug.Log("敵にダメージを与えられた");

                //ダメージを与えるものにぶつかったら
                 photonView.RPC("Hurt",RpcTarget.All,
                     damageManager.damage, other.transform.root.gameObject.GetPhotonView().Owner.NickName,
                     PhotonNetwork.PlayerListOthers[0].ActorNumber);
            }
        }
        
    }
}
