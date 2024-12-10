using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PokerGame;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    public GameObject canvas;
    private GameObject communityGameCardSpace;

    public GameObject dealerPrefab;
    private GameObject dealerGameObject;
    private TextMeshProUGUI currentPlayerText;
    private TextMeshProUGUI potText;


    public GameObject playerPrefab;
    private GameObject playerGameObject;
    private GameObject playerCardSpace;
    private GameObject playerActionSpace;
    private TextMeshProUGUI playerChips;
    private TextMeshProUGUI playerHighestRank;

    public GameObject botPrefab;
    private GameObject botGameObject;
    private GameObject botCardSpace;
    private TextMeshProUGUI botChips;

    public GameObject cardPrefab;
    private GameObject cardGameObject;

    private Button foldButton;
    private Button callButton;
    private Button decrementRaiseButton;
    private Button raiseButton;
    private Button incrementRaiseButton;
    private Button allInButton;

    GameManager gameManager;
    HandEvaluator handEvaluator;

    private GameObject gameOverMenu;

    void initializeUI()
    {
        drawDealer();
        drawPlayer();
        drawHand(gameManager.listOfPlayers[0], playerCardSpace);
        drawBot();
        drawHand(gameManager.listOfPlayers[1], botCardSpace);
        //Get community card space
        communityGameCardSpace = canvas.transform.GetChild(1).gameObject;
        //Get gameover text
        gameOverMenu = canvas.transform.GetChild(2).gameObject;

    }
    private void drawDealer()
    {
        dealerGameObject = Instantiate(dealerPrefab, new Vector3(0, 329, 0), Quaternion.identity);
        dealerGameObject.transform.SetParent(canvas.transform, false);
        currentPlayerText = dealerGameObject.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        potText = dealerGameObject.transform.GetChild(1).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        potText.text = "Pot: $" + gameManager.pot.ToString();


    }
    private void drawPlayer()
    {
        playerGameObject = Instantiate(playerPrefab, new Vector3(0, -250, 0), Quaternion.identity);
        playerGameObject.transform.SetParent(canvas.transform, false);
        //Get player card space
        playerCardSpace = playerGameObject.transform.GetChild(3).gameObject;
        //Get player action space
        playerActionSpace = playerGameObject.transform.GetChild(4).gameObject;
        //Get player chips text
        playerChips = playerGameObject.transform.GetChild(2).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        playerChips.text = gameManager.listOfPlayers[0].chips.ToString();
        //Get player highest rank text
        playerHighestRank = playerGameObject.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        //Get Action Buttons
        foldButton = playerActionSpace.transform.GetChild(5).gameObject.GetComponent<Button>();
        callButton = playerActionSpace.transform.GetChild(4).gameObject.GetComponent<Button>();
        decrementRaiseButton = playerActionSpace.transform.GetChild(3).gameObject.GetComponent<Button>();
        raiseButton = playerActionSpace.transform.GetChild(2).gameObject.GetComponent<Button>();
        incrementRaiseButton = playerActionSpace.transform.GetChild(1).gameObject.GetComponent<Button>();
        allInButton = playerActionSpace.transform.GetChild(0).gameObject.GetComponent<Button>();
        // Attach button listeners
        foldButton.onClick.AddListener(() => PerformAction("Fold"));
        callButton.onClick.AddListener(() => PerformAction("Call"));
        decrementRaiseButton.onClick.AddListener(() => decrementRaise());
        raiseButton.onClick.AddListener(() => PerformAction("Raise"));
        incrementRaiseButton.onClick.AddListener(() => incrementRaise());
        allInButton.onClick.AddListener(() => PerformAction("AllIn"));

    }
    private void updateHighestRankText()
    {
        playerHighestRank.text = handEvaluator.EvaluateHandToString(gameManager.listOfPlayers[0].hand);
    }
    private void drawBot()
    {
        botGameObject = Instantiate(botPrefab, new Vector3(550, 0, 0), Quaternion.identity);
        botGameObject.transform.SetParent(canvas.transform, false);
        botCardSpace = botGameObject.transform.GetChild(2).gameObject;
        botChips = botGameObject.GetComponentInChildren<TextMeshProUGUI>();
        botChips.text = gameManager.listOfPlayers[1].chips.ToString();
    }
    private void drawHand(Player p, GameObject cardSpace)
    {
        //Define Sprite object
        Sprite cardSprite;

        foreach (var card in p.hand)
        {
            //Instatiate a card and set its parent to player space
            cardGameObject = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            cardGameObject.transform.SetParent(cardSpace.transform, false);
            cardGameObject.transform.localScale = new Vector3(0.47f, 0.47f, 0.47f);
            //Get image component on cardGameObject and change its sprite to the appropriate card sprite
            Image image = cardGameObject.GetComponent<Image>();

            if (p.isNpc)
            {
                cardSprite = Resources.Load<Sprite>("playing cards/" + "card back");
            }
            else
            {
                cardSprite = Resources.Load<Sprite>("playing cards/" + card.ToString());
            }

            image.sprite = cardSprite;

        }
    }

    private void drawCommunityCards(string phase)
    {

        Image image;
        Sprite cardSprite;

        switch (phase)
        {
            case "flop":
                foreach (var card in gameManager.communityCards)
                {
                    //Instatiate a card and set its parent to player space
                    cardGameObject = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    cardGameObject.transform.SetParent(communityGameCardSpace.transform, false);
                    cardGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    //Get image component on cardGameObject and change its sprite to the appropriate card sprite
                    cardSprite = Resources.Load<Sprite>("playing cards/" + card.ToString());
                    image = cardGameObject.GetComponent<Image>();
                    image.sprite = cardSprite;
                }
                break;
            case "turn":
                //Instatiate a card and set its parent to player space
                cardGameObject = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                cardGameObject.transform.SetParent(communityGameCardSpace.transform, false);
                cardGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //Get image component on cardGameObject and change its sprite to the appropriate card sprite
                image = cardGameObject.GetComponent<Image>();
                cardSprite = Resources.Load<Sprite>("playing cards/" + gameManager.communityCards[3].ToString());
                image.sprite = cardSprite;
                break;
            case "river":
                //Instatiate a card and set its parent to player space
                cardGameObject = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                cardGameObject.transform.SetParent(communityGameCardSpace.transform, false);
                cardGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //Get image component on cardGameObject and change its sprite to the appropriate card sprite
                image = cardGameObject.GetComponent<Image>();
                cardSprite = Resources.Load<Sprite>("playing cards/" + gameManager.communityCards[4].ToString());
                image.sprite = cardSprite;
                break;
        }
        

        

    }

    void PerformAction(string action)
    {
        if (action.Equals("AllIn"))
        {
            if (gameManager.listOfPlayers[0].chips > gameManager.listOfPlayers[1].chips)
            {
                gameManager.listOfPlayers[0].selectedRaiseValue = gameManager.listOfPlayers[1].chips - gameManager.currentBet;
            }
            else
            {
                gameManager.listOfPlayers[0].selectedRaiseValue = gameManager.listOfPlayers[0].chips - gameManager.currentBet;
            }

        }
        gameManager.listOfPlayers[gameManager.currentPlayerIndex].PerformAction(action, gameManager);
    }

    //Subscribe to the phase change event
    private void handlePhaseChanged(string phase)
    {
        switch(phase)
        {
            case "preFlop":
                //Draw player hand
                drawHand(gameManager.listOfPlayers[0], playerCardSpace);
                //Show player cards highest rank
                updateHighestRankText();
                //Draw bot hand
                drawHand(gameManager.listOfPlayers[1], botCardSpace);
                break;
            case "flop":
            case "turn":
            case "river":
                drawCommunityCards(phase);
                updateHighestRankText();
                break;
            case "roundEnd":
                //Remove player's cards
                foreach (Transform child in playerCardSpace.transform)
                {
                    Destroy(child.gameObject);
                }
                //Remove bot's cards
                foreach (Transform child in botCardSpace.transform)
                {
                    Destroy(child.gameObject);
                }
                //Remove community's cards
                foreach (Transform child in communityGameCardSpace.transform)
                {
                    Destroy(child.gameObject);
                }
                potText.text = gameManager.pot.ToString();
                playerHighestRank.text = "Highest Rank";
                currentPlayerText.text = "";
                playerChips.text = gameManager.listOfPlayers[0].chips.ToString();
                botChips.text = gameManager.listOfPlayers[1].chips.ToString();
                break;
        }
    }

    private void handleStartTurn()
    {

        currentPlayerText.text = gameManager.listOfPlayers[gameManager.currentPlayerIndex].name+"'s Turn";

        if (!gameManager.listOfPlayers[gameManager.currentPlayerIndex].isNpc)
        {
            playerActionSpace.SetActive(true);
            if (!(gameManager.currentPhase == GameManager.Phase.preFlop) && gameManager.currentBet == 0)
            {
                callButton.GetComponentInChildren<TextMeshProUGUI>().text = "CHECK";
                callButton.onClick.RemoveAllListeners();
                callButton.onClick.AddListener(() => PerformAction("Check"));
            }
            else
            {
                callButton.GetComponentInChildren<TextMeshProUGUI>().text = "CALL \n" + gameManager.currentBet.ToString();
                callButton.onClick.RemoveAllListeners();
                callButton.onClick.AddListener(() => PerformAction("Call"));
                

            }
            raiseButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + gameManager.listOfPlayers[0].selectedRaiseValue.ToString();

        }

    }

    private void handleEndTurn()
    {
        if (!gameManager.listOfPlayers[gameManager.currentPlayerIndex].isNpc)
        {
            playerActionSpace.SetActive(false);
        }

        potText.text = "Pot: $" + gameManager.pot.ToString();
        playerChips.text = gameManager.listOfPlayers[0].chips.ToString();
        botChips.text = gameManager.listOfPlayers[1].chips.ToString();
    }

    void incrementRaise()
    {
        gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue += 40;

        if (gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue + gameManager.currentBet >= gameManager.listOfPlayers[gameManager.currentPlayerIndex].chips)
        {
            gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue = gameManager.listOfPlayers[gameManager.currentPlayerIndex].chips - gameManager.currentBet;
            incrementRaiseButton.interactable = false;
        }

        raiseButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue).ToString();
        decrementRaiseButton.interactable = true;
    }

    void decrementRaise()
    {
        gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue -= 40;

        if (gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue <= gameManager.minimumBet)
        {
            gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue = gameManager.minimumBet;
            decrementRaiseButton.interactable = false;
        }

        raiseButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue).ToString();
        incrementRaiseButton.interactable = true;
    }

    private void handleGameOver()
    {
        Destroy(dealerGameObject);
        Destroy(playerGameObject);
        Destroy(botGameObject);
        foreach (Transform child in communityGameCardSpace.transform)
        {
            Destroy(child.gameObject);
        }
        gameOverMenu.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = new GameManager();
        handEvaluator = new HandEvaluator();
        gameManager.initializeGame();
        initializeUI();

        // Subscribe to the phase change event
        gameManager.OnPhaseChanged += handlePhaseChanged;
        gameManager.OnTurnStart += handleStartTurn;
        gameManager.OnTurnEnd += handleEndTurn;
        gameManager.onGameOver += handleGameOver;

        gameManager.StartPreFlopPhase();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
