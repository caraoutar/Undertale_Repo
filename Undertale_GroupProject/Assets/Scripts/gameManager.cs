using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script manages the game dialogue and UI; the current object will automatically change (there is no need to adjust it through the editor)

//test line

public class gameManager : MonoBehaviour
{
    //================================== VARIABLES ==========================================
    #region VARIABLES
    //refernces to cameras in the main room, the bedroom, and the combat "room"
    [Tooltip("These are the camera in the scene")]
    [Header("Cameras")]
    [SerializeField] Camera mainCam; //this is camera 0
    [SerializeField] Camera bedroomCam; //camera 1
    [SerializeField] Camera combatCam; //camera 2

    [Tooltip("Reference to the player script & bedroom spawn point")]
    [Header("Player Script")]
    [SerializeField] GameObject p; //player object
    [SerializeField] playerMovement player; //reference to player's script
    [SerializeField] GameObject playerSpawn;
    [SerializeField] GameObject bedroomSpawn;
    [SerializeField] GameObject mainSpawn;


    [Tooltip("Adjust the typing effect speed; change fonts; add SFX")]
    [Header("Adjustable Dialogue Variables")]
    [SerializeField] int letterPerSec; //how fast the typing is
    [SerializeField] Font papyrusFont;
    [SerializeField] Font narrativeFont;

    [Tooltip("These variables are for debugging")]
    [Header("Non-Adjustable Dialogue Variables")]
    [SerializeField] int index; //which line is being typed
    [SerializeField] int maxIndex; //the maximum number of lines (list size)
    [SerializeField] bool isTyping = false; //condition for whether text is curerntly being typed
    [SerializeField] bool isTypingChoice = false;
    [SerializeField] bool openText = false; //condition for whether the textbox is currently open
    [SerializeField] bool typedChoices = false;

    // DIALOGUE SFX
    [Header("Audio")]
    [SerializeField] public AudioSource TXTsfx; // sfx for the typing
    [SerializeField] public AudioSource arrowMOVEsfx; // sfx when using the left/right arrow keys during CHOICES
    [SerializeField] public AudioSource SELECTsfx; // sfx when player presses ENTER

    //setting Game Objects for background music
    [Header("Background Music")]
    [SerializeField] public GameObject sans_music; //game object for house music
    [SerializeField] public GameObject date_start_music; //game object for date start music
    [SerializeField] public GameObject date_fight_music; //game object for date fight music

    [Tooltip("object dialogue will show up here (please adjust dialogue on the interactable object)")]
    [Header("Dialogue Text")] //dialgoue variables
    public List <string> dialogue = new List<string>(); //the dialogue that will be typed
    
    [Tooltip("choice will show up here if different from default yes/no")]
    [Header("Choice Text")]
    public string[] choice = new string[2]; //choice dialogue to be added
    public bool hasChoice;
    [SerializeField] string[] defaultChoice = {"YES", "NO"};

    [Tooltip("reference to canvas; there is no need to add the other references")]
    [Header("Main UI Objects")] //references to the dialogue objects/components
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject dialogueTextBox;
    [SerializeField] Text dialogueText;
    [SerializeField] Text choice1;
    [SerializeField] Text choice2;
    [SerializeField] Image heart1;
    [SerializeField] Image heart2;
    [SerializeField] Image papyrusHead;

    public bool uiAtTop = false;
    [SerializeField] Vector2 textStartingPosition;
    [SerializeField] GameObject dialogueTextObject;
    [SerializeField] Vector2 dialogueStartingPosition;
    [SerializeField] Vector2 dialogueSize;
    [SerializeField] float sizeOfPapyrusText = 0.8f;
    [SerializeField] float sizeOfPapyrusBox = 0.38f;


    [Header("Combat UI Objects")]
    [SerializeField] GameObject combatCanvas;
    [SerializeField] GameObject papyrusTextBox;
    [SerializeField] Text papyrusText;
    [SerializeField] GameObject whiteScreen;
    [SerializeField] Text whiteScreenText;
    [SerializeField] GameObject datingHUD;
    [SerializeField] GameObject datingPower;
    [SerializeField] GameObject datingTension;

    [Tooltip("this is the object currently being interacted with")]
    [Header("Object in Interaction")]
    public float distToClosestObj;
    public GameObject closestObj;
    public interactableObj currentObj; //reference to the current object being interacted with; this will automatically change

    [Tooltip("add different empty gameobjects for each section of the combat")]
    [Header("Combat Variables")]
    [SerializeField] List <GameObject> combat;
    int combatSize;
    [SerializeField] int currentSeq = -1;
    [SerializeField] GameObject inspector;
    [SerializeField] bool canRunMiniGame;
    int speed = 1;
    [SerializeField] bool givingPresent;
    [SerializeField] bool foundPresent;
    [SerializeField] bool canPressEnter = true;


    [Tooltip("adjust the maximum time before the player can interact with a new object")]
    [Header("Countdown Variables")]
    //countdown timer code
    [SerializeField] float maxTime = 0.5f;
    [SerializeField] float timeRemaining;
    [SerializeField] bool runTimer = false;
    #endregion


    //===================================== START & UPDATE ===========================================
    public void Start(){
        //on start, set up the variables for UI
        p = GameObject.FindWithTag("Player");
        player = (playerMovement) p.GetComponent(typeof(playerMovement));
        playerSpawn = GameObject.FindWithTag("Player");

        closestObj = GameObject.FindWithTag("Interactable");
        dialogueTextBox = canvas.transform.GetChild(0).gameObject;
        textStartingPosition = dialogueTextBox.GetComponent<RectTransform>().anchoredPosition;
        
        dialogueTextObject = dialogueTextBox.transform.GetChild(0).gameObject;
        dialogueStartingPosition = dialogueTextObject.GetComponent<RectTransform>().anchoredPosition;
        dialogueSize = dialogueTextObject.GetComponent<RectTransform>().sizeDelta;

        dialogueText = dialogueTextBox.transform.GetChild(0).GetComponent<Text>();
        choice1 = dialogueTextBox.transform.GetChild(1).GetComponent<Text>();
        choice2 = dialogueTextBox.transform.GetChild(2).GetComponent<Text>();
        heart1 = dialogueTextBox.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        heart2 = dialogueTextBox.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        papyrusHead = dialogueTextObject.transform.GetChild(0).GetComponent<Image>();

        papyrusTextBox = combatCanvas.transform.GetChild(0).gameObject;
        papyrusText = papyrusTextBox.transform.GetChild(0).GetComponent<Text>();
        datingHUD = combatCanvas.transform.GetChild(1).gameObject;
        datingPower = combatCanvas.transform.GetChild(2).gameObject;
        datingTension = combatCanvas.transform.GetChild(3).gameObject;

        whiteScreen = combatCanvas.transform.GetChild(4).gameObject;
        whiteScreenText = whiteScreen.transform.GetChild(0).GetComponent<Text>();

        combatSize = combat.Count;

        // DIALOGUE SFX
            // assigning the sound !
        TXTsfx = TXTsfx.GetComponent<AudioSource>(); 
        arrowMOVEsfx = arrowMOVEsfx.GetComponent<AudioSource>(); 
        SELECTsfx = SELECTsfx.GetComponent<AudioSource>(); 
    }

