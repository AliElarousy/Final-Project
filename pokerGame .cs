using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace PokerGame
{
    public class Deck
    {
        private List<Card> cards;
        public Deck()
        {
            cards = new List<Card>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

            foreach (var suit in suits)
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    cards.Add(new Card(ranks[i], suit, i + 2));
                }
            }
        }

        public void Shuffle()
        {
            System.Random random = new System.Random();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                // Swap cards[i] with cards[j]
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public Card DrawCard()
        {
            if (cards.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty!");
            }

            Card drawnCard = cards[0];
            cards.RemoveAt(0); // Remove the card from the deck
            return drawnCard;
        }

        public List<Card> DealHand(int numberOfCards)
        {
            List<Card> hand = new List<Card>();
            for (int i = 0; i < numberOfCards; i++)
            {
                hand.Add(DrawCard());
            }
            return hand;
        }

        // Check how many cards are left in the deck
        public int CardsRemaining()
        {
            return cards.Count;
        }

    }

    public class Card
    {
        public string Rank { get; private set; }
        public string Suit { get; private set; }
        public int RankValue { get; private set; }

        public Card(string rank, string suit, int rankValue)
        {
            Rank = rank;
            Suit = suit;
            RankValue = rankValue;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }

    public class Player
    {
        public bool isFolded;
        public bool isShortStacked;
        //// A list to store player's hand
        public List<Card> hand;

        // Attributes
        public string Role { get; set; }  // Player's name
        public float Balance { get; set; } // Player's balance (money)

        // Constructor
        public Player(string role, float initialBalance)
        {
            hand = new List<Card>();
            Role = role;
            Balance = initialBalance;
            isFolded = false;
            isShortStacked = false;
        }


        public string EvaluateHandToString(List<Card> hand)
        {
            //Ensures the cards are ordered by rank value for accurate evaluation
            hand = hand.OrderBy(card => card.RankValue).ToList();

            if (IsRoyalFlush(hand)) return "Royal Flush";
            if (IsStraightFlush(hand)) return "Straight Flush";
            if (IsFourOfAKind(hand)) return "Four of a Kind";
            if (IsFullHouse(hand)) return "Full House";
            if (IsFlush(hand)) return "Flush";
            if (IsStraight(hand)) return "Straight";
            if (IsThreeOfAKind(hand)) return "Three of a Kind";
            if (IsTwoPair(hand)) return "Two Pair";
            if (IsOnePair(hand)) return "One Pair";

            // High Card
            Card highCard = hand.OrderByDescending(card => card.RankValue).First();
            return $"High Card: {highCard.Rank}";
        }

        public int EvaluateHand(List<Card> hand)
        {
            //Ensures the cards are ordered by rank value for accurate evaluation
            hand = hand.OrderBy(card => card.RankValue).ToList();

            if (IsRoyalFlush(hand)) return 10;
            if (IsStraightFlush(hand)) return 9;
            if (IsFourOfAKind(hand)) return 8;
            if (IsFullHouse(hand)) return 7;
            if (IsFlush(hand)) return 6;
            if (IsStraight(hand)) return 5;
            if (IsThreeOfAKind(hand)) return 4;
            if (IsTwoPair(hand)) return 3;
            if (IsOnePair(hand)) return 2;

            // High card (sum of all ranks as a tiebreaker)
            return 1;
        }

        private bool IsRoyalFlush(List<Card> hand)
        {
            return IsStraightFlush(hand) && hand[4].Rank == "Ace";
        }

        private bool IsStraightFlush(List<Card> hand)
        {
            return IsFlush(hand) && IsStraight(hand);
        }

        private bool IsFourOfAKind(List<Card> hand)
        {
            return hand.GroupBy(card => card.RankValue).Any(group => group.Count() == 4);
        }

        private bool IsFullHouse(List<Card> hand)
        {
            var groups = hand.GroupBy(card => card.RankValue).ToList();
            return groups.Count == 2 && groups.Any(group => group.Count() == 3) && groups.Any(group => group.Count() == 2);
        }

        private bool IsFlush(List<Card> hand)
        {
            if (hand.Count < 5)
            {
                return false;
            }

            return hand.All(card => card.Suit == hand[0].Suit);
        }

        private bool IsStraight(List<Card> hand)
        {
            if (hand.Count < 5)
            {
                return false;
            }

            for (int i = 0; i < hand.Count - 1; i++)
            {
                if (hand[i + 1].RankValue != hand[i].RankValue + 1)
                    return false;
            }
            return true;
        }

        private bool IsThreeOfAKind(List<Card> hand)
        {
            return hand.GroupBy(card => card.RankValue).Any(group => group.Count() == 3);
        }

        private bool IsTwoPair(List<Card> hand)
        {
            return hand.GroupBy(card => card.RankValue).Count(group => group.Count() == 2) == 2;
        }

        private bool IsOnePair(List<Card> hand)
        {
            return hand.GroupBy(card => card.RankValue).Any(group => group.Count() == 2);
        }

    }

    public class pokerTable
    {
        public float ante;
        public int numberOfPlayers;
        public int currentPlayerIndex;
        public Player currentPlayerTurn;
        public List<Card> tableCards;
        public List<Player> listOfPlayers;
        public float mainpot;
        public float sidepot;
        public float currentBet;


        public pokerTable(int NumberOfPlayers)
        {
            ante = 40;
            listOfPlayers = new List<Player>();
            currentPlayerIndex = 0;
            currentPlayerTurn = listOfPlayers[0];
            tableCards = new List<Card>();
            mainpot = 0;
            sidepot = 0;
            currentBet = 40;
        }

     

        public void getCurrentPlayerTurn(Player currentPlayer)
        {
            currentPlayerTurn = currentPlayer;
        }

        public void endTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % listOfPlayers.Count();
            currentPlayerTurn = listOfPlayers[currentPlayerIndex];
        }

        public void endRound()
        {

        }

    }

}