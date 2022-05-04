using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
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
    private PlayerUIManager playerUIManager;
    private SpawnManager spawnManager;

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

    bool firstMoment = false;

    private void Awake()
    {
        //SpawnMangaer�i�[
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        //UIManager�i�[
        playerUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<PlayerUIManager>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        stamina = maxStamina;

        playerUIManager.Init(this);


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
        if (isDie)
        {
            return;
        }

        //�ړ��֐����Ă�
        PlayerMove();
        //StartCoroutine(PlayerMove());


        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Attack();
            photonView.RPC("Attack",
                    RpcTarget.All);
        }
        //�h�����
        Defend();
        //photonView.RPC("Defend",
        //            RpcTarget.All);
        
        animator.SetFloat("Speed",rb.velocity.magnitude);

        IncreaseStamina();

        //�A�j���[�^�[�J��
        //AnimatorSet();
    }
    public void PlayerMove()
    {
        
        // 3�b�ԑ҂�
        //yield return new WaitForSeconds(3);

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Player�̈ʒu���W�𖈃t���[���Ō�Ɏ擾����
        
        latestPos = transform.position;�@�@�@�@�@�@�@�@�@�@�@�@�@//Palyer�̈ʒu���W���X�V����@

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;�@�@�@�@�@�@�@//�������x

        /*
        if (firstMoment)
        {
            //diff = transform.position;
            firstMoment = true;
            return;
        }
        */
        
        //�����̂����ŉ�]���Ƃ�I�I
        if (diff.magnitude > 0.01f)
        {
            if (!firstMoment)
            {
                firstMoment = true;
                diff = new Vector3(0,0,0);
                Debug.Log(diff);
                return;
            }
            //�L�[�����������]��
            Quaternion rotation = Quaternion.LookRotation(diff);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);
        }
        
    }

    [PunRPC]
    public  void Attack()
    {
        animator.SetTrigger("Attack");
        //Debug.Log("�U������");
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
        playerUIManager.UpdateStamina(stamina);
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

    //��_���[�W�i�S�v���C���[���L�j
    [PunRPC]
    void Hurt(int damage)
    {
        if (isDie)
            {
                return;
            }
        animator.SetTrigger("Hurt");

        if (photonView.IsMine)
        {
            hp -= damage;
            if (hp <= 0)
            {
                //���S�֐�
                Death(damage);
            }
            //playerUIManager.UpdateHP(hp);
            Debug.Log("�c��HP" + hp);
        }
        else
        {
        

        }
    }


    //���S�֐�
    public void Death(int damage)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;
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
                
                //Damage(damageManager.damage);
                 photonView.RPC("Hurt",
                 RpcTarget.All,
                 damageManager.damage);
            }
        }
        
    }
}
