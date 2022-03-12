using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;


public class UnityTPS : MonoBehaviour
{

    public CharacterController controller;
    public Transform cameraX;
    public Transform uiCamera;
    private Animator anim;


    [Header("Waypoint Marker")]
    public RawImage waypointMarker;
    public Vector3 offset;

    [Header("Main Settings")]

    public Vector3 originPosition;
    public GameObject currentUIIndicator;
    public GameObject dialogueIndicator;
    public GameObject livesRepUI;
    public GameObject persistentUI;
    public GameObject pubDesUI;
    public GameObject designatedBoard;
    public GameObject questUI;
    public GameObject questAcceptUI;
    public GameObject fadeUI;
    public GameObject smartContractUI;
    public GameObject introUI;
    public GameObject cryptoUI;
    public GameObject computerUI;
    public GameObject designatedKeyboard;

    public GameObject questCompleteIndicator;

    public GameObject npcTextname;

    public GameObject currentGameObject;

    public GameObject[] interactables;

    public Collider latestCollision;

    public Text lul;

    [Header("Date")]
    public int day = 1;
    public bool dayChangeState = false;
    public int afterDayDialogueCounter;
    public int afterDayMaxDialogueCounter;

    [Header("Quest Status")]
    public QuestGiver currentQuest;

    public string playerID;
    public string currentQuestName;
    public bool globalQuestTracker;


    [Header("Gameplay stats")]
    public int currentReputation;
    public float dialogueTimer;

    [Header("Movement")]
    [SerializeField] public bool allowMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float smoothVelocity;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 cameraMovResult;
    private float targetAngle;
    private float angleDeg;
    private float smoothAngle;

    [Header("Jump")]
    [SerializeField] private float jumpHeight;

    [Header("Interaction query")]
    [SerializeField] public string currentInteractable;

    public GameObject dialogueProgressImage;

    public GameObject displayedImageMidDialogue;

    public bool starterDialogue;

    private GameObject[] uiElements;
    public bool uiHidden;


