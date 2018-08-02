using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Streamdesk_neo
{
    public static class Pausable
    {
        const string PAUSED_KEY = "PAUSED";

        [DisplayName("pause")]
        public static event Action Paused;

        [DisplayName("unpause")]
        public static event Action Unpaused;

        public static uint IsPaused(StorageContext context)
        {
            return (uint)Storage.Get(context, PAUSED_KEY).AsBigInteger();
        }

        public static void SetPaused(StorageContext context, uint paused)
        {
            Storage.Put(context, PAUSED_KEY, paused);
        }

        public static bool Pause(StorageContext context)
        {
            if (!Helper.IsOwner())
            {
                return false;
            }

            SetPaused(context, 1);
            Paused();

            return true;
        }

        public static bool Unpause(StorageContext context)
        {
            if (!Helper.IsOwner())
            {
                return false;
            }

            SetPaused(context, 0);
            Unpaused();

            return true;
        }
    }
}
