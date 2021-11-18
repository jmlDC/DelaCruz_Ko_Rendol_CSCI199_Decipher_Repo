using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestGiver : MonoBehaviour
{

    public CharacterController player;


    private GameObject questUI;

    [SerializeField]
    public string questTitle;
    [SerializeField]
    public string questDesc;

    [SerializeField]
    public string[] startingDialogue;
    public string[] ogDialogue;

    [SerializeField]
    public Objective[] objectiveList;



    [SerializeField]
    public bool isActive;
    [SerializeField]
    public int reputationReward;

    public int objectiveCounter;

    public bool isComplete;

    public bool parentConvoTrackerState;

    public Dictionary<string, string[]> dialogueDict = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> customDialogueDict = new Dictionary<string, string[]>();

    public int debugCounter;

    public GameObject designatedMarkerLocation;

    void Start()
    {
        questUI = player.GetComponent<UnityTPS>().questUI;
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

            if (Vector3.Dot((designatedMarkerLocation.transform.position - player.transform.position), player.transform.forward) < 0)
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
            string stringifiedDistance = ((int)Vector3.Distance(designatedMarkerLocation.transform.position, player.transform.position)).ToString()+"m";
            questUI.transform.Find("Marker/Distance").GetComponent<Text>().text = stringifiedDistance;
            questUI.transform.Find("uiDistance").GetComponent<Text>().text = stringifiedDistance;
            player.GetComponent<UnityTPS>().waypointMarker.transform.position = waypointPosition;
        }
    }
    public void startQuestUI()
    {
        objectiveCounter = 0;
        debugCounter = 0;
        if (!isActive)
        {

            designatedMarkerLocation = objectiveList[0].requiredInteractionObject;
            questUI.SetActive(true);
            questUI.transform.Find("questName").gameObject.GetComponent<Text>().text = questTitle;
            questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = objectiveList[objectiveCounter].objectiveDesc;
            isActive = true;
            player.GetComponent<UnityTPS>().globalQuestTracker = true;

            if (startingDialogue.Length != 0)
            {
                ogDialogue = gameObject.GetComponent<Dialogue>().sentences;
            }



            if (dialogueDict.Count == 0)
            {
                debugCounter++;
                foreach (var x in objectiveList)
                {
                    // foreach (var x in objective.requiredInteractionObject.GetComponent<Dialogue>().sentences)
                    // {
                    //     Debug.Log(">>>: "+ x + " | " + objective.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName());
                    // }
                    // tempDialogueBackupList.Add(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName());
                    dialogueDict.Add(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), x.requiredInteractionObject.GetComponent<Dialogue>().sentences);
                    if (x.customDialogue.Length != 0)
                    {
                        customDialogueDict.Add(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), x.customDialogue);
                    }
                    else
                    {
                        customDialogueDict.Add(x.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName(), x.requiredInteractionObject.GetComponent<Dialogue>().sentences);
                    }

                    if (x.customDialogue.Length != 0)
                    {
                        x.requiredInteractionObject.GetComponent<Dialogue>().sentences = x.customDialogue;
                    }
                }

                if (startingDialogue.Length != 0)
                {
                    gameObject.GetComponent<Dialogue>().sentences = startingDialogue;
                }

            }
        }

    }

    public void updateQuestUI()
    {
        Debug.Log(objectiveCounter + 1 + " | " + objectiveList.Length);
        if (customDialogueDict.Count != 0)
        {
            foreach (var t in objectiveList)
            {
                foreach (var u in customDialogueDict)
                {
                    if (t.requiredInteractionObject.GetComponent<Dialogue>().returnGameObjectName() == u.Key)
                    {
                        t.requiredInteractionObject.GetComponent<Dialogue>().sentences = u.Value;
                    }

                }

            }
        }


        if (objectiveCounter + 1 == objectiveList.Length)
        {
            if (parentConvoTrackerState)
            {
                Debug.Log("Quest is complete!");
                questUI.SetActive(false);
                setQuestAsComplete();
                questUI.transform.Find("questName").gameObject.GetComponent<Text>().text = null;
                questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = null;
                player.GetComponent<UnityTPS>().currentQuest = null;
                player.GetComponent<UnityTPS>().globalQuestTracker = false;
                player.GetComponent<UnityTPS>().currentReputation += reputationReward;
                designatedMarkerLocation = null;
                // player.GetComponent<UnityTPS>().designatedBoard=player.GetComponent<UnityTPS>().pubDesUI;
                foreach (var objective in objectiveList)
                {
                    foreach (var dialogue in dialogueDict)
                    {
                        if (objective.requiredInteractionObject.name == dialogue.Key)
                        {
                            // Debug.Log(objective.requiredInteractionObject.name + " | " + dialogue.returnGameObjectName());
                            objective.requiredInteractionObject.GetComponent<Dialogue>().sentences = dialogue.Value;
                        }
                    }
                }

                if (startingDialogue.Length != 0)
                {
                    gameObject.GetComponent<Dialogue>().sentences = ogDialogue;
                }
                dialogueDict.Clear();

            }
        }
        else if (objectiveCounter + 1 < objectiveList.Length)
        {
            Debug.Log("Quest is ongoing!");
            if (objectiveList[objectiveCounter].checkObjectiveState())
            {
                if (parentConvoTrackerState)
                {
                    objectiveCounter++;
                    designatedMarkerLocation = objectiveList[objectiveCounter].requiredInteractionObject;
                    questUI.transform.Find("Quest description/objDesc").gameObject.GetComponent<Text>().text = objectiveList[objectiveCounter].objectiveDesc;
                }

            }
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

        if( player.GetComponent<UnityTPS>().day < 10){
            questTitleAppended = "0"+player.GetComponent<UnityTPS>().day+questTitle;
        } else {
            questTitleAppended = player.GetComponent<UnityTPS>().day+questTitle;
        }

        player.GetComponent<UnityTPS>().completedQuests.Add(questTitleAppended);
        player.GetComponent<UnityTPS>().completedQuestsDay.Add(questTitleAppended);
        player.GetComponent<UnityTPS>().completedQuestsSubstring.Add(questTitleAppended.Substring(2));
    }

    public bool isQuestComplete()
    {
        return isComplete;
    }




}