    //updates the dialogue based on player input, and closes dialogue
    void Update(){
        if(canRunMiniGame){
            Debug.Log("Running mini game");
            runMiniGame();
        }
        
        //change the heart image based on what the player wants to select (select with left/right arrow and enter)
        if(typedChoices && Input.GetKeyDown(KeyCode.RightArrow)){
            heart1.enabled = false;
            heart2.enabled = true;

            // DIALOGUE SFX
            arrowMOVEsfx.Play(); // plays the sound when the player selects with right arrow key
        }
        else if(typedChoices && Input.GetKeyDown(KeyCode.LeftArrow)){
            heart2.enabled = false;
            heart1.enabled = true;

            // DIALOGUE SFX
            arrowMOVEsfx.Play(); // plays the sound when the player selects with left arrow key
        }

        //code for when the player wants to make a selection
        if(Input.GetKeyDown(KeyCode.Return) && typedChoices){
            if(heart1.enabled){
                Debug.Log("Choice 1");
                checkChoice1(currentObj.name);

                SELECTsfx.Play(); // plays the sound when the player presses ENTER
            }else if(heart2.enabled){
                Debug.Log("Choice 2");
                checkChoice2();
                
                // DIALOGUE SFX
                SELECTsfx.Play(); // plays the sound when the player presses ENTER
            }
        }
        //change dialogue if the player presses enter key
        else if(Input.GetKeyDown(KeyCode.Return) && openText && canPressEnter){
            if(isTyping && !isTypingChoice){
                dialogueText.text=dialogue[index];
                
                if(!combatCam.enabled){ //if the game isn't in the combat scene, then run this code
                    resetDialogue();
                    if(dialogueText.text.Contains("*")) dialogueText.font = narrativeFont;
                    else{
                        setPapyrusDialogue();
                        dialogueText.text = dialogueText.text.ToUpper();
                    }
                }

                isTyping = false;
            }
            else if(index < maxIndex){
                ++index;
                if(index < maxIndex){
                    StartCoroutine(typeDialogue(dialogue[index]));
                }
            }

            if(index >= maxIndex){ //if the index is greater than or equal to the size of the list, then close the dialogue
                if(!hasChoice){
                    if(combatCam.enabled){
                        if(currentSeq == -1){
                            dialogueText.text = "";
                            dialogue.Clear();
                            runCombat(++currentSeq);
                        }
                        else if(canRunMiniGame){
                            papyrusTextBox.SetActive(false);
                        }
                        else if(foundPresent && !givingPresent){
                            givingPresent = true;
                            index = 1;
                            runCombat(currentSeq);
                        }
                        else checkEndOfSeq(currentSeq);
                    }
                    else{
                        timeRemaining = maxTime; //begin countdown to allow player to interact again
                        runTimer = true;
                        closeDialogue();
                    }
                }
                    
            }
        }


        //timer code (counts down from timeRemaining);
        //see maxTime in the inspector for current time limit
        if (runTimer){
            if(timeRemaining > 0){
                timeRemaining -= Time.deltaTime;
            }
            else{
                timeRemaining = 0;
                if(combatCam.enabled){
                    if(currentSeq == 0){
                        datingHUD.SetActive(true);
                        //or run an animation of papyrus looking around and then on stop, runCombat
                        runCombat(++currentSeq);
                    }
                }else{//this is to limit how long after the player interacts with an object can they interact again
                    if(currentObj != null){
                        currentObj.isInteracting = false;
                        currentObj = null;
                    }
                }
                runTimer = false;
            }
        }


        if(Input.GetKeyDown(KeyCode.C) && combatCam.enabled && currentSeq == 0){
            //start animation for dating HUD
            datingHUD.SetActive(true);
            runTimer = false;
            runCombat(++currentSeq);
            // checkEndOfSeq(currentSeq);
        }
        
    }

   
    //=======================================   CHOICE CHECK & TYPING METHODS   ==========================================
    #region choice
    void checkChoice1(string name){
        index = 0;
        disableChoice();
        switch(name){
            case "book":
                dialogue = ((interactableObj)currentObj.GetComponent(typeof(interactableObj))).myNextDialogue;
                StartCoroutine(displayDialogue());
            break;

            case "bedroom_door":
                swapCamera(1);
                timeRemaining = maxTime; //begin countdown to allow player to interact again
                runTimer = true;
                closeDialogue();
            break;

            case "closet":
                dialogue = ((interactableObj)currentObj.GetComponent(typeof(interactableObj))).myNextDialogue;
                StartCoroutine(displayDialogue());
            break;

            case "bedroom_papyrus":
                timeRemaining = maxTime; //begin countdown to allow player to interact again
                runTimer = true;
                closeDialogue();
                dialogue.Clear();
                dialogue.Add("*Dating... start!");
                swapCamera(2);
                StartCoroutine(displayDialogue());
            break;

            //combat case
            case "seq11":
            currentSeq+=2;
            runCombat(currentSeq);
            break;
            default:
                if(combatCam.enabled) currentSeq++;
                runCombat(currentSeq); //run the next sequence
            break;
        } 
    }

    void checkChoice2(){
        if(combatCam.enabled){ //combat scene
        if (currentSeq==12) currentSeq++;
            currentSeq+=2;
            runCombat(currentSeq); //run the next sequence
        }
        else{
            timeRemaining = maxTime; //begin countdown to allow player to interact again
            runTimer = true;
            closeDialogue();
        }
    }

