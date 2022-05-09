using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GhostManager : MonoBehaviourPunCallbacks
{
    //�J�����̐e�I�u�W�F�N�g
    public Transform viewPoint;

    //�J����
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
        //GameManager�i�[
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = PlayerManager.instance.maxHp;

        HideColliderWeapn();


        //�J�����i�[
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || isDie)
        {
            return;
        }

        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("Attack",RpcTarget.All);
        }
        //�h�����
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
        //�v���C���[���K�[�h���Ă���Βx��ăK�[�h
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

    //����̔����L���ɂ�����E�����ɂ����肷��֐�
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
            //���S�֐�
            Death(name, actor);
        }
        //Debug.Log("�c��HP" + hp);
    }

    public void Death(string name, int actor)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //�L���f�X�C�x���g�Ăяo��
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        gameManager.ScoreGet(actor,0,1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine�@|| hp <= 0)
        {
            return;
        }

        if (other.gameObject.tag == "Weapon")
        {
            if (other.transform.root.gameObject.GetPhotonView().Owner.NickName ==photonView.Owner.NickName)
            {
                return;
            }
            //DamageManager�����R���C�_�[�ɂԂ������ۂɍU�����󂯂�
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {

                if (animator.GetBool("Defend"))
                {
                    Debug.Log("�G�̍U����h�����I");
                    other.transform.root.gameObject.GetPhotonView().RPC("Repelled",RpcTarget.All);
                    return;
                }
                //Debug.Log("�G�Ƀ_���[�W��^����ꂽ");

                //�_���[�W��^������̂ɂԂ�������
                 photonView.RPC("Hurt",RpcTarget.All,
                     damageManager.damage, other.transform.root.gameObject.GetPhotonView().Owner.NickName,
                     PhotonNetwork.PlayerListOthers[0].ActorNumber);
            }
        }
        
    }
}
