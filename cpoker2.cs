using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsolePoker2
{
    public static class HandEvaluation
    {
        private static long getTieBreaker(long allCards)
        {

            int first = getHighestRank(allCards);
            int tiebreaker = first << 16;
            for (int i = 0; i < 4; i++)
            {
                first = getHighestRank(allCards ^ (1 << first));
                tiebreaker |= first << 16 - ((i + 1) * 4);

            }
            return tiebreaker;
        }

        private static bool isStraight(long solo)
        {
            long lsb = solo &= -solo;
            long normalized = solo / lsb;
            return normalized == 31;

        }
        private static int getHighestRank(long allCards)
        {
            /*if (HighRankTable.ContainsKey(allCards))
            {
                Console.WriteLine("ok");
                return HighRankTable[allCards];
            }*/
            if (allCards == 0)
                return 0;

            int pos = 0;
            allCards = allCards / 2;

            while (allCards != 0)
            {
                allCards = allCards / 2;
                pos++;
            }
            //HighRankTable[allCards] = pos;
            return pos;
        }
        private static int getScore(long solo, long allCards, long suits)
        {
            bool flush = suits == 4369 || suits == 8738 || suits == 17476 || suits == 34952;
            bool straight = isStraight(solo);
            if (straight && flush)
            {
                if (solo == 31744)
                {
                    return 10;
                }
                return 9;
            }
            int score = 0;
            switch ((allCards) % 15)
            {
                case 1:
                    return 8;
                case 10:
                    return 7;
                case 9:
                    score = 4;
                    break;
                case 7:
                    score = 3;
                    break;
                case 6:
                    score = 2;
                    break;
                case 5:
                    score = 1;
                    break;
                default:
                    break;
            }
            if (flush)
            {
                return 6;
            }
            else if (straight || solo == 16444)
            {

                return 5;
            }
            return score;
        }
        public static Hand ParseAsHand(List<KeyValuePair<int, int>> cards)
        {



            cards.OrderBy(kvp => kvp.Key);
            cards.Reverse();
            long solo = 0;
            int[] ranks = cards.Select(kvp => kvp.Key).ToArray();
            for (int i = 0; i < ranks.Length; i++)
            {
                solo |= 1L << ranks[i];
            }
            Dictionary<int, int> instances = new Dictionary<int, int>();
            long allCards = 0;

            for (int i = 0; i < ranks.Length; i++)
            {
                int offset = 0;
                int rank = ranks[i];
                if ((solo & 1 << rank) > 0)
                {
                    if (!instances.ContainsKey(rank))
                    {
                        instances.Add(rank, 1);
                    }
                    else
                    {
                        instances[rank] = instances[rank] + 1;

                    }
                    offset = instances[rank];
                }
                long addition = (long)Math.Pow(2, rank * 4);
                addition = addition << offset;
                allCards |= addition;
            }
            allCards = allCards >> 1;

            long suits = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                suits |= (1L << ((4 * i) + cards[i].Value));
            }
            return new Hand { Score = getScore(solo, allCards, suits), TieBreaker = getTieBreaker(allCards) };
        }
        public static IEnumerable<List<KeyValuePair<int, int>>> CardCombos(IEnumerable<KeyValuePair<int, int>> cards, int count)
        {
            int i = 0;
            foreach (var card in cards)
            {
                if (count == 1)
                {
                    yield return new List<KeyValuePair<int, int>>() { card };
                }

                else
                {
                    foreach (var result in CardCombos(cards.Skip(i + 1), count - 1))
                    {

                        yield return new List<KeyValuePair<int, int>>(result) { card };
                    }
                    //yield return new Card[] { card }.Concat(result);
                }

                ++i;
            }
        }

        

    }
    public static class Table
    {
        public static int Chips;
        public static List<KeyValuePair<int,int>> CommunityCards;
    }
    public class Player
    {
        public List<KeyValuePair<int, int>> HoleCards;
        public int Chips;
        public  Hand HandToPlay()
        {


            Hand maxHand = new Hand { Score = -1000, TieBreaker = -1000 };
            List<List<KeyValuePair<int, int>>> allCombos = new List<List<KeyValuePair<int, int>>>(HandEvaluation.CardCombos(Table.CommunityCards, 3));
            allCombos.AddRange(HandEvaluation.CardCombos(Table.CommunityCards, 4));
            foreach (var combosList in allCombos)
            {

                if (combosList.Count == 3)
                {
                    List<KeyValuePair<int, int>> myHand = new List<KeyValuePair<int, int>>(combosList);
                    myHand.AddRange(HoleCards);
                    Hand hand = HandEvaluation.ParseAsHand(myHand);
                    if (hand > maxHand)
                    {
                        maxHand = hand;
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        List<KeyValuePair<int, int>> myHand = new List<KeyValuePair<int, int>>(combosList);
                        myHand.Add(HoleCards[i]);
                        Hand hand = HandEvaluation.ParseAsHand(myHand);
                        if (hand > maxHand)
                        {
                            maxHand = hand;
                        }
                    }
                }


            }
            return maxHand;
        }
    }
    public static class Poker
    {
        public static List<KeyValuePair<int, int>> Deck = new List<KeyValuePair<int, int>>();
        public static Random rand = new Random();

      
        public static void GetDeck()
        {
            for (int i = 2; i <= 14; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    KeyValuePair<int, int> kvp = new KeyValuePair<int, int>(i, j);
                    if (!Deck.Contains(kvp))
                    {
                        Deck.Add(kvp);
                    }


                }
            }
        }
        public static List<KeyValuePair<int,int>> GenerateCards(int n)
        {
            List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>();
           for (int i = 0; i <n; i++)
            {
                var randomCard = Deck[rand.Next(0, Deck.Count)];
                result.Add(randomCard);
                Deck.Remove(randomCard);
            }
            return result;
        }
        
    }


    public struct Hand : IComparable<Hand>
    {

        public int Score;
        public long TieBreaker;
        public int CompareTo(Hand hand)
        {
            int scoreComparer = Score.CompareTo(hand.Score);
            if (scoreComparer != 0)
            {
                return scoreComparer;
            }
            else
            {
                return TieBreaker.CompareTo(hand.TieBreaker);
            }
        }

        public static bool operator >(Hand hand1, Hand hand2)
        {
            return hand1.CompareTo(hand2) > 0;
        }
        public static bool operator <(Hand hand1, Hand hand2)
        {
            return hand1.CompareTo(hand2) < 0;
        }



    }

    class Program
    {



        













        static void Main(string[] args)
        {

            Poker.GetDeck();
            int[] holeCardSuits = new int[] { 0, 0 };
            int[] holeCardRanks = new int[] { 14, 13 };
            int[] tableCardSuits = new int[] { 2, 3, 1 };
            int[] tableCardRanks = new int[] { 10, 8, 4 };
            List<KeyValuePair<int, int>> myHoleCards = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> tableCards = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < 2; i++)
            {
                myHoleCards.Add(new KeyValuePair<int, int>(holeCardRanks[i], holeCardSuits[i]));
            }
            Console.WriteLine(myHoleCards.Count);

            for (int i = 0; i < 3; i++)
            {
                tableCards.Add(new KeyValuePair<int, int>(tableCardRanks[i], tableCardSuits[i]));
            }
            Console.WriteLine(tableCards.Count);


        }
    }
}