    //displays dialogue with choice yes/no
    public void displayChoice(){
        isTypingChoice = true;
        enableChoice();
        StartCoroutine(typeDialogue(choice[0],choice[1])); //begin typing the choices
    }
    //overloadded method to type choices
    public IEnumerator typeDialogue(string c1, string c2){
        dialogueText = dialogueTextBox.transform.GetChild(0).GetComponent<Text>();
        //maria dialogue test
        TXTsfx.Play(); // plays the typing SFX ,

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

        TXTsfx.Stop(); // stops playing the typing SFX after typing is complete !!
    }

    //enable the choice gameobjects
    void enableChoice(){
        choice1.enabled = true; choice2.enabled = true;
        heart2.enabled = false; heart1.enabled = true;
    }

    void disableChoice(){ //reset choice system
        choice1.enabled = false; choice2.enabled = false;
        heart2.enabled = false; heart1.enabled = false;
        
        choice = defaultChoice;
        
        isTypingChoice = false;
        hasChoice = false;
        typedChoices = false;
    }
    #endregion
   
   
   
    // ======================================   DIALOGUE (NON CHOICE) METHODS   ==============================================
    #region dialogue
    //displays dialogue
    public IEnumerator displayDialogue(){
        yield return new WaitForEndOfFrame();
        maxIndex = dialogue.Count; //set max index to the list size
       
        //move ui to the top or bottom
        if (uiAtTop) dialogueTextBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(textStartingPosition.x, canvas.GetComponent<RectTransform>().rect.height - dialogueTextBox.GetComponent<RectTransform>().rect.height -20);
        else dialogueTextBox.GetComponent<RectTransform>().anchoredPosition = textStartingPosition;
        
        if(p != null){
            dialogueTextBox.SetActive(true); //actives the dialgoue box
            player.canMove = false; //the player cannot move when interacting with objects -> adjusts the condition in player script
        }
        StartCoroutine(typeDialogue(dialogue[index])); //begin typing the text
        openText = true; //condition: dialogue is now open
    }

