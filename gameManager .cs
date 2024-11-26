using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PokerGame;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class gameManager : MonoBehaviour
{

    public GameObject canvas;

    public GameObject dealerPrefab;
    private GameObject dealerGameObject;

    public GameObject playerPrefab;
    private GameObject playerGameObject;
    private GameObject playerCardSpace;
    private TextMeshProUGUI playerBalance;
    private TextMeshProUGUI playerHighestRank;

    public GameObject botPrefab;
    private GameObject botGameObject;
    private GameObject botCardSpace;
    private TextMeshProUGUI botBalance;

    public GameObject cardPrefab;
    private GameObject cardGameObject;

    public List<Player> listOfPlayers;
    Player dealer, player, bot;

    private pokerTable table;



    private void drawDealer()
    {
        dealerGameObject = Instantiate(dealerPrefab, new Vector3(0, 329, 0), Quaternion.identity);
        dealerGameObject.transform.SetParent(canvas.transform, false);
    }

    private void drawPlayer()
    {
        playerGameObject = Instantiate(playerPrefab, new Vector3(0, -250, 0), Quaternion.identity);
        playerGameObject.transform.SetParent(canvas.transform, false);
        playerBalance = playerGameObject.GetComponentInChildren<TextMeshProUGUI>();
        playerBalance.text = player.Balance.ToString();
    }

    private void drawBot()
    {
        botGameObject = Instantiate(botPrefab, new Vector3(550, 0, 0), Quaternion.identity);
        botGameObject.transform.SetParent(canvas.transform, false);
        botBalance = botGameObject.GetComponentInChildren<TextMeshProUGUI>();
        botBalance.text = bot.Balance.ToString();
    }

    private void drawPlayerHand(Player p, GameObject cardSpace, bool isNpc)
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

            if (isNpc)
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

    //Player actions
    public void fold()
    {
        table.currentPlayerTurn.isFolded = true;
        table.endTurn();
    }

    public void call()
    {
        if (table.currentPlayerTurn.Balance >= table.currentBet)
        {
            table.mainpot += table.currentBet;
            table.currentPlayerTurn.Balance -= table.currentBet;
            table.endTurn();
        }
    }

    public void raise(float selectedRaiseValue)
    {
        if (table.currentPlayerTurn.Balance >= selectedRaiseValue)
        {
            table.mainpot += selectedRaiseValue;
            table.currentPlayerTurn.Balance -= selectedRaiseValue;
        }
    }

    public void allIn()
    {
        table.mainpot += table.currentPlayerTurn.Balance;
        table.currentPlayerTurn.Balance = 0;
    }

    //Bot action function
    public void botAction()
    {

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Create objects from Player class
        dealer = new Player("dealer", 0);
        player = new Player("player", 2000);
        bot = new Player("bot", 2000);
        listOfPlayers.Add(player);
        listOfPlayers.Add(bot);

        //Draw functions to show players on screen
        drawDealer();
        drawPlayer();
        drawBot();

        //Create a new deck and shuffle it
        Deck deck = new Deck();
        //deck.Shuffle();

        //Deal player hand
        player.hand = deck.DealHand(2);
        //Get player card space gameObject
        playerCardSpace = playerGameObject.transform.GetChild(3).gameObject;
        //draw player hand on screen
        drawPlayerHand(player, playerCardSpace, false);
        //[Show player cards highest rank]
        //get highest rank TextMeshPro
        playerHighestRank = playerGameObject.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        playerHighestRank.text = player.EvaluateHandToString(player.hand);

        //Deal bot hand
        bot.hand = deck.DealHand(2);
        //Get bot card space gameObject
        botCardSpace = botGameObject.transform.GetChild(2).gameObject;
        //draw bot hand on screen
        drawPlayerHand(bot, botCardSpace, true);


        //Create an object of pokerTable Class
        table = new pokerTable(2);
        table.listOfPlayers = listOfPlayers;




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
