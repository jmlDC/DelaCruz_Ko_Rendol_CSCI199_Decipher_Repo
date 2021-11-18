using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Objective
{
    public string objectiveDesc;
    public GameObject requiredInteractionObject;

    public bool isAccomplished;

    public string[] customDialogue;


    public bool checkObjectiveState()
    {
        return isAccomplished;
    }

    public void setAccomplishedState(){
        isAccomplished = true;
    }



}
