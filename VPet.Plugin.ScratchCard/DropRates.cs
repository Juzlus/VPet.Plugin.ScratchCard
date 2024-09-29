using System.Collections.Generic;

namespace VPet.Plugin.ScratchCard
{
    public class Drop
    {
        public int RateFrom;
        public int RateTo;
        public string Text;
        public int WinMoney;

        public Drop(int rateFrom, int rateTo, string text, int winMoney)
        {
            RateFrom = rateFrom;
            RateTo = rateTo;
            Text = text;
            WinMoney = winMoney;
        }
    }

    public class DropRates
    {
        public List<Drop> normal = new List<Drop>
        {
            new Drop(0, 100, "So close! Better luck next time.", 0),  // 10%
            new Drop(101, 200, "Almost got it, but no prize.", 0),  // 10%
            new Drop(201, 300, "Not your lucky day!", 0),  // 10%
            new Drop(301, 400, "No prize this time. Try again!", 0),  // 10%
            new Drop(401, 500, "So close, but no reward.", 0),  // 10%

            new Drop(501, 550, "A small fortune – 25$!", 25),  // 5%
            new Drop(551, 650, "Lucky win! You win 50$!", 50),  // 10%
            new Drop(651, 750, "Nice job! You win 75$!", 75),  // 10%
            new Drop(751, 900, "You got your money back – 100$!", 100),  // 15%
            new Drop(901, 960, "Awesome! You win 200$!", 200),  // 6%
            new Drop(961, 990, "Double the fun! You win 500$!", 500),  // 3%
            new Drop(991, 1000, "Jackpot! You win 950$!", 950),  // 1%
        };

        public List<Drop> premium = new List<Drop>
        {
            new Drop(0, 100, "So close! Better luck next time.", 0),  // 10%
            new Drop(101, 200, "Almost got it, but no prize.", 0),  // 10%
            new Drop(201, 300, "Not your lucky day!", 0),  // 10%
            new Drop(301, 400, "No prize this time. Try again!", 0),  // 10%
            new Drop(401, 500, "So close, but no reward.", 0),  // 10%

            new Drop(501, 550, "Nice catch! 250$!", 250),  // 5%
            new Drop(551, 600, "A decent win – 500$!", 500),  // 5%
            new Drop(601, 700, "You got your money back – 950$!", 950),  // 10%
            new Drop(701, 830, "Not bad! You win 1200$!", 1200),  // 13%
            new Drop(831, 900, "Double your luck! You win 1900$!", 1900),  // 7%
            new Drop(901, 950, "Great job! You win 3000$!", 3000),  // 5%
            new Drop(951, 980, "Huge win! You win 5000$!", 5000),  // 3%
            new Drop(981, 995, "Incredible! You win 10,000$!", 10000),  // 1.5%
            new Drop(996, 1000, "Jackpot! You win 25,000$!", 25000),  // 0.5%
        };

        public List<Drop> premiumPlus = new List<Drop>
        {
            new Drop(0, 100, "So close! Better luck next time.", 0),  // 10%
            new Drop(101, 200, "Almost got it, but no prize.", 0),  // 10%
            new Drop(201, 300, "Not your lucky day!", 0),  // 10%
            new Drop(301, 400, "No prize this time. Try again!", 0),  // 10%
            new Drop(401, 500, "So close, but no reward.", 0),  // 10%

            new Drop(501, 550, "Nice! You win 1000$!", 1000),  // 5%
            new Drop(551, 670, "Good win! You win 2000$!", 2000),  // 12%
            new Drop(671, 818, "You got your money back – 2500$!", 2500),  // 14.8%
            new Drop(819, 918, "Big win – 3500$!", 3500),  // 10%
            new Drop(919, 968, "Double up! You win 5000$!", 5000),  // 5%
            new Drop(969, 988, "Solid win – 12,500$!", 12500),  // 2%
            new Drop(989, 998, "Huge payout – 50,000$!", 50000),  // 1%
            new Drop(999, 1000, "Massive win! You win 100,000$!", 100000),  // 0.2%
        };
    }
}