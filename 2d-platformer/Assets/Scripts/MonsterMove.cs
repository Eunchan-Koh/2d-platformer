using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;

    SpriteRenderer spriteRenderer;
   
    Animator anim;

    CapsuleCollider2D capsuleCollider;
    int jumpPower;
    bool is_jumping;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpPower = 5;
        is_jumping = false;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Think();

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
        RaycastHit2D down_side = Physics2D.Raycast(rigid.position, UnityEngine.Vector2.down, 1, LayerMask.GetMask("Platform"));
        //check if jumping
        if(down_side.collider == null){
            is_jumping = true;
        }else{
            is_jumping = false;
        }
        //checking if monster is at edge
        if(!is_jumping){
            if(left_side.collider == null){
                nextMove = Mathf.Abs(nextMove);
            }else if(right_side.collider == null){
                nextMove = -1*Mathf.Abs(nextMove);
            }
        }
        

        rigid.velocity = new UnityEngine.Vector2(nextMove, rigid.velocity.y);
    }

    void Think(){
        
        //considering where to go. -1: left, 1: right, 0: none
        nextMove = Random.Range(-1,2);// least num is included, but max num is not included in the range. <= consider -1, 0, 1
        //consider if jump or not. -1: none, 0: jump
        if(Think2() == 0){
            rigid.AddForce(UnityEngine.Vector2.up*jumpPower, ForceMode2D.Impulse);
            is_jumping = true;
        }
        float nextThinkTime = Random.Range(2f,5f);
        
        Invoke("Think", nextThinkTime);
    }
    int Think2(){
        return Random.Range(-1,1);
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
        nextMove = 0;
        CancelInvoke();
        Invoke("Deactivate", 1);
    }

    void Deactivate(){
        gameObject.SetActive(false);
    }

}
