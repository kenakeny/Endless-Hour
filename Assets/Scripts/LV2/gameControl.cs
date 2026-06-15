using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameControl : MonoBehaviour


{
    //unity 2 main
    public static bool youWin = false; //tracks the wining state, static belongs to the class not to an instance
    public Transform[] pieces; //array assiging the peices 3ndk f unity el 9 peices
    public Text winText; 

    void Start()
    {
        youWin = false; 
        if (winText != null) // ui text connected //teamates//nullReferenceException, game crashes or freezes
        {//s3at other scripts momken heya elly tdestroy el object bel8alat
            winText.gameObject.SetActive(false);// el wintext bta3et el gameobject  mt4a8laha4 
        }
    }

    void Update()
    {
        if (!youWin)//law lessa mksb4 checkforwin
        {
            CheckForWin();
        }
    }

    void CheckForWin()
    {
        int correctPieces = 0;// counter for the peoces in the correct position

        foreach (Transform piece in pieces) //loop 3la kol peice f le array
        {
            //
            // Use an angle tolerance instead of exact float equality (eulerAngles drift to 359.999/0.001)
            if (Mathf.Abs(Mathf.DeltaAngle(piece.rotation.eulerAngles.z, 0f)) < 1f)//Quaternion->rotaion data
            {
                correctPieces++;
            }
        }

        if (correctPieces == pieces.Length)//9=9
        {
            youWin = true;
            Debug.Log("YOU WIN!");
            SceneManager.LoadScene("LV2Scene1PLZWork"); // load by name (was hard-coded index 6)

            if (winText != null) // ui text connected 
            {
                winText.gameObject.SetActive(true);//visiblleee
            }

        }
    }
}