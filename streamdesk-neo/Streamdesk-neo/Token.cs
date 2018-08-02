using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Streamdesk_neo
{
    public static class Token
    {
        public static string Name() => "Streamity";
        public static string Symbol() => "STM";
        public static byte Decimals() => 8;
        public static ulong Factor() => 100000000;

        public static readonly byte[] TOKEN_OWNER = "AWwf7QuVk6EdTZWd7sANnejcTDWCQYSUTk".ToScriptHash();

        public const string TOKEN_TOTAL_SUPPLY_KEY = "TOTAL_SUPPLY";
        public const string TOKEN_AVAILABLE_SUPPLY_KEY = "AVAILABLE_SUPPLY";

        // This generates a public event on the blockchain that will notify clients.
        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        [DisplayName("burn")]
        public static event Action<byte[], BigInteger> Burnt;

        [DisplayName("approve")]
        public static event Action<byte[], byte[], BigInteger> Approved;

        #region Methods

        const string METHOD_NAME = "name";
        const string METHOD_SYMBOL = "symbol";
        const string METHOD_DECIMALS = "decimals";
        const string METHOD_FACTOR = "factor";
        const string METHOD_DEPLOY = "deploy";
        const string METHOD_BALANCE_OF = "balanceOf";
        const string METHOD_APPROVAL_OF = "approvalOf";
        const string METHOD_TOTAL_SUPPLY = "totalSupply";
        const string METHOD_AVAILABLE_SUPPLY = "availableSupply";
        const string METHOD_TRANSFER = "transfer";
        const string METHOD_TRANSFER_FROM = "transferFrom";
        const string METHOD_APPROVE = "approve";
        const string METHOD_INCREASE_APPROVAL = "increaseApproval";
        const string METHOD_DECREASE_APPROVAL = "decreaseApproval";
        const string METHOD_BURN = "burn";
        const string METHOD_BURN_FROM = "burnFrom";

        public static string[] Methods()
        {
            return new[]
            {
                METHOD_NAME,
                METHOD_SYMBOL,
                METHOD_DECIMALS,
                METHOD_FACTOR,
                METHOD_DEPLOY,
                METHOD_BALANCE_OF,
                METHOD_TOTAL_SUPPLY,
                METHOD_AVAILABLE_SUPPLY,
                METHOD_TRANSFER,
                METHOD_TRANSFER_FROM,
                METHOD_APPROVE,
                METHOD_INCREASE_APPROVAL,
                METHOD_DECREASE_APPROVAL,
                METHOD_BURN,
                METHOD_BURN_FROM
            };
        }

        #endregion

        public static Object HandleMethod(StorageContext context, string operation, params object[] args)
        {
            if (operation.Equals(METHOD_NAME))
                return Name();
            if (operation.Equals(METHOD_SYMBOL))
                return Symbol();
            if (operation.Equals(METHOD_DECIMALS))
                return Decimals();
            if (operation.Equals(METHOD_FACTOR))
                return Factor();
            if (operation.Equals(METHOD_DEPLOY))
                return Deploy(context);
            if (operation.Equals(METHOD_BALANCE_OF))
                return BalanceOf(context, (byte[]) args[0]);
            if (operation.Equals(METHOD_APPROVAL_OF))
                return ApprovalOf(context, (byte[])args[0], (byte[])args[1]);
            if (operation.Equals(METHOD_TOTAL_SUPPLY))
                return TotalSupply(context);
            if (operation.Equals(METHOD_AVAILABLE_SUPPLY))
                return AvailableSupply(context);
            if (operation.Equals(METHOD_TRANSFER))
                return Transfer(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);
            if (operation.Equals(METHOD_TRANSFER_FROM))
                return TransferFrom(context, (byte[]) args[0], (byte[]) args[1], (byte[]) args[2], (BigInteger) args[3]);
            if (operation.Equals(METHOD_APPROVE))
                return Approve(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);
            if (operation.Equals(METHOD_INCREASE_APPROVAL))
                return IncreaseApproval(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);
            if (operation.Equals(METHOD_DECREASE_APPROVAL))
                return DecreaseApproval(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);
            if (operation.Equals(METHOD_BURN))
                return Burn(context, (byte[]) args[0], (BigInteger) args[1]);
            if (operation.Equals(METHOD_BURN_FROM))
                return BurnFrom(context, (byte[]) args[0], (byte[]) args[1], (BigInteger) args[2]);
            return false;
        }

        public static BigInteger TotalSupply(StorageContext context)
        {
            return Storage.Get(context, TOKEN_TOTAL_SUPPLY_KEY).AsBigInteger();
        }

        public static BigInteger AvailableSupply(StorageContext context)
        {
            return Storage.Get(context, TOKEN_AVAILABLE_SUPPLY_KEY).AsBigInteger();
        }

        public static bool SetTotalSupply(StorageContext context, BigInteger totalSupply)
        {
            Storage.Put(context, TOKEN_TOTAL_SUPPLY_KEY, totalSupply);
            return true;
        }

        public static bool SetAvailableSupply(StorageContext context, BigInteger availableSupply)
        {
            Storage.Put(context, TOKEN_AVAILABLE_SUPPLY_KEY, availableSupply);
            return true;
        }

        public static bool Deploy(StorageContext context)
        {
            if (!Helper.IsOwner()) return false;

            BigInteger totalSupply = TotalSupply(context);

            if (totalSupply > 0)
            {
                Runtime.Notify("Tokens already deployed");
                return false;
            }

            totalSupply = (BigInteger)13000000 * (BigInteger)Factor();
            SetTotalSupply(context, totalSupply);
            SetAvailableSupply(context, totalSupply);
            Storage.Put(context, TOKEN_OWNER, totalSupply);
            return true;
        }

        public static BigInteger BalanceOf(StorageContext context, byte[] address)
        {
            return Storage.Get(context, address).AsBigInteger();
        }

        public static BigInteger ApprovalOf(StorageContext context, byte[] owner, byte[] spender)
        {
            return Storage.Get(context, owner.Concat(spender)).AsBigInteger();
        }

        /**
         * Internal transfer, only can be called by this contract
         *
         * @param from - address of the contract
         * @param to - address of the investor
         * @param amount - tokens for the investor
         */
        public static bool Transfer(StorageContext context, byte[] from, byte[] to, BigInteger amount)
        {
            Storage.Put(context, "CheckPointH", from);
            Storage.Put(context, "CheckPointG", to);
            Storage.Put(context, "CheckPointI", amount);
            if (from.Length != 20 || to.Length != 20) return false;
            if (amount <= 0) return false;
            if (!Runtime.CheckWitness(from)) return false;
            if (from == to) return true;

            Storage.Put(context, "CheckPointJ", "passed");

            BigInteger fromBalance = Storage.Get(context, from).AsBigInteger();
            if (fromBalance < amount) return false;
            if (fromBalance == amount)
                Storage.Delete(context, from);
            else
                Storage.Put(context, from, fromBalance - amount);

            Storage.Put(context, "CheckPointK", "passed");
            BigInteger toBalance = Storage.Get(context, to).AsBigInteger();
            Storage.Put(context, to, toBalance + amount);
            Transferred(from, to, amount);
            Storage.Put(context, "CheckPointL", "passed");
            return true;
        }

        /**
         * Transfer tokens from other address
         *
         * Send `amount` tokens to `to` in behalf of `from`
         *
         * @param from The address of the sender
         * @param to The address of the recipient
         * @param amount the amount to send
         */
        public static bool TransferFrom(StorageContext context, byte[] owner, byte[] spender, byte[] to, BigInteger amount)
        {
            if (owner.Length != 20 || spender.Length != 20 || to.Length != 20) return false;
            if (!Runtime.CheckWitness(spender)) return false;
            BigInteger allowance = Storage.Get(context, owner.Concat(spender)).AsBigInteger();
            BigInteger fromBalance = Storage.Get(context, owner).AsBigInteger();
            BigInteger toBalance = Storage.Get(context, to).AsBigInteger();

            if (amount >= 0 && allowance >= amount && fromBalance >= amount)
            {
                if (allowance - amount == 0)
                {
                    Storage.Delete(context, owner.Concat(spender));
                }
                else
                {
                    Storage.Put(context, owner.Concat(spender), IntToBytes(allowance - amount));
                }

                if (fromBalance - amount == 0)
                {
                    Storage.Delete(context, owner);
                }
                else
                {
                    Storage.Put(context, owner, IntToBytes(fromBalance - amount));
                }

                Storage.Put(context, to, IntToBytes(toBalance + amount));
                Transferred(owner, to, amount);
                return true;
            }

            return false;
        }

        /**
         * Set allowance for other address
         *
         * Allows `spender` to spend no more than `amount` tokens in your behalf
         *
         * @param spender The address authorized to spend
         * @param amount the max amount they can spend
         */
        public static bool Approve(StorageContext context, byte[] owner, byte[] spender, BigInteger amount)
        {
            if (owner.Length != 20 || spender.Length != 20) return false;
            if (!Runtime.CheckWitness(owner)) return false;
            if (owner == spender) return true;
            if (amount < 0) return false;
            if (amount == 0)
            {
                Storage.Delete(context, owner.Concat(spender));
                Approved(owner, spender, amount);
                return true;
            }
            Storage.Put(context, owner.Concat(spender), amount);
            Approved(owner, spender, amount);
            return true;
        }

        /**
         * approve should be called when allowed[spender] == 0. To increment
         * allowed value is better to use this function to avoid 2 calls (and wait until
         * the first transaction is mined)
         */
        public static bool IncreaseApproval(StorageContext context, byte[] owner, byte[] spender, BigInteger amount)
        {
            if (owner.Length != 20 || spender.Length != 20) return false;
            if (!Runtime.CheckWitness(owner)) return false;
            if (owner == spender) return true;
            if (amount < 0) return false;

            BigInteger balance = Storage.Get(context, owner.Concat(spender)).AsBigInteger();
            Storage.Put(context, owner.Concat(spender), balance + amount);
            return true;
        }

        public static bool DecreaseApproval(StorageContext context, byte[] owner, byte[] spender, BigInteger amount)
        {
            if (owner.Length != 20 || spender.Length != 20) return false;
            if (!Runtime.CheckWitness(owner)) return false;
            if (owner == spender) return true;
            if (amount < 0) return false;

            BigInteger balance = Storage.Get(context, owner.Concat(spender)).AsBigInteger();

            if (balance < amount)
                Storage.Delete(context, owner.Concat(spender));
            else
                Storage.Put(context, owner.Concat(spender), balance - amount);

            return true;
        }

        /**
         * Destroy tokens
         *
         * Remove `amount` tokens from the system irreversibly
         *
         * @param amount the amount of money to burn
         */
        public static bool Burn(StorageContext context, byte[] owner, BigInteger amount)
        {
            if (owner.Length != 20 || amount < 0) return false;
            if (!Runtime.CheckWitness(owner)) return false;

            BigInteger balance = Storage.Get(context, owner).AsBigInteger();
            BigInteger totalSupply = Storage.Get(context, TOKEN_TOTAL_SUPPLY_KEY).AsBigInteger();
            BigInteger availableSupply = Storage.Get(context, TOKEN_AVAILABLE_SUPPLY_KEY).AsBigInteger();

            if (balance < amount) return false;
            if (totalSupply < amount) return false;
            if (availableSupply < amount) return false;

            Storage.Put(context, owner, balance - amount);
            Storage.Put(context, TOKEN_TOTAL_SUPPLY_KEY, totalSupply - amount);
            Storage.Put(context, TOKEN_AVAILABLE_SUPPLY_KEY, availableSupply - amount);

            Burnt(owner, amount);
            return true;
        }

        /**
         * Destroy tokens from other account
         *
         * Remove `amount` tokens from the system irreversibly on behalf of `from`.
         *
         * @param from the address of the sender
         * @param amount the amount of money to burn
         */
        public static bool BurnFrom(StorageContext context, byte[] owner, byte[] from, BigInteger amount)
        {
            if (owner.Length != 20 || from.Length != 20 || amount < 20) return false;
            if (!Runtime.CheckWitness(from)) return false;

            BigInteger balance = Storage.Get(context, from).AsBigInteger();
            BigInteger allowance = Storage.Get(context, owner.Concat(from)).AsBigInteger();
            BigInteger totalSupply = Storage.Get(context, TOKEN_TOTAL_SUPPLY_KEY).AsBigInteger();
            BigInteger availableSupply = Storage.Get(context, TOKEN_AVAILABLE_SUPPLY_KEY).AsBigInteger();

            if (balance < amount) return false;
            if (allowance < amount) return false;
            if (totalSupply < amount) return false;
            if (availableSupply < amount) return false;

            Storage.Put(context, from, balance - amount);
            Storage.Put(context, owner.Concat(from), allowance - amount);
            Storage.Put(context, TOKEN_TOTAL_SUPPLY_KEY, totalSupply - amount);
            Storage.Put(context, TOKEN_AVAILABLE_SUPPLY_KEY, availableSupply - amount);

            Burnt(from, amount);
            return true;
        }

        private static byte[] IntToBytes(BigInteger value)
        {
            byte[] buffer = value.ToByteArray();
            return buffer;
        }

        private static BigInteger BytesToInt(byte[] array)
        {
            var buffer = new BigInteger(array);
            return buffer;
        }
    }
}
