using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovementType2 : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;

    SpriteRenderer spriteRenderer;
   
    Animator anim;

     CapsuleCollider2D capsuleCollider;
    int a;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        a = -1;


    }

    void Update(){
        if(rigid.velocity.x != 0){
            anim.SetBool("is_running", true);
        }else{
            anim.SetBool("is_running", false);
        }
        if(rigid.velocity.x > 0){
            spriteRenderer.flipX = true;
        }else if (rigid.velocity.x < 0){
            spriteRenderer.flipX = false;
        }
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        UnityEngine.Vector2 left_end = rigid.position+UnityEngine.Vector2.left*0.455f;
        UnityEngine.Vector2 right_end = rigid.position+UnityEngine.Vector2.right*0.43f;
        
        Debug.DrawRay(left_end, UnityEngine.Vector2.down, new Color(0,1,0));
        Debug.DrawRay(right_end, UnityEngine.Vector2.down, new Color(0,1,0));
        Debug.DrawRay(rigid.position, UnityEngine.Vector2.down, new Color(1, 0, 0));
        RaycastHit2D left_side = Physics2D.Raycast(left_end, UnityEngine.Vector2.down, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D right_side = Physics2D.Raycast(right_end, UnityEngine.Vector2.down, 1, LayerMask.GetMask("Platform"));

        if(left_side.collider == null && right_side.collider != null){
            a = 1;
        }
        if (right_side.collider == null && left_side.collider != null){
            a = -1;
        }
        

        rigid.velocity = new UnityEngine.Vector2(a, rigid.velocity.y);
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
