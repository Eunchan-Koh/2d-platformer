using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovementType3 : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rigid;
    Animator anim;

    SpriteRenderer spriteRenderer;

    CapsuleCollider2D capsuleCollider;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("is_running", true);
        rigid.velocity = new Vector2(-1, rigid.velocity.y);
    }

    public void OnDamaged(){
        //sprite alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //monster filps y
        spriteRenderer.flipY = true;
        //monster falls
        //monster looses collision
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up*5,ForceMode2D.Impulse);
        //call deactivate after 5 sec
        Invoke("Deactivate", 5);
    }

    void Deactivate(){
        gameObject.SetActive(false);
    }
}
