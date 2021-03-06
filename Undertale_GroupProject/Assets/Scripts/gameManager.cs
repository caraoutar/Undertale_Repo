using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    //INTRO CARD REFERENCES
    [Header("Intro Card references")]
    [SerializeField] GameObject intro_bg; // the black bg for the intro
    [SerializeField] GameObject intro; 
    [SerializeField] Image intro_image;
    [SerializeField] GameObject menu; // holds all the menu objects ( papyrus, frisk , txtbox, etc - so they're easy to organize + adjust!)
    [SerializeField] GameObject arrow; // small arrow sprite to indicate moving forward
    [SerializeField] bool runIntro = true;
    [SerializeField] Sprite[] intro_cards;
    [SerializeField] int image_index; 
    [SerializeField] Image image_rend; 

    [Header("Scene 1 Animation")]
    [SerializeField] Animator carAnim;
    [SerializeField] Animator tvAnim; 

// ANIMATING PAPYRUS

    // sprite in dialogue box
    [Header("Papyrus Dialogue Head Animation")]
    [SerializeField] Animator dialoguePAP;

    // sprites for dating... START!
    [Header("animating Papyrus during dating START!")]
    [SerializeField] Animator datepyrusAnim; // papyrus' head ...
    [SerializeField] Animator datearmAnim; // papyrus' arms ...

// ANIMATING dating... START! OBJECTS
    [Header("dating START! Animation")]
    [SerializeField] Animator hatAnim;
    [SerializeField] Animator presentAnim;

    [SerializeField] Animator spaghettAnim;

    [Header("DATE POWER references")]

    [SerializeField] GameObject tensionBox;
    [SerializeField] GameObject datepower_normal;
    [SerializeField] GameObject datepower_tension;

    // ANIMATION ...
    [SerializeField] Animator tensionData; // the tension box
        
        // regular date power
    [SerializeField] Animator dateBox;
    [SerializeField] Animator dateData;

        // datepower during tension ... !
    [SerializeField] Animator dateBox_tension;
    [SerializeField] Animator dateData_tension;

    // DIALOGUE SFX
    [Header("Audio")]
    [SerializeField] AudioSource TXTsfx; // sfx for the typing
    [SerializeField] AudioSource arrowMOVEsfx; // sfx when using the left/right arrow keys during CHOICES
    [SerializeField] AudioSource SELECTsfx; // sfx when player presses ENTER
    [SerializeField] AudioClip narrativeTXTsfx; //sfx for narrative typing
    [SerializeField] AudioClip papyrusTXTsfx; //sfx for papyrus typing
    [SerializeField] AudioClip sansTXTsfx; //sfx for sans typing 

    //setting Game Objects for background music
    [Header("Background Music")]
    [SerializeField] GameObject sans_music; //game object for house music
    [SerializeField] GameObject date_start_music; //game object for date start music
    [SerializeField] GameObject date_tense_music; //game object for date tense music 
    [SerializeField] GameObject date_fight_music; //game object for date fight music
    [SerializeField] GameObject intro_and_end_music; //game object for intro and ending music

    //--------------- dialogue variables --------------------//

    [Tooltip("Adjust the typing effect speed; change fonts; add SFX")]
    [Header("Adjustable Dialogue Variables")]
    [SerializeField] int letterPerSec; //how fast the typing is
    [SerializeField] Font papyrusFont;
    [SerializeField] Font narrativeFont;
    [SerializeField] Font sansFont; //font for sans dialogue in post-it note object

    [Tooltip("These dialogue variables should not be adjusted")]
    [Header("Non-Adjustable Dialogue Variables")]
    [SerializeField] int index; //which line is being typed
    [SerializeField] int maxIndex; //the maximum number of lines (list size)
    [SerializeField] bool isTyping = false; //condition for whether text is curerntly being typed
    [SerializeField] bool isTypingChoice = false;
    [SerializeField] bool openText = false; //condition for whether the textbox is currently open
    [SerializeField] bool typedChoices = false;
    [SerializeField] bool whiteScreen_occurred = false; //condition for if white screen text has occured

    [Tooltip("object dialogue will show up here (please adjust dialogue on the interactable object)")]
    [Header("Dialogue Text")] //dialgoue variables
    public List <string> dialogue = new List<string>(); //the dialogue that will be typed
    [SerializeField] float maxLen;
    [SerializeField] float defLen;
    [SerializeField] float lenDiff;
    
    [Tooltip("choice will show up here if different from default yes/no")]
    [Header("Choice Text")]
    public string[] choice = new string[2]; //choice dialogue to be added
    public bool hasChoice;
    [SerializeField] string[] defaultChoice = {"YES", "NO"};

    //---------------- UI -----------------------//
    [Header("Starting and Ending Scenes")]
    [SerializeField] GameObject bg_end;
    [SerializeField] GameObject endCard;

    [Tooltip("reference to canvas; there is no need to add the other references")]
    [Header("Main UI Objects")] //references to the dialogue objects/components
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject dialogueTextBox;
    [SerializeField] Text dialogueText;
    [SerializeField] TMP_Text shakyText;
    [SerializeField] Text choice1;
    [SerializeField] Text choice2;
    [SerializeField] Image heart1;
    [SerializeField] Image heart2;
    [SerializeField] Image papyrusHead;
    [SerializeField] GameObject heartChange; //transition heart
    [SerializeField] Animator heartAnim;

    [Header("UI Variables")] //references to the dialogue objects/components
    public bool uiAtTop = false;
    [SerializeField] Vector2 textStartingPosition;
    [SerializeField] GameObject dialogueTextObject;
    [SerializeField] Vector2 dialogueStartingPosition;
    [SerializeField] Vector2 dialogueSize;
    [SerializeField] float sizeOfPapyrusText = 0.8f;
    [SerializeField] float sizeOfPapyrusBox = 0.38f;
    [SerializeField] Vector2 choice1StartingPosition;
    [SerializeField] Vector2 choice2StartingPosition;
    [SerializeField] float choiceHeight=0.05f;

    [Header("Combat UI Objects")]
    [SerializeField] GameObject combatCanvas;
    [SerializeField] GameObject papyrusTextBox;
    [SerializeField] Text papyrusText;
    [SerializeField] GameObject whiteScreen;
    [SerializeField] Text whiteScreenText;
    [SerializeField] GameObject datingHUD;
    [SerializeField] GameObject datingPower;
    [SerializeField] GameObject datingTension;


    //----------- object variables ---------------------//
    [Tooltip("this is the object currently being interacted with")]
    [Header("Object in Interaction")]
    public interactableObj currentObj; //reference to the current object being interacted with; this will automatically change
    [SerializeField] GameObject mainRoom;
    [SerializeField] GameObject bedroom;
    public bool canInteract = true;
    
    [Tooltip("add different empty gameobjects for each section of the combat")]
    [Header("Combat Variables")]
    [SerializeField] List <GameObject> combat;
    [SerializeField] int combatSize;
    [SerializeField] int currentSeq = -1;
    [SerializeField] GameObject inspector;
    [SerializeField] bool canRunMiniGame;
    [SerializeField] bool canMoveInspector;
    int speed = 1;
    [SerializeField] bool givingPresent;
    [SerializeField] bool foundPresent;
    [SerializeField] bool canPressEnter = true;

    [Header("Combat Papyrus References")]
    [SerializeField] GameObject combatPapyrus;
    [SerializeField] GameObject combatPapyrusDate;

    // ----------------- timer -----------------------//
    [Tooltip("adjust the maximum time before the player can interact with a new object")]
    [Header("Countdown Variables")]
    //countdown timer code
    [SerializeField] float maxTime = 0.5f;
    [SerializeField] float timeRemaining;
    [SerializeField] bool runTimer = false;
    #endregion


    //===================================== START & UPDATE ===========================================
    public void Start(){
        // bedroom.SetActive(false); //make sure the objects in the bedroom are not active
        //on start, set up the variables for UI
        canInteract = true;
        p = GameObject.FindWithTag("Player");
        player = (playerMovement) p.GetComponent(typeof(playerMovement));
        playerSpawn = GameObject.FindWithTag("Player");

        // closestObj = GameObject.FindWithTag("Interactable");
        dialogueTextBox = canvas.transform.GetChild(0).gameObject;
        textStartingPosition = dialogueTextBox.GetComponent<RectTransform>().anchoredPosition;
        
        dialogueTextObject = dialogueTextBox.transform.GetChild(0).gameObject;
        dialogueStartingPosition = dialogueTextObject.GetComponent<RectTransform>().anchoredPosition;
        dialogueSize = dialogueTextObject.GetComponent<RectTransform>().sizeDelta;

        dialogueText = dialogueTextBox.transform.GetChild(0).GetComponent<Text>();
        choice1 = dialogueTextBox.transform.GetChild(1).GetComponent<Text>();
        choice2 = dialogueTextBox.transform.GetChild(2).GetComponent<Text>();
        choice1StartingPosition = dialogueTextBox.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition;
        choice2StartingPosition = dialogueTextBox.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition;
        
        heart1 = dialogueTextBox.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        heart2 = dialogueTextBox.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        papyrusHead = dialogueTextObject.transform.GetChild(0).GetComponent<Image>();

        papyrusTextBox = combatCanvas.transform.GetChild(0).gameObject;
        papyrusText = papyrusTextBox.transform.GetChild(0).GetComponent<Text>();
        datingHUD = combatCanvas.transform.GetChild(1).gameObject;
        datingPower = datingHUD.transform.GetChild(3).gameObject;
        // datingTension = combatCanvas.transform.GetChild(3).gameObject;

        whiteScreen = combatCanvas.transform.GetChild(2).gameObject;
        whiteScreenText = whiteScreen.transform.GetChild(0).GetComponent<Text>();

        combatSize = combat.Count;

        // DIALOGUE SFX
            // assigning the sound !
        // TXTsfx = TXTsfx.GetComponent<AudioSource>(); 
        // arrowMOVEsfx = arrowMOVEsfx.GetComponent<AudioSource>(); 
        // SELECTsfx = SELECTsfx.GetComponent<AudioSource>(); 
        //narrativeTXTsfx = narrativeTXTsfx.GetComponent<AudioSource>();
        //papyrusTXTsfx = papyrusTXTsfx.GetComponent<AudioSource>();

        //CAR AND TV ANIMATION
        carAnim = carAnim.GetComponent<Animator>();
        tvAnim = tvAnim.GetComponent<Animator>();
        carAnim.enabled = true; 
        tvAnim.enabled = true; 

        // DIALOGUE BOX PAPYRUS ANIMATION
        dialoguePAP = dialoguePAP.GetComponent<Animator>();

        // DATING START !!! ANIMATIONS

        hatAnim = hatAnim.GetComponent<Animator>();
        presentAnim = presentAnim.GetComponent<Animator>();
        spaghettAnim = spaghettAnim.GetComponent<Animator>();

        datepyrusAnim = datepyrusAnim.GetComponent<Animator>();
        datearmAnim = datearmAnim.GetComponent<Animator>();

        tensionData = tensionData.GetComponent<Animator>();
        dateBox = dateBox.GetComponent<Animator>();
        dateData = dateData.GetComponent<Animator>();

        dateBox_tension = dateBox_tension.GetComponent<Animator>();
        dateData_tension = dateData_tension.GetComponent<Animator>();

         //get reference to image component of intro_image object 
        image_rend = intro.GetComponent<Image>();

    }

    //various update cases
    void Update(){
        //transition from room to combat scene
        // if(heartChange.activeSelf && heartAnim.GetCurrentAnimatorStateInfo(0).IsName("EnterCombat")){
        //     bg_end.SetActive(false);
        //     heartChange.SetActive(false);
        //     swapCamera(2);
        //     StartCoroutine(displayDialogue());
        //     canInteract = true;
        // }
                    
        //updating typing sound
        if (!isTyping) {
            TXTsfx.loop = false;

        } else {
            TXTsfx.loop = true;
        }

        //run the intro sequence
         if (runIntro == true) {
            intro_and_end_music.SetActive(true); 
            runIntroduction();
        }
        
        //car animation
        if (dialogueText.text.Equals("THE BEST FEATURE, THOUGH...")) { //set the animation of the racecar
                    // Debug.Log("car anim");
                    carAnim.SetBool("StartRunning", true);
                    carAnim.SetBool("StopRunning", false); 
        }

        //change the heart image based on what the player wants to select (select with left/right arrow and enter)
        if(typedChoices && (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D))){
            heart1.enabled = false;
            heart2.enabled = true;

            // DIALOGUE SFX
            arrowMOVEsfx.Play(); // plays the sound when the player selects with right arrow key
        }
        else if(typedChoices && (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))){
            heart2.enabled = false;
            heart1.enabled = true;

            // DIALOGUE SFX
            arrowMOVEsfx.Play(); // plays the sound when the player selects with left arrow key
        }

        //code for when the player wants to make a selection
        if(Input.GetKeyDown(KeyCode.Return) && typedChoices && canPressEnter){
            if(heart1.enabled){
                // (Debug.Log"Choice 1");
                if(currentObj!=null) checkChoice1(currentObj.name);
                else checkChoice1("none");

                SELECTsfx.Play(); // plays the sound when the player presses ENTER
            }else if(heart2.enabled){
                // Debug.Log("Choice 2");
                checkChoice2();
                
                // DIALOGUE SFX
                SELECTsfx.Play(); // plays the sound when the player presses ENTER
            }
        }
        //change dialogue if the player presses enter key
        else if(Input.GetKeyDown(KeyCode.Return) && openText && canPressEnter){
            if(combatCam.enabled){

                if(currentSeq == 7 && dialogueText.text.Equals("BEHOLD!")){ //swap clothing in seq7
                    combatPapyrus.SetActive(false);
                    combatPapyrusDate.SetActive(true);
                }
                else if(currentSeq == 4 && index >= 2 && datingHUD.activeSelf){ //close dating hud in seq4
                datingHUD.SetActive(false);
                }
            }

            if(isTyping && !isTypingChoice){ // if the dialogue is still being typed, finish typing
                dialogueText.text=dialogue[index];
                
                if(!combatCam.enabled){ //if the game isn't in the combat scene, then run this code
                    resetDialogue();
                    if(dialogueText.text.Contains("*")){
                        dialogueText.font = narrativeFont;
                    } else if (dialogueText.text.Contains("- Sans") || dialogueText.text.Contains("- Papyrus")){
                        Debug.Log("dirty sock interaction confirmed ...");

                        // NOTE:
                            // there was a small bug that if the player presses ENTER and the dialogue for the DIRTY SOCKS
                            // isn't done typing, it will automatically change the txt to Papyrus' font and have Papyrus' head appear
                            
                            // so even if there's just the Debug.Log + no actual code here, it somehow prevents it lol
                    }
                    else{
                        setPapyrusDialogue();
                        dialogueText.text = dialogueText.text.ToUpper();
                    }
                }

                isTyping = false;
            }
            else if(index < maxIndex){
                if(currentSeq == 11 && canRunMiniGame && currentObj==null) return;
                ++index;
                if(index < maxIndex){
                    StartCoroutine(typeDialogue(dialogue[index]));
                }
            }

            if(index >= maxIndex){ //if the index is greater than or equal to the size of the list, then close the dialogue
                if(!hasChoice){
                    if(combatCam.enabled){
                        if(currentSeq == -1){
                            currentObj = null;
                            dialogueText.text = "";
                            dialogue.Clear();
                            runCombat(++currentSeq);
                        }
                        else if(canRunMiniGame){
                            papyrusTextBox.SetActive(false);
                            canMoveInspector = true;
                        }
                        else if(foundPresent && !givingPresent){
                            canPressEnter = false;
                            givingPresent = true;
                            index = 1;
                            runCombat(currentSeq);
                            currentObj = null;
                        }
                        else{
                            // Debug.Log("Will check end of Seq");
                            checkEndOfSeq(currentSeq);
                        }
                    }
                    else{
                        timeRemaining = maxTime; //begin countdown to allow player to interact again
                        runTimer = true;
                        closeDialogue();
                    }
                }
                    
            }
        }

        if(canRunMiniGame){ //run mini game
            // Debug.Log("Running mini game");
            if(isTyping) canMoveInspector = false;
            else canMoveInspector = true;

            runMiniGame();
        
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
                        canInteract = true;
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
            //checkEndOfSeq(currentSeq);
        }
        
        animatingDATE(); // void function(?) that holds all the animations for the dating START! scene!
    }

    //function to run game intro 
    void runIntroduction() {
            sans_music.SetActive(false); // to stop the sans music from playing

            // Debug.Log("run intro");

            //if player presses enter
            if (Input.GetKeyDown(KeyCode.Return)) {
                //increase image index
                image_index++; 

                if (image_index < 11) {
                    //set sprite to sprite of index value in array 
                    image_rend.sprite = intro_cards[image_index];
                    // Debug.Log(image_index); 

                    arrow.SetActive(true); // enable the arrow

                } else if (image_index == 11){ // if the index is EXACTLY 11 ,

                    menu.SetActive(true); // activate the menu , 
                    arrow.SetActive(false); // disable the arrow , since its not needed anymore

                }
                else {
                    //if we hit the highest index, stop intro and set run_intro to false 
                    runIntro = false; 
                    intro.SetActive(false);
                    intro_bg.SetActive(false);
                    menu.SetActive(false);
                    intro_and_end_music.SetActive(false); // to stop the music once the intro cutscene is over
                    sans_music.SetActive(true); // to start the sans music !

                    //starting dialogue
                // maria : moved the starting dialogue here so it starts once the intro scene is done!
                    currentObj = (interactableObj)GameObject.Find("Interactables/MainRoom/papyrus").GetComponent(typeof(interactableObj));
                    dialogue = currentObj.myDialogue;
                    player.canMove = false;
                    canInteract = false;
                    StartCoroutine(displayDialogue());
                }

                
            }
            

    }

    //combat transition
    IEnumerator waitCombat(float time){
     yield return new WaitForSeconds(time);
     // Now do some stuff...
    heartAnim.SetBool("combat", true);
    intro_bg.SetActive(false);
    heartChange.SetActive(false);
    swapCamera(2);
    StartCoroutine(displayDialogue());
    canInteract = true;
    }

   
    //=======================================   CHOICE CHECK & TYPING METHODS   ==========================================
    #region choice
    void checkChoice1(string name){
        index = 0;
        disableChoice();

        if(combatCam.enabled){
            // Debug.Log("Should go to next scene");
            if (currentSeq == 11) currentSeq+=2; //special case for 11; skip to 13
            else currentSeq++; 
            runCombat(currentSeq); //run the next sequence
        }
        else{
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
                    
                    // StartCoroutine(waitCombat(GetCurrentAnimatorClipInfo(0)[0].clip.length));
                    intro_bg.SetActive(true);
                    heartChange.SetActive(true);
                    mainRoom.SetActive(false);
                    bedroom.SetActive(false);
                    p.SetActive(false);
                    // Debug.Log(((AnimatorClipInfo[])heartAnim.GetCurrentAnimatorClipInfo(0))[0].clip.length * 1.8f);
                    StartCoroutine(waitCombat(3.42f));
                    canInteract = false;
                    // swapCamera(2);
                    // StartCoroutine(displayDialogue());
                break;
            }
        }
    }

    void checkChoice2(){
        disableChoice();
        if(combatCam.enabled){ //combat scene
            if (currentSeq==11) currentSeq++;
            else currentSeq+=2;
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

        if (!TXTsfx.isPlaying) {
        TXTsfx.Play(); // plays the typing SFX ,
        }

        // Debug.Log("Typing choices");
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

        //TXTsfx.Stop(); // stops playing the typing SFX after typing is complete !!
    }

    //enable the choice gameobjects
    void enableChoice(){
        if(dialogueTextBox.transform.GetChild(0).GetComponent<Text>().text.Equals("")){
            dialogueTextBox.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition
                 = new Vector2(choice1StartingPosition.x, (choiceHeight)*dialogueTextBox.GetComponent<RectTransform>().rect.height);
            dialogueTextBox.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition
                 = new Vector2(choice2StartingPosition.x, (choiceHeight)*dialogueTextBox.GetComponent<RectTransform>().rect.height);
        
        }else{
            dialogueTextBox.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = choice1StartingPosition;
            dialogueTextBox.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = choice2StartingPosition;
        }
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
        if(whiteScreen_occurred){
            StartCoroutine(typeShakyDialogue(str));
            yield return null;
        }
        else{
        if(!combatCam.enabled){ //if the game isn't in the combat scene, then run this code
            resetDialogue();
            dialoguePAP.GetComponent<Animator>().enabled = true; // enables the animation to play at the start of typing
            //change the font of the dialogue if there is an astericks
            if(str.Contains("*")) {
                dialogueText.font = narrativeFont;
                TXTsfx.clip = narrativeTXTsfx;
                maxLen = defLen + lenDiff; //change the max length of the textbox
            }
            else if(str.Contains("- Sans")) {
                dialogueText.font = sansFont; 
                TXTsfx.clip = sansTXTsfx;
            }
            else if(str.Contains("- Papyrus")) {
                dialogueText.font = papyrusFont;
                TXTsfx.clip = papyrusTXTsfx;
            }
            else{
                maxLen = defLen; //change the max length of the textbox
                TXTsfx.clip = papyrusTXTsfx;
                setPapyrusDialogue();
                str = str.ToUpper();
            }
            if (currentObj.name == "television") {
                    // Debug.Log("tv anim");
                    //set bools to make tv animation play
                    tvAnim.SetBool("StopRunning", false);
                    tvAnim.SetBool("StartRunning", true);
                }
        }
        else{ //if we're in combat then set the dialogue text to the approriate box depending on who is talking
        

            if(str.Contains("*")){
                maxLen = defLen + lenDiff; //change the max length of the textbox
                TXTsfx.clip = narrativeTXTsfx;
                dialogueText = dialogueTextBox.transform.GetChild(0).GetComponent<Text>();
                dialogueText.font = narrativeFont;
                
            }
            else{ 
                maxLen = (0.46f)*defLen; //change the max length of the textbox
                // Debug.Log(papyrusTextBox.transform.GetChild(0).GetComponent<RectTransform>().rect.x);
                // Debug.Log(papyrusTextBox.transform.GetComponent<RectTransform>().rect.x);
                // maxLen = -(papyrusTextBox.transform.GetChild(0).GetComponent<RectTransform>().rect.x);
                if( currentSeq == 4 && str == "") datingHUD.SetActive(false);
                if (!whiteScreen.activeSelf) dialogueText = papyrusText;
                dialogueText.font = papyrusFont;
                TXTsfx.clip = papyrusTXTsfx;
                datepyrusAnim.GetComponent<Animator>().enabled = true; // to start the animation of papyrus' head once in combat!
                
            }

        // to handle specific animation of papyrus' parts during the minigame 
            if (str.Contains("...MY HAT.")){

                // Debug.Log("sus...");
                datepyrusAnim.SetBool("Sus", true);

                datepyrusAnim.SetBool("Determined", false);

            } else if (str.Contains("NYEH HEH HEH!!")){

                datepyrusAnim.SetBool("Determined", true);

                datepyrusAnim.SetBool("Sus", false);

            } else if (str.Contains("THIS IS MY SECRET...")){

                // Debug.Log("manifesting... PRESENT...");
                datepyrusAnim.SetBool("Blush", true);
                hatAnim.SetBool("hatlift", true);

                datepyrusAnim.SetBool("Determined", false);

            } else if (str.Contains("DO YOU KNOW WHAT THIS IS?")){

                // Debug.Log("unmanifests... PRESENT...");
                presentAnim.SetBool("gone", true);

            } else if (str.Contains("take a bite")){

                // Debug.Log("unmanifests da spaghetti");
                spaghettAnim.SetBool("eaten", true);

            }
        }

        
        TXTsfx.Play(); 
        

        //the actual typing effect; do not add to these loops
        dialogueText.text="";

        //local variables; this resets each time the method is called
        int wordIndex = 1;
        string [] words = str.Split(' ');
        isTyping = true;
        int len = 0;
        bool isSpace = false;
        string currLine = "";

        foreach(var letter in str.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            // Debug.Log(wordIndex);
            if(isTyping && letter == ' ' && wordIndex < words.Length){ //if there is a new word do the following
                string line = currLine + words[wordIndex]; //add the new word to the current line
               
                // Debug.Log(words[wordIndex]);

                Font myFont = dialogueText.font; //we need the font to know the size of a string
                myFont.RequestCharactersInTexture(line, dialogueText.fontSize, dialogueText.fontStyle); //find the font info
                
                len = 0; //reset the length of the current line
                foreach(char c in line){ //for each character
                    CharacterInfo charInfo = new CharacterInfo(); //find the font/text info
                    myFont.GetCharacterInfo(c, out charInfo, dialogueText.fontSize); //get its width
                    //Debug.Log(charInfo.advance);
                    len += charInfo.advance; //go on to the next character
                }
                isSpace = true;
                wordIndex++;
            }
            if(isTyping){
                if (len > maxLen && isSpace){ //if the length of the line with the new word will be larger the textbox, move to a new line
                    // Debug.Log("len > maxLen");
                    dialogueText.text += '\n';
                    isSpace = false;
                    // wordIndex++;
                    currLine = "";
                    len = 0;
                }
                else{ 
                    dialogueText.text +=letter; //otherwise just add the letter
                    currLine = currLine +=letter;
                }
                // Debug.Log(currLine);
                isSpace = false;
                len = 0;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;
        if(hasChoice && index == (maxIndex-1)){
            // enableChoice();
            if (!isTypingChoice) displayChoice();
        }

      
        dialoguePAP.GetComponent<Animator>().enabled = false; // stops playing the talking animation after typing is complete !!
        datepyrusAnim.GetComponent<Animator>().enabled = false; // stops playing papyrus' talking animation in combat scene !!
        }
    }



    //---------------- shaky ------------------------------//
    IEnumerator typeShakyDialogue(string str){
        canPressEnter = false;
        if(shakyText.gameObject.transform.childCount >0)
            shakyText.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        
        maxLen = defLen + (lenDiff); //change the max length of the textboxs
        // maxLen = -(whiteScreen.transform.GetChild(1).GetComponent<RectTransform>().rect.x-100);
        // Debug.Log("Size is: " + -(whiteScreen.transform.GetChild(1).GetComponent<RectTransform>().rect.x-100));
        TXTsfx.clip = papyrusTXTsfx;
        
        TXTsfx.Play();

        //the actual typing effect; do not add to these loops
        shakyText.text="";

        //local variables; this resets each time the method is called
        int wordIndex = 1;
        string [] words = str.Split(' ');
        isTyping = true;
        int len = 0;
        bool isSpace = false;
        string currLine = "";

        foreach(var letter in str.ToCharArray()){ //add a single character to the text at a time, thus creating a typing effect
            if(isTyping && letter == ' ' && wordIndex < words.Length){ //if there is a new word do the following
                string line = currLine + words[wordIndex]; //add the new word to the current line
                // Debug.Log(line);

                Font myFont = papyrusFont; //we need the font to know the size of a string
                myFont.RequestCharactersInTexture(line, 100, dialogueText.fontStyle); //find the font info
                
                len = 0; //reset the length of the current line
                foreach(char c in line){ //for each character
                    CharacterInfo charInfo = new CharacterInfo(); //find the font/text info
                    myFont.GetCharacterInfo(c, out charInfo, 100); //get its width
                    //Debug.Log(charInfo.advance);
                    len += charInfo.advance; //go on to the next character
                }
                isSpace = true;
                wordIndex++;
            }
            if(isTyping){
                if (len > maxLen && isSpace){ //if the length of the line with the new word will be larger the textbox, move to a new line
                    // Debug.Log("len > maxLen");
                    shakyText.text += '\n';
                    isSpace = false;
                    // wordIndex++;
                    currLine = "";
                    len = 0;
                }
                else{
                    shakyText.text +=letter; //otherwise just add the letter
                    currLine = currLine +=letter;
                }
                // Debug.Log(currLine);
                isSpace = false;
                len = 0;
                yield return new WaitForSeconds(1f/letterPerSec);
            }
        }
        isTyping = false;
        canPressEnter = true;

        //TXTsfx.Stop(); // stops playing the typing SFX after typing is complete !!
        // dialoguePAP.GetComponent<Animator>().enabled = false; // stops playing the talking animation after typing is complete !!
       
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

        carAnim.SetBool("StopRunning", true);
        carAnim.SetBool("StartRunning", false);

        tvAnim.SetBool("StopRunning", true);
        tvAnim.SetBool("StartRunning", false);

        //tvAnim.GetComponent<Animator>().enabled = false; 
        //carAnim.GetComponent<Animator>().enabled = false; 
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
            case 0: //main room
                mainCam.enabled = true;
                // mainRoom.SetActive(true);
                // bedroom.SetActive(false);
                playerSpawn.transform.position = mainSpawn.transform.position;
                canvas.GetComponent<Canvas>().worldCamera = mainCam;
                
            break;
            case 1: //bedroom
                // Debug.Log("should swap cam");
                bedroomCam.enabled = true;
                // bedroom.SetActive(true);
                // mainRoom.SetActive(false);
                playerSpawn.transform.position = bedroomSpawn.transform.position;
                canvas.GetComponent<Canvas>().worldCamera = bedroomCam;
            break;
            case 2: //combatroom
                sans_music.SetActive(false);
                date_start_music.SetActive(true);
                runTimer = false;
                combatCam.enabled = true;
                uiAtTop = false;
                resetDialogue();
                canvas.GetComponent<Canvas>().worldCamera = combatCam;
                // GameObject[] objs = GameObject.FindGameObjectsWithTag("Interactable");
                // foreach (GameObject o in objs){
                //     o.SetActive(false);
                // }
                // mainRoom.SetActive(false);
                // bedroom.SetActive(false);
                // p.SetActive(false);
            break;
        }

    }
    #endregion



    //==========================================    COMBAT METHODS     ==================================================
    #region combat
    void runCombat(int n){
        papyrusTextBox.SetActive(true);

        //check if white screen sequence has occurred; if so, stop music 
        if(whiteScreen_occurred == true) {
            date_start_music.SetActive(false);
            date_fight_music.SetActive(false);
        }

        if(n >= combatSize) return;

        if(givingPresent){
            date_fight_music.SetActive(true);
            foundPresent = false;  givingPresent = false;
            runCombat(currentSeq);
            // canPressEnter = false;
            //call present animation -> the end of animation will trigger runCombat again and set givingPresent to false
            return;
        }
        if (currentObj == null || currentSeq!= 11) index = 0;
        interactableObj obj = (interactableObj)combat[n].GetComponent(typeof(interactableObj));
        dialogue = obj.myDialogue;
        if(obj.hasChoice){
            hasChoice = true;
            if(obj.changeChoice) choice = obj.myChoice;
        }
        StartCoroutine(displayDialogue());
        canPressEnter = true;
    }


    /*at the end of the sequence (last dialogue in the dialogue lisst), this method is called to check if any
    event should happen when the current sequence ends.*/
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
                // Debug.Log("Checking end of seq10");
                currentSeq++;
                canRunMiniGame = true;
                // dialogueText.text = "";
                inspector.SetActive(true);
                // canMoveInspector = true;
            break;
            case 19: //transition to white screen ()just use a white sprite; next
                // canvas.SetActive(false);
                whiteScreen_occurred = true;
                dialogueTextBox.transform.GetChild(0).GetComponent<Text>().text="";
                whiteScreen.SetActive(true);
                dialogueText.text = "";
                // dialogueText = whiteScreenText;
                currentSeq++;
            break;
            case 20: //transition out of white screen (disable object)
                whiteScreen.SetActive(false);
                whiteScreen_occurred = false;
                Debug.Log("checking end of seq case 20");
                canvas.SetActive(true);
                index = 0;
                currentSeq++;
                runCombat(currentSeq);
                
            break;
            case 21: //the end of the case
                canvas.SetActive(false);
                bg_end.SetActive(true); // added bg  for the end card !
                endCard.SetActive(true);
                intro_and_end_music.SetActive(true); 
            break;

            //choices; 2,5, 8, 14, 17 are the sequences that occur when the first choice was picked, and so they
            //need to skip one sequence (which would be the choice 2 sequence) and start the dequence there
            default:
                if(currentSeq == 2 || currentSeq == 5 || currentSeq == 8 || currentSeq == 14 || currentSeq == 17){
                    currentSeq+=2;
                }
                else{ //all other choices not listed above (including specific cases in the switch statement)
                    currentSeq++;
                }
            break;
        }
        if (currentSeq != 0 && currentSeq != 21) runCombat(currentSeq);
    }

    void runMiniGame(){

        //change music from date start to date tense
        //date_tense_music.SetActive(true);
        if (canMoveInspector){
            if(Input.GetKey(KeyCode.LeftArrow)){
                inspector.transform.Translate(Vector3.left*Time.deltaTime *speed);
            }else if(Input.GetKey(KeyCode.RightArrow)){
                inspector.transform.Translate(Vector3.right*Time.deltaTime *speed);
            }else if(Input.GetKey(KeyCode.UpArrow)){
                inspector.transform.Translate(Vector3.up*Time.deltaTime *speed);
            }else if(Input.GetKey(KeyCode.DownArrow)){
                inspector.transform.Translate(Vector3.down*Time.deltaTime *speed);
            }
        }
        
        //check if the player has selected any object
        if(Input.GetKeyDown(KeyCode.Z) && canMoveInspector){
            RaycastHit2D hit = Physics2D.Raycast(inspector.transform.position, inspector.transform.position);

            if(hit.collider != null){
                canMoveInspector = false;
                // Debug.Log(hit.collider.gameObject.name);
                GameObject obj = hit.collider.gameObject;
                currentObj = (interactableObj)obj.GetComponent(typeof(interactableObj));
                string name = obj.name;
                index = 0;
                // dialogue.Clear();
                hasChoice = false;
                switch(name){
                    case "Hat":
                        dialogue = ((interactableObj)obj.GetComponent(typeof(interactableObj))).myDialogue;
                        StartCoroutine(displayDialogue());
                        foundPresent = true;
                        canRunMiniGame = false;
                        inspector.SetActive(false);
                        dialogueTextBox.transform.GetChild(0).GetComponent<Text>().text="";
                        //stop minigame music
                        date_tense_music.SetActive(false);
                        // runCombat(currentSeq);
                    break;
                    default:
                        dialogue = ((interactableObj)obj.GetComponent(typeof(interactableObj))).myDialogue;
                        StartCoroutine(displayDialogue());
                    break;
                }
                papyrusTextBox.SetActive(true);
            } 
        }
    }
    #endregion

//              ===============      ANIMATING PAPYRUS IN THE DATING...START! SCENE      ==================

    void animatingDATE(){
        if(combatCam.enabled){
            if(currentSeq == 0){
                // Debug.Log("000000000");
                if(dialogueText.text.Contains("WOULD")) {

                    datepyrusAnim.SetBool("Blush", true);

                } else if (dialogueText.text.Contains("LUCKILY")){

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Blush", false);

                } else if (dialogueText.text.Contains("ALL")){

                    datepyrusAnim.SetBool("Sus", true);
                    datearmAnim.SetBool("reading",true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if (dialogueText.text.Contains("PRESS")){

                    datepyrusAnim.SetBool("Default", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("Sus", false);
                    datearmAnim.SetBool("reading", false);

                }
            } else if (currentSeq == 1){
                // Debug.Log("111111111111111111");
                if(dialogueText.text.Contains("WOW!")) {

                    datepyrusAnim.SetBool("Anime", true);
                    datearmAnim.SetBool("shy", true);

                    datepyrusAnim.SetBool("Default", false);

                } else if (dialogueText.text.Contains("I THINK")){

                    datepyrusAnim.SetBool("Determined", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("Anime", false);
                    datearmAnim.SetBool("shy", false);
                    

                } else if (dialogueText.text.Contains("LE")){

                    datepyrusAnim.SetBool("Sus", true);
                    datearmAnim.SetBool("reading",true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if (dialogueText.text.Contains("HUMAN!")){
                    
                    datepyrusAnim.SetBool("Determined", true);
                    datearmAnim.SetBool("default", true);
                    
                    datearmAnim.SetBool("reading", false);
                    datepyrusAnim.SetBool("Sus", false);
                
                }
            }  else if (currentSeq == 2){
                // Debug.Log("2222222222222");
                if(dialogueText.text.Contains("REA")) {

                    datepyrusAnim.SetBool("Anime", true);
                    datearmAnim.SetBool("shy", true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if (dialogueText.text.Contains("AHEM")){

                    datepyrusAnim.SetBool("Sus", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("Anime", false);
                    datearmAnim.SetBool("shy", false);
                    

                } else if (dialogueText.text.Contains(",")){
                    
                    datepyrusAnim.SetBool("Determined", true);
                    datepyrusAnim.SetBool("Sus", false);

                }
            }  else if (currentSeq == 3){
                // Debug.Log("3333333333333");
                if(dialogueText.text.Contains("BUT")) {

                    datepyrusAnim.SetBool("Blush", true);
                    datepyrusAnim.SetBool("Determined", false);

                }
            }  else if (currentSeq == 4){
                // Debug.Log("444444444");
                if(dialogueText.text.Contains("THREE")) {

                    datepyrusAnim.SetBool("Sus", true);
                    datearmAnim.SetBool("reading", true);

                    datepyrusAnim.SetBool("Determined", false);
                    datepyrusAnim.SetBool("Blush", false);

                } else if (dialogueText.text.Contains("YOUR")){

                    datepyrusAnim.SetBool("Default", true);

                    datepyrusAnim.SetBool("Sus", false);

                } else if (dialogueText.text.Contains("WAIT")){
                    
                    datepyrusAnim.SetBool("Sus", true);
                    datepyrusAnim.SetBool("Default", false);

                } else if (dialogueText.text.Contains("BEEN")){

                    datepyrusAnim.SetBool("Sus", true);

                    datearmAnim.SetBool("default", true);
                    datearmAnim.SetBool("reading", false);

                } else if (dialogueText.text.Contains("WANTED")){
                    
                    datepyrusAnim.SetBool("Blush", true);
                    datearmAnim.SetBool("shy", true);
                    
                   datepyrusAnim.SetBool("Sus", false);
                   datepyrusAnim.SetBool("Default", false);
                   datearmAnim.SetBool("reading", false);
                    }
                }  else if (currentSeq == 5){
                // Debug.Log("55555555");
                    if(dialogueText.text.Contains("YOUR")) {

                    datepyrusAnim.SetBool("AHHH", true);

                    datepyrusAnim.SetBool("Blush", false);
                    datepyrusAnim.SetBool("Default", false);

                    // date power !
                    datepower_normal.SetActive(true);
                    dateData.SetBool("power_start", true);
                }
            }  else if (currentSeq == 6){
                // Debug.Log("666666");
                if(dialogueText.text.Contains(",")) {

                    datepyrusAnim.SetBool("Sus", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("Blush", false);
                    datearmAnim.SetBool("shy", false);

                } else if(dialogueText.text.Contains("NEVER")) {

                    datepyrusAnim.SetBool("Determined", true);
                    datepyrusAnim.SetBool("Sus", false);

                    // date power !
                    datepower_normal.SetActive(true);
                    dateData.SetBool("power_start", true);

                }
            }  else if (currentSeq == 7){
                // Debug.Log("777777777");
                if(dialogueText.text.Contains("MAGNIFICENT")) {

                    datepyrusAnim.SetBool("Determined", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("AHHH", false);
                    datearmAnim.SetBool("shy", false);

                    // date power !

                    tensionBox.SetActive(true);

                    tensionData.SetBool("appear", true);

                } else if(dialogueText.text.Contains("SHALL")) {

                    date_start_music.SetActive(false);
                    date_fight_music.SetActive(true);

                    tensionData.SetBool("appear", false);

                    tensionData.SetBool("permanent", true);

                } else if(dialogueText.text.Contains("BEHOLD!")) {

                    datepyrusAnim.SetBool("Anime", true);

                    datepyrusAnim.SetBool("Determined", false);


                }
            }  else if (currentSeq == 8){
                // Debug.Log("8888888");
                if(dialogueText.text.Contains("HUMAN!")) {
                    
                    datepyrusAnim.SetBool("AHHH", true);

                    datepyrusAnim.SetBool("Anime", false);

                } else if (dialogueText.text.Contains("POWER")){

                    // date power !
                    dateData.SetBool("power_increase1", true);

                    dateData.SetBool("power_start", false);

                }
            }  else if (currentSeq == 9){
                // Debug.Log("9999999");
                if(dialogueText.text.Contains("HUMAN...")) {

                    datepyrusAnim.SetBool("AHHH", true);

                    datepyrusAnim.SetBool("Anime", false);

                } else if (dialogueText.text.Contains("POWER...")){
                    
                    // date power !
                    dateData.SetBool("power_increase1", true);

                    dateData.SetBool("power_start", false);
                }
            }  else if (currentSeq == 10){
                // Debug.Log("101010101010");
                if(dialogueText.text.Contains("HEH.")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("AHHH", false);

                    // date power !
                    tensionData.SetBool("leave", true);

                } else if(dialogueText.text.Contains("BUT")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if(dialogueText.text.Contains("INVALID!")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Sus", false);
                    datepyrusAnim.SetBool("Sus", false);

                    // date power !
                    dateData.SetBool("power_decrease", true);

                    dateData.SetBool("power_increase1", false);

                } else if(dialogueText.text.Contains("HAPPEN!")) {
                    
                    // date power !
                    dateBox.SetBool("gone", true);
                    date_fight_music.SetActive(false);
                    date_tense_music.SetActive(true);

                }
            }  else if (currentSeq == 11){
                // Debug.Log("ELEVENELEVENELEVENELEVEN");
                if(dialogueText.text.Contains("SECRET...")) {

                    datepyrusAnim.SetBool("Default", true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if(dialogueText.text.Contains("PRESENT.")) {

                    datepyrusAnim.SetBool("Blush", true);

                    datepyrusAnim.SetBool("Default", false);

                }
            }  else if (currentSeq == 12){
                // Debug.Log("1212121212121212");
                if(dialogueText.text.Contains("HUMAN!")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Blush", false);

                } else if(dialogueText.text.Contains("'")) {

                    datepyrusAnim.SetBool("Anime", true);

                    datepyrusAnim.SetBool("Determined", false);

                }
            }  else if (currentSeq == 13){
                // Debug.Log("131313131313");
                if(dialogueText.text.Contains("DO")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Anime", false);
                    datepyrusAnim.SetBool("Blush", false);

                }
            }  else if (currentSeq == 14){
                // Debug.Log("141414141414");
                if(dialogueText.text.Contains("THINK")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if(dialogueText.text.Contains("HEH!")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Sus", false);

                    // date power but ... tension!
                    datepower_tension.SetActive(true);
                    dateBox_tension.SetBool("defaulTension", true);

                    dateBox_tension.SetBool("return", false);

                }
            }  else if (currentSeq == 15){
                // Debug.Log("1515151515");
                if(dialogueText.text.Contains("MORE.")) {

                    datepyrusAnim.SetBool("Anime", true);
                    datearmAnim.SetBool("shy", true);

                    // date power but ... tension!
                    datepower_tension.SetActive(true);
                    dateBox_tension.SetBool("defaulTension", true);

                    dateBox_tension.SetBool("return", false);
                    datepyrusAnim.SetBool("Determined", false);

                }
            }  else if (currentSeq == 16){
                // Debug.Log("1616161616");
                if(dialogueText.text.Contains("PASTA")) {

                    datepyrusAnim.SetBool("Determined", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("Anime", false);
                    datearmAnim.SetBool("shy", false);

                    // date power but ... tension!
                    dateBox_tension.SetBool("returnTension", true);
                    dateData_tension.SetBool("powerTension_start", true);

                    dateBox_tension.SetBool("return", false);
                    dateData_tension.SetBool("power_decrease", false);

                } else if(dialogueText.text.Contains("HUMAN!")) {

                    datepyrusAnim.SetBool("AHHH", true);

                    datepyrusAnim.SetBool("Anime", false);
                    datepyrusAnim.SetBool("Determined", false);

                }
            }  else if (currentSeq == 17){
                // Debug.Log("1717717171717");
                if(dialogueText.text.Contains("LOOK")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("AHHH", false);

                } else if(dialogueText.text.Contains("COULD")) {

                    datepyrusAnim.SetBool("Blush", true);
                    datearmAnim.SetBool("shy", true);

                    datepyrusAnim.SetBool("Sus", false);
                    datepyrusAnim.SetBool("Determined", false);

                } else if(dialogueText.text.Contains("...A")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("Blush", false);

                } else if(dialogueText.text.Contains("BE")) {

                    datepyrusAnim.SetBool("AHHH", true);

                    datepyrusAnim.SetBool("Sus", false);
                    
                    // date power but ... tension!

                    dateData_tension.SetBool("powerTension_increase1", true);

                    dateData_tension.SetBool("powerTension_start", false);
                    dateData_tension.SetBool("power_decrease", false);

                }
            }  else if (currentSeq == 18){
                // Debug.Log("1818181818");
                if(dialogueText.text.Contains("WAIT")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("AHHH", false);

                } else if(dialogueText.text.Contains("YOU")) {

                    datepyrusAnim.SetBool("Blush", true);

                    datepyrusAnim.SetBool("Sus", false);

                } else if(dialogueText.text.Contains("HUMAN...")) {

                    datepyrusAnim.SetBool("AHHH", true);

                    datepyrusAnim.SetBool("Blush", false);

                } else if(dialogueText.text.Contains("BE")) {
                   
                    // date power but ... tension!

                    dateData_tension.SetBool("powerTension_increase1", true);

                    dateData_tension.SetBool("powerTension_start", false);
                    dateData_tension.SetBool("power_decrease", false);
                }
            } else if (currentSeq == 19){
                // Debug.Log("191919191919");
                if (dialogueText.text.Contains("AH")){
                    
                    // date power but ... tension!
                    dateData_tension.SetBool("powerTension_increase2", true);

                    dateData_tension.SetBool("powerTension_increase1", false);
                    dateData_tension.SetBool("powerTension_start", false);
                } else if(dialogueText.text.Contains("NO")){   
                    
                    // date power but ... tension!
                    dateData_tension.SetBool("powerTension_final", true);

                    dateData_tension.SetBool("powerTension_increase2", false);
                    dateData_tension.SetBool("powerTension_start", false);
                }
            } else if (currentSeq == 20){

                datepower_tension.SetActive(false);

            } else if (currentSeq == 21){

                // Debug.Log("21212121212121");
                if(dialogueText.text.Contains("I...")) {

                    datepyrusAnim.SetBool("Blush", true);

                    datepyrusAnim.SetBool("AHHH", false);

                } else if(dialogueText.text.Contains("WELL...")) {

                    datepyrusAnim.SetBool("Sus", true);
                    datearmAnim.SetBool("default", true);

                    datepyrusAnim.SetBool("Blush", false);
                    datearmAnim.SetBool("shy", false);

                } else if(dialogueText.text.Contains("SHOOT,")) {

                    datepyrusAnim.SetBool("Awkward", true);

                    datepyrusAnim.SetBool("Sus", false);

                } else if(dialogueText.text.Contains("IT'S")) {

                    datepyrusAnim.SetBool("Nani", true);

                    datepyrusAnim.SetBool("Awkward", false);

                } else if(dialogueText.text.Contains("FRUITION")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Nani", false);

                } else if(dialogueText.text.Contains("ALAS,")) {

                    datepyrusAnim.SetBool("Awkward", true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if(dialogueText.text.Contains("WAIT.")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("Awkward", false);
                    datepyrusAnim.SetBool("Determined", false);
                    datepyrusAnim.SetBool("Default", false);

                } else if(dialogueText.text.Contains("I,")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Sus", false);

                } else if(dialogueText.text.Contains("BECAUSE...")) {

                    datepyrusAnim.SetBool("Anime", true);

                    datepyrusAnim.SetBool("Determined", false);

                } else if(dialogueText.text.Contains("FRIENDSHIP")) {

                    datepyrusAnim.SetBool("Blush", true);

                    datepyrusAnim.SetBool("Anime", false);

                } else if(dialogueText.text.Contains("SO, DON'T BE SAD!")) {

                    datepyrusAnim.SetBool("Determined", true);

                    datepyrusAnim.SetBool("Blush", false);

                } else if(dialogueText.text.Contains("AS GREAT AS")) {

                    datepyrusAnim.SetBool("Sus", true);

                    datepyrusAnim.SetBool("Determined", false);
                    datepyrusAnim.SetBool("Blush", false);

                } else if(dialogueText.text.Contains("STILL")) {

                    datepyrusAnim.SetBool("Default", true);

                    datepyrusAnim.SetBool("Sus", false);
                    datepyrusAnim.SetBool("Blush", false);

                } else if(dialogueText.text.Contains("THANK YOU")) {

                    datepyrusAnim.SetBool("Anime", true);

                    datepyrusAnim.SetBool("Default", false);
                    datepyrusAnim.SetBool("Blush", false);

                }
            }
        }
    }
}