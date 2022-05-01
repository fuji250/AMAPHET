using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //�J�����̐e�I�u�W�F�N�g
    public Transform viewPoint;

    //�J����
    private Camera cam;

    private float x;
    private float z;
    public float Speed = 1.0f;
    private Vector3 latestPos;

    //��]���x
    public  float smooth = 150f;
    //���͂��ꂽ�l�i�[
    private Vector3 moveDir;

    //���C���΂��I�u�W�F�N�g�̈ʒu
    public Transform groundCheckPoint;

    private Rigidbody rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //�J�����i�[
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //�ړ��֐����Ă�
        PlayerMove();

        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        //�h�����
        Defend();
        
        animator.SetFloat("Speed",rb.velocity.magnitude);

        //�A�j���[�^�[�J��
        //AnimatorSet();
    }
    public void PlayerMove()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Player�̈ʒu���W�𖈃t���[���Ō�Ɏ擾����
        latestPos = transform.position;�@�@�@�@�@�@�@�@�@�@�@�@�@//Palyer�̈ʒu���W���X�V����@

        rb.velocity = new Vector3(x, 0, z) * Speed;�@�@�@�@�@�@�@//�������x�@�@
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("defend", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("defend", false);
        }
    }

    private void AnimatorSet()
    {
        //Walk����
        if (moveDir != Vector3.zero)
        {
            animator.SetBool("Walk", true);

        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }
}
