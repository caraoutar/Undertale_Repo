using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script manages the game dialogue and UI; the current object will automatically change (there is no need to adjust it through the editor)

public class gameManager : MonoBehaviour
{
    #region VARIABLES
    //refernces to cameras in the main room, the bedroom, and the combat "room"
    [SerializeField] Camera mainCam; //this is camera 0
    [SerializeField] Camera bedroomCam; //camera 1
    [SerializeField] Camera combatCam; //camera 2

    [SerializeField] playerMovement player; //reference to player's script
    
    //dialgoue variables
    public List <string> dialogue = new List<string>(); //the dialogue that will be typed
    public interactableObj currentObj; //reference to the current object being interacted with; this will automatically change

    [SerializeField] int letterPerSec; //how fast the typing is
    [SerializeField] int index; //which line is being typed
    [SerializeField] int maxIndex; //the maximum number of lines (list size)

    bool isTyping = false; //condition for whether text is curerntly being typed
    public bool openText = false; //condition for whether the textbox is currently open

    //reference to the textbox and text
    public GameObject dialogueTextBox;
    public Text dialogueText;

    //countdown timer code
    [SerializeField] float maxTime = 0.5f;
    [SerializeField] float timeRemaining;
    bool runTimer = false;
    #endregion

    //updates the dialogue based on player input, and closes dialogue
    //also updates camera
    public void Update(){
        //checking camera swap
        if(Input.GetKeyDown(KeyCode.B)){
            swapCamera(1);
        }

        //change dialogue if the player presses enter key
        if(Input.GetKeyDown(KeyCode.Return) && openText){
                if(isTyping){
                    dialogueText.text=dialogue[index];
                    isTyping = false;
                }
                else{
                    ++index;
                    if(index < maxIndex){
                        StartCoroutine(typeDialogue(dialogue[index]));
                    }
                }
                if(index >= maxIndex){ //if the index is greater than or equal to the size of the list, then close the dialogue
                         closeDialogue();
                    }
            }


        //timer code (counts down from timeRemaining); this is to limit how long after the player interacts with an object can they interact again
        //see maxTime in the inspector for current time limit
        if (runTimer){
            if(timeRemaining > 0){
                timeRemaining -= Time.deltaTime;
            }
            else{
                timeRemaining = 0;
                if(currentObj != null){
                    currentObj.isInteracting = false;
                    currentObj = null;
                }
                runTimer = false;
            }

        }
    }

    //displays dialogue
    public void displayDialogue(){
        maxIndex = dialogue.Count; //set max index to the list size
        dialogueTextBox.SetActive(true); //actives the dialgoue box
        player.canMove = false; //the player cannot move when interacting with objects -> adjusts the condition in player script

        StartCoroutine(typeDialogue(dialogue[index])); //begin typing the text
        openText = true; //condition: dialogue is now open
        
        //ignore this. we may need this later for edge cases
        // yield return new WaitForEndOfFrame();
    }

    //method to type dialogue
    public IEnumerator typeDialogue(string str){
        isTyping = true;
        dialogueText.text="";
        foreach(var letter in str.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            if(isTyping){
                dialogueText.text +=letter;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;
    }

    //closes the dialogue
    public void closeDialogue(){
        //this if condition is not used right now
        if(isTyping){ //in the case that the text is still being typed. (for if the player can leave dialogue without finishing it)
            dialogueText.text="";
            isTyping = false;
        }
        //close the dialogue box
        dialogueTextBox.SetActive(false);
        index = 0; //reset the index
        openText = false; //condition: dialogue is no longer open
        player.canMove = true; //let the player move again

        timeRemaining = maxTime; //begin countdown to allow player to interact again
        runTimer = true;
    }


    //displays dialogue with choice yes/no
    public void displayChoice(){

    }

    //overloaded function that displays dialogue with choice that is not yes/no
    public void displayChoice( string c1, string c2){

    }

    //swap the cameras (moving to different rooms)
    public void swapCamera(int camNum){
        //disable all cameras
        mainCam.enabled = false;
        bedroomCam.enabled = false;
        combatCam.enabled = false;

        //enable the necessary camera
        switch(camNum){
            case 0: 
                mainCam.enabled = true;
                
            break;
            case 1: 
                Debug.Log("should swap cam");
                bedroomCam.enabled = true;
            break;
            case 2: 
                combatCam.enabled = true;
            break;
        }

    }
}