    void setPapyrusDialogue(){
        dialogueTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeOfPapyrusText*dialogueTextObject.GetComponent<RectTransform>().rect.width, dialogueSize.y);
        dialogueTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector3((dialogueTextBox.GetComponent<RectTransform>().rect.width) - sizeOfPapyrusBox*dialogueTextBox.GetComponent<RectTransform>().rect.width, dialogueStartingPosition.y);
        dialogueText.font = papyrusFont;
        papyrusHead.enabled = true;
    }
    void resetDialogue(){
        papyrusHead.enabled = false;
        dialogueTextObject.GetComponent<RectTransform>().sizeDelta = dialogueSize;
        dialogueTextObject.GetComponent<RectTransform>().anchoredPosition = dialogueStartingPosition;
    }

    //method to type dialogue
    public IEnumerator typeDialogue(string str){
        // DIALOGUE SFX
        TXTsfx.Play(); // plays the typing SFX ,
        
        if(!combatCam.enabled){ //if the game isn't in the combat scene, then run this code
            resetDialogue();
            //change the font of the dialogue if there is an astericks
            if(str.Contains("*")) dialogueText.font = narrativeFont;
            else{
                setPapyrusDialogue();
                str = str.ToUpper();
            }
        }
        else{ //if we're in combat then set the dialogue text to the approriate box depending on who is talking
            if(str.Contains("*")){
                dialogueText.font = narrativeFont;
                dialogueText = dialogueTextBox.transform.GetChild(0).GetComponent<Text>();
            }
            else{ 
                if( currentSeq == 4 && str == "") datingHUD.SetActive(false);
                if (!whiteScreen.activeSelf) dialogueText = papyrusText;
                dialogueText.font = papyrusFont;
            }
        }

        dialogueText.text="";
        isTyping = true;
        foreach(var letter in str.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            if(isTyping){
                dialogueText.text +=letter;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;
        if(hasChoice && index == (maxIndex-1)){
            // enableChoice();
            if (!isTypingChoice) displayChoice();
        }
        
        TXTsfx.Stop(); // stops playing the typing SFX after typing is complete !!
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
        if (p != null) player.canMove = true; //let the player move again
        
        disableChoice();
    }
    #endregion
    
    
    // ======================================     SWAP CAMERA     ==============================================
    #region camera
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
                playerSpawn.transform.position = mainSpawn.transform.position;
                canvas.GetComponent<Canvas>().worldCamera = mainCam;
                
            break;
            case 1: 
                Debug.Log("should swap cam");
                bedroomCam.enabled = true;
                playerSpawn.transform.position = bedroomSpawn.transform.position;
                canvas.GetComponent<Canvas>().worldCamera = bedroomCam;
            break;
            case 2: 
                runTimer = false;
                combatCam.enabled = true;
                uiAtTop = false;
                resetDialogue();
                canvas.GetComponent<Canvas>().worldCamera = combatCam;
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Interactable");
                foreach (GameObject o in objs){
                    o.SetActive(false);
                }
                p.SetActive(false);
            break;
        }

    }
    #endregion



    //==========================================    COMBAT METHODS     ==================================================
    #region combat
    void runCombat(int n){

        sans_music.SetActive(false);
        date_start_music.SetActive(true);
        
        if(n >= combatSize || canRunMiniGame) return;
        if(givingPresent) {
            //call present animation -> the end of animation will trigger runCombat again and set givingPresent to false
            return;
        }
        index = 0;
        interactableObj obj = (interactableObj)combat[n].GetComponent(typeof(interactableObj));
        dialogue = obj.myDialogue;
        if(obj.hasChoice){
            hasChoice = true;
            if(obj.changeChoice) choice = obj.myChoice;
        }
        StartCoroutine(displayDialogue());
        canPressEnter = true;
    }

    void checkEndOfSeq(int n){
        canPressEnter = false;
        switch(n){
            case 0: //start timer in case player doesnt press c
            if(!datingHUD.activeSelf){
                timeRemaining = 10f;
                runTimer = true;
            }
            else runCombat(++currentSeq);
            break;
            case 10: //start minigame; next
                currentSeq++;
                canRunMiniGame = true;
            break;
            case 19: //transition to white screen ()just use a white sprite; next
                whiteScreen.SetActive(true);
                dialogueText = whiteScreenText;
                currentSeq++;
            break;
            case 20: //transition out of white screen (disable object)
                whiteScreen.SetActive(false);
                currentSeq++;
            break;
            default:
                if(n == 5 || n == 6){ //animation (1/3)
                    
                }else if(n == 8 || n == 9){ //animation (2/3)

                }else if(n == 14 || n == 15){ //bring up date power

                }
                else if(n == 17 || n == 18){ //power overflows anim

                }
                currentSeq++;
            break;
        }
        if (currentSeq != 0) runCombat(currentSeq);
    }

    void runMiniGame(){
        if(Input.GetKey(KeyCode.LeftArrow)){
            inspector.transform.Translate(Vector3.left*Time.deltaTime *speed);
        }else if(Input.GetKey(KeyCode.RightArrow)){
            inspector.transform.Translate(Vector3.right*Time.deltaTime *speed);
        }else if(Input.GetKey(KeyCode.UpArrow)){
            inspector.transform.Translate(Vector3.up*Time.deltaTime *speed);
        }else if(Input.GetKey(KeyCode.DownArrow)){
            inspector.transform.Translate(Vector3.down*Time.deltaTime *speed);
        }
        
        //check if the player has selected any object
        if(Input.GetKeyDown(KeyCode.K)){
            RaycastHit2D hit = Physics2D.Raycast(inspector.transform.position, inspector.transform.position);

            if(hit.collider != null){
                Debug.Log(hit.collider.gameObject.name);
                GameObject obj = hit.collider.gameObject;
                string name = obj.name;
                index = 0;
                switch(name){
                    case "Hat":
                        dialogue = ((interactableObj)obj.GetComponent(typeof(interactableObj))).myDialogue;
                        StartCoroutine(displayDialogue());
                        foundPresent = true;
                        canRunMiniGame = false;
                        // runCombat(currentSeq);
                    break;
                    default:
                        dialogue = ((interactableObj)obj.GetComponent(typeof(interactableObj))).myDialogue;
                    break;
                }
                papyrusTextBox.SetActive(true);
            } 
        }
    }
#endregion
}