using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    AudioSource audioSource;

    CapsuleCollider2D capsuleCollider;

    int jump_count;
    int max_jump;
    bool is_on_wall;
    bool is_wall_jump;
    int wall_jump_dir;
    static bool gotHit;

    int control_lock;

    // Start is called before the first frame update
    void Start()
    {
        //maximum movement (horizontal) speed character can achieve
        maxSpeed = 5;

        //power of jump - character
        jumpPower = 15f;

        //getting components
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        //setting animation booleans
        anim.SetBool("is_moving", false);
        anim.SetBool("is_jumping", false);
        anim.SetBool("is_going_up", false);
        anim.SetBool("is_going_down", false);

        //initial hp of the player character
        gameManager.health = 3;

        //int for current jump count - used for counting double jump
        jump_count = 0;

        //maximum number of jumps once leaving the floor - double jump allowed for now
        max_jump = 2;

        //boolean checking for wall jump - initial is false
        is_on_wall = false;

        //bool to check if wall jumping
        is_wall_jump = false;

        //int for jump direction when on wall. if wall is on left, dir is 1, and if wall is on right, dir is -1. available dirs: 1 or -1
        wall_jump_dir = 0;

        //used for checking collision only once at a time - not working as intended right now
        gotHit = false;

        //control lock for wall jump, 1 for moving and 0 for lock
        control_lock = 1;
    }


    void PlaySound(string action){
        switch (action){
            case "Jump":
                audioSource.clip = audioJump;
            break;
            case "Attack":
                audioSource.clip = audioAttack;
            break;
            case "Damaged":
                audioSource.clip = audioDamaged;
            break;
            case "Item":
                audioSource.clip = audioItem;
            break;
            case "Die":
                audioSource.clip = audioDie;
            break;
            case "Finish":
                audioSource.clip = audioFinish;
            break;
            default:

            break;
        }
        audioSource.PlayOneShot(audioSource.clip);
    }

    // Update is called once per frame
    void Update()
    {
        //Jump when pushed space
        if (Input.GetButtonDown("Jump"))
        {
            //wall jump
            if(is_on_wall == true){
                control_lock = 0;
                is_wall_jump = true;
                PlaySound("Jump");
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y*0);
                rigid.AddForce((Vector2.up * jumpPower*1.3f) + (Vector2.right*wall_jump_dir*7), ForceMode2D.Impulse);
                anim.SetBool("is_jumping", true);
                anim.SetBool("is_going_up", true);
                anim.SetBool("is_going_down", false);
                Invoke("FreeLock", 0.3f);
                //does not increase jump_count
            }else{
                //normal jump from floor
                if(jump_count < max_jump){
                    PlaySound("Jump");
                    rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y*0);
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    anim.SetBool("is_jumping", true);
                    anim.SetBool("is_going_up", true);
                    anim.SetBool("is_going_down", false);
                    jump_count++;
                }
            }
            
        }
        
        //Stop Speed when leaving button
        if (Input.GetButtonUp("Horizontal"))
        {
            // spriteRenderer.flipX = Input.GetAxisRaw("Horizontal")==-1;
            rigid.velocity = new Vector2(rigid.velocity.normalized.x *0.3f, rigid.velocity.y);
        }

        //character's facing direction
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal")==-1;
        }

        //Animation setting
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("is_moving", false);
        }
        else
        {
            anim.SetBool("is_moving", true);
        }
        if(anim.GetBool("is_jumping")){
            anim.SetBool("is_jumping", false);
            anim.SetBool("is_jumping", true);
            if(rigid.velocity.y > 0){
                anim.SetBool("is_going_up", true);
                anim.SetBool("is_going_down", false);
            }else if(rigid.velocity.y < 0){
                anim.SetBool("is_going_up", false);
                anim.SetBool("is_going_down", true);
            }else{
                anim.SetBool("is_going_up", false);
                anim.SetBool("is_going_down", false);
            }
        }else{
            anim.SetBool("is_going_up", false);
            anim.SetBool("is_going_down", false);
        }


    }

    void FixedUpdate()
    {
        //Move By Key Control - Horizontal. -1 if left, +1 if right
        float h = Input.GetAxisRaw("Horizontal");

        //checking if left shift is pushed - dash available
        bool is_running = Input.GetButton(KeyCode.LeftShift.ToString());
        float run_val = is_running?1:0;

        //if shift is pushed, maxSpeed increase to allow running
        if(is_running){//is pushing shift
            anim.SetFloat("curSpeed", 2f);
            maxSpeed = 8;   
        }else{//normal form
            anim.SetFloat("curSpeed", 1f);
            maxSpeed = 5;
        }
        
        //movement force
        if(!is_on_wall){//if not on wall, force given(*2 because of uphills)
            rigid.AddForce(Vector2.right * h *(2+run_val) * control_lock, ForceMode2D.Impulse);
        }else{//if on wall, movement key does not work
            rigid.AddForce(Vector2.right * h*control_lock, ForceMode2D.Impulse);
        }
        
        //limiting the speed to maxSpeed
        if (rigid.velocity.x > maxSpeed)//right
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -1 * maxSpeed)//left
        {
            rigid.velocity = new Vector2(-1 * maxSpeed, rigid.velocity.y);
        }

        //adding wall ray for wall jump
        Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 0, 1, 1));
        Vector2 position_right;
        Vector2 position_left;
        if(spriteRenderer.flipX){
            position_right = rigid.position+Vector2.right*0.455f + Vector2.up*0.5f;
            position_left = rigid.position+Vector2.left*0.395f+ Vector2.up*0.5f;
        }else{
            position_right = rigid.position+Vector2.right*0.395f+ Vector2.up*0.5f;
            position_left = rigid.position+Vector2.left*0.455f+ Vector2.up*0.5f;
        }
        Debug.DrawRay(position_right,Vector2.down*0.6f, new Color(1, 1, 1, 1));
        Debug.DrawRay(position_left,Vector2.down*0.6f, new Color(1, 1, 1, 1));
        RaycastHit2D wallc_right = Physics2D.Raycast(position_right,Vector2.down*0.6f, 0.6f, LayerMask.GetMask("Platform"));
        RaycastHit2D wallc_left = Physics2D.Raycast(position_left,Vector2.down*0.6f, 0.6f, LayerMask.GetMask("Platform"));

        //checking if character is colliding with wall on right/left side
        if(wallc_right.collider != null || wallc_left.collider != null){//colliding
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
            //setting wall jump(reaction force) direction
            if(wallc_right.collider != null){
                wall_jump_dir = -1;
            }else{
                wall_jump_dir = 1;
            }
            is_on_wall = true;
        }else{//not colliding
            is_on_wall = false;
        }
        
        


        //Landing Platform
        if (rigid.velocity.normalized.y <= 0)
        {
            Vector3 dir;//direction ray where player is facing
            if(rigid.velocity.x>0){
                dir = Vector3.down+Vector3.right;
            }else if(rigid.velocity.x<0){
                dir = Vector3.down+Vector3.left;
            }else{
                dir = Vector3.down;
            }

            //used to get the left edge position of the character for raycast
            float tempX;
            if(spriteRenderer.flipX == true){
                tempX = 0.315f;
            }else{
                tempX = 0.375f;
            }

            //raycast on bottom of the character to check if player landed
            Vector2 tempVec = new Vector2(rigid.position.x-tempX, rigid.position.y-0.6f);
            Debug.DrawRay(tempVec, Vector3.right*0.69f, new Color(0, 0, 1));
            RaycastHit2D rayHitDown = Physics2D.Raycast(tempVec, Vector2.right*0.69f, 0.69f, LayerMask.GetMask("Platform"));
            if(rayHitDown.collider != null && !is_on_wall){//is on floor, reset jump count, returning animation to idle
                if(rigid.velocity.y<0){
                    // rigid.gravityScale = 1;//now don't need it since movement issue is solved by increasing movement force vector
                }else{
                    rigid.gravityScale = 3;
                }
                Debug.Log("아와와와왕ㅇ");
                anim.SetBool("is_jumping", false);
                jump_count = 0;
            }else{// is on wall or on air
                    anim.SetBool("is_jumping", true);
                    if(jump_count == 0){//first jump chance is gone if falling
                        jump_count = 1;
                    }
                    
            }
            
        }
         

        
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "enemy"){
            // Collider2D tempa = Physics2D.OverlapCapsule(rigid.position, rigid.position, new CapsuleDirection2D(), 360);
            // Debug.Log("tempa: " + tempa);
            // Debug.Log("현재 위치: " + rigid.position.y);
            // Debug.Log("충돌 위치: " + collision.transform.position.y);
            // Debug.Log("이건 모임 위치: " + transform.position.x);
            //짧은 시간 내에 한번에 두 enemy와 충돌해서 hp가 두개가 연달아 사라지는 버그 있음. 수정해야함

            // int county = 0;
            // foreach (ContactPoint2D contact in collision.contacts)
            // {
            //     county++;
            //     Debug.DrawRay(contact.point, contact.normal, Color.white);
            //     Debug.Log("Name=" + collision.collider.name);
            //     Debug.Log("county: "+county);
            // }
            if((transform.position.y >= collision.transform.position.y+0.5f) && collision.gameObject.layer!=11){
                KillMonster(collision.transform);
            }else{
                // Debug.Log("collided with: " + collision.gameObject.tag);
                //checking that already got hit
                // Debug.Log(collision.contacts[0].point);
                // Debug.Log(rigid.position);
                OnDamaged(collision.contacts[0].point);
                
            }
        }
        else if(collision.gameObject.layer == 8){
            // Debug.Log("floor!");

        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "item"){
            //get point
            // Debug.Log("어서오게 필멸자여");
            bool is_bronze = collision.gameObject.name.Contains("bronze");
            bool is_silver = collision.gameObject.name.Contains("silver");
            bool is_gold = collision.gameObject.name.Contains("gold");
            if(is_bronze){
                gameManager.stagePoint+=100;
            }else if(is_silver){
                gameManager.stagePoint+=200;
            }else if(is_gold){
                gameManager.stagePoint+=500;
            }

            //play audio
            PlaySound("Item");

            //deactivate item
            collision.gameObject.SetActive(false);
        }else if(collision.gameObject.tag == "Finish"){
            //play audio
            PlaySound("Finish");



            // next stage
            gameManager.NextStage();

        }
    }

    void OnDamaged(Vector2 targetPos){
        if(gotHit)
            return;
        
        gotHit = true;
        //getting damage(hp down)
        gameManager.healthDown();

        // change layer
        gameObject.layer = 13;

        //change color
        spriteRenderer.color = new Color(1,1,1, 0.5f);

        //animation
        anim.SetTrigger("doDamaged");

        //넉백
        int dirc = rigid.position.x-targetPos.x > 0 ? 1:-1;
        rigid.AddForce(new Vector2(dirc, 1)*7,ForceMode2D.Impulse);

        //play audio
        PlaySound("Damaged");

        //recursive
        Invoke("OffDamage", 2);
        
    }

    void CheckDamage(Vector2 targetPos){
        if(Physics2D.OverlapCircle(rigid.position, 1)){//if still on collision area. overlap해서 주변에 tag:enemy있으면 ondamaged 한번 더 콜, 아니면 offdamage(여기서 ondamaged안부르면
                                    //콜라이더가 두 enemy사이에 낀 상태로 layer 변환되는 경우 체력이 한번에 여러개 빠짐. 이 overlap 작동 안함 - 고쳐야함
            OnDamaged(targetPos);
        }else{//if now on safe place
            OffDamage();
        }
    }
    void OffDamage(){
        spriteRenderer.color = new Color(1, 1, 1, 1);
        gameObject.layer = 12;
        gotHit = false;
    }

    void KillMonster(Transform enemy){
        // point
        gameManager.stagePoint += 100;
        
        //reaction force
        rigid.AddForce(Vector2.up*5, ForceMode2D.Impulse);
        //check enemy type by layer - total 3 types
        if(enemy.gameObject.layer == 9){
            //enemy die
            MonsterMove enemyMove = enemy.GetComponent<MonsterMove>();
            enemyMove = enemy.GetComponent<MonsterMove>();
            enemyMove.OnDamaged();
        }else if(enemy.gameObject.layer == 10){
            //enemy die
            MonsterMovementType2 enemyMove = enemy.GetComponent<MonsterMovementType2>();
            enemyMove = enemy.GetComponent<MonsterMovementType2>();
            enemyMove.OnDamaged();
        }else if(enemy.gameObject.layer == 14){
            //enemy die
            MonsterMovementType3 enemyMove = enemy.GetComponent<MonsterMovementType3>();
            enemyMove = enemy.GetComponent<MonsterMovementType3>();
            enemyMove.OnDamaged();
        }
        PlaySound("Attack");
        
        
    }

    public void OnDie(){
        //sprite alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //monster filps y
        spriteRenderer.flipY = true;
        //monster falls
        //monster looses collision
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up*5,ForceMode2D.Impulse);
        //call deactivate after 5 sec
        PlaySound("Die");
    }

    public void VelocityZero(){
        rigid.velocity = new Vector2(0,0);
    }

    void FreeLock(){
        control_lock = 1;
    }
}
