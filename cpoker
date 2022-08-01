using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ConsolePoker
{
    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        J = 11,
        K = 12,
        Q = 13,
        A = 14
    }
    public enum Suit
    {
        S,
        H,
        C,
        D
    }

    public class Hand : IComparable<Hand>
    {
        public List<Card> CardList;
        public string HandString;
        public int Score;
        public int RankBonus;

        public int CompareTo(Hand hand)
        {
            int result = Score.CompareTo(hand.Score);
            if (result != 0)
            {
                return result;
            }
            result = RankBonus.CompareTo(hand.RankBonus);
            if (result != 0)
            {
                return result;
            }

            for (int i = 0; i < CardList.Count; i++)
            {
                result = CardList[i].CompareTo(hand.CardList[i]);
                if (result < 0)
                {
                    return result;
                }
            }
            return result;
        }
    }
    public struct Card : IComparable<Card>
    {
        public Rank RankState;
        public Suit SuitState;
        public int CompareTo(Card card)
        {
            return RankState.CompareTo(card.RankState);
        }
        public override string ToString() {
            if ((int)RankState > 10)
            {

                return $"{RankState.ToString()}{SuitState.ToString()}";
            }
            else
            {
                return $"{(int)RankState}{SuitState.ToString()}";
            }
        }

    }
    public class Player
    {
        public string Name;
        public List<Card> HoleCards = new List<Card>();
        public double Chips;
        public int Holding;
        public enum Action
        {
            Betting,
            None,
            Check,
            Done
        }
        public Action ActionState;

    }

    public static class Poker
    {
        public static List<Card> Deck = new List<Card>();
        public static List<Card> CommunityCards = new List<Card>();
        public static Stage StageState = Stage.Preflop;
        public static Player Bob = new Player { Name = "Bob" };
        public static Player Alice = new Player{Name = "Alice"};
        public static Player DealerButton = Bob;
        public static Player NonDB = Alice;
        public static bool RoundEnded;
        public static double Pot;
        public enum Stage
        {
            Preflop,
            Flop,
            Turn,
            River,
            Showdown
        }
        public static bool FiveOfAKind(this Hand hand)
        {
            if (hand.NOfAKind(5))
            {
                return true;
            }
            else if (hand.CardList.Count(card => card.RankState == Rank.A) == 4 && hand.CardList.Any(card => card.RankState == Rank.J))
            {
                hand.RankBonus = (int)Rank.A;
                return true;
            }
            return false;
        }
        public static bool Straight(this Hand hand)
        {
            
            IEnumerable<int> desired = Enumerable.Range(hand.CardList.Min(card => (int)card.RankState), hand.CardList.Count).Reverse();
            if (hand.CardList.Select(card => (int)card.RankState).SequenceEqual(desired))
            {

                hand.RankBonus = (int)hand.CardList.Max().RankState;
                return true;
            }

            else if (hand.CardList.Select(card => (int)card.RankState).ToList().SequenceEqual(new List<int> { 14, 5, 4, 3, 2 }))
            {
                hand.RankBonus = 5;
                return true;
            }
            return false;
        }
        public static bool NOfAKind(this Hand hand, int num)
        {
            foreach (Card card in hand.CardList)
            {
                if (hand.CardList.Count(foo => foo.RankState == card.RankState) == num)
                {
                    hand.RankBonus = (int)card.RankState;
                    return true;
                }
            }
            return false;

        }
        public static bool Flush(this Hand hand)
        {

            if (hand.CardList.Select(card => card.SuitState).Distinct().Count() == 1)
            {

                hand.RankBonus = (int)hand.CardList.Max().RankState;
                return true;
            }
            return false;
        }
        public static bool TwoPair(this Hand hand)
        {
            List<int> pairs = new List<int>();
            foreach (Card card in hand.CardList)
            {
                if (hand.CardList.Count(foo => foo.RankState == card.RankState) == 2)
                {
                    Debug.WriteLine($"Occurences of {card.RankState}:{hand.CardList.Count(foo => foo.RankState == card.RankState)}");

                    if (!pairs.Contains((int)card.RankState))
                    {
                        pairs.Add((int)card.RankState);
                    }
                }

            }
            if (pairs.Count == 2)
            {
                hand.RankBonus = pairs.Max();
                return true;
            }
            return false;
        }
        public static bool IsWinnerHand(this Hand hand, List<Hand> handList)
        {

            for (int i = 0; i < handList.Count; i++)
            {
                if (handList[i] != hand)
                {
                    if (hand.CompareTo(handList[i]) < 0)
                    {
                        return false;
                    }

                }

            }
            return true;

        }
        public static void GenerateDeck()
        {
            var ranks = Enum.GetValues(typeof(Rank));
            var suits = Enum.GetValues(typeof(Suit));
            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    Card foo = new Card { RankState = (Rank)rank, SuitState = (Suit)suit };
                    if (!Deck.Contains(foo))
                    {
                        Deck.Add(foo);
                    }
                    
                    Debug.WriteLine($"{rank},{suit}");
                }
            }
            Debug.WriteLine(Deck.Count);

        }

        public static void GenerateRound()
        {
            ClearInfo();
            GenerateDeck();
            CommunityCards = GenerateCards(5);
            Bob.HoleCards = GenerateCards(2);
            Alice.HoleCards = GenerateCards(2);
            DealerButton = DealerButton.Equals(Alice) ? Bob : Alice;
            NonDB = DealerButton.Equals(Alice) ? Bob : Alice;
            Debug.WriteLine($"DealerButton Equals Player Two? {DealerButton.Equals(Alice)}, NonDealer Equals Player One: {NonDB.Equals(Bob)}");
        }
        public static void ClearInfo()
        {
            Pot = 0;
            StageState = Stage.Preflop;
            Deck.Clear();
            CommunityCards.Clear();
            Bob.HoleCards.Clear();
            Alice.HoleCards.Clear();
        }
        public static List<Card> GenerateCards(int num)
        {
            Random rand = new Random();
            List<Card> cards = new List<Card>();
            for (int i = 0; i < num; i++)
            {
                Card card = Deck[rand.Next(Deck.Count)];
                cards.Add(card);
                Deck.Remove(card);
               
            }
            return cards;
        }
        public static void DisplayTable(this Player player)
        {
            Console.WriteLine($"{player.Name} Screen",ConsoleColor.Blue);
            switch (StageState)
            {
                case Stage.Flop:
                    DisplayCommunityCards(3);
                    break;
                case Stage.Turn:
                    DisplayCommunityCards(4);
                    break;
                case Stage.River:
                    DisplayCommunityCards(5);
                    break;
            }
            Console.WriteLine("\n");
            Console.WriteLine($"The pot is {Pot} chips");
            Console.WriteLine($"Bob has {Bob.Chips}");
            Console.WriteLine($"Alice has {Alice.Chips}");
            Console.WriteLine("\n");
            Console.WriteLine("Your hole cards are:");
            foreach (Card card in player.HoleCards)
            {
                Console.Write(card.ToString() + " ");
            }
            Console.WriteLine("\n");

        }
        public static void PlayAgainst(this Player firstPlayer, Player nextPlayer)
        {
            RoundEnded = false;
            Console.WriteLine($"playing as {firstPlayer.Name}");
            do
            {
                firstPlayer.PlayStreet(nextPlayer);
            } while (!RoundEnded);
            PlayRound();
            
            
            
            

        }
        public static void DoAction(this Player player, Player other)
        {
            player.DisplayTable();
            if (Bob.Chips < 0 || Alice.Chips < 0)
            {
                RoundEnded = true;
                return;
            }
            switch (other.ActionState)
            {
                case Player.Action.Betting:
                    Console.WriteLine("C for Call, R for Raise, F for fold");
                    switch (Console.ReadLine())
                    {
                        case "C":
                            player.Bet(other.Holding - player.Holding);
                            player.ActionState = Player.Action.Done;
                            StageState++;
                            Console.WriteLine(StageState.ToString());
                            return;
                        case "R":
                            Console.WriteLine("Choose amount to raise:");
                            player.Raise(int.Parse(Console.ReadLine()),other);
                            player.ActionState = Player.Action.Betting;
                            break;
                        case "F":
                            RoundEnded = true;
                            return;

                    }
                    break;
                case Player.Action.None:
                case Player.Action.Check:
                    Console.WriteLine("B for Bet, C for Check");
                    switch (Console.ReadLine())
                    {
                        case "B":
                            Console.WriteLine("How much to bet?");
                            player.Bet(int.Parse(Console.ReadLine()));
                            player.ActionState = Player.Action.Betting;
                            break;
                        case "C":
                            switch (other.ActionState)
                            {
                                case Player.Action.None:
                                    player.ActionState = Player.Action.Check;
                                    break;
                                case Player.Action.Check:
                                    player.ActionState = Player.Action.Done;
                                    StageState++;
                                    return;

                            }
                            break;

                    }
                    break;
                case Player.Action.Done:
                    return;
            }
            

            other.DoAction(player);
        }
        
        public static void Bet(this Player player, int amount)
        {
            player.Holding += amount;
            player.Chips -= amount;
            Console.WriteLine($"{player.Name} is holding {player.Holding} chips, increase of {amount}");
        }
        public static void Raise(this Player player, int amount, Player other)
        {
            player.Holding += amount + other.Holding;
            player.Chips -= amount;
            Console.WriteLine($"{player.Name} is holding {player.Holding} chips, increase of {amount}");
        }
        public static void AccumulatePot()
        {
            Pot += (DealerButton.Holding + NonDB.Holding);
            DealerButton.Holding = 0;
            NonDB.Holding = 0;
            Debug.WriteLine($"Pot = {Pot}, {DealerButton.Holding} = DB holding, {NonDB.Holding} = ND holding");
        }
        public static void PlayStreet(this Player player, Player other)
        {
            
            switch (StageState)
            {
                
                case Stage.Preflop:
                    NonDB.ActionState = Player.Action.Betting;
                    NonDB.Bet(100);
                    DealerButton.Bet(50);
                    DealerButton.DoAction(NonDB);
                    AccumulatePot();
                    break;
                case Stage.Flop:
                case Stage.Turn:
                case Stage.River:
                    DealerButton.ActionState = Player.Action.None;
                    NonDB.DoAction(DealerButton);
                    AccumulatePot();
                    break;
                case Stage.Showdown:
                    Hand hand1 = DealerButton.HandToPlay();
                    Hand hand2 = NonDB.HandToPlay();
                    List < Hand > handList = new List<Hand>() { hand1, hand2 };
                    if (hand1.Equals(hand2))
                    {
                        DealerButton.Chips += Pot/2;
                        NonDB.Chips += Pot / 2;
                    }
                    else
                    {
                        if (WinningHand(handList).Equals(hand1))
                        {
                            DealerButton.Chips += Pot;
                            Console.Write($"{DealerButton.Name} won {Pot} chips with the hand");
                            foreach (Card card in hand1.CardList)
                            {
                                Console.Write(card.ToString() + " ");
                            }
                            Console.WriteLine("lol");
                            Console.Write($"They won with {hand1.Score} and {hand1.RankBonus}, other lost with {hand2.Score} and {hand2.RankBonus}");
                            foreach (Card card in hand2.CardList)
                            {
                                Console.Write(card.ToString() + " ");
                            }
                        }
                        else
                        {
                            NonDB.Chips += Pot;
                            Console.WriteLine($"{NonDB.Name} won {Pot} chips");
                            foreach (Card card in hand2.CardList)
                            {
                                Console.Write(card.ToString() + " ");
                            }
                            Console.WriteLine($"They won with {hand2.Score} and {hand2.RankBonus}, other lost with  {hand1.Score} and {hand1.RankBonus}  ");
                            foreach (Card card in hand1.CardList)
                            {
                                Console.Write(card.ToString() + " ");
                            }
                        }
                    }
                    RoundEnded = true;
                    break;



            }


          
            
        }
        public static void DisplayCommunityCards(int num)
        {
            Console.WriteLine("Community cards:");
            foreach (Card card in CommunityCards.Take(num))
            {
                Console.Write(card.ToString() + " ");
            }
        }
        public static void GenerateGame()
        {
            Console.WriteLine("New game starting");
            Console.WriteLine("Bob: Choose how many chips");
            Bob.Chips = int.Parse(Console.ReadLine());
            Console.WriteLine("Alice: Choose how many chips");
            Alice.Chips = int.Parse(Console.ReadLine());
            PlayRound();

        }
        public static void PlayRound()
        {
            if (Bob.Chips < 0 || Alice.Chips < 0)
            {
                GenerateGame();
            }
            else
            {
                Console.WriteLine("New round starting");
                GenerateRound();
                Alice.PlayAgainst(Bob);
            }
           
           
        }
        public static void AssignScores(this Hand hand)
        {
            hand.CardList.Sort();
            hand.CardList.Reverse();
            if (hand.FiveOfAKind())
            {
                hand.Score = 10;
                Debug.WriteLine("Five Of A Kind");
                return;
            }
            else if (hand.Flush() && hand.Straight())
            {
                hand.Score = 9;
                Debug.WriteLine("Straight Flush");
                return;
            }
            else if (hand.NOfAKind(4))
            {
                hand.Score = 8;
                Debug.WriteLine("Four of A Kind");
                return;

            }
            else if (hand.NOfAKind(2) && hand.NOfAKind(3))
            {
                hand.Score = 7;
                Debug.WriteLine("Full House");
                return;

            }
            else if (hand.Flush())
            {
                hand.Score = 6;
                Debug.WriteLine("Flush");
                return;

            }
            else if (hand.Straight())
            {
                hand.Score = 5;
                Debug.WriteLine("Straight");
                return;

            }
            else if (hand.NOfAKind(3))
            {
                hand.Score = 4;
                Debug.WriteLine("Three of a Kind");
                return;

            }
            else if (hand.TwoPair())
            {
                hand.Score = 3;
                Debug.WriteLine("Two pair");
                return;

            }
            else if (hand.NOfAKind(2))
            {
                hand.Score = 2;
                Debug.WriteLine("One Pair");
                return;

            }
            else
            {
                hand.Score = 1;
                hand.RankBonus = (int)hand.CardList.Max().RankState;
                Debug.WriteLine("High Card");
                return;

            }
        }
        public static Hand WinningHand(List<Hand> hands)
        {
            Hand winner = new Hand();
            foreach (Hand hand in hands)
            {
                hand.AssignScores();
            }
           
            foreach (Hand hand in hands)
            {
                if (hand.IsWinnerHand(hands))
                {
                    Debug.WriteLine($"Winner is {hand.Score},{hand.RankBonus}");
                    winner = hand;
                }
            }
            return winner;


        }
        public static Hand HandToPlay(this Player player)
        {
            List<Hand> handsToCompare = new List<Hand>();
            for (int i = 0; i<2;i++)
            {
                Hand option = new Hand { CardList = new List<Card>() { player.HoleCards[i] }.Concat(CommunityCards.GetRange(i,4)).ToList()};
                if (!handsToCompare.Contains(option))
                {
                    handsToCompare.Add(option);
                }

            }
            for (int i = 0; i < 3; i++)
            {
                Hand option = new Hand { CardList = player.HoleCards.Concat(CommunityCards.GetRange(i, 3)).ToList() };
                if (!handsToCompare.Contains(option))
                {
                    handsToCompare.Add(option);
                }
            }
            return WinningHand(handsToCompare);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Poker.GenerateGame();
        }
    }
}
