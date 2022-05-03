using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
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
        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Invoke(nameof(Attack), 2.0f);
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
    void Attack()
    {
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

    void Damage(int damage)
    {
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
        if (hp <= 0)
        {
            return;
        }

        DamageManager damager = other.GetComponent<DamageManager>();
        if (damager != null) 
        {
            //�_���[�W��^������̂ɂԂ�������
            animator.SetTrigger("Hurt");
            Damage(damager.damage);
        }
    }
}
