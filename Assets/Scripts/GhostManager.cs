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
            //Invoke(nameof(Attack), 2.0f);
            //Invoke(nameof(photonView.RPC("Attack", RpcTarget.All)), 2.0f);
            //photonView.RPC("Invoke(nameof(Attack), 2.0f)",RpcTarget.All);
            photonView.RPC("Attack",RpcTarget.All);
            //StartCoroutine(photonView.RPC(("Attack", RpcTarget.All)));

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
    void Hurt(int damage)
    {
        animator.SetTrigger("Hurt");

        hp -= damage;
        if (hp <= 0)
        {
            //���S�֐�
            Death(damage);
        }
        //Debug.Log("�c��HP" + hp);
    }

    public void Death(int damage)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //�L���f�X�C�x���g�Ăяo��
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
        if (!photonView.IsMine�@|| hp <= 0)
        {
            return;
        }


        if (other.CompareTag("Weapon"))
        {
            //DamageManager�����R���C�_�[�ɂԂ������ۂɍU�����󂯂�
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                Debug.Log("�G�Ƀ_���[�W��^����ꂽ");

                if (animator.GetBool("Defend"))
                {
                    Debug.Log("�G�̍U����h�����I");
                    return;
                }
                //�_���[�W��^������̂ɂԂ�������
                //Hurt(damageManager.damage);
                
                 photonView.RPC("Hurt",
                 RpcTarget.All,
                 damageManager.damage);
                
            }
        }
        
    }
}
