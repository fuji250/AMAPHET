using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //カメラの親オブジェクト
    public Transform viewPoint;

    //カメラ
    private Camera cam;

    private float x;
    private float z;
    public float moveSpeed = 1.0f;
    private Vector3 latestPos;

    public Collider weaponCollider;
    //public PlayerUIManager playerUIManager;

    //回転速度
    public  float smooth = 150f;
    //入力された値格納
    private Vector3 moveDir;

    //レイを飛ばすオブジェクトの位置
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


        //カメラ格納
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //移動関数を呼ぶ
        //PlayerMove();

        //攻撃入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        Attack();
        //防御入力
        //Defend();
        
        animator.SetFloat("Speed",rb.velocity.magnitude);

        IncreaseStamina();

        //アニメーター遷移
        //AnimatorSet();
    }
    public void PlayerMove()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Playerの位置座標を毎フレーム最後に取得する
        latestPos = transform.position;　　　　　　　　　　　　　//Palyerの位置座標を更新する　

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;　　　　　　　//歩く速度　　
        //animator.SetFloat("Walk", rb.velocity.magnitude);        //歩くアニメーションに切り替える

        if (diff.magnitude > 0.01f)
        {
            //キーを押し方向転換
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
        //playerUIManager.UpdateStamina(stamina);
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
        Debug.Log("残りHP" + hp);
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
            //ダメージを与えるものにぶつかったら
            animator.SetTrigger("Hurt");
            Damage(damager.damage);
        }
    }
}
