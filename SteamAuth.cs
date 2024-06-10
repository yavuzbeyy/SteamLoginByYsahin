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
        public SteamClient steamClient;
        public CallbackManager manager;
        public SteamUser steamUser;
        public bool isRunning;

        public string userName = "";
        public string password = "";
        public string authCode;  // Steam Guard authentication code

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

        public void OnConnected(SteamClient.ConnectedCallback callback)
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

        public void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            if (isRunning)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                steamClient.Connect();
            }
        }

        public void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Console.WriteLine("Successfully logged on!");

                // Perform actions after successful logon

            }
            else
            {
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);
                isRunning = false;
            }
        }

        public void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }
    }
}
