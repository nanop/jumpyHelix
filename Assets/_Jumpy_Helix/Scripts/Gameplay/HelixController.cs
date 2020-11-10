﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixController : MonoBehaviour {

    [SerializeField] private HelixPieceController[] helixPieces = null;

    public void HandleHelix(int disablePieces, int deadPieces, Color nomalPieceColor, Color deadPieceColor)
    {
        StartCoroutine(HandlingHelix(disablePieces, deadPieces, nomalPieceColor, deadPieceColor));
    }
    private IEnumerator HandlingHelix(int disablePieces, int deadPieces, Color nomalPieceColor, Color deadPieceColor)
    {
        List<HelixPieceController> listHelixPieceControl = new List<HelixPieceController>();
        foreach (HelixPieceController o in helixPieces)
        {
            listHelixPieceControl.Add(o);
        }

        //Handle disable pieces
        while (disablePieces > 0)
        {
            int index = Random.Range(0, listHelixPieceControl.Count);
            listHelixPieceControl[index].Disable();
            listHelixPieceControl.Remove(listHelixPieceControl[index]);
            disablePieces--;
            yield return null;
        }

        //Handle dead pieces
        while (deadPieces > 0)
        {
            int index = Random.Range(0, listHelixPieceControl.Count);
            listHelixPieceControl[index].SetDeadPiece();
            listHelixPieceControl.Remove(listHelixPieceControl[index]);
            deadPieces--;
            yield return null;
        }

        //Handle normal pieces
        foreach (HelixPieceController o in listHelixPieceControl)
        {
            o.SetNormalPiece();
        }
    }

    public void ShatterAllPieces()
    {
        foreach(HelixPieceController o in helixPieces)
        {
            o.Shatter();
        }
        GameManager.Instance.CreateFadingHelix(transform.position);
    }
}
