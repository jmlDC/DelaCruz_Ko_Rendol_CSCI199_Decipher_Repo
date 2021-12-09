using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Objective
{
    public string objectiveDesc;
    public GameObject requiredInteractionObject;

    public GameObject changedObject;

    // true = appear when objective is finished, false = disappear after objective
    public bool trueForDisappear;

    public bool isDestination;

    public bool isAccomplished;

    public string[] customDialogue;

    public string objectiveHash;

    public void changeObjectState()
    {
        changedObject.SetActive(!trueForDisappear);
    }

    public void initializeChangedObjectState()
    {
        changedObject.SetActive(trueForDisappear);
    }

    public bool checkObjectiveState()
    {
        return isAccomplished;
    }

    public bool checkChangedObjectExistence()
    {
        if (changedObject != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void setAccomplishedState()
    {
        isAccomplished = true;
        if (checkChangedObjectExistence())
        {
            changeObjectState();
        }
    }

    public void setObjectiveHash(string hash){
        objectiveHash = hash;
    }




}
