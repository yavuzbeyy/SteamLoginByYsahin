using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLogin
{
    public class SteamAuth
    {
        private SteamClient steamClient;
        private CallbackManager manager;
        private SteamUser steamUser;
        private bool isRunning;

        private string userName = "";
        private string password = "";
        private string authCode;  // Steam Guard authentication code

        private SteamTrade.SteamTrade steamTrade;

        public void connectToSteam()
        {
            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);

            steamUser = steamClient.GetHandler<SteamUser>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            isRunning = true;

            Console.WriteLine("Connecting to Steam...");

            steamClient.Connect();

            while (isRunning)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam! Logging in '{0}'...", userName);

            if (String.IsNullOrEmpty(authCode))
            {
                Console.Write("Please enter your Steam Guard Auth Code: ");
                authCode = Console.ReadLine();
            }

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = userName,
                Password = password,
                TwoFactorCode = authCode
            });
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            if (isRunning)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                steamClient.Connect();
            }
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Console.WriteLine("Successfully logged on!");

                // Perform actions after successful logon
                Console.WriteLine("Logged in User SteamID: {0}", steamUser.SteamID);

                // SteamTrade nesnesi oluşturuluyor
                steamTrade = new SteamTrade.SteamTrade(steamClient, steamUser);

            }
            else
            {
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);
                isRunning = false;
            }
        }

        private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }
    }
}
