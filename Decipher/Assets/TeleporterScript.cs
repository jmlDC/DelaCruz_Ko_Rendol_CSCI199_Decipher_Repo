using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public GameObject player;

    public Vector3 tpLocation;

    public void teleportPlayer(){
        player.GetComponent<UnityTPS>().controller.transform.position = tpLocation;
    }

}
