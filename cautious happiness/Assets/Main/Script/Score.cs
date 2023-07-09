using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text text;

    public GameProgressionManager progression;

    public GameObject endScreen;

    public TMP_Text finalScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Score: " + progression.score;

        finalScore.text = "Final Score: " + (progression.score - 1);
    }

    public void ActivateEndScreen()
    {
        endScreen.SetActive((true));
    }
}
