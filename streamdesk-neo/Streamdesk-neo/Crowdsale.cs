using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Streamdesk_neo
{
    public static class Crowdsale
    {
        public const String STAGE_KEY = "STAGE";
        public const String BUY_PRICE_KEY = "BUY_PRICE";
        public const String TOKENS_KEY = "TOKENS";
        public const String START_TIME_KEY = "START_TIME";
        public const String END_TIME_KEY = "END_TIME";
        public const String DISCOUNT_KEY = "DISCOUNT";
        public const String DISCOUNT_FIRST_DAY_KEY = "DISCOUNT_FIRST_DAY";

        public static readonly byte[] NEO_ASSET_ID = { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147, 229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 };

        [DisplayName("crowdsaleFinish")]
        public static event Action<string> CrowdsaleFinished;

        #region Methods

        const string METHOD_STAGE = "stage";
        const string METHOD_BUY_PRICE = "buyPrice";
        const string METHOD_TOKENS = "tokens";
        const string METHOD_START_TIME = "startTime";
        const string METHOD_END_TIME = "endTime";
        const string METHOD_DISCOUNT = "discount";
        const string METHOD_DISCOUNT_FIRST_DAY = "discountFirstDay";
        const string METHOD_SET_STAGE = "setStage";
        const string METHOD_SET_TOKENS = "setTokens";
        const string METHOD_SET_START_TIME = "setStartTime";
        const string METHOD_SET_END_TIME = "setEndTime";
        const string METHOD_SET_DISCOUNT = "setDiscount";
        const string METHOD_SET_DISCOUNT_FIRST_DAY = "setDiscountFirstDay";
        const string METHOD_CHANGE_RATE = "changeRate";
        const string METHOD_CROWDSALE_STATUS = "crowdsaleStatus";
        const string METHOD_SELL = "sell";
        const string METHOD_START_CROWD = "startCrowd";

        public static string[] Methods()
        {
            return new[]
            {
                METHOD_STAGE,
                METHOD_BUY_PRICE,
                METHOD_TOKENS,
                METHOD_START_TIME,
                METHOD_END_TIME,
                METHOD_DISCOUNT,
                METHOD_DISCOUNT_FIRST_DAY,
                METHOD_SET_STAGE,
                METHOD_SET_TOKENS,
                METHOD_SET_START_TIME,
                METHOD_SET_END_TIME,
                METHOD_SET_DISCOUNT,
                METHOD_SET_DISCOUNT_FIRST_DAY,
                METHOD_CHANGE_RATE,
                METHOD_CROWDSALE_STATUS,
                METHOD_SELL,
                METHOD_START_CROWD
            };
        }

        public static Object HandleMethod(StorageContext context, string operation, params object[] args)
        {
            if (operation.Equals(METHOD_STAGE))
                return Stage(context);
            if (operation.Equals(METHOD_BUY_PRICE))
                return BuyPrice(context);
            if (operation.Equals(METHOD_TOKENS))
                return Tokens(context);
            if (operation.Equals(METHOD_START_TIME))
                return StartTime(context);
            if (operation.Equals(METHOD_END_TIME))
                return EndTime(context);
            if (operation.Equals(METHOD_DISCOUNT))
                return Discount(context);
            if (operation.Equals(METHOD_DISCOUNT_FIRST_DAY))
                return DiscountFirstDay(context);
            if (operation.Equals(METHOD_SET_STAGE))
                return SetStage(context, (uint) args[0]);
            if (operation.Equals(METHOD_SET_TOKENS))
                return SetTokens(context, (BigInteger) args[0]);
            if (operation.Equals(METHOD_SET_START_TIME))
                return SetStartTime(context, (uint) args[0]);
            if (operation.Equals(METHOD_SET_END_TIME))
                return SetEndTime(context, (uint) args[0]);
            if (operation.Equals(METHOD_SET_DISCOUNT))
                return SetDiscount(context, (uint) args[0]);
            if (operation.Equals(METHOD_SET_DISCOUNT_FIRST_DAY))
                return SetDiscountFirstDay(context, (uint) args[0]);
            if (operation.Equals(METHOD_CHANGE_RATE))
                return ChangeRate(context, (BigInteger) args[0], (BigInteger) args[1]);
            if (operation.Equals(METHOD_CROWDSALE_STATUS))
                return CrowdsaleStatus(context);
            if (operation.Equals(METHOD_SELL))
                return Sell(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);
            if (operation.Equals(METHOD_START_CROWD))
                return StartCrowd(context, (BigInteger) args[0], (uint) args[1], (uint) args[2], (uint) args[3], (uint) args[4]);
            return false;
        }

        #endregion

        public static uint Stage(StorageContext context)
        {
            return (uint)Storage.Get(context, STAGE_KEY).AsBigInteger();
        }

        public static BigInteger BuyPrice(StorageContext context)
        {
            return Storage.Get(context, BUY_PRICE_KEY).AsBigInteger();
        }

        public static uint Discount(StorageContext context)
        {
            return (uint) Storage.Get(context, DISCOUNT_KEY).AsBigInteger();
        }

        public static uint DiscountFirstDay(StorageContext context)
        {
            return (uint) Storage.Get(context, DISCOUNT_FIRST_DAY_KEY).AsBigInteger();
        }

        public static uint StartTime(StorageContext context)
        {
            return (uint) Storage.Get(context, START_TIME_KEY).AsBigInteger();
        }

        public static uint EndTime(StorageContext context)
        {
            return (uint) Storage.Get(context, END_TIME_KEY).AsBigInteger();
        }

        public static BigInteger Tokens(StorageContext context)
        {
            return Storage.Get(context, TOKENS_KEY).AsBigInteger();
        }

        public static bool SetStage(StorageContext context, uint stage)
        {
            Storage.Put(context, STAGE_KEY, stage);
            return true;
        }

        public static bool SetDiscount(StorageContext context, uint discount)
        {
            Storage.Put(context, DISCOUNT_KEY, discount);
            return true;
        }

        public static bool SetDiscountFirstDay(StorageContext context, uint discountFirstDay)
        {
            Storage.Put(context, DISCOUNT_FIRST_DAY_KEY, discountFirstDay);
            return true;
        }

        public static bool SetStartTime(StorageContext context, uint startTime)
        {
            Storage.Put(context, START_TIME_KEY, startTime);
            return true;
        }

        public static bool SetEndTime(StorageContext context, uint endTime)
        {
            Storage.Put(context, END_TIME_KEY, endTime);
            return true;
        }

        public static bool SetTokens(StorageContext context, BigInteger tokens)
        {
            Storage.Put(context, TOKENS_KEY, tokens);
            return true;
        }

        public static bool ChangeRate(StorageContext context, BigInteger numerator, BigInteger denominator)
        {
            if (!Helper.IsOwner())
            {
                return false;
            }

            if (numerator == 0) numerator = 1;
            if (denominator == 0) denominator = 1;

            Storage.Put(context, BUY_PRICE_KEY, numerator * Token.Factor() / denominator);

            return true;
        }

        public static string CrowdsaleStatus(StorageContext context)
        {
            uint stage = Stage(context);
            if (stage == 1)
            {
                return "Pre-ICO";
            }
            else if (stage == 2)
            {
                return "ICO first stage";
            }
            else if (stage == 3)
            {
                return "ICO second stage";
            }
            else if (stage == 4)
            {
                return "Feature stage";
            }

            return "There is no stage at present";
        }

        public static byte[] GetSender()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput[] references = tx.GetReferences();

            foreach(TransactionOutput output in references)
            {
                if (output.AssetId.Equals(NEO_ASSET_ID))
                {
                    return output.ScriptHash;
                }
            }

            return new byte[] { };
        }

        public static BigInteger GetContributionAmount()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput[] outputs = tx.GetOutputs();
            BigInteger value = 0;
            
            foreach (TransactionOutput output in outputs)
            {
                if (output.ScriptHash == GetSender() && output.AssetId == NEO_ASSET_ID)
                {
                    value = value + output.Value;
                }
            }

            return value;
        }

        public static bool Contribute(StorageContext context)
        {
            BigInteger amount = GetContributionAmount();
            return Sell(context, Token.TOKEN_OWNER, GetSender(), amount);
        }

        public static bool Sell(StorageContext context, byte[] owner, byte[] investor, BigInteger _amount)
        {
            if (owner.Length != 20 || investor.Length != 20) return false;
            if (_amount <= 0) return false;
            if (!Runtime.CheckWitness(owner)) return false;

            BigInteger buyPrice = BuyPrice(context);
            BigInteger amount = _amount * Token.Factor() / buyPrice;
            uint stage = Stage(context);

            if (stage == 1)
            {
                amount += WithDiscount(amount, Discount(context));
            }
            else if (stage == 2)
            {
                if (DateTime.Now.Ticks <= (int) Storage.Get(context, START_TIME_KEY).AsBigInteger() + 24 * 60 * 60)
                {
                    if (DiscountFirstDay(context) == 0)
                    {
                        SetDiscountFirstDay(context, 20);
                    }

                    amount += WithDiscount(amount, DiscountFirstDay(context));
                }
                else
                {
                    amount += WithDiscount(amount, Discount(context));
                }
            }
            else if (stage == 3)
            {
                amount += WithDiscount(amount, Discount(context));
            }

            if (Tokens(context) < amount)
            {
                CrowdsaleFinished(CrowdsaleStatus(context));
                Pausable.Pause(context);

                return false;
            }

            SetTokens(context, Tokens(context) - amount);
            Token.SetAvailableSupply(context, Token.AvailableSupply(context) - amount);

            return Token.Transfer(context, owner, investor, amount);
        }

        public static bool StartCrowd(StorageContext context, BigInteger tokens, uint startTime, uint endTime, uint discount, uint discountFirstDay)
        {
            SetTokens(context, tokens * Token.Factor());
            SetStartTime(context, startTime);
            SetEndTime(context, endTime);
            SetDiscount(context, discount);
            SetDiscountFirstDay(context, discountFirstDay);
            SetStage(context, 1);
            Pausable.Unpause(context);
            return true;
        }

        public static BigInteger WithDiscount(BigInteger amount, uint percent)
        {
            return amount * percent / 100;
        }
    }
}
