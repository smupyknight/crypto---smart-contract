using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Streamdesk_neo
{
    class Helper
    {
        public static bool IsOwner()
        {
            return Runtime.CheckWitness(Token.TOKEN_OWNER);
        }
    }
}
