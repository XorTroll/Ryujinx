using Ryujinx.HLE.HOS.Services.Account.Acc;
using Ryujinx.HLE.HOS.Services.Account.Dauth;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Account
{
    class AccountServer : ServerManager
    {
        public AccountServer() : base("account", 0x010000000000001E, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "acc:u0", () => new IAccountServiceForApplication(AccountServiceFlag.Application) },
            { "acc:u1", () => new IAccountServiceForSystemService(AccountServiceFlag.SystemService) },
            { "acc:su", () => new IAccountServiceForAdministrator(AccountServiceFlag.Administrator) },
            { "acc:aa", () => new IBaasAccessTokenAccessor() },
            { "dauth:0", () => new IService() }
        };
    }
}
