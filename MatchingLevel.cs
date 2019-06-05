using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/*
 * In this level, player has to match two same relating cards by clicking the them. 30 Seconds will be given
 * and 5 excused trials. If they miss more than five trials, they lose. If they finish matching up all the
 * cards, they win. 
*/

public class MatchingLevel : MonoBehaviour
{

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject gameOver;

    //timer variables
    public Button startButton;
    public Text timeText;
    public int minutes;
    public int sec;
    private int totalSeconds = 0;
    public bool isStart;

    //csv parser variables
    private CSVParser csvReader;

    //button variables
    public Sprite bgImage;
    public List<Button> btns = new List<Button>();
    private bool firstPick, secondPick;
    private int firstIndex, secondIndex;
    private int counter;
    private int correctPair;
    private int numOfGuess;
    private string firstCardTag, secondCardTag;

    public int trialcount;
    public Text trial;




    void Start()
    {
        csvReader = GetComponent<CSVParser>();
        isStart = false;
        trialcount = 0;
        InitPuzzle();
    }

    //display number of wrong guesses
    void UpdateTrial()
    {
        trial.text = "Total wrong guess: " + trialcount;
    }

    // starting timer
    public void StartTimer()
    {
        isStart = true;
        startButton.interactable = false;
    }


    private void Update()
    {
        UpdateTrial();
        TimerSetting();

        //update timer
        if (sec == 0 && minutes == 0)
        {
            timeText.text = "Time's Up!";
            StopCoroutine(Second());
            timeText.GetComponent<Text>().enabled = false;
            loseScreen.SetActive(true);

        }
    }

    // if player clicks the button, picking a puzzle
    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickAPuzzle());
        }
    }

    //initializing the cards
    void InitPuzzle()
    {
        // get data from csv file
        Dictionary<string, string> dataDictionary = new Dictionary<string,string>(csvReader.kvArrays[2]); //{key, value}, {key, Value}...
        string[] keys = dataDictionary.Keys.ToArray();
        string[] vals = dataDictionary.Values.ToArray();
        var var = new List<string>(keys.Length + vals.Length);

        for (int i = 0; i < dataDictionary.Count; i++) //adding keys and values to the list
        {
            var.Add(keys[i]);
            var.Add(vals[i]);
        }

        //change each button's text with data from csv 
        btns[0].GetComponentInChildren<Text>().text = var[0]; //pair (0,3) (1,7) (4,5) (2,6)
        btns[1].GetComponentInChildren<Text>().text = var[2]; 
        btns[2].GetComponentInChildren<Text>().text = var[4]; 
        btns[3].GetComponentInChildren<Text>().text = var[1]; 
        btns[4].GetComponentInChildren<Text>().text = var[6];
        btns[5].GetComponentInChildren<Text>().text = var[7];
        btns[6].GetComponentInChildren<Text>().text = var[5]; 
        btns[7].GetComponentInChildren<Text>().text = var[3]; 

    }

    // when player picks the card, their picks's tag will be stored
    void PickAPuzzle()
    {
        if (!firstPick)
        {
            firstPick = true;
            firstIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstCardTag = btns[firstIndex].tag;
        } else if (!secondPick)
        {
            secondPick = true;
            secondIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondCardTag = btns[secondIndex].tag;
            counter++;
            StartCoroutine(CheckPuzzlePick());
        }
    }

    //checking their picks if they match
    IEnumerator CheckPuzzlePick()
    {
        yield return new WaitForSeconds(.6f);
        if (firstCardTag == secondCardTag)
        {
            yield return new WaitForSeconds(.3f);
            btns[firstIndex].interactable = false; //if correct, buttons get disabled
            btns[secondIndex].interactable = false;

            btns[firstIndex].image.color = new Color(0, 0, 0); // button will disappear
            btns[secondIndex].image.color = new Color(0, 0, 0);

            btns[firstIndex].GetComponentInChildren<Text>().text = " ";
            btns[secondIndex].GetComponentInChildren<Text>().text = " ";

            CheckStatus(); //check if it is last pick or chance

        } else { //if they are not matching
            trialcount++;
            if (counter == 5)
            {
                timeText.GetComponent<Text>().enabled = false;
                loseScreen.SetActive(true);
            }

        }
        firstPick = secondPick = false;
    }

    //check if player finished the game
    void CheckStatus()
    {
        correctPair++;
        if (correctPair == numOfGuess)
        {
            StartCoroutine(Second());
            timeText.GetComponent<Text>().enabled = false;
            winScreen.SetActive(true);
        }
    }


    // setting the timer
    void TimerSetting()
    {
        if (isStart)
        {
            AddListeners();
            numOfGuess = btns.Count / 2;
            timeText.text = minutes + " : " + sec;
            if (minutes > 0)
            {
                totalSeconds += minutes * 60;
            }
            if (sec > 0)
            {
                totalSeconds += sec;
            }
            StartCoroutine(Second());
            isStart = false;
        }
    }

    // setting the second for timer
    IEnumerator Second()
    {
        yield return new WaitForSeconds(1f);

        if (sec > 0)
        {
            sec--;
        }

        if (sec == 0 && minutes != 0)
        {
            sec = 15;
            minutes--;
        }

        timeText.text = minutes + " : " + sec;
        StartCoroutine(Second());
    }

}
