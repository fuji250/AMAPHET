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
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public GhostManager ghostManager;
    public Collider weaponCollider;
    public GameObject barrier;

    public GameObject atackChecker;
    public GameObject barrierChecker;
    private UIManager uiManager;
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
    bool isDie = false;
    bool isProtected = false;

    

    //Spawn����̂����}����t���O
    bool firstMoment = false;

    // �ǂ�����ł��g����悤�ɂ���
    public static PlayerManager instance;

    public List<Renderer> renderers = new List<Renderer>();//����̊i�[�z��

    //public GameObject[] playerModel;//�v���C���[���f�����i�[
    public Renderer[] myselfHolder;//Material�z���_�[

    public int spawnPointNumber;

    public Transform barrierCheckPoint;//�n�ʂɌ����ă��C���΂��I�u�W�F�N�g 
    public LayerMask barrierLayers;//�n�ʂ��ƔF�����郌�C���[ 

    private void Awake()
    {
        instance = this;

        //SpawnMangaer�i�[
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        //UIManager�i�[
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
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
        uiManager.Init(this);

        //����̓����蔻�������
        HideColliderWeapn();
        atackChecker.SetActive(false);
        barrierChecker.SetActive(false);

        renderers.Clear();//������

        

        if (!photonView.IsMine)//��������Ȃ�������
        {
            foreach (Renderer renderer in myselfHolder)//Material�̐������[�v
            {
                renderers.Add(renderer);//���X�g�ɒǉ�
            }

            Disappear();

            //uIManager.UpdateHP(maxHP, currentHp);//HP���X���C�_�[�ɔ��f

        }
        else
        {
            //�J�����i�[
            cam = Camera.main;
            //�J�������v���C���[�̎q�ɂ���̂ł͂Ȃ��A�X�N���v�g�ňʒu�����킹��
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }
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
        if (spawnPointNumber == 0)
        {
            rb.velocity = new Vector3(x, 0, z).normalized * moveSpeed;�@�@�@�@�@�@�@//�������x
        }
        else if(spawnPointNumber == 1)
        {
            rb.velocity = new Vector3(x, 0, z).normalized * -moveSpeed;�@�@�@�@�@�@�@//�������x
        }

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
        if (stamina >= 30f)
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
            stamina -= 50f * Time.deltaTime;
            if (stamina <= 0)
            {
                animator.SetBool("Defend", false);
            }
        }
    }

    void IncreaseStamina()
    {
        stamina += 20f * Time.deltaTime;;
        if(stamina >= maxStamina)
        {
            stamina = maxStamina;
        }
        uiManager.UpdateStamina(stamina);
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

    //��_���[�W�i�S�v���C���[���L�j
    [PunRPC]
    void Hurt(int damage, string enymyName,int actor)
    {
        animator.SetTrigger("Hurt");

        hp -= damage;
        if (hp <= 0)
        {
            string myName  = PhotonNetwork.NickName;
            //���S�֐�
            Death(myName, enymyName, actor);
        }
        //Debug.Log("�c��HP" + hp);
    }

    //���S�֐�
    public void Death(string myName,string enymyName, int actor)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //�L���f�X�C�x���g�Ăяo��
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        gameManager.ScoreGet(actor,0,1);
        uiManager.GetWinnerName(myName, enymyName);
    }

    
    public IEnumerator GradationAppear()
    {
        for (int i = 0; i < 255; i += 45)
        {
            foreach (Renderer mesh in renderers)//���X�g�����[�v����
            {
                mesh.material.color = mesh.material.color + new Color32(0,0,0,45);
                    
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void Appear()
    {
        foreach (Renderer mesh in renderers)//���X�g�����[�v����
            {
                mesh.material.color = new Color32(255,255,255,255);
            }
    }

    public void Disappear()
    {
        foreach (Renderer mesh in renderers)//���X�g�����[�v����
            {
                mesh.material.color = new Color32(255,255,255,0);//�����ɂ���
            }
    }

    public bool IsBarrier()
    {
        Debug.DrawRay(barrierCheckPoint.position, Vector3.down, Color.blue, 1f);
        return Physics.Raycast(barrierCheckPoint.position, Vector3.down, 1f, barrierLayers);
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
                //if (animator.GetBool("Defend"))
                if (IsBarrier())
                {
                    Debug.Log("�G�̍U����h�����I");
                    other.transform.root.gameObject.GetPhotonView().RPC("Repelled",RpcTarget.All);
                    return;
                }
                Debug.Log("�G�Ƀ_���[�W��^����ꂽ");
                //�_���[�W��^������̂ɂԂ�������
                 photonView.RPC("Hurt",RpcTarget.All,
                     damageManager.damage, other.transform.root.gameObject.GetPhotonView().Owner.NickName,
                     PhotonNetwork.PlayerListOthers[0].ActorNumber);
            }
        }

        if (other.gameObject.tag == "Barrier")
        {
            isProtected = true;
        }
    }

}
