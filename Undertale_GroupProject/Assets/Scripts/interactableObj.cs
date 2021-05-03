using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*place this script on any object that is interactable. In the inspector, you can add the dialogue asscoiated with the
object by adjusting the size, and inputting your desired text

Note: for this script to work, there cannot be multiple objects within the maxDistance from player.
*/
public class interactableObj : MonoBehaviour
{
    #region VARIABLES
    //references to gamemanger and player
    [Header("System References")]
    [SerializeField] gameManager gameManager;
    [SerializeField] GameObject player;

    [Tooltip("add dialogue by changing the size, and then adding the dialogue to the new boxes")]
    [Header("Dialogue Text")]
    public List <string> myDialogue = new List<string>(); //object's dialogue
    
    [Tooltip("add choice dialogue here if you want to change the default yes/no")]
    [Header("Choice Text")]
    public string[] myChoice = new string[2]; //object's choices
    public bool hasChoice; //check if the object gives a choice
    public bool changeChoice; //check if the choice is different from yes/no

    [Tooltip("add dialogue here if there is more dialogue after a choice")]
    [Header("More Dialogue Text")]
    public List <string> myNextDialogue = new List<string>(); //object's dialogue

    [Tooltip("adjust maxDistance to change the distance the player has to be within to interact with this object")]
    [Header("Object Variables")]
    public bool isInteracting = false; //boolean holding the condition of whether the object can be interacted with
    // [SerializeField] float maxDistance = 2.2f; //maximum distance to player to interact with object
    // [SerializeField] Vector2 offset;
    // [SerializeField] float distToPlayer;
    #endregion

    void Start(){ //auto add reference to gameManager and player
        gameManager = (gameManager) GameObject.FindWithTag("GameController").GetComponent(typeof(gameManager));
        player = GameObject.FindWithTag("Player");
    }

    // void Update(){ //call checkdistance
    //     if(gameObject.tag != "Combat"){
    //         checkDistance();
    //     }
    // }

    //checks the distance from this object to the player, and if they are within distance, checks for player input for interaction
    /*void checkDistance(){
        offset = gameObject.transform.position - player.transform.position;
        distToPlayer = offset.sqrMagnitude;

        // if(this.name == "computer" || this.name == "bedroom_papyrus" || this.name == "box_of_bones"){
        //     Debug.Log(this.name + " distance to player: " + distToPlayer);
        // }

        if(gameManager.closestObj.name == this.name){
            // gameManager.distToClosestObj = distToPlayer;
            //if the object is not being interacted with, and the player presses space, interact with the object
            if(distToPlayer <= maxDistance && Input.GetKeyDown(KeyCode.Return)){
                if(!isInteracting){
                    gameManager.dialogue = myDialogue; //add this object's dialogue to the gamemanager's dialogue
                
                    gameManager.currentObj = this; //set the gamemanager's obj script to this obj's
                    isInteracting = true; //player is now interacting with this object
                    StartCoroutine(gameManager.displayDialogue()); //call gameManager's displayDialogue function
                    
                    if(hasChoice){ //if the object gives the player a choice, point the choice array in gameManager to this object's array
                        if (changeChoice) gameManager.choice = myChoice;
                        gameManager.hasChoice = true;
                    }
                }
            }
        }
        else if (gameManager.distToClosestObj > distToPlayer){
            Debug.Log("Player is near me: " + this.name);
            gameManager.closestObj = this.gameObject;
            gameManager.distToClosestObj = distToPlayer;
        }

    }*/
}
