using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //�J�����̐e�I�u�W�F�N�g
    public Transform viewPoint;

    //�J����
    private Camera cam;

    private float x;
    private float z;
    public float moveSpeed = 1.0f;
    private Vector3 latestPos;

    public Collider weaponCollider;
    //public PlayerUIManager playerUIManager;

    //��]���x
    public  float smooth = 150f;
    //���͂��ꂽ�l�i�[
    private Vector3 moveDir;

    //���C���΂��I�u�W�F�N�g�̈ʒu
    public Transform groundCheckPoint;

    
    public  int maxHp = 100;
    int hp;
    public  int maxStamina = 100;
    float stamina;
    bool isDie;

    private Rigidbody rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        stamina = maxStamina;

        //playerUIManager.Init(this);


        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        HideColliderWeapn();


        //�J�����i�[
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //�ړ��֐����Ă�
        //PlayerMove();

        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        Attack();
        //�h�����
        //Defend();
        
        animator.SetFloat("Speed",rb.velocity.magnitude);

        IncreaseStamina();

        //�A�j���[�^�[�J��
        //AnimatorSet();
    }
    public void PlayerMove()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Player�̈ʒu���W�𖈃t���[���Ō�Ɏ擾����
        latestPos = transform.position;�@�@�@�@�@�@�@�@�@�@�@�@�@//Palyer�̈ʒu���W���X�V����@

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;�@�@�@�@�@�@�@//�������x�@�@
        //animator.SetFloat("Walk", rb.velocity.magnitude);        //�����A�j���[�V�����ɐ؂�ւ���

        if (diff.magnitude > 0.01f)
        {
            //�L�[�����������]��
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
        //�X�^�~�i���\���ɂ���΃K�[�h
        if (stamina >= 10f)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                animator.SetBool("Defend", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("Defend", false);
        }
        //�X�^�~�i�������Ȃ�΃K�[�h���O���
        if (animator.GetBool("Defend"))
        {
            stamina -= 0.1f;
            if (stamina <= 0)
            {
                animator.SetBool("Defend", false);
            }
        }
    }

    void IncreaseStamina()
    {
        stamina += 0.01f;
        if(stamina >= maxStamina)
        {
            stamina = maxStamina;
        }
        //playerUIManager.UpdateStamina(stamina);
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
        //playerUIManager.UpdateHP(hp);
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
