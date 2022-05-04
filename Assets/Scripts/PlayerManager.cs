using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    //カメラの親オブジェクト
    public Transform viewPoint;

    //カメラ
    private Camera cam;

    //移動
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


    //回転速度
    public  float smooth = 150f;
    //入力された値格納
    private Vector3 moveDir;


    //体力スタミナ
    public  int maxHp = 100;
    int hp;
    public  int maxStamina = 100;
    float stamina;
    //生きているかどうか
    bool isDie;

    

    //Spawn直後のずれを抑えるフラグ
    bool firstMoment = false;

    // どこからでも使えるようにする
    public static PlayerManager instance;

    public List<Renderer> renderers = new List<Renderer>();//武器の格納配列

    //public GameObject[] playerModel;//プレイヤーモデルを格納
    public Renderer[] myselfHolder;//Materialホルダー

    private void Awake()
    {
        instance = this;

        //SpawnMangaer格納
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        //UIManager格納
        playerUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<PlayerUIManager>();
        //GameManager格納
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //HPとスタミナの初期化
        hp = maxHp;
        stamina = maxStamina;

        //UIの初期化
        playerUIManager.Init(this);

        //武器の当たり判定を消す
        HideColliderWeapn();

        //カメラ格納
        cam = Camera.main;

        renderers.Clear();//初期化

        if (!photonView.IsMine)//自分じゃなかったら
        {
            /*
            foreach (var model in playerModel)//モデルのパーツ分ループ
            {
                model.SetActive(false);//非表示
            }
            */
            foreach (Renderer renderer in myselfHolder)//Materialの数分ループ
            {
                renderers.Add(renderer);//リストに追加
            }

            
            foreach (Renderer mesh in renderers)//リスト分ループを回す
            {
                mesh.material.color = new Color32(255,255,255,0);//透明にする
            }

            //uIManager.UpdateHP(maxHP, currentHp);//HPをスライダーに反映

        }/*
        else//他人だったらotherPlayerHolderを表示させる
        {
            foreach (Material material in otherPlayerHolder)//Materialの数分ループ
            {
                materials.Add(material);//リストに追加
            }
        }
        */

        //switchGun();//銃を表示させるため
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || isDie)
        {
            return;
        }

        //移動関数を呼ぶ
        PlayerMove();

        //攻撃入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Attack();
            photonView.RPC("Attack",
                    RpcTarget.All);
        }
        //防御入力
        Defend();
        
        //歩きアニメーションの判定
        animator.SetFloat("Speed",rb.velocity.magnitude);

        //スタミナの自動回復
        IncreaseStamina();
    }

    public void PlayerMove()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Playerの位置座標を毎フレーム最後に取得する
        latestPos = transform.position;　　　　　　　　　　　　　//Palyerの位置座標を更新する
        rb.velocity = new Vector3(x, 0, z) * moveSpeed;　　　　　　　//歩く速度

        //こいつのせいで回転しとる！！
        if (diff.magnitude > 0.01f)
        {
            if (!firstMoment)
            {
                firstMoment = true;
                diff = new Vector3(0,0,0);
                return;
            }
            //キーを押し方向転換
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
        //スタミナが十分にあればガード
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
        //スタミナが無くなればガードが外れる
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

    //武器の判定を有効にしたり・無効にしたりする関数
    public void HideColliderWeapn()
    {
        weaponCollider.enabled = false;
    }

    public void ShowColliderWeapn()
    {
        weaponCollider.enabled = true;
    }

    //被ダメージ（全プレイヤー共有）
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

    //死亡関数
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
        if (!photonView.IsMine　|| hp <= 0 ||
            other.transform.root.gameObject.GetPhotonView().Owner.NickName ==photonView.Owner.NickName)
        {
            return;
        }

        if (other.gameObject.tag == "Weapon")
        {
            //DamageManagerを持つコライダーにぶつかった際に攻撃を受ける
            DamageManager damageManager = other.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                if (animator.GetBool("Defend"))
                {
                    //Debug.Log("敵の攻撃を防いだ！");
                    return;
                }
                Debug.Log("敵にダメージを与えられた");
                //ダメージを与えるものにぶつかったら
                 photonView.RPC("Hurt",RpcTarget.All,
                     damageManager.damage, other.transform.root.gameObject.GetPhotonView().Owner.NickName,
                     PhotonNetwork.PlayerListOthers[0].ActorNumber);
            }
        }
    }
}
