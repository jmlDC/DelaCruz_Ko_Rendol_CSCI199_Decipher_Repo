using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


[System.Serializable]
public class Dialogue : MonoBehaviour
{
    
    [TextArea(3,10)]
    public string[] sentences;
    public string name;

    public string gameObjectName;


    void Start(){
        getObjectName();

        try{
            gameObject.transform.Find("npcText").GetComponent<TMP_Text>().text = name;
            Debug.Log("Applied name to floating tag: "+name);
        } catch (Exception e){
            Debug.Log("No npcText element inside gameObject.");
        }
        
    }
    void getObjectName(){
        gameObjectName = gameObject.name;  
    }

    public string returnName(){
        return name;
    }

    public string returnGameObjectName(){
        return gameObjectName;
    }

    public string[] returnDialogue(){
        return sentences;
    }
    
}
