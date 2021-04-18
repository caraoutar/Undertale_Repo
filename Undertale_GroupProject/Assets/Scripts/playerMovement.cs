using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    #region VARIABLES
    //reference to gamemanager
    // [SerializeField] GameManager gameManager;

    //player variables
    Animator myAnim;
    SpriteRenderer myRenderer;

    //movement variables
    float moveDirX;
    float moveDirY;
    [SerializeField] float speed;
    [SerializeField] float speedLim = 0.7f; //to limit horizontal speed
    public bool canMove = true;

    #endregion
   
    void Start()
    {
        myAnim = gameObject.GetComponent<Animator>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        move();
    }

    //movement function
    void move(){
        //reset animation -> for when there is animation
        resetAnim();

        //get the directional movement of the player
        moveDirX = Input.GetAxis("Horizontal");
        moveDirY = Input.GetAxis("Vertical");

        //if the player is moving right
        if(moveDirX > 0){
            Debug.Log("moving left");
            myAnim.SetBool("walkingSide",true);
            myRenderer.flipX = true;
        }
        //if player is moving left
        else if(moveDirX < 0){
            Debug.Log("moving right");
            myAnim.SetBool("walkingSide",true);
            myRenderer.flipX = false;
        }

        if(moveDirY > 0 && moveDirX == 0){
            Debug.Log("moving up");
            myAnim.SetBool("walkingUp",true);
        }
        else if(moveDirY < 0 && moveDirX == 0){
            Debug.Log("moving down");
            myAnim.SetBool("walkingDown",true);
        }

        //limit horizontal speed
        if (moveDirX !=0 && moveDirY !=0){
            moveDirX *= speedLim;
            moveDirY *= speedLim;
        }

        //movement
        if(canMove){
            transform.Translate(Vector3.up*moveDirY*Time.deltaTime*speed);
            transform.Translate(Vector3.right*moveDirX*Time.deltaTime*speed);
        }
        
    }

        //reset the animation
    void resetAnim(){
        Debug.Log("resetting animations");
        myAnim.SetBool("walkingSide",false);
        myAnim.SetBool("walkingUp",false);
        myAnim.SetBool("walkingDown",false);
    }
}
