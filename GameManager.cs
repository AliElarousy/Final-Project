using PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameManager
{
    public HandEvaluator handEvaluator;
    public List<Player> listOfPlayers;
    Deck deck;
    public int currentPlayerIndex;
    public int minimumBet;
    public int currentBet;
    public int pot;
    public int totalChipsInPlay;

    public List<Card> communityCards;
    public enum Phase
    {
        preFlop,
        flop,
        turn,
        river,
        roundEnd
    }

    public Phase currentPhase;

    // Event for phase change
    public event Action<string> OnPhaseChanged;
    // Event for turn start
    public event Action OnTurnStart;
    //Event for turn end
    public event Action OnTurnEnd;
    //Event for GameOver
    public event Action onGameOver;

    public void initializeGame()
    {
        //Initialize deck and shuffle it
        deck = new Deck();
        deck.Shuffle();
        //initialize players
        listOfPlayers = new List<Player>
        {
            new Player("Ali",false),
            new Player("Jessica", true)
        };


        //Initialize community cards
        communityCards = new List<Card>();

        currentPlayerIndex = 0;
        minimumBet = 40;
        currentBet = 0;
        pot = 0;

        // Calculate total chips in play
        totalChipsInPlay = listOfPlayers.Sum(player => player.chips);

        handEvaluator = new HandEvaluator();
    }

    public void DealInitialCards()
    {
        foreach (var player in listOfPlayers)
        {
            player.hand = deck.DealHand(2);
        }
    }

    public void StartTurn()
    {
        OnTurnStart?.Invoke();

        Player currentPlayer = listOfPlayers[currentPlayerIndex];
        Debug.Log($"{currentPlayer.name}'s turn!");

        if (!listOfPlayers[currentPlayerIndex].isActive)
        {
            EndTurn(); // Skip folded players
            return;
        }

        if (currentPlayer.isNpc)
        {
            currentPlayer.TakeTurn(this);
        }
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke();
        // Move to the next player
        do
        {
            listOfPlayers[currentPlayerIndex].selectedRaiseValue = minimumBet;
            currentPlayerIndex = (currentPlayerIndex + 1) % listOfPlayers.Count;
        }
        while (!listOfPlayers[currentPlayerIndex].isActive); // Skip inactive (folded) players

        // Check if only one player remains
        if (listOfPlayers.Count(player => player.isActive) == 1)
        {
            Debug.Log("Only one player remains. Ending the round.");
            currentPhase = Phase.roundEnd;
            EndRound(); // End the round if only one player is left
            return;
        }


        // Check if all players have acted in this phase
        if (listOfPlayers.All(player => player.hasActed || !player.isActive))
        {
            currentBet = minimumBet;
            currentPhase++;

            switch (currentPhase)
            {
                case Phase.flop:
                    StartFlopPhase();
                    break;
                case Phase.turn:
                    StartTurnPhase();
                    break;
                case Phase.river:
                    StartRiverPhase();
                    break;
                case Phase.roundEnd:
                    EndRound();
                    break;
            }

        }
        else
        {
            // Start the next player's turn
            StartTurn();
        }
    }

    public void StartPreFlopPhase()
    {
        currentPhase = Phase.preFlop;
        //Deal initial cards
        DealInitialCards();
        currentBet = 40;

        OnPhaseChanged?.Invoke(currentPhase.ToString());
        StartTurn();
    }

    private void StartFlopPhase()
    {


        currentBet = 0;
        // Deal the first three community cards (flop)
        //Also add these cards to each player's hand
        for (int i = 0; i < 3; i++)
        {
            Card flopCard = deck.DrawCard();
            communityCards.Add(flopCard);
            foreach(Player player in listOfPlayers)
            {
                player.hand.Add(flopCard);
            }
        }

        // Reset player states for the Flop phase
        foreach (var player in listOfPlayers)
        {
            if (player.isActive)
            {
                player.hasActed = false; // Reset action state for new betting round
            }
        }

        OnPhaseChanged?.Invoke(currentPhase.ToString());

        // Start the first player's turn
        currentPlayerIndex = listOfPlayers.FindIndex(player => player.isActive);
        StartTurn();
    }

    private void StartTurnPhase()
    {

        currentBet = 0;

        //Deal the fourth community card and add it to each player's hand
        Card flopCard = deck.DrawCard();
        communityCards.Add(flopCard);
        foreach (Player player in listOfPlayers)
        {
            player.hand.Add(flopCard);
        }

        // Reset player states for the turn phase
        foreach (var player in listOfPlayers)
        {
            if (player.isActive)
            {
                player.hasActed = false; // Reset action state for new betting round
            }
        }

        OnPhaseChanged?.Invoke(currentPhase.ToString());

        // Start the first player's turn
        currentPlayerIndex = listOfPlayers.FindIndex(player => player.isActive);
        StartTurn();
    }

    private void StartRiverPhase()
    {

        currentBet = 0;

        //Deal the fourth community card and add it to each player's hand
        Card flopCard = deck.DrawCard();
        communityCards.Add(flopCard);
        foreach (Player player in listOfPlayers)
        {
            player.hand.Add(flopCard);
        }

        // Reset player states for the turn phase
        foreach (var player in listOfPlayers)
        {
            if (player.isActive)
            {
                player.hasActed = false; // Reset action state for new betting round
            }
        }

        OnPhaseChanged?.Invoke(currentPhase.ToString());

        // Start the first player's turn
        currentPlayerIndex = listOfPlayers.FindIndex(player => player.isActive);
        StartTurn();
    }


    public void EndRound()
    {
        bool currentBullet;
        // Determine the winner
        //If one is folded, the other wins by default
        if (listOfPlayers.Count(player => player.isActive) == 1)
        {
            listOfPlayers.SingleOrDefault(player => player.isActive).chips += pot;
            Debug.Log(listOfPlayers.SingleOrDefault(player => !player.isActive).name + " folded. " + listOfPlayers.SingleOrDefault(player => player.isActive).name + " wins. Current chips: "+ listOfPlayers.SingleOrDefault(player => player.isActive).chips);
        }
        else
        {
            int result = handEvaluator.CompareHands(listOfPlayers[0].hand, listOfPlayers[1].hand);
            if (result == 1)
            {
                //player wins
                listOfPlayers[0].chips += pot;
                Debug.Log(listOfPlayers[0].name + " wins. Current chips: " + listOfPlayers[0].chips);
                currentBullet = listOfPlayers[1].PullRevolver();
                if (currentBullet)
                {
                    Debug.Log(listOfPlayers[1].name + " shot her gun and it was loaded. " + listOfPlayers[0].name + " wins.");
                    onGameOver?.Invoke();
                }
                else
                {
                    Debug.Log(listOfPlayers[1].name + " shot his gun and it was empty.");
                }
            }
            else if (result == -1)
            {
                //Bot wins
                listOfPlayers[1].chips += pot;
                Debug.Log(listOfPlayers[1].name + " wins. Current chips: " + listOfPlayers[1].chips);
                currentBullet = listOfPlayers[0].PullRevolver();
                if (currentBullet)
                {
                    Debug.Log(listOfPlayers[0].name + " shot his gun and it was loaded. " + listOfPlayers[1].name + " wins.");
                    onGameOver?.Invoke();
                }
                else
                {
                    Debug.Log(listOfPlayers[0].name + " shot his gun and it was empty.");
                }

            }
            else if (result == 0)
            {
                //Tie
                //Split bot
                listOfPlayers[0].chips += pot / 2;
                listOfPlayers[1].chips += pot / 2;
                Debug.Log("Tie. player chips: " + listOfPlayers[0].chips + " pot chips: " + listOfPlayers[1].chips);

            }
        }


        if (checkGameOver() != null)
        {
            onGameOver?.Invoke();
        }



        // Reset player states for the next round
        foreach (var player in listOfPlayers)
        {
            player.hand.Clear();
            player.selectedRaiseValue = minimumBet;
            player.hasActed = false;
            player.isActive = true; // Reactivate players for the next round
        }

        //Reset community cards
        communityCards.Clear();
        // Shuffle the deck and reset the pot
        deck = new Deck();
        deck.Shuffle();
        ResetPot();

        OnPhaseChanged?.Invoke(currentPhase.ToString());

        // Start the next round
        currentPlayerIndex = 0;
        minimumBet = 40;
        currentBet = 40;
        DealInitialCards();
        currentPhase = Phase.preFlop;
        OnPhaseChanged?.Invoke(currentPhase.ToString());
        StartTurn();
    }

    public Player checkGameOver()
    {
        // Check if any player has all the chips
        Player winningPlayer = listOfPlayers.FirstOrDefault(player => player.chips == totalChipsInPlay);

        if (winningPlayer != null)
        {
            Debug.Log($"Game Over! {winningPlayer.name} wins with all {totalChipsInPlay} chips.");
        }
        return winningPlayer;
    }

    public int GetPot()
    {
        return pot;
    }

    public void ResetPot()
    {
        pot = 0;
    }

    public bool AllPlayersHaveActed()
    {
        return listOfPlayers.TrueForAll(player => player.hasActed);
    }

}