using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Streamdesk_neo
{
    class StreamityContract : SmartContract
    {
        public static readonly byte[] Address1 = "AWwf7QuVk6EdTZWd7sANnejcTDWCQYSUTk".ToScriptHash();
        public static readonly byte[] Address2 = "AKfAotS5rZmkVEeB72WXpDvFk2RK68DE12".ToScriptHash();
        public static readonly byte[] Address3 = "AWPqceGCCZKkAUjXMsvPWj21wuorvdSF9Z".ToScriptHash();

        public static object Main(string operation, params object[] args)
        {
            // This is used in the Verification portion of the contract to determine 
            // whether a transfer of NEO involving this contract's address can proceed
            if (Runtime.Trigger == TriggerType.Verification)
            {
                // If owner, proceed
                if (Helper.IsOwner())
                {
                    return true;
                }

                return false;
            }

            if (Runtime.Trigger == TriggerType.VerificationR)
            {
                return Crowdsale.Contribute(Storage.CurrentContext);
            }

            if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "address1")
                {
                    return Address1;
                }
                if (operation == "address2")
                {
                    return Address2;
                }
                if (operation == "address3")
                {
                    return Address3;
                }

                foreach (var method in Token.Methods())
                {
                    if (operation == method)
                    {
                        return Token.HandleMethod(Storage.CurrentContext, operation, args);
                    }
                }

                foreach (string method in Crowdsale.Methods())
                {
                    if (operation == method)
                    {
                        return Crowdsale.HandleMethod(Storage.CurrentContext, operation, args);
                    }
                }

                foreach (string method in StreamityEscrow.Methods())
                {
                    if (operation == method)
                    {
                        return StreamityEscrow.HandleMethod(Storage.CurrentContext, operation, args);
                    }
                }
            }

            return false;
        }
    }
}
