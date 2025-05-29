using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animControl;

    [SerializeField]
    private bool isDead;
    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Rigidbody2D target;
    private Animator anim;
    private Collider2D col;

    WaitForFixedUpdate wait;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isDead = false;
        col.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        //Debug.Log(animControl[data.spriteType]);
        anim.runtimeAnimatorController = animControl[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (isDead || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") )
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (isDead)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || isDead)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            //hit action
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isDead = true;
            col.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder  = 1; 
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetEXP();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    private IEnumerator KnockBack()
    {
        //1Physics frame delay
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    private void Dead()
    {
        //Dead Action / Animation Event Play
        gameObject.SetActive(false);
    }
}
