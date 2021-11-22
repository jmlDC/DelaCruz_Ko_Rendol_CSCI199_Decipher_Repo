using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using System.Linq;
using TMPro;


public class UnityTPS : MonoBehaviour
{

    public CharacterController controller;
    public Transform cameraX;
    private Animator anim;

    [Header("Waypoint Marker")]
    public RawImage waypointMarker;
    public Vector3 offset;

    [Header("Main Settings")]
    public GameObject currentUIIndicator;
    public GameObject dialogueIndicator;
    public GameObject livesRepUI;

    public GameObject persistentUI;
    public GameObject pubDesUI;
    public GameObject designatedBoard;
    public GameObject questUI;
    public GameObject fadeUI;
    public GameObject smartContractUI;

    public GameObject npcTextname;

    public GameObject currentGameObject;

    public GameObject[] interactables;

    public Text lul;

    [Header("Date")]
    public int day = 1;
    public bool dayChangeState = false;
    public int afterDayDialogueCounter;
    public int afterDayMaxDialogueCounter;

    [Header("Quest Status")]
    public QuestGiver currentQuest;
    public bool globalQuestTracker;

    [Header("Gameplay stats")]
    public int currentReputation;

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




    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        currentGameObject = new GameObject();
        allowMove = false;

        questUI.SetActive(false);
        livesRepUI.SetActive(false);
        pubDesUI.SetActive(false);
        fadeUI.SetActive(false);
        persistentUI.SetActive(false);
        // smartContractUI.SetActive(false);
        initializeNPCTags();

        float dialogueIndicatorYSize = dialogueIndicator.transform.Find("dialogueBox").GetComponent<RectTransform>().sizeDelta.y;
        dialogueIndicator.transform.Find("dialogueBox").GetComponent<RectTransform>().sizeDelta = new Vector2(2* Screen.width,dialogueIndicatorYSize);

        afterDayDialogueCounter = 0;
        afterDayMaxDialogueCounter = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueArray().Length;

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
        
    }

    void FixedUpdate()
    {
        updateReputation();
        
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
    }

    private void playerWalk(float multiplier)
    {
        moveSpeed = walkSpeed * multiplier;
        anim.SetFloat("Blend", 1f, 0.15f, Time.deltaTime);
    }

    private void playerIdle()
    {
        moveSpeed = 0;
        anim.SetFloat("Blend", 0f, 0.15f, Time.deltaTime);
    }

    private void playerJump()
    {
        anim.SetTrigger("Jump");
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
            Debug.Log(afterDayDialogueCounter);
            if (afterDayDialogueCounter <= afterDayMaxDialogueCounter)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (afterDayDialogueCounter < afterDayMaxDialogueCounter)
                    {
                        try
                        {
                            fadeUI.transform.Find("afterDayText").gameObject.GetComponent<Text>().text = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueArray()[afterDayDialogueCounter];
                        }
                        catch (Exception e)
                        {

                        }

                        afterDayDialogueCounter += 1;
                    }
                    else if (afterDayDialogueCounter >= afterDayMaxDialogueCounter)
                    {
                        allowMove = true;
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

                    if (currentGameObject.GetComponent<QuestGiver>() != null)
                    {
                        // Debug.Log("This object can provide a quest.");
                    }

                    if (Input.GetKeyDown(KeyCode.E) && !Cursor.visible)
                    {
                        if (currentGameObject.GetComponent<QuestGiver>() != null)
                        {
                            QuestGiver[] questList = currentGameObject.GetComponents<QuestGiver>();

                            foreach (QuestGiver quest in questList)
                            {
                                if (!quest.isQuestActive())
                                {
                                    if (!globalQuestTracker)
                                    {
                                        if (!quest.isQuestComplete())
                                        {
                                            currentQuest = quest;
                                            quest.startQuestUI();
                                        }

                                    }

                                }
                            }

                        }
                        dialogueIndicator.SetActive(true);
                        // dialogueIndicator.transform.Find("ObjectName").gameObject.GetComponent<Text>().text = limaw;
                        if (currentGameObject.GetComponent<Dialogue>().returnDialogue().Length > dialogueCounter)
                        {
                            Debug.Log("Message:" + currentGameObject.GetComponent<Dialogue>().returnDialogue()[dialogueCounter]);
                            dialogueIndicator.transform.Find("DialogueText").gameObject.GetComponent<Text>().text = currentGameObject.GetComponent<Dialogue>().returnDialogue()[dialogueCounter];
                            dialogueCounter++;
                            currentConversationEnd = false;
                        }
                        else
                        {
                            Debug.Log("End of conversation with " + dialogueIndicator.transform.Find("ObjectName").gameObject.GetComponent<Text>().text);
                            if (currentGameObject == designatedBoard)
                            {
                                pubDesUI.SetActive(true);
                                pubDesUI.GetComponent<PubDesScript>().pubDesConsolidatedMethod();
                                setFocusToUI();
                            }
                            // dialogueIndicator.transform.Find("ObjectName").gameObject.GetComponent<Text>().text = null;
                            dialogueIndicator.SetActive(false);
                            dialogueCounter = 0;
                            currentConversationEnd = true;
                        }



                        if (currentQuest != null)
                        {
                            currentQuest.setCurrentConvoTracker(currentConversationEnd);
                            if (currentQuest.returnIndicatedGameObject() == currentGameObject)
                            {
                                Debug.Log("Update quest condition met.");
                                currentQuest.returnCurrentObjective().setAccomplishedState();
                                currentQuest.updateQuestUI();
                                currentConversationEnd = false;
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
    }

    public void setFocusToGame()
    {
        allowMove = true;
        Cursor.visible = false;
        cameraX.gameObject.GetComponent<CinemachineBrain>().enabled = true;
    }

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
                        fadeUI.transform.Find("afterDayText").gameObject.GetComponent<Text>().text = gameObject.GetComponent<dayQuestTracker>().listQuestDays[day - 1].returnAfterDayDialogueArray()[afterDayDialogueCounter];
                    }
                    catch (Exception e)
                    {
                        fadeUI.transform.Find("afterDayText").gameObject.GetComponent<Text>().text = "No set starting message. Continue as usual.";
                    }

                }
                fadeUI.SetActive(true);

            }
        }
        catch (Exception e)
        {
            Debug.Log("No length is detected.");
        }
    }

    public void callSmartContractUI(){
        if (Input.GetKey(KeyCode.Q)){
            smartContractUI.GetComponent<SmartContractUIAnim>().onPress();
        } else {
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
        Vector3 originPosition = new Vector3(85, 0.55f, 65);
        controller.transform.position = originPosition;
    }

    public void npcNameWTSP(){

        foreach(var interactable in interactables){
            try{
                interactable.transform.Find("npcText").transform.LookAt(cameraX.transform);
            }catch (Exception e){
                // Debug.Log("No npcText tag for this interactable.");
            }
            
        }
        
    }

    public void initializeNPCTags(){
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    public void updateDay(){
        persistentUI.transform.Find("dayText").gameObject.GetComponent<Text>().text = "Day "+day;
    }

}





