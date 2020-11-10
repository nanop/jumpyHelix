using OnefallGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixDetector : MonoBehaviour {


    public int PassedCount { private set; get; }

    private GameObject currentHelix = null;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            GameObject parent = other.transform.parent.gameObject;
            if (parent != currentHelix)
            {
                ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.passedPieces);
                currentHelix = parent;
                PassedCount++;
            }
        }
    }

    public void ResetPassedCount()
    {
        PassedCount = 0;
    }
}
