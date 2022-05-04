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

    //�ړ�
    private float x;
    private float z;
    public float moveSpeed = 1.0f;
    private Vector3 latestPos;

    private Rigidbody rb;
    public Animator animator;
    public GhostManager ghostManager;
    public Collider weaponCollider;
    private PlayerUIManager playerUIManager;
    private SpawnManager spawnManager;
    GameManager gameManager;


    //��]���x
    public  float smooth = 150f;
    //���͂��ꂽ�l�i�[
    private Vector3 moveDir;


    //�̗̓X�^�~�i
    public  int maxHp = 100;
    int hp;
    public  int maxStamina = 100;
    float stamina;
    //�����Ă��邩�ǂ���
    bool isDie;

    

    //Spawn����̂����}����t���O
    bool firstMoment = false;

    // �ǂ�����ł��g����悤�ɂ���
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;

        //SpawnMangaer�i�[
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        //UIManager�i�[
        playerUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<PlayerUIManager>();
        //GameManager�i�[
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //HP�ƃX�^�~�i�̏�����
        hp = maxHp;
        stamina = maxStamina;

        //UI�̏�����
        playerUIManager.Init(this);


        

        //����̓����蔻�������
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

        //�ړ��֐����Ă�
        PlayerMove();

        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Attack();
            photonView.RPC("Attack",
                    RpcTarget.All);
        }
        //�h�����
        Defend();
        
        //�����A�j���[�V�����̔���
        animator.SetFloat("Speed",rb.velocity.magnitude);

        //�X�^�~�i�̎�����
        IncreaseStamina();
    }

    public void PlayerMove()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Player�̈ʒu���W�𖈃t���[���Ō�Ɏ擾����
        latestPos = transform.position;�@�@�@�@�@�@�@�@�@�@�@�@�@//Palyer�̈ʒu���W���X�V����
        rb.velocity = new Vector3(x, 0, z) * moveSpeed;�@�@�@�@�@�@�@//�������x

        //�����̂����ŉ�]���Ƃ�I�I
        if (diff.magnitude > 0.01f)
        {
            if (!firstMoment)
            {
                firstMoment = true;
                diff = new Vector3(0,0,0);
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
        animator.SetTrigger("Hurt");

        hp -= damage;
        if (hp <= 0)
        {
            //���S�֐�
            Death(damage);
        }
        //Debug.Log("�c��HP" + hp);
    }

    //���S�֐�
    public void Death(int damage)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //�L���f�X�C�x���g�Ăяo��
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        //gameManager.ScoreSet(actor,0,1);
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
                //Debug.Log("�G�Ƀ_���[�W��^����ꂽ");

                if (animator.GetBool("Defend"))
                {
                    //Debug.Log("�G�̍U����h�����I");
                    return;
                }

                //�_���[�W��^������̂ɂԂ�������
                 photonView.RPC("Hurt",
                 RpcTarget.All,
                 damageManager.damage);
            }
        }
    }
}
