using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*place this script on any object that is interactable. In the inspector, you can add the dialogue asscoiated with the
object by adjusting the size, and inputting your desired text

Note: for this script to work, there cannot be multiple objects within the maxDistance from player.
*/
public class InteractableObj : MonoBehaviour
{
    #region VARIABLES
    //references to gamemanger and player
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject player;

    [SerializeField] float maxDistance = 2.2f; //maximum distance to player to interact with object

    //object's dialogue
    [SerializeField] List <string> myDialogue = new List<string>();
    public bool isInteracting = false; //boolean holding the condition of whether the object can be interacted with
    #endregion

    void Update()
    {
        checkDistance();
    }

    //checks the distance from this object to the player, and if they are within distance, checks for player input for interaction
    void checkDistance(){
        if (Vector3.Distance(transform.position, player.transform.position) <= maxDistance){
            Debug.Log("Player is near me: " + this.name);
            //if the object is not being interacted with, and the player presses space, interact with the object
            if(!isInteracting && Input.GetKeyDown(KeyCode.Return)){
                gameManager.dialogue = myDialogue; //add this object's dialogue to the gamemanager's dialogue
                gameManager.currentObj = this; //set the gamemanager's obj script to this obj's
                isInteracting = true; //player is now interacting with this object
                gameManager.displayDialogue(); //call gameManager's displayDialogue function
            }
        }

    }
}
