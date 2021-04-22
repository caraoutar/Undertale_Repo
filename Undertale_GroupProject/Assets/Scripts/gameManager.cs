using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script manages the game dialogue and UI; the current object will automatically change (there is no need to adjust it through the editor)

//test line

public class gameManager : MonoBehaviour
{
    #region VARIABLES
    //refernces to cameras in the main room, the bedroom, and the combat "room"
    [Tooltip("These are the camera in the scene")]
    [Header("Cameras")]
    [SerializeField] Camera mainCam; //this is camera 0
    [SerializeField] Camera bedroomCam; //camera 1
    [SerializeField] Camera combatCam; //camera 2

    [Tooltip("Reference to the player script")]
    [Header("Player Script")]
    [SerializeField] playerMovement player; //reference to player's script

    [Tooltip("These variables are for debugging")]
    [Header("Non-Adjustable Dialogue Variables")]
    [SerializeField] int index; //which line is being typed
    [SerializeField] int maxIndex; //the maximum number of lines (list size)
    [SerializeField] bool isTyping = false; //condition for whether text is curerntly being typed
    [SerializeField] bool isTypingChoice = false;
    [SerializeField] bool openText = false; //condition for whether the textbox is currently open
    [SerializeField] bool typedChoices = false;

    [Tooltip("Adjust the typing effect speed; change fonts")]
    [Header("Adjustable Dialogue Variables")]
    [SerializeField] int letterPerSec; //how fast the typing is
    [SerializeField] Font papyrusFont;
    [SerializeField] Font narrativeFont;

    [Tooltip("object dialogue will show up here (please adjust dialogue on the interactable object)")]
    [Header("Dialogue Text")] //dialgoue variables
    public List <string> dialogue = new List<string>(); //the dialogue that will be typed
    
    [Tooltip("choice will show up here if different from default yes/no")]
    [Header("Choice Text")]
    public string[] choice = new string[2]; //choice dialogue to be added
    public bool hasChoice;
    [SerializeField] string[] defaultChoice = {"yes", "no"};

    [Tooltip("this is the object currently being interacted with")]
    [Header("Object in Interaction")]
    public interactableObj currentObj; //reference to the current object being interacted with; this will automatically change

    [Tooltip("reference to canvas; there is no need to add the other references")]
    [Header("Dialogue Objects")] //references to the dialogue objects/components
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject dialogueTextBox;
    [SerializeField] Text dialogueText;
    [SerializeField] Text choice1;
    [SerializeField] Text choice2;
    [SerializeField] Image heart1;
    [SerializeField] Image heart2;

    [Tooltip("adjust the maximum time before the player can interact with a new object")]
    [Header("Countdown Variables")]
    //countdown timer code
    [SerializeField] float maxTime = 0.5f;
    [SerializeField] float timeRemaining;
    [SerializeField] bool runTimer = false;
    #endregion