    [Header("Gravity")]
    [SerializeField] private bool gravityCheck;
    [SerializeField] private float gravityCheckDistance;
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] Vector3 velocity;

    [Header("Quest Completion")]
    public List<string> completedQuests = new List<string>();

    public List<string> completedQuestsDay = new List<string>();

    public List<string> completedQuestsSubstring = new List<string>();

    public List<string> blocksCreated = new List<string>();


    // [Header("Idle blend value")]

    // public float idleBlendValue;

    // public bool dumbBoolCheck;

    // public bool dumbBoolCheck2 = false;



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        currentGameObject = new GameObject();
        allowMove = false;
        uiHidden = false;

        questUI.SetActive(false);
        livesRepUI.SetActive(false);
        pubDesUI.SetActive(false);
        fadeUI.SetActive(false);
        persistentUI.SetActive(false);
        introUI.SetActive(false);
        questAcceptUI.SetActive(false);
        smartContractUI.SetActive(false);
        questCompleteIndicator.SetActive(false);
        cryptoUI.SetActive(false);
        computerUI.SetActive(false);
        dialogueProgressImage.SetActive(false);
        initializeNPCTags();
        StartCoroutine(removeQuestCompleteUI());
        // StartCoroutine(changeIdleBlendValue());

        uiElements = GameObject.FindGameObjectsWithTag("UI");

        float dialogueIndicatorYSize = dialogueIndicator.transform.Find("dialogueBox").GetComponent<RectTransform>().sizeDelta.y;
        dialogueIndicator.transform.Find("dialogueBox").GetComponent<RectTransform>().sizeDelta = new Vector2(2 * Screen.width, dialogueIndicatorYSize);

        afterDayDialogueCounter = 0;
        afterDayMaxDialogueCounter = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueImageArray().Length;


        Application.targetFrameRate = 60;


    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        interactionQuery();
        checkDayStatus();
        npcNameWTSP();
        updateDay();
        callSmartContractUI();
        setUIVisibility();
        dialogueTimerMethod();




        // if (moveSpeed == 0){
        //     setIdleRandomly(idleBlendValue,dumbBoolCheck);
        //     dumbBoolCheck2 = true;
        // }

    }

    void FixedUpdate()
    {
        updateReputation();
        lulw();
    }

    void dialogueTimerMethod()
    {
        if (dialogueTimer > 0)
        {
            dialogueTimer -= Time.deltaTime;
        }

        if (dialogueIndicator.activeSelf && dialogueTimer <= 0)
        {
            dialogueProgressImage.SetActive(true);
        }
        else if (dialogueIndicator.activeSelf && dialogueTimer > 0)
        {
            dialogueProgressImage.SetActive(false);
        }
        else if (!dialogueIndicator.activeSelf)
        {
            dialogueProgressImage.SetActive(false);
        }
    }

    public void lulw()
    {
        try
        {
            currentQuestName = currentQuest.questTitle;
        }
        catch (Exception e)
        {
        }

    }

    private void movePlayer()
    {

        //gravityCheck = Physics.CheckSphere(transform.position, gravityCheckDistance, groundMask);
        gravityCheck = controller.isGrounded;

        if (gravityCheck && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (allowMove)
        {
            moveDirection = new Vector3(horizontal, 0, vertical);

            if (moveDirection != Vector3.zero)
            {
                targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z);
                angleDeg = targetAngle * Mathf.Rad2Deg + cameraX.eulerAngles.y;
                smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleDeg, ref smoothVelocity, smoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            }
        }


        if (gravityCheck)
        {
            if (allowMove)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerJump();
                }

                if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
                {
                    //walk
                    playerWalk(1f);
                    cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

                }
                else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
                {
                    //run
                    playerRun(1f);
                    cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

                }
                else if (moveDirection == Vector3.zero)
                {
                    //idle
                    playerIdle();
                }

                moveDirection *= moveSpeed;
            }


        }
        else
        {
            if (allowMove)
            {
                if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
                {
                    //walk
                    playerWalk(0.6f);
                    cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

                }
                else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
                {
                    //run
                    playerRun(1.2f);
                    cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

                }
                else if (moveDirection == Vector3.zero)
                {
                    //idle
                    playerIdle();
                }
            }

        }

        moveDirection = cameraMovResult;
        moveDirection *= moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        controller.Move((moveDirection + velocity) * Time.deltaTime);


    }

    private void playerRun(float multiplier)
    {
        moveSpeed = runSpeed * multiplier;
        anim.SetFloat("Blend", 2f, 0.15f, Time.deltaTime);
        // anim.SetFloat("IdleBlend", 0f, 0, Time.deltaTime);
    }

    private void playerWalk(float multiplier)
    {
        moveSpeed = walkSpeed * multiplier;
        anim.SetFloat("Blend", 1f, 0.15f, Time.deltaTime);
        // anim.SetFloat("IdleBlend", 0f, 0, Time.deltaTime);
    }

    private void playerIdle()
    {
        moveSpeed = 0;
        anim.SetFloat("Blend", 0f, 0.15f, Time.deltaTime);

    }

    private void playerJump()
    {
        anim.SetTrigger("Jump");
        // anim.SetFloat("IdleBlend", 0f, 0, Time.deltaTime);
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);


    }
    private int dialogueCounter = 0;
    private float closestDistance = 2f;

    private bool currentConversationEnd;


    private void interactionQuery()
    {

        if (fadeUI.activeSelf)
        {
            allowMove = false;
            resetPlayerPosition();
            setFocusToGame();
            if (afterDayDialogueCounter <= afterDayMaxDialogueCounter)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (afterDayDialogueCounter < afterDayMaxDialogueCounter)
                    {
                        try
                        {
                            fadeUI.transform.Find("afterDayText").gameObject.GetComponent<Text>().text = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueImageArray()[afterDayDialogueCounter].line;
                            fadeUI.transform.Find("setImage").gameObject.GetComponent<RawImage>().texture = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueImageArray()[afterDayDialogueCounter].image;
                        }
                        catch (Exception e)
                        {

                        }

                        afterDayDialogueCounter += 1;
                    }
                    else if (afterDayDialogueCounter >= afterDayMaxDialogueCounter)
                    {
                    
                        if (day+1 > gameObject.GetComponent<dayQuestTracker>().listQuestDays.Length){
                            SceneManager.LoadScene("ConclusionScene");
                            allowMove = false;
                            
                        } else {
                            allowMove = true;
                        }
                        fadeUI.SetActive(false);
                        dayChangeState = false;
                        

                        afterDayDialogueCounter = 0;
                        day += 1;
                        completedQuestsDay.Clear();



                    }

                }

            }
        }


        bool LULCHECK = false;
        string limaw = "";
        // Debug.Log("Dialogue counter: " + dialogueCounter);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.transform.tag == "Interactable")
            {
                float playerDistanceObject = Vector3.Distance(hitCollider.transform.position, transform.position);
                // Debug.Log(hitCollider.GetComponent<Dialogue>().returnName()+": "+playerDistanceObject);
                // Debug.Log("Collided with: " + hitCollider.name + " | Current tag: " + hitCollider.transform.tag );

                if (playerDistanceObject == 2f || playerDistanceObject < closestDistance)
                {
                    closestDistance = playerDistanceObject;
                    currentGameObject = hitCollider.transform.gameObject;
                    // Debug.Log(currentGameObject);
                }
                // Debug.Log(currentGameObject+" | "+hitCollider.transform.gameObject);
                if (GameObject.ReferenceEquals(currentGameObject, hitCollider.transform.gameObject))
                {
                    closestDistance = playerDistanceObject;
                    limaw = currentGameObject.name;
                    LULCHECK = true;

                    // if (currentGameObject.GetComponent<QuestGiver>() != null)
                    // {
                    //     // Debug.Log("This object can provide a quest.");
                    // }

                    if (Input.GetKeyDown(KeyCode.E) && !Cursor.visible && dialogueTimer <= 0)
                    {





                        if (currentGameObject.GetComponent<QuestGiver>() != null)
                        {
                            QuestGiver[] questList = currentGameObject.GetComponents<QuestGiver>();

                            // foreach (QuestGiver quest in questList)
                            // {
                            if (!questList[0].isQuestActive())
                            {
                                if (!globalQuestTracker)
                                {
                                    if (!questList[0].isQuestComplete())
                                    {
                                        questList[0].copyStartingDialogue();
                                        currentQuest = questList[0];
                                        Debug.Log("Changed object starting dialogue.");

                                    }

                                }

                            }
                            // }
                        }

                        if (currentQuest != null)
                        {
                            currentQuest.updateNPCDialogue();
                        }

                        dialogueIndicator.SetActive(true);
                        Debug.Log("Dialogue UI set active");
                        // dialogueIndicator.transform.Find("ObjectName").gameObject.GetComponent<Text>().text = limaw;





                        if (currentGameObject.GetComponent<Dialogue>().returnDialogue().Length > dialogueCounter)
                        {
                            try
                            {
                                if (currentQuest.objectiveList[currentQuest.objectiveCounter].supplyOwnImageBool && currentQuest.objectiveList[currentQuest.objectiveCounter].requiredInteractionObject == currentGameObject)
                                {
                                    displayedImageMidDialogue.GetComponent<RawImage>().color = new Color(255, 255, 255, 255);

                                    displayedImageMidDialogue.GetComponent<RawImage>().texture = currentQuest.objectiveList[currentQuest.objectiveCounter].displayImageWhileConvo;
                                }
                            }
                            catch (Exception e)
                            {
                            }

                            dialogueTimer = (float)(currentGameObject.GetComponent<Dialogue>().returnDialogue()[dialogueCounter].Length / 28);
                            if (dialogueTimer < 1)
                            {
                                dialogueTimer = 1f;
                            }

                            Debug.Log("Message:" + currentGameObject.GetComponent<Dialogue>().returnDialogue()[dialogueCounter]);
                            dialogueIndicator.transform.Find("DialogueText").gameObject.GetComponent<Text>().text = currentGameObject.GetComponent<Dialogue>().returnDialogue()[dialogueCounter];
                            dialogueCounter++;
                            currentConversationEnd = false;
                        }
                        else
                        {
                            Debug.Log("End of conversation with " + currentGameObject.name);

                            try
                            {
                                if (currentQuest.objectiveList[currentQuest.objectiveCounter].supplyOwnImageBool)
                                {
                                    displayedImageMidDialogue.GetComponent<RawImage>().texture = null;
                                    displayedImageMidDialogue.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
                                }
                                Debug.Log(currentQuest.objectiveList[currentQuest.objectiveCounter].requiredInteractionObject.name + " | " + currentQuest.objectiveList[currentQuest.objectiveCounter].isVirtualPuzzleObjective);
                            }
                            catch (Exception e)
                            {
                            }

                            try
                            {
                                if (currentGameObject.GetComponent<TeleporterScript>())
                                {
                                    currentGameObject.GetComponent<TeleporterScript>().teleportPlayer();
                                }
                            }
                            catch (Exception e)
                            {
                            }

                            try
                            {
                                if (currentGameObject.name == currentQuest.objectiveList[currentQuest.objectiveCounter].requiredInteractionObject.name && currentQuest.objectiveList[currentQuest.objectiveCounter].isVirtualPuzzleObjective)
                                {
                                    controller.transform.position = new Vector3(74, 1, 97);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Log("No quest applied.");
                            }


                            if (currentGameObject == designatedBoard)
                            {
                                pubDesUI.SetActive(true);
                                pubDesUI.GetComponent<PubDesScript>().pubDesConsolidatedMethod();
                                setFocusToUI();
                            }
                            else if (currentGameObject == designatedKeyboard)
                            {
                                computerUI.SetActive(true);
                                computerUI.GetComponent<ComputerMinigameScript>().solutionIsAchieved = true;
                                setFocusToUI();
                            }

                            // dialogueIndicator.transform.Find("ObjectName").gameObject.GetComponent<Text>().text = null;
                            dialogueIndicator.SetActive(false);
                            dialogueCounter = 0;
                            currentConversationEnd = true;
                        }

                        try
                        {
                            if (currentConversationEnd && !currentQuest.isActive && globalQuestTracker && !questCompleteIndicator.activeSelf)
                            {
                                currentQuest.displayQuestAcceptUI();
                            }
                        }
                        catch (Exception e)
                        {

                        }


                        if (currentQuest != null)
                        {
                            currentQuest.setCurrentConvoTracker(currentConversationEnd);
                            if (currentQuest.returnIndicatedGameObject() == currentGameObject)
                            {
                                if (currentGameObject == designatedKeyboard)
                                {
                                    Debug.Log("KB QUEST condition met.");
                                    currentConversationEnd = false;
                                }
                                else if (currentGameObject != designatedKeyboard)
                                {
                                    Debug.Log("Update quest condition met.");
                                    currentQuest.returnCurrentObjective().setAccomplishedState();
                                    currentQuest.updateQuestUI();
                                    currentConversationEnd = false;
                                }

                            }


                        }


                    }
                    // Debug.Log("Current object distance "+currentGameObject.name+": "+closestDistance+" | playerDistanceObj: "+playerDistanceObject);
                }
                // } else if (!GameObject.ReferenceEquals(currentGameObject, hitCollider.transform.gameObject) && closestDistance > playerDistanceObject){
                //     closestDistance = playerDistanceObject;
                //     currentGameObject = hitCollider.transform.gameObject;
                //     Debug.Log("Switched object focus to: "+currentGameObject.name);
                //     dialogueCounter = 0;
                // }


            }

        }




        if (LULCHECK)
        {
            currentInteractable = limaw;
            currentUIIndicator.transform.Find("interactText").gameObject.GetComponent<Text>().text = limaw;
            currentUIIndicator.SetActive(true);
        }
        else
        {
            currentInteractable = null;
            dialogueIndicator.transform.Find("ObjectName").gameObject.GetComponent<Text>().text = null;
            dialogueCounter = 0;
            closestDistance = 2f;
            currentUIIndicator.SetActive(false);
            dialogueIndicator.SetActive(false);
            currentConversationEnd = false;
        }

    }

    public void updateReputation()
    {
        livesRepUI.transform.Find("reputationText").gameObject.GetComponent<Text>().text = currentReputation.ToString();
    }

    public void toggleCursorState()
    {
        Cursor.visible = !Cursor.visible;
    }

    public void setFocusToUI()
    {
        allowMove = false;
        Cursor.visible = true;
        cameraX.gameObject.GetComponent<CinemachineBrain>().enabled = false;
        uiCamera.gameObject.GetComponent<CinemachineBrain>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void setFocusToUIIntroMode()
    {
        allowMove = false;
        Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    public void setFocusToGame()
    {
        allowMove = true;
        Cursor.visible = false;
        cameraX.gameObject.GetComponent<CinemachineBrain>().enabled = true;
        uiCamera.gameObject.GetComponent<CinemachineBrain>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void fadeInQuestComplete()
    {
        questCompleteIndicator.SetActive(true);

        //set opacity to 0
        questCompleteIndicator.transform.Find("complete").GetComponent<Text>().CrossFadeAlpha(0f, 0f, false);
        questCompleteIndicator.transform.Find("desc").GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0f, 0f, false);
        questCompleteIndicator.transform.Find("shadow").GetComponent<RawImage>().CrossFadeAlpha(0f, 0f, false);

        //fade in to 1
        questCompleteIndicator.transform.Find("complete").GetComponent<Text>().CrossFadeAlpha(1f, 1f, false);
        questCompleteIndicator.transform.Find("desc").GetComponent<TextMeshProUGUI>().CrossFadeAlpha(1f, 1f, false);
        questCompleteIndicator.transform.Find("shadow").GetComponent<RawImage>().CrossFadeAlpha(1f, 1f, false);

    }

    // public void fadeInUI(Transform ui){
    //     ui.GetComponent<CanvasGroup>().CrossFadeAlpha(1f,1f,false);
    // }

    public void checkDayStatus()
    {

        bool tempDayStatusCheck = true;

        try
        {
            foreach (var quest in gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnArray())
            {
                tempDayStatusCheck = tempDayStatusCheck && completedQuestsSubstring.Contains(quest);
            }
        }
        catch (Exception e)
        {
            Debug.Log("No further quest lines are defined.");
        }



        dayChangeState = tempDayStatusCheck;

        try
        {
            if (dayChangeState && (completedQuestsDay.Count >= gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnArray().Length))
            {
                if (afterDayDialogueCounter == 0)
                {
                    try
                    {
                        fadeUI.transform.Find("afterDayText").gameObject.GetComponent<Text>().text = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueImageArray()[afterDayDialogueCounter].line;
                        fadeUI.transform.Find("setImage").gameObject.GetComponent<RawImage>().texture = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueImageArray()[afterDayDialogueCounter].image;
                    }
                    catch (Exception e)
                    {
                        fadeUI.transform.Find("afterDayText").gameObject.GetComponent<Text>().text = "No set starting message. Continue as usual.";
                    }

                }

                if (!questCompleteIndicator.activeSelf)
                {
                    fadeUI.SetActive(true);
                }

            }
        }
        catch (Exception e)
        {
            Debug.Log("No length is detected.");
        }
    }

    public void callSmartContractUI()
    {
        if (Input.GetKey(KeyCode.Tab) && allowMove)
        {
            smartContractUI.GetComponent<SmartContractUIAnim>().onPress();
        }
        else
        {
            smartContractUI.GetComponent<SmartContractUIAnim>().onRelease();
        }
    }

    IEnumerator uiFadeFiveSeconds()
    {

        yield return new WaitForSecondsRealtime(5);
        // fadeUI.SetActive(false);
    }


    public void resetPlayerPosition()
    {
        // Vector3 originPosition = new Vector3(85, 0.55f, 65);
        controller.transform.position = originPosition;
    }

    public void npcNameWTSP()
    {

        foreach (var interactable in interactables)
        {
            try
            {
                interactable.transform.Find("npcText").transform.LookAt(uiCamera.transform);
                interactable.transform.Find("npcTextOccupation").transform.LookAt(uiCamera.transform);
            }
            catch (Exception e)
            {
                // Debug.Log("No npcText tag for this interactable.");
            }

        }

    }

    public void initializeNPCTags()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    public void updateDay()
    {
        persistentUI.transform.Find("dayText").gameObject.GetComponent<Text>().text = "Day " + day;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Trigger")
        {
            Debug.Log("Collision with a trigger object detected.");
        }
    }

    public void disableUI()
    {

        foreach (var x in uiElements)
        {
            x.SetActive(false);
        }

    }

    public void enableUI()
    {

        foreach (var x in uiElements)
        {
            x.SetActive(true);
        }

    }


    public void setUIVisibility()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (uiHidden)
            {
                enableUI();
                uiHidden = !uiHidden;
            }
            else
            {
                disableUI();
                uiHidden = !uiHidden;
            }

        }

    }



    IEnumerator removeQuestCompleteUI()
    {
        for (; ; )
        {
            // Debug.Log("Performing coroutine quest complete UI check.");
            yield return new WaitForSeconds(2f);
            if (questCompleteIndicator.activeSelf)
            {
                // Debug.Log("Quest Complete UI is active!! Attempting to fade out.");
                yield return new WaitForSeconds(3f);
                //fade out
                questCompleteIndicator.transform.Find("complete").GetComponent<Text>().CrossFadeAlpha(0f, 1f, false);
                questCompleteIndicator.transform.Find("desc").GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0f, 1f, false);
                questCompleteIndicator.transform.Find("shadow").GetComponent<RawImage>().CrossFadeAlpha(0f, 1f, false);

                yield return new WaitForSeconds(1f);

                questCompleteIndicator.SetActive(false);
                Debug.Log("Successfully faded out quest complete indicator.");
            }
        }

    }



    // public void setIdleRandomly(float blend, bool boolx)
    // {

    //     if (boolx)
    //     {
    //         anim.SetFloat("IdleBlend", blend, 1f, Time.deltaTime);
    //         // dumbBoolCheck2 = true;
    //     }
    //     else
    //     {
    //         anim.SetFloat("IdleBlend", 0, 2f, Time.deltaTime);
    //         // dumbBoolCheck2 = false;
    //     }

    // }


    // IEnumerator changeIdleBlendValue()
    // {
    //     for (; ; )
    //     {

    //         if (moveSpeed == 0 || persistentUI.activeSelf == false)
    //         {   
    //             if(dumbBoolCheck2){
    //                 yield return new WaitForSeconds(0.05f);
    //                 dumbBoolCheck2 = false;
    //             }

    //             idleBlendValue = 1; //UnityEngine.Random.Range(1, 5);
    //             // Debug.Log("Current idle blend value:" + idleBlendValue);
    //             dumbBoolCheck = true;
    //             // if (dumbBoolCheck2)
    //             // {
    //             //     yield return new WaitForSeconds(0.15f);
    //             //     yield return new WaitForEndOfFrame();
    //             // }
    //             Debug.Log("Waiting for 9seconds (ANIMATION LENGTH)");
    //             yield return new WaitForSeconds(9f);
    //             dumbBoolCheck = false;
    //             // Debug.Log("Waiting for 5s");
    //             yield return new WaitForSeconds(15f);
    //         }


    //     }
    // }
}





