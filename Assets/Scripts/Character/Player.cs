using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    private void OnEnable()
    {
        speed = Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        /* 움직이는 방법에 대한 종류
        1.Forced(계속 움직임 힘을 더하기만함)
        rigid.AddForce(inputVec);

        2.Velocity control(미끄러지는 느낌)
        rigid.velocity = inputVec;
        */
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        //3.Position Move(프레임마다 움직임을 가함)
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
            spriter.flipX = inputVec.x < 0 ? true : false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.instance.isLive)
            return;

        //if(collision.)

        GameManager.instance.health -= Time.deltaTime * 10;

        if(GameManager.instance.health<0)
        {
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }
}
