using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestGiver : MonoBehaviour
{

    public CharacterController player;
    [SerializeField]
    public int optionalPriorityCount;


    private GameObject questUI;
    private GameObject smartContractUI;

    private GameObject questCompleteIndicator;

    private GameObject questAcceptUI;

    [SerializeField]
    public string questTitle;
    [SerializeField]
    public string questDesc;

    private string questLineText;

    [SerializeField]
    public string[] startingDialogue;
    public string[] ogDialogue;

    [SerializeField]
    public Objective[] objectiveList;



    [SerializeField]
    public bool isActive;


    [SerializeField]
    public bool questAnchoredToPlayer;

    [SerializeField]
    public int reputationReward;

    public int objectiveCounter;

    public bool isComplete;

    public bool parentConvoTrackerState;

    public bool collisionRedundancyChecker;
    public bool collisionRedundancyChecker2;

    public List<KeyValuePair<string, dialogueIndex>> dialogueDict = new List<KeyValuePair<string, dialogueIndex>>();
    public List<KeyValuePair<string, dialogueIndex>> customDialogueDict = new List<KeyValuePair<string, dialogueIndex>>();

    public int debugCounter;

    public Collider playerCollider;

    public GameObject designatedMarkerLocation;

    void Start()
    {
        questUI = player.GetComponent<UnityTPS>().questUI;
        smartContractUI = player.GetComponent<UnityTPS>().smartContractUI;
        questAcceptUI = player.GetComponent<UnityTPS>().questAcceptUI;
        questCompleteIndicator = player.GetComponent<UnityTPS>().questCompleteIndicator;

    }

    public struct dialogueIndex
    {
        public string[] dialogue;
        public int index;
    }

    void Update()
    {
        if (designatedMarkerLocation != null)
        {
            float minX = player.GetComponent<UnityTPS>().waypointMarker.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = player.GetComponent<UnityTPS>().waypointMarker.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            // player.GetComponent<UnityTPS>().waypointMarker.transform.position

            Vector2 waypointPosition = Camera.main.WorldToScreenPoint(designatedMarkerLocation.transform.position + player.GetComponent<UnityTPS>().offset);

            if (Vector3.Dot((designatedMarkerLocation.transform.position - player.GetComponent<UnityTPS>().uiCamera.transform.position), player.GetComponent<UnityTPS>().uiCamera.transform.forward) < 0)
            {
                if (waypointPosition.x < Screen.width / 2)
                {
                    waypointPosition.x = maxX;
                }
                else
                {
                    waypointPosition.x = minX;
                }
            }

            waypointPosition.x = Mathf.Clamp(waypointPosition.x, minX, maxX);
            waypointPosition.y = Mathf.Clamp(waypointPosition.y, minY, maxY);
            // Debug.Log(">>>>>>>>>>: " + Vector3.Distance(designatedMarkerLocation.transform.position, player.transform.position).ToString());
            string stringifiedDistance = ((int)Vector3.Distance(designatedMarkerLocation.transform.position, player.transform.position)).ToString() + "m";
            questUI.transform.Find("Marker/Distance").GetComponent<Text>().text = stringifiedDistance;
            questUI.transform.Find("uiDistance").GetComponent<Text>().text = stringifiedDistance;
            player.GetComponent<UnityTPS>().waypointMarker.transform.position = waypointPosition;

            if (Input.GetKeyDown(KeyCode.L))
            {
                player.GetComponent<UnityTPS>().currentQuest.returnCurrentObjective().setAccomplishedState();
                player.GetComponent<UnityTPS>().currentQuest.updateQuestUI();
            }

            if (isActive)
            {
                if (designatedMarkerLocation != null)
                {
                    if (true)
                    {
                        if (objectiveList[objectiveCounter].isDestination)
                        {
                            playerCollider = player.GetComponent<Collider>();
                            Collider objectiveCollider = objectiveList[objectiveCounter].requiredInteractionObject.GetComponent<Collider>();

                            if (playerCollider.bounds.Intersects(objectiveCollider.bounds))
                            {

                                collisionRedundancyChecker = true;
                                collisionRedundancyChecker2 = true;

                            }
                            else
                            {
                                collisionRedundancyChecker = false;

                                if (collisionRedundancyChecker2 && !collisionRedundancyChecker)
                                {
                                    Debug.Log("Collider objective completed.");
                                    objectiveList[objectiveCounter].setAccomplishedState();
                                    player.GetComponent<UnityTPS>().currentQuest.updateQuestUI();
                                }
                                collisionRedundancyChecker2 = false;
                            }
                        }
                    }

                }
            }

        }
    }

    public void copyStartingDialogue()
    {   
        
        player.GetComponent<UnityTPS>().globalQuestTracker = true;
        if (startingDialogue.Length != 0)
        {   
            ogDialogue = gameObject.GetComponent<Dialogue>().sentences;
            gameObject.GetComponent<Dialogue>().sentences = startingDialogue;
        }

    }

    public void displayQuestAcceptUI()
    {
        questAcceptUI.transform.Find("QuestTitle").gameObject.GetComponent<Text>().text = questTitle;
        questAcceptUI.transform.Find("Contents").GetComponent<TextMeshProUGUI>().text = questDesc;
        player.GetComponent<UnityTPS>().questAcceptUI.SetActive(true);
        player.GetComponent<UnityTPS>().setFocusToUI();
        

    }


    public void startQuestUI()
    {

        Debug.Log(objectiveCounter + 1 + " | " + objectiveList.Length);
        objectiveCounter = 0;
        debugCounter = 0;
        if (!isActive)
        {

            designatedMarkerLocation = objectiveList[0].requiredInteractionObject;
            questUI.SetActive(true);
            questUI.transform.Find("questName").gameObject.GetComponent<Text>().text = questTitle;
            questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = objectiveList[objectiveCounter].objectiveDesc;

            smartContractUI.transform.Find("QuestTitle").GetComponent<Text>().text = questTitle;
            smartContractUI.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = questDesc;
            isActive = true;
            player.GetComponent<UnityTPS>().globalQuestTracker = true;


            if (dialogueDict.Count == 0)
            {
                debugCounter++;
                int objDialogueIndex = 0;

                foreach (var x in objectiveList)
                {
                    
                    if (x.checkObjectiveState())
                    {
                        questLineText += "Complete | " + x.objectiveDesc + "\n";
                    }
                    else
                    {
                        questLineText += "Pending | " + x.objectiveDesc + "\n";
                    }
                    // foreach (var x in objective.requiredInteractionObject.GetComponent<Dialogue>().sentences)
                    // {
                    //     Debug.Log(">>>: "+ x + " | " + objective.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName());
                    // }
                    // tempDialogueBackupList.Add(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName());
                    if (!x.isDestination)
                    {
                        dialogueIndex tempDiaIndex = new dialogueIndex();
                        tempDiaIndex.dialogue = x.requiredInteractionObject.GetComponent<Dialogue>().sentences;
                        tempDiaIndex.index = objDialogueIndex;
                        KeyValuePair<string, dialogueIndex> temp = new KeyValuePair<string, dialogueIndex>(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), tempDiaIndex);
                        dialogueDict.Add(temp);

                        if (x.customDialogue.Length != 0)
                        {
                            dialogueIndex tempCustomDiaIndex = new dialogueIndex();
                            tempCustomDiaIndex.dialogue = x.customDialogue;
                            tempCustomDiaIndex.index = objDialogueIndex;

                            KeyValuePair<string, dialogueIndex> tempCustom = new KeyValuePair<string, dialogueIndex>(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), tempCustomDiaIndex);
                            customDialogueDict.Add(tempCustom);
                        }
                        else
                        {
                            dialogueIndex tempCustomDiaIndex = new dialogueIndex();
                            tempCustomDiaIndex.dialogue = x.requiredInteractionObject.GetComponent<Dialogue>().sentences;
                            tempCustomDiaIndex.index = objDialogueIndex;

                            KeyValuePair<string, dialogueIndex> tempCustom = new KeyValuePair<string, dialogueIndex>(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), tempCustomDiaIndex);
                            customDialogueDict.Add(tempCustom);
                            // customDialogueDict.Add(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), x.requiredInteractionObject.GetComponent<Dialogue>().sentences);
                        }

                        // if (x.customDialogue.Length != 0)
                        // {
                        //     x.requiredInteractionObject.GetComponent<Dialogue>().sentences = x.customDialogue;
                        // }

                    }

                    // Debug.Log("Objective state:" + x.checkChangedObjectExistence() + " | changed obj name: " + x.changedObject);


                    if (x.checkChangedObjectExistence())
                    {
                        x.initializeChangedObjectState();
                        // Debug.Log("Set variable object status should have changed. | " + x.changedObject);
                    }
                    else
                    {
                        // Debug.Log("No object to be changed for this objective.");
                    }

                    objDialogueIndex++;

                    

                }

                for (int i = 0; i < objectiveList.Length; i++){
                    objectiveList[i].setObjectiveHash("0x"+createRandomHash());
                }

                smartContractUI.transform.Find("Questline").GetComponent<TextMeshProUGUI>().text = questLineText;

            }
        }



    }

    public void updateQuestUI()
    {
        questLineText = null;
        Debug.Log(objectiveCounter + 1 + " | " + objectiveList.Length);
        if (customDialogueDict.Count != 0)
        {
            foreach (var t in objectiveList)
            {
                if (!t.isDestination)
                {
                    foreach (var u in customDialogueDict)
                    {
                        if (t.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName() == u.Key && objectiveCounter == u.Value.index)
                        {
                            t.requiredInteractionObject.GetComponent<Dialogue>().sentences = u.Value.dialogue;
                            Debug.Log("Replaced "+t.requiredInteractionObject.name+"'s dialogue.");
                        }

                    }

                }

                if (t.checkObjectiveState())
                {
                    questLineText += "Complete | " + t.objectiveDesc + "\n";
                }
                else
                {
                    questLineText += "Pending | " + t.objectiveDesc + "\n";
                }

            }
        }


        if (objectiveCounter + 1 == objectiveList.Length)
        {
            Debug.Log("Current quest is set up to finish.");
            if (parentConvoTrackerState)
            {
                Debug.Log("Quest is complete!");
                player.GetComponent<UnityTPS>().blocksCreated.Add(objectiveList[objectiveCounter].objectiveHash);
                questUI.SetActive(false);
                setQuestAsComplete();
                questCompleteIndicator.transform.Find("desc").GetComponent<TextMeshProUGUI>().text = questTitle;
                player.GetComponent<UnityTPS>().fadeInQuestComplete();
                
                questUI.transform.Find("questName").gameObject.GetComponent<Text>().text = null;
                questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = null;
                smartContractUI.transform.Find("QuestTitle").GetComponent<Text>().text = "No quest active.";
                smartContractUI.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "If you don't know what to do, talk to the manager!";
                smartContractUI.transform.Find("Questline").GetComponent<TextMeshProUGUI>().text = "No objectives.";
                player.GetComponent<UnityTPS>().currentQuest = null;
                player.GetComponent<UnityTPS>().globalQuestTracker = false;
                player.GetComponent<UnityTPS>().currentReputation += reputationReward;
                designatedMarkerLocation = null;
                // player.GetComponent<UnityTPS>().designatedBoard=player.GetComponent<UnityTPS>().pubDesUI;
                foreach (var objective in objectiveList)
                {
                    if (!objective.isDestination)
                    {
                        foreach (var dialogue in dialogueDict)
                        {
                            if (objective.requiredInteractionObject.name == dialogue.Key)
                            {
                                // Debug.Log(objective.requiredInteractionObject.name + " | " + dialogue.returnGameObjectName());
                                objective.requiredInteractionObject.GetComponent<Dialogue>().sentences = dialogue.Value.dialogue;
                            }
                        }

                    }

                }

                if (startingDialogue.Length !=0){
                    gameObject.GetComponent<Dialogue>().sentences = ogDialogue;
                }
                
                dialogueDict.Clear();
                Destroy(this);

            }
        }
        else if (objectiveCounter + 1 < objectiveList.Length)
        {
            Debug.Log("Quest is ongoing!");
            if (objectiveList[objectiveCounter].checkObjectiveState())
            {
                if (!questAnchoredToPlayer)
                {
                    if (parentConvoTrackerState)
                    {
                        player.GetComponent<UnityTPS>().blocksCreated.Add(objectiveList[objectiveCounter].objectiveHash);
                        objectiveCounter++;
                        designatedMarkerLocation = objectiveList[objectiveCounter].requiredInteractionObject;
                        questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = objectiveList[objectiveCounter].objectiveDesc;
                    }

                }
                else
                {   
                    player.GetComponent<UnityTPS>().blocksCreated.Add(objectiveList[objectiveCounter].objectiveHash);
                    objectiveCounter++;
                    designatedMarkerLocation = objectiveList[objectiveCounter].requiredInteractionObject;
                    // Debug.Log(objectiveList[objectiveCounter].objectiveDesc);
                    questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = objectiveList[objectiveCounter].objectiveDesc;
                }


            }
            smartContractUI.transform.Find("Questline").GetComponent<TextMeshProUGUI>().text = questLineText;
        }
        else
        {
            Debug.Log("Neither function condition is passed.");
        }
    }


    public bool isQuestActive()
    {
        return isActive;
    }

    public void setCurrentConvoTracker(bool state)
    {
        parentConvoTrackerState = state;
    }

    public GameObject returnIndicatedGameObject()
    {
        return objectiveList[objectiveCounter].requiredInteractionObject;
    }

    public Objective returnCurrentObjective()
    {
        return objectiveList[objectiveCounter];
    }

    public void setQuestAsComplete()
    {
        isComplete = true;

        string questTitleAppended = "";

        if (player.GetComponent<UnityTPS>().day < 10)
        {
            questTitleAppended = "0" + player.GetComponent<UnityTPS>().day + questTitle;
        }
        else
        {
            questTitleAppended = player.GetComponent<UnityTPS>().day + questTitle;
        }

        player.GetComponent<UnityTPS>().completedQuests.Add(questTitleAppended);
        player.GetComponent<UnityTPS>().completedQuestsDay.Add(questTitleAppended);
        player.GetComponent<UnityTPS>().completedQuestsSubstring.Add(questTitleAppended.Substring(2));
    }

    public bool isQuestComplete()
    {
        return isComplete;
    }

    

    public string createRandomHash(){
        var chars = "abcdef0123456789";
        char[] stringChars;
        
        stringChars = new char[8];
        
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[UnityEngine.Random.Range(0,chars.Length)];
        }
        var finalString = "";
        finalString = new string(stringChars);

        Debug.Log("Generated new hash: "+finalString);

        return finalString;
    }






}
