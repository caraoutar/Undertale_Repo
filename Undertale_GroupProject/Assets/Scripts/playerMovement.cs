using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    #region VARIABLES
    //reference to gamemanager
    [SerializeField] gameManager gameManager;
    [SerializeField] GameObject tester;

    //player variables
    Animator myAnim;
    SpriteRenderer myRenderer;
    Rigidbody2D rb2d;

    //movement variables
    Vector3 movement;
    [SerializeField] float moveDirX;
    [SerializeField] float moveDirY;
    [SerializeField] float speed;
    // [SerializeField] float speedLim = 0.7f; //to limit horizontal speed
    public bool canMove = true;

    #endregion
   
    void Start()
    {
        gameManager = (gameManager) GameObject.FindWithTag("GameController").GetComponent(typeof(gameManager));
        canMove = true;
        myAnim = gameObject.GetComponent<Animator>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Debug.Log(tester.n  + " distance to player: " + Vector3.Distance(tester.GetComponent<Renderer>().bounds.center, gameObject.transform.position));
        movement = Vector3.zero;
        // if (movement != Vector3.zero){
            move();
        // }
        if(transform.position.y < 0) gameManager.uiAtTop = true;
        else gameManager.uiAtTop = false;
    }

    //movement function
    void move(){
        //reset animation -> for when there is animation
        resetAnim();

        //get the directional movement of the player
        moveDirX = Input.GetAxisRaw("Horizontal");
        moveDirY = Input.GetAxisRaw("Vertical");

        if(canMove){ //if the player can move (the player cannot move when interacting with objects)
        //if the player is moving right
            if(moveDirX > 0){
                moveDirY = 0;
                // Debug.Log("moving left");
                myAnim.SetBool("walkingSide",true);
                myRenderer.flipX = true;
            }
            //if player is moving left
            else if(moveDirX < 0){
                moveDirY = 0;
                // Debug.Log("moving right");
                myAnim.SetBool("walkingSide",true);
                myRenderer.flipX = false;
            }

            if(moveDirY > 0){
                moveDirX = 0;
                // Debug.Log("moving up");
                myAnim.SetBool("walkingUp",true);
            }
            else if(moveDirY < 0){
                moveDirX = 0;
                // Debug.Log("moving down");
                myAnim.SetBool("walkingDown",true);
            }

            //limit horizontal speed
            // if (moveDirX !=0 && moveDirY !=0){
            //     moveDirX *= speedLim;
            //     moveDirY *= speedLim;
            // }
            
            movement.x = moveDirX;
            movement.y = moveDirY;
            if ((moveDirX !=0 && moveDirY == 0) || (moveDirX ==0 && moveDirY !=0)){
                //movement
                rb2d.MovePosition(transform.position + movement * speed* Time.deltaTime);
                // transform.Translate(Vector3.up*moveDirY*Time.deltaTime*speed);
                // transform.Translate(Vector3.right*moveDirX*Time.deltaTime*speed);
            }
        }
        
    }

        //reset the animation
    void resetAnim(){
        // Debug.Log("resetting animations");
        myAnim.SetBool("walkingSide",false);
        myAnim.SetBool("walkingUp",false);
        myAnim.SetBool("walkingDown",false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name == "LeaveBedroom") gameManager.swapCamera(0);
    }
}
