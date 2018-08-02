using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Streamdesk_neo
{
    class StreamityEscrow
    {
        const int STATUS_NO_DEAL = 0;
        const int STATUS_DEAL_WAIT_CONFIRMATION = 1;
        const int STATUS_DEAL_APPROVE = 2;
        const int STATUS_DEAL_RELEASE = 3;

        const string AVAILABLE_FOR_WITHDRAWAL_KEY = "AVAILABLE_FOR_WITHDRAWAL";
        const string AVAILABLE_FOR_WITHDRAWAL_STM_KEY = "AVAILABLE_FOR_WITHDRAWAL_STM";

        const uint requestCancellationTime = 2 * 60 * 60; // 2 hours
        const uint gasPrice = 3000;

        #region Methods

        const string METHOD_AVAILABLE_FOR_WITHDRAWAL = "availableForWithdrawal";
        const string METHOD_AVAILABLE_FOR_WITHDRAWAL_STM = "availableForWithdrawalSTM";
        const string METHOD_SET_AVAILABLE_FOR_WITHDRAWAL = "setAvailableForWithdrawal";
        const string METHOD_SET_AVAILABLE_FOR_WITHDRAWAL_STM = "setAvailableForWithdrawalSTM";
        const string METHOD_PAY = "pay";
        const string METHOD_PAY_ALTCOIN = "payAltcoin";
        const string METHOD_GET_DEAL = "getDeal";
        const string METHOD_SET_DEAL = "setDeal";
        const string METHOD_VERIFY_DEAL = "verifyDeal";
        const string METHOD_START_DEAL_FOR_USER = "startDealForUser";
        const string METHOD_WITHDRAW_COMMISSION_TO_ADDRESS = "withdrawCommisionToAddress";
        const string METHOD_WITHDRAW_COMMISSION_TO_ADDRESS_ALTCOIN = "withdrawCommissionToAddressAltcoin";
        const string METHOD_GET_STATUS_DEAL = "getStatusDeal";
        const string METHOD_RELEASE_TOKENS = "releaseTokens";
        const string METHOD_RELEASE_TOKENS_FORCE = "releaseTokensForce";
        const string METHOD_CANCEL_SELLER = "cancelSeller";
        const string METHOD_APPROVE_DEAL = "approveDeal";
        const string METHOD_TRANSFER_MINUS_COMMISSION = "transferMinusCommission";
        const string METHOD_TRANSFER_MINUS_COMMISSION_ALTCOIN = "transferMinusCommissionAltcoin";
        const string METHOD_TRANSFER_TOKEN = "transferToken";
        const string METHOD_TRANSFER_TOKEN_FROM = "transferTokenFrom";
        const string METHOD_APPROVE_TOKEN = "approveToken";

        const string METHOD_GET_CHECKPOINT = "getCheckPoint";

        public static string[] Methods()
        {
            return new[]
            {
                METHOD_AVAILABLE_FOR_WITHDRAWAL,
                METHOD_AVAILABLE_FOR_WITHDRAWAL_STM,
                METHOD_SET_AVAILABLE_FOR_WITHDRAWAL,
                METHOD_SET_AVAILABLE_FOR_WITHDRAWAL_STM,
                METHOD_PAY,
                METHOD_PAY_ALTCOIN,
                METHOD_GET_DEAL,
                METHOD_SET_DEAL,
                METHOD_VERIFY_DEAL,
                METHOD_START_DEAL_FOR_USER,
                METHOD_WITHDRAW_COMMISSION_TO_ADDRESS,
                METHOD_WITHDRAW_COMMISSION_TO_ADDRESS_ALTCOIN,
                METHOD_GET_STATUS_DEAL,
                METHOD_RELEASE_TOKENS,
                METHOD_RELEASE_TOKENS_FORCE,
                METHOD_CANCEL_SELLER,
                METHOD_APPROVE_DEAL,
                METHOD_TRANSFER_MINUS_COMMISSION,
                METHOD_TRANSFER_MINUS_COMMISSION_ALTCOIN,
                METHOD_TRANSFER_TOKEN,
                METHOD_TRANSFER_TOKEN_FROM,
                METHOD_APPROVE_TOKEN,

                METHOD_GET_CHECKPOINT
            };
        }

        #endregion

        public static Object HandleMethod(StorageContext context, string operation, params object[] args)
        {
            if (operation.Equals(METHOD_AVAILABLE_FOR_WITHDRAWAL))
                return AvailableForWithdrawal(context);

            if (operation.Equals(METHOD_AVAILABLE_FOR_WITHDRAWAL_STM))
                return AvailableForWithdrawalSTM(context);

            if (operation.Equals(METHOD_SET_AVAILABLE_FOR_WITHDRAWAL))
                return SetAvailableForWithdrawal(context, (BigInteger) args[0]);

            if (operation.Equals(METHOD_SET_AVAILABLE_FOR_WITHDRAWAL_STM))
                return SetAvailableForWithdrawal(context, (BigInteger) args[0]);

            if (operation.Equals(METHOD_PAY))
                return Pay(context, (byte[]) args[0], (byte[]) args[1], (byte[]) args[2], (BigInteger) args[3], (BigInteger) args[4], (byte[]) args[5]);

            if (operation.Equals(METHOD_PAY_ALTCOIN))
                return PayAltCoin(context, (byte[]) args[0], (byte[]) args[1], (byte[]) args[2], (BigInteger) args[3], (BigInteger)args[4], (byte[])args[5]);

            if (operation.Equals(METHOD_GET_DEAL))
                return GetDeal(context, (byte[])args[0], (string) args[1]);

            if (operation.Equals(METHOD_SET_DEAL))
                return SetDeal(context, (byte[])args[0], (string)args[1], args[2]);

            if (operation.Equals(METHOD_VERIFY_DEAL))
                return VerifyDeal(context, (byte[]) args[0], (byte[]) args[1]);

            if (operation.Equals(METHOD_START_DEAL_FOR_USER))
                return StartDealForUser(context, (byte[]) args[0], (byte[]) args[1], (byte[]) args[2], (BigInteger) args[3], (BigInteger) args[4], (bool) args[5]);

            if (operation.Equals(METHOD_WITHDRAW_COMMISSION_TO_ADDRESS))
                return WithdrawCommissionToAddress(context, (byte[]) args[0], (BigInteger) args[1]);

            if (operation.Equals(METHOD_WITHDRAW_COMMISSION_TO_ADDRESS_ALTCOIN))
                return WithdrawCommissionToAddressAltcoin(context, (byte[]) args[0], (BigInteger) args[1]);

            if (operation.Equals(METHOD_GET_STATUS_DEAL))
                return GetStatusDeal(context, (byte[]) args[0]);

            if (operation.Equals(METHOD_RELEASE_TOKENS))
                return ReleaseTokens(context, (byte[]) args[0],(BigInteger) args[1]);

            if (operation.Equals(METHOD_RELEASE_TOKENS_FORCE))
                return ReleaseTokensForce(context, (byte[]) args[0]);

            if (operation.Equals(METHOD_CANCEL_SELLER))
                return CancelSeller(context, (byte[]) args[0], (BigInteger) args[1]);

            if (operation.Equals(METHOD_APPROVE_DEAL))
                return ApproveDeal(context, (byte[]) args[0]);

            if (operation.Equals(METHOD_TRANSFER_MINUS_COMMISSION))
                return TransferMinusCommission(context, (byte[]) args[0], (BigInteger) args[1], (BigInteger) args[2]);

            if (operation.Equals(METHOD_TRANSFER_MINUS_COMMISSION_ALTCOIN))
                return TransferMinusCommissionAltCoin(context, (byte[])args[0], (BigInteger)args[1], (BigInteger)args[2]);

            if (operation.Equals(METHOD_TRANSFER_TOKEN))
                return TransferToken(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);

            if (operation.Equals(METHOD_TRANSFER_TOKEN_FROM))
                return TransferTokenFrom(context, (byte[]) args[0], (byte[]) args[1], (byte[]) args[2], (BigInteger) args[3]);

            if (operation.Equals(METHOD_APPROVE_TOKEN))
                return ApproveToken(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);


            if (operation.Equals(METHOD_GET_CHECKPOINT))
            {
                return Storage.Get(context, (byte[])args[0]);
            }
            return false;
        }

        public static BigInteger AvailableForWithdrawal(StorageContext context)
        {
            return Storage.Get(context, AVAILABLE_FOR_WITHDRAWAL_KEY).AsBigInteger();
        }

        public static BigInteger AvailableForWithdrawalSTM(StorageContext context)
        {
            return Storage.Get(context, AVAILABLE_FOR_WITHDRAWAL_STM_KEY).AsBigInteger();
        }

        public static bool SetAvailableForWithdrawal(StorageContext context, BigInteger availableForWithdrawal)
        {
            Storage.Put(context, AVAILABLE_FOR_WITHDRAWAL_KEY, availableForWithdrawal);
            return true;
        }

        public static bool SetAvailableForWithdrawalSTM(StorageContext context, BigInteger availableForWithdrawalSTM)
        {
            Storage.Put(context, AVAILABLE_FOR_WITHDRAWAL_STM_KEY, availableForWithdrawalSTM);
            return true;
        }

        [DisplayName("startDealEvent")]
        public static event Action<byte[], byte[], byte[]> StartedDealEvent;

        [DisplayName("approveDealEvent")]
        public static event Action<byte[], byte[], byte[]> ApprovedDealEvent;

        [DisplayName("releaseEvent")]
        public static event Action<byte[], byte[], byte[]> ReleasedEvent;

        [DisplayName("sellerCancelEvent")]
        public static event Action<byte[], byte[], byte[]> SellerCancelledEvent;

        public static bool Pay(StorageContext context, byte[] tradeId, byte[] seller, byte[] buyer, BigInteger value, BigInteger commission, byte[] sign)
        {
            byte[] hashDeal = tradeId.Concat(seller).Concat(buyer).Concat(value.AsByteArray()).Concat(commission.ToByteArray());
            if (!VerifyDeal(context, hashDeal, sign))
                return false;

            StartDealForUser(context, hashDeal, seller, buyer, commission, value, false);
            return true;
        }

        public static bool PayAltCoin(StorageContext context, byte[] tradeId, byte[] seller, byte[] buyer, BigInteger value, BigInteger commission, byte[] sign)
        {
            byte[] hashDeal = tradeId.Concat(seller).Concat(buyer).Concat(value.AsByteArray()).Concat(commission.ToByteArray());

            if (!VerifyDeal(context, hashDeal, sign))
                return false;

            bool result = Token.TransferFrom(context, Token.TOKEN_OWNER, seller, buyer, value);
            if (result == false)
                return false;

            StartDealForUser(context, hashDeal, seller, buyer, commission, value, true);
            return true;
        }

        public static bool VerifyDeal(StorageContext context, byte[] hashDeal, byte[] sign)
        {
            byte[] deal = (byte[])GetDeal(context, hashDeal, "seller");

            if (deal.Length != 0)
                return false;
            return true;
        }

        public static byte[] StartDealForUser(StorageContext context, byte[] hashDeal, byte[] seller, byte[] buyer, BigInteger commission, BigInteger value, bool isAltcoin)
        {
            SetDeal(context, hashDeal, "seller", seller);
            SetDeal(context, hashDeal, "buyer", buyer);
            SetDeal(context, hashDeal, "commission", commission);
            SetDeal(context, hashDeal, "value", value);
            SetDeal(context, hashDeal, "isAltcoin", isAltcoin);
            SetDeal(context, hashDeal, "cancelTime", Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp + requestCancellationTime);
            SetDeal(context, hashDeal, "status", STATUS_DEAL_WAIT_CONFIRMATION);

            StartedDealEvent(hashDeal, seller, buyer);
            return hashDeal;
        }

        public static bool WithdrawCommissionToAddress(StorageContext context, byte[] to, BigInteger amount)
        {
            if (!Helper.IsOwner())
                return false;

            if (amount > AvailableForWithdrawal(context)) return false;

            SetAvailableForWithdrawal(context, AvailableForWithdrawal(context) - amount);
            Token.Transfer(context, Token.TOKEN_OWNER, to, amount);

            return true;
        }

        public static bool WithdrawCommissionToAddressAltcoin(StorageContext context, byte[] to, BigInteger amount)
        {
            if (!Helper.IsOwner())
                return false;

            if (amount > AvailableForWithdrawalSTM(context)) return false;

            SetAvailableForWithdrawalSTM(context, AvailableForWithdrawalSTM(context) - amount);
            Token.Transfer(context, Token.TOKEN_OWNER, to, amount);

            return true;
        }

        public static int GetStatusDeal(StorageContext context, byte[] hashDeal)
        {
            return (int)GetDeal(context, hashDeal, "status");
        }


        public static BigInteger GAS_releaseTokens = 60000;
        public static bool ReleaseTokens(StorageContext context, byte[] hashDeal, BigInteger additionalGas)
        {
            //if (!Helper.IsOwner())
            //    return false;

            int status = GetStatusDeal(context, hashDeal);
            bool isAltcoin = (bool)GetDeal(context, hashDeal, "isAltcoin");
            byte[] buyer = (byte[])GetDeal(context, hashDeal, "buyer");
            byte[] seller = (byte[])GetDeal(context, hashDeal, "seller");
            BigInteger value = (BigInteger)GetDeal(context, hashDeal, "value");
            BigInteger commission = (BigInteger)GetDeal(context, hashDeal, "commission");

            Storage.Put(context, "CheckPointA", "passed");
            if (status == STATUS_DEAL_APPROVE)
            {
                bool result = false;
                status = STATUS_DEAL_RELEASE;

                Storage.Put(context, "CheckPointB", "passed");
                //if (isAltcoin == false)
                //    result = TransferMinusCommission(context, buyer, value, commission + (GAS_releaseTokens + additionalGas) * gasPrice);
                //else
                //    result = TransferMinusCommissionAltCoin(context, buyer, value, commission);

                if (result == false)
                {
                    Storage.Put(context, "CheckPointC", "passed");
                    status = STATUS_DEAL_APPROVE;
                    SetDeal(context, hashDeal, "status", status);
                    return false;
                }

                Storage.Put(context, "CheckPointD", "passed");

                SetDeal(context, hashDeal, "status", status);
                ReleasedEvent(hashDeal, seller, buyer);
                Storage.Delete(context, hashDeal);

                return true;
            }

            return false;
        }

        public static bool ReleaseTokensForce(StorageContext context, byte[] hashDeal)
        {
            if (!Helper.IsOwner())
                return false;

            int status = GetStatusDeal(context, hashDeal);
            bool isAltcoin = (bool)GetDeal(context, hashDeal, "isAltcoin");
            byte[] buyer = (byte[])GetDeal(context, hashDeal, "buyer");
            byte[] seller = (byte[])GetDeal(context, hashDeal, "seller");
            BigInteger value = (BigInteger)GetDeal(context, hashDeal, "value");
            BigInteger commission = (BigInteger)GetDeal(context, hashDeal, "commission");

            int prevStatus = status;

            if (status != STATUS_NO_DEAL)
            {
                bool result = false;

                status = STATUS_DEAL_RELEASE;
                if (isAltcoin == false)
                    result = TransferMinusCommission(context,buyer, value, commission);
                else
                    result = TransferMinusCommissionAltCoin(context, buyer, value, commission);

                if (result == false)
                {
                    status = prevStatus;
                    SetDeal(context, hashDeal, "status", status);
                    return false;
                }

                SetDeal(context, hashDeal, "status", status);
                ReleasedEvent(hashDeal, seller, buyer);
                Storage.Delete(context, hashDeal);

                return true;
            }

            return false;
        }

        public static BigInteger GAS_cancelSeller = 30000;
        public static bool CancelSeller(StorageContext context, byte[] hashDeal, BigInteger additionalGas)
        {
            if (!Helper.IsOwner())
                return false;

            int status = GetStatusDeal(context, hashDeal);
            bool isAltcoin = (bool)GetDeal(context, hashDeal, "isAltcoin");
            byte[] buyer = (byte[])GetDeal(context, hashDeal, "buyer");
            byte[] seller = (byte[])GetDeal(context, hashDeal, "seller");
            int cancelTime = (int)GetDeal(context, hashDeal, "cancelTime");
            BigInteger value = (BigInteger)GetDeal(context, hashDeal, "value");
            BigInteger commission = (BigInteger)GetDeal(context, hashDeal, "commission");

            if (cancelTime > Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp)
                return false;

            if (status == STATUS_DEAL_WAIT_CONFIRMATION)
            {
                bool result = false;

                status = STATUS_DEAL_RELEASE;
                if (isAltcoin == false)
                    result = TransferMinusCommission(context, seller, value, GAS_cancelSeller + additionalGas);
                else
                    result = TransferMinusCommissionAltCoin(context, seller, value, additionalGas);

                if (result == false)
                {
                    status = STATUS_DEAL_WAIT_CONFIRMATION;
                    SetDeal(context, hashDeal, "status", status);
                    return false;
                }

                SetDeal(context, hashDeal, "status", status);
                SellerCancelledEvent(hashDeal, seller, buyer);
                Storage.Delete(context, hashDeal);

                return true;
            }
            return false;
        }

        public static bool ApproveDeal(StorageContext context, byte[] hashDeal)
        {
            if (!Helper.IsOwner())
                return false;

            int status = GetStatusDeal(context, hashDeal);
            byte[] buyer = (byte[])GetDeal(context, hashDeal, "buyer");
            byte[] seller = (byte[])GetDeal(context, hashDeal, "seller");
;
            if (status == STATUS_DEAL_WAIT_CONFIRMATION)
            {
                status = STATUS_DEAL_APPROVE;
                SetDeal(context, hashDeal, "status", status);
                ApprovedDealEvent(hashDeal, seller, buyer);
                return true;
            }

            return false;
        }

        public static object GetDeal(StorageContext context, byte[] hashDeal, string key)
        {
            if (key == "commission" || key == "value")
                return Storage.Get(context, hashDeal.Concat(key.AsByteArray())).AsBigInteger();

            return Storage.Get(context, hashDeal.Concat(key.AsByteArray()));
        }

        public static bool SetDeal(StorageContext context, byte[] hashDeal, string key, object value)
        {
            Storage.Put(context, hashDeal.Concat(key.AsByteArray()), (byte[])value);
            return true;
        }

        public static bool TransferMinusCommission(StorageContext context, byte[] to, BigInteger value, BigInteger commission)
        {
            //if (value < commission) return false;
            //if (commission < 0) return false;
            Storage.Put(context, "CheckPointE", "passed");
            Storage.Put(context, "CheckPointF", to);
            Storage.Put(context, "CheckPointG", value - commission);
            SetAvailableForWithdrawal(context, AvailableForWithdrawal(context) + commission);
            return Token.Transfer(context, Token.TOKEN_OWNER, to, value - commission);
        }

        public static bool TransferMinusCommissionAltCoin(StorageContext context, byte[] to, BigInteger value, BigInteger commission)
        {
            //if (value < commission) return false;
            //if (commission < 0) return false;

            SetAvailableForWithdrawalSTM(context, AvailableForWithdrawalSTM(context) + commission);
            return Token.Transfer(context, Token.TOKEN_OWNER, to, value - commission);
        }

        public static bool TransferToken(StorageContext context, byte[] owner, byte[] to, BigInteger value)
        {
            if (!Runtime.CheckWitness(owner))
                return false;

            return Token.Transfer(context, owner, to, value);
        }

        public static bool TransferTokenFrom(StorageContext context, byte[] owner, byte[] to, byte[] from, BigInteger value)
        {
            if (!Runtime.CheckWitness(owner))
                return false;

            return Token.TransferFrom(context, owner, to, from, value);
        }

        public static bool ApproveToken(StorageContext context, byte[] owner, byte[] spender, BigInteger amount)
        {
            if (!Runtime.CheckWitness(owner))
                return false;

            return Token.Approve(context, owner, spender, amount);
        }
    }
}
