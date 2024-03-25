using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //manages score and stage process
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;

    public PlayerMove player;
    public GameObject[] Stages;
    
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject restartButton;
    public GameObject clearMessage;

    public AudioSource audioSource;

    public Text timeText;
    float curTime;
    public bool is_time_over;

    void Start(){
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
    void Update(){
        //point ui
        UIPoint.text = (stagePoint + totalPoint).ToString();
        //timer ui
        curTime += Time.deltaTime;
        timeText.text = string.Format("{0:N2}", 30f - curTime);
        if(30-curTime<=0 && !is_time_over){
            is_time_over = true;
            Time.timeScale = 0;
            health = 1;
            healthDown();
        }
        
    }
    public void NextStage()
    {
        //change stage
        if(stageIndex<Stages.Length-1){
            PlayerReposition();
            curTime = 0;//reset timer
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            //calculate point
            totalPoint = stagePoint;
            stagePoint = 0;
            UIStage.text = "STAGE " + (stageIndex + 1).ToString();
        }else{
            //game clear
            //player control lock
            Time.timeScale = 0;
            //reset UI
            clearMessage.SetActive(true);
            //restart button UI
            restartButton.SetActive(true);

        }
        
    }

    public void healthDown(){
        if(health>1){
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.2f);
        } else {
            //player die effect
            player.OnDie();
            //result ui
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.2f);
            // Debug.Log("으앙 쥬금!");
            //retry button ui
            restartButton.SetActive(true);
            Text buttonText = restartButton.GetComponentInChildren<Text>();
            
        }
        
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player"){
            //player reposition
            PlayerReposition();
            
            //health down
            healthDown();
        }
        
    }

    void PlayerReposition(){
        if(health>0){
            player.VelocityZero();
            player.transform.position = new Vector2(-4.33f, 1.79f);
        }
    }

    public void Restart(){
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
