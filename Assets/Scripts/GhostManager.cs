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


        //�J�����i�[
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
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
        Debug.Log("�c��HP" + hp);
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
            //DamageManager�����R���C�_�[�ɂԂ������ۂɍU�����󂯂�
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                Debug.Log("�G�Ƀ_���[�W��^����2");

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
