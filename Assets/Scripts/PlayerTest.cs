using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private float x;
    private float z;
    public float moveSpeed = 1.0f;
    private Vector3 latestPos;

    private Rigidbody rb;
    private Animator animator;

    public  float smooth = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();   //�A�j���[�V�������擾����
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 diff = transform.position - latestPos;           //Player�̈ʒu���W�𖈃t���[���Ō�Ɏ擾����
        latestPos = transform.position;�@�@�@�@�@�@�@�@�@�@�@�@�@//Palyer�̈ʒu���W���X�V����@

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;�@�@�@�@�@�@�@//�������x�@�@
        animator.SetFloat("Walk", rb.velocity.magnitude);        //�����A�j���[�V�����ɐ؂�ւ���

        if (diff.magnitude > 0.01f)
        {
            //transform.rotation = Quaternion.LookRotation(diff);  //�L�[�������������������悤�ɂ���

            //�L�[�����������]��
            Quaternion rotation = Quaternion.LookRotation(diff);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");    //�}�E�X�N���b�N�ōU�����[�V����
        }
    }
}