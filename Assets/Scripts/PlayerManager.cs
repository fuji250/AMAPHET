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
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public GhostManager ghostManager;
    public Collider weaponCollider;
    public GameObject barrier;

    public GameObject atackChecker;
    public GameObject barrierChecker;
    private UIManager uiManager;
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
    bool isDie = false;
    bool isProtected = false;

    

    //Spawn直後のずれを抑えるフラグ
    bool firstMoment = false;

    // どこからでも使えるようにする
    public static PlayerManager instance;

    public List<Renderer> renderers = new List<Renderer>();//武器の格納配列

    //public GameObject[] playerModel;//プレイヤーモデルを格納
    public Renderer[] myselfHolder;//Materialホルダー

    public int spawnPointNumber;

    public Transform barrierCheckPoint;//地面に向けてレイを飛ばすオブジェクト 
    public LayerMask barrierLayers;//地面だと認識するレイヤー 

    private void Awake()
    {
        instance = this;

        //SpawnMangaer格納
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        //UIManager格納
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
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
        uiManager.Init(this);

        //武器の当たり判定を消す
        HideColliderWeapn();
        atackChecker.SetActive(false);
        barrierChecker.SetActive(false);

        renderers.Clear();//初期化

        

        if (!photonView.IsMine)//自分じゃなかったら
        {
            foreach (Renderer renderer in myselfHolder)//Materialの数分ループ
            {
                renderers.Add(renderer);//リストに追加
            }

            Disappear();

            //uIManager.UpdateHP(maxHP, currentHp);//HPをスライダーに反映

        }
        else
        {
            //カメラ格納
            cam = Camera.main;
            //カメラをプレイヤーの子にするのではなく、スクリプトで位置を合わせる
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
        if (spawnPointNumber == 0)
        {
            rb.velocity = new Vector3(x, 0, z).normalized * moveSpeed;　　　　　　　//歩く速度
        }
        else if(spawnPointNumber == 1)
        {
            rb.velocity = new Vector3(x, 0, z).normalized * -moveSpeed;　　　　　　　//歩く速度
        }

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
        //スタミナが無くなればガードが外れる
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

    //被ダメージ（全プレイヤー共有）
    [PunRPC]
    void Hurt(int damage, string enymyName,int actor)
    {
        animator.SetTrigger("Hurt");

        hp -= damage;
        if (hp <= 0)
        {
            string myName  = PhotonNetwork.NickName;
            //死亡関数
            Death(myName, enymyName, actor);
        }
        //Debug.Log("残りHP" + hp);
    }

    //死亡関数
    public void Death(string myName,string enymyName, int actor)
    {
        hp = 0;
        isDie = true;
        animator.SetTrigger("Die");
        //gameOverText.SetActive(true);
        rb.velocity = Vector3.zero;

        //キルデスイベント呼び出し
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        gameManager.ScoreGet(actor,0,1);
        uiManager.GetWinnerName(myName, enymyName);
    }

    
    public IEnumerator GradationAppear()
    {
        for (int i = 0; i < 255; i += 45)
        {
            foreach (Renderer mesh in renderers)//リスト分ループを回す
            {
                mesh.material.color = mesh.material.color + new Color32(0,0,0,45);
                    
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void Appear()
    {
        foreach (Renderer mesh in renderers)//リスト分ループを回す
            {
                mesh.material.color = new Color32(255,255,255,255);
            }
    }

    public void Disappear()
    {
        foreach (Renderer mesh in renderers)//リスト分ループを回す
            {
                mesh.material.color = new Color32(255,255,255,0);//透明にする
            }
    }

    public bool IsBarrier()
    {
        Debug.DrawRay(barrierCheckPoint.position, Vector3.down, Color.blue, 1f);
        return Physics.Raycast(barrierCheckPoint.position, Vector3.down, 1f, barrierLayers);
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
                //if (animator.GetBool("Defend"))
                if (IsBarrier())
                {
                    Debug.Log("敵の攻撃を防いだ！");
                    other.transform.root.gameObject.GetPhotonView().RPC("Repelled",RpcTarget.All);
                    return;
                }
                Debug.Log("敵にダメージを与えられた");
                //ダメージを与えるものにぶつかったら
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
