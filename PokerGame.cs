using System;
using System.Collections.Generic;
using System.Linq;
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
        public string name;
        public bool isNpc;
        public List<Card> hand;
        public int chips;
        public int selectedRaiseValue;
        public bool hasActed;
        public bool isActive;
        // Constructor
        public Player(string Name, bool IsNpc)
        {
            hand = new List<Card>();
            name = Name;
            isNpc = IsNpc;
            chips = 2000;
            selectedRaiseValue = 40;
            hasActed = false;
            isActive = true;
        }



        public void TakeTurn(GameManager gameManager)
        {
            Debug.Log("TakeTurn function");

            if (gameManager.currentBet == 0)
            {
                PerformAction("Check", gameManager);
            }
            else
            {
                // For simplicity, simulate an action (replace with UI later)
                PerformAction("Call", gameManager);

            }
            
        }

        public void PerformAction(string action, GameManager gameManager)
        {
            switch (action)
            {

                case "Call":
                    Call(gameManager);
                    break;
                case "Check":
                    Check();
                    break;
                case "Raise":
                    Raise(gameManager, gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue);
                    break;
                case "Fold":
                    Fold();
                    break;
                case "AllIn":
                    Raise(gameManager, gameManager.listOfPlayers[gameManager.currentPlayerIndex].selectedRaiseValue);
                    break;
            }

            hasActed = true;
            gameManager.EndTurn();
        }

        void Call(GameManager gameManager)
        {
            if (gameManager.currentBet > chips)
            {
                Debug.Log($"{name} doesn't have enough chips to call {gameManager.currentBet}!");
                return;
            }

            chips -= gameManager.currentBet;
            gameManager.pot += gameManager.currentBet;
            Debug.Log($"(CALL) {name} calls {gameManager.currentBet} chips!");
        }

        void Raise(GameManager gameManager, int raiseAmount)
        {
            gameManager.currentBet += raiseAmount;
            chips -= gameManager.currentBet;
            gameManager.pot += gameManager.currentBet;

            Debug.Log($"{name} raises by {raiseAmount}. Current bet is now {gameManager.currentBet}. Remaining chips: {chips}.");

            // Mark the player as having acted this turn
            hasActed = true;

            // Reset hasActed for other players who need to act on the new raise
            foreach (Player otherPlayer in gameManager.listOfPlayers)
            {
                if (otherPlayer != this && otherPlayer.isActive)
                {
                    otherPlayer.hasActed = false;
                }
            }


        }

        void Fold()
        {
            Debug.Log($"(FOLD) {name} folds!");
            isActive = false;
        }

        void Check()
        {
            Debug.Log($"(CHECK) {name} checks!");
        }

    }

    public class HandEvaluator
    {
        public enum HandRank
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
            RoyalFlush
        }

        public class EvaluatedHand
        {
            public HandRank Rank { get; set; }
            public List<int> HighCards { get; set; } // For tie-breaking (e.g., kicker cards)

            public EvaluatedHand(HandRank rank, List<int> highCards)
            {
                Rank = rank;
                HighCards = highCards;
            }
        }
        // Compare two hands and return the winner
        public int CompareHands(List<Card> hand1, List<Card> hand2)
        {
            EvaluatedHand evaluatedHand1 = EvaluateHand(hand1);
            EvaluatedHand evaluatedHand2 = EvaluateHand(hand2);

            if (evaluatedHand1.Rank > evaluatedHand2.Rank)
                return 1; // hand1 wins
            if (evaluatedHand1.Rank < evaluatedHand2.Rank)
                return -1; // hand2 wins

            // If ranks are the same, compare high cards
            for (int i = 0; i < evaluatedHand1.HighCards.Count; i++)
            {
                if (evaluatedHand1.HighCards[i] > evaluatedHand2.HighCards[i])
                    return 1;
                if (evaluatedHand1.HighCards[i] < evaluatedHand2.HighCards[i])
                    return -1;
            }

            return 0; // Tie
        }

        public static IEnumerable<List<Card>> GetCombinations(List<Card> hand, int combinationSize)
        {
            if (combinationSize == 0) yield return new List<Card>();
            else if (hand.Count == 0) yield break;
            else
            {
                // Include the first card in the combination
                foreach (var combination in GetCombinations(hand.Skip(1).ToList(), combinationSize - 1))
                {
                    combination.Insert(0, hand[0]);
                    yield return combination;
                }

                // Exclude the first card from the combination
                foreach (var combination in GetCombinations(hand.Skip(1).ToList(), combinationSize))
                {
                    yield return combination;
                }
            }
        }

        public string EvaluateHandToString(List<Card> hand)
        {
            // Ensure the hand is sorted for consistency (not strictly necessary but can help)
            hand = hand.OrderBy(card => card.RankValue).ToList();

            // Generate all 5-card combinations
            var fiveCardCombinations = GetCombinations(hand, 5);

            string bestHand = "High Card "+hand.Last();
            int bestRank = 0;

            // Evaluate each 5-card combination
            foreach (var combination in fiveCardCombinations)
            {
                if (IsRoyalFlush(combination)) { bestHand = "Royal Flush"; bestRank = 10; }
                else if (IsStraightFlush(combination) && bestRank < 9) { bestHand = "Straight Flush"; bestRank = 9; }
                else if (IsFourOfAKind(combination) && bestRank < 8) { bestHand = "Four of a Kind"; bestRank = 8; }
                else if (IsFullHouse(combination) && bestRank < 7) { bestHand = "Full House"; bestRank = 7; }
                else if (IsFlush(combination) && bestRank < 6) { bestHand = "Flush"; bestRank = 6; }
                else if (IsStraight(combination) && bestRank < 5) { bestHand = "Straight"; bestRank = 5; }
                else if (IsThreeOfAKind(combination) && bestRank < 4) { bestHand = "Three of a Kind"; bestRank = 4; }
                else if (IsTwoPair(combination) && bestRank < 3) { bestHand = "Two Pair"; bestRank = 3; }
                else if (IsOnePair(combination) && bestRank < 2) { bestHand = "One Pair"; bestRank = 2; }
            }

            return bestHand;
        }

        public EvaluatedHand EvaluateHand(List<Card> hand)
        {
            // Ensure the hand is sorted for consistency
            hand = hand.OrderBy(card => card.RankValue).ToList();

            // Generate all possible 5-card combinations
            var fiveCardCombinations = GetCombinations(hand, 5);

            EvaluatedHand bestHand = null;

            // Evaluate each combination and track the best hand
            foreach (var combination in fiveCardCombinations)
            {
                List<int> sortedRanks = combination.Select(card => card.RankValue).OrderByDescending(rank => rank).ToList();

                if (IsRoyalFlush(combination) && (bestHand == null || HandRank.RoyalFlush > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.RoyalFlush, sortedRanks);
                else if (IsStraightFlush(combination) && (bestHand == null || HandRank.StraightFlush > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.StraightFlush, sortedRanks);
                else if (IsFourOfAKind(combination) && (bestHand == null || HandRank.FourOfAKind > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.FourOfAKind, sortedRanks);
                else if (IsFullHouse(combination) && (bestHand == null || HandRank.FullHouse > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.FullHouse, sortedRanks);
                else if (IsFlush(combination) && (bestHand == null || HandRank.Flush > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.Flush, sortedRanks);
                else if (IsStraight(combination) && (bestHand == null || HandRank.Straight > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.Straight, sortedRanks);
                else if (IsThreeOfAKind(combination) && (bestHand == null || HandRank.ThreeOfAKind > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.ThreeOfAKind, sortedRanks);
                else if (IsTwoPair(combination) && (bestHand == null || HandRank.TwoPair > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.TwoPair, sortedRanks);
                else if (IsOnePair(combination) && (bestHand == null || HandRank.OnePair > bestHand.Rank))
                    bestHand = new EvaluatedHand(HandRank.OnePair, sortedRanks);
                else if (bestHand == null || HandRank.HighCard > bestHand.Rank)
                    bestHand = new EvaluatedHand(HandRank.HighCard, sortedRanks);
            }

            return bestHand;
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

    }

}