using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //�J�����̐e�I�u�W�F�N�g
    public Transform viewPoint;

    //�J����
    private Camera cam;

    //�������x
    public float walkSpeed = 4f;
    //����X�s�[�h
    public float runSpeed = 8f;
    //���͂��ꂽ�l�i�[
    private Vector3 moveDir;
    //�i�����i�[
    private Vector3 movement;
    //���ۂ̈ړ����x
    private float activeMoveSpeed = 4f;
    //�W�����v��
    public Vector3 jumpForce = new Vector3(0,6,0);
    //���_�ړ��̑��x
    public float mouseSensitivity = 1f;
    //���[�U�[�̃}�E�X���͊i�[
    private Vector2 mouseInpt;
    //y���̉�]�i�[
    private float verticalMouseInput;

    //���C���΂��I�u�W�F�N�g�̈ʒu
    public Transform groundCheckPoint;

    //�n�ʃ��C���[
    public LayerMask groundLayers;

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

        //���_�ړ��֐��̌Ăяo��
        PlayerRotate();

        //�n�ʂɑ������Ă����
        if (IsGround())
        {
            //����֐��Ăяo��
            Run();

            //�W�����v�֐����Ă�
            Jump();
        }
        //�U������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        animator.SetFloat("Speed",rb.velocity.magnitude);

        //�A�j���[�^�[�J��
        AnimatorSet();

    }

    private void LateUpdate()
    {
        //�J�����̈ʒu����
        cam.transform.position = viewPoint.position;
        //��]
        cam.transform.rotation = viewPoint.rotation;
    }

    public void PlayerMove()
    {
        //�ړ��L�[�̓��͌��m���Ēl���i�[����
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0,
            Input.GetAxisRaw("Vertical"));

        //�i�������o���ĕϐ��Ɋi�[
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        //���݈ʒu�ɔ��f���Ă���
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    public void PlayerRotate()
    {
        //�ϐ��Ƀ��[�U�[�̃}�E�X�̓������i�[
        mouseInpt = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        //�}�E�X�̂����̓����𔽉f
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInpt.x,
            transform.eulerAngles.z);

        //�����̒l�Ɍ��݂̒l�𑫂�
        verticalMouseInput += mouseInpt.y;

        //���l���ۂ߂�
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f,60f);

        //viewpoint�Ɋۂ߂����l�𔽉f
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput,
            viewPoint.transform.rotation.eulerAngles.y,
            viewPoint.transform.rotation.eulerAngles.z);
    }

    public bool IsGround()
    {
        //���肵��bool�l��Ԃ�
        return Physics.Raycast(groundCheckPoint.position,Vector3.down,0.25f,groundLayers);
    }

    public void Jump()
    {
        //�n�ʂɂ��Ă��āA�X�y�[�X�L�[�������ꂽ�Ƃ�
        if (IsGround()&&Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce,ForceMode.Impulse);
        }
    }

    public void Run()
    {
        //�V�t�g������Ă���ԃX�s�[�h�؂�ւ���
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = walkSpeed;
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
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

        /*
        //run����
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }*/

    }
}