    public void Start(){
        //on start, set up the variables for UI
        player = (playerMovement) GameObject.FindWithTag("Player").GetComponent(typeof(playerMovement));
        dialogueTextBox = canvas.transform.GetChild(0).gameObject;
        dialogueText = dialogueTextBox.transform.GetChild(0).GetComponent<Text>();
        choice1 = dialogueTextBox.transform.GetChild(1).GetComponent<Text>();
        choice2 = dialogueTextBox.transform.GetChild(2).GetComponent<Text>();
        heart1 = dialogueTextBox.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        heart2 = dialogueTextBox.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        // dialogueTextBox.SetActive(false);

        //find all interactable objects and set their depth
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject o in interactables){
            o.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -(int)Mathf.Ceil(o.transform.position.y);
        }
    }

    //updates the dialogue based on player input, and closes dialogue
    //also updates camera
    public void Update(){
        //checking camera swap
        if(Input.GetKeyDown(KeyCode.B)){
            swapCamera(0);
        }

        //change the heart image based on what the player wants to select (select with left/right arrow and enter)
        if(typedChoices && Input.GetKeyDown(KeyCode.RightArrow)){
            heart1.enabled = false;
            heart2.enabled = true;
        }
        else if(typedChoices && Input.GetKeyDown(KeyCode.LeftArrow)){
            heart2.enabled = false;
            heart1.enabled = true;
        }

        //code for when the player wants to make a selection
        if(Input.GetKeyDown(KeyCode.Return) && typedChoices){
            if(heart1.enabled){
                Debug.Log("Choice 1");
            }else if(heart2.enabled){
                Debug.Log("Choice 2");
            }
            timeRemaining = maxTime; //begin countdown to allow player to interact again
            runTimer = true;
            closeDialogue();
        }
        
        //change dialogue if the player presses enter key
        if(Input.GetKeyDown(KeyCode.Return) && openText){
            if(isTyping && !isTypingChoice){
                dialogueText.text=dialogue[index];
                isTyping = false;
            }
            else if(index < maxIndex){
                ++index;
                if(index < maxIndex){
                    StartCoroutine(typeDialogue(dialogue[index]));
                }
            }

            if(index >= maxIndex){ //if the index is greater than or equal to the size of the list, then close the dialogue
                if(hasChoice && maxIndex>=1){
                    // enableChoice();
                    if (!isTypingChoice) displayChoice();
                }
                else{
                    timeRemaining = maxTime; //begin countdown to allow player to interact again
                    runTimer = true;
                    closeDialogue();
                }
                    
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


    // ======================================     DIALOGUE METHODS     ==============================================
    //displays dialogue
    public IEnumerator displayDialogue(){
        yield return new WaitForEndOfFrame();
        maxIndex = dialogue.Count; //set max index to the list size
        dialogueTextBox.SetActive(true); //actives the dialgoue box
        player.canMove = false; //the player cannot move when interacting with objects -> adjusts the condition in player script
        StartCoroutine(typeDialogue(dialogue[index])); //begin typing the text
        openText = true; //condition: dialogue is now open
    }
    
    //displays dialogue with choice yes/no
    public void displayChoice(){
        isTypingChoice = true;
        enableChoice();
        StartCoroutine(typeDialogue(choice[0],choice[1])); //begin typing the choices
    }

    //method to type dialogue
    public IEnumerator typeDialogue(string str){
        //change the font of the dialogue if there is an astericks
        if(str.Contains("*")) dialogueText.font = narrativeFont;
        else dialogueText.font = papyrusFont;
       
        dialogueText.text="";
        isTyping = true;
        foreach(var letter in str.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            if(isTyping){
                dialogueText.text +=letter;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;
    }

    //overloadded method to type choices
    public IEnumerator typeDialogue(string c1, string c2){
        Debug.Log("Typing choices");
        isTyping = true;
        choice1.text="";
        choice2.text="";
        foreach(var letter in c1.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            if(isTyping && isTypingChoice){
                choice1.text +=letter;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        foreach(var letter in c2.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            if(isTyping && isTypingChoice && (choice1.text == c1)){
                choice2.text +=letter;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;
        typedChoices = true;
    }

    //enable the choice gameobjects
    void enableChoice(){
        choice1.enabled = true;
        choice2.enabled = true;
        heart2.enabled = false;
        heart1.enabled = true;
    }

    void disableChoice(){ //reset choice system
        choice1.enabled = false;
        choice2.enabled = false;
        heart2.enabled = false;
        heart1.enabled = false;
        
        choice = defaultChoice;
        
        isTypingChoice = false;
        hasChoice = false;
        typedChoices = false;
    }

    //closes the dialogue
    public void closeDialogue(){
        openText = false; //condition: dialogue is no longer open
        index = 0; //reset the index
        //this if condition is not used right now
        if(isTyping){ //in the case that the text is still being typed. (for if the player can leave dialogue without finishing it)
            dialogueText.text="";
            isTyping = false;
        }
        //close the dialogue box
        dialogueTextBox.SetActive(false);
        player.canMove = true; //let the player move again
        
        disableChoice();
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
