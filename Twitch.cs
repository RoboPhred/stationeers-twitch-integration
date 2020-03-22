
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchIntegration
{

    public class SubscribeEventArgs : EventArgs
    {
        public string SubscribedUser { get; private set; }
        public SubscribeEventArgs(string subscribedUser)
        {
            this.SubscribedUser = subscribedUser;
        }
    }

    public static class Twitch
    {
        private static TwitchClient client;

        public static event EventHandler<SubscribeEventArgs> OnSubscription;
        public static event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;
        public static event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        public static void Init()
        {
            if (Twitch.client != null)
            {
                return;
            }
            var creds = new ConnectionCredentials(Config.TwitchUsername, Config.TwitchAccessToken);
            var opts = new ClientOptions();
            var wsClient = new WebSocketClient(opts);
            client = new TwitchClient(wsClient);
            client.Initialize(creds, Config.TwitchChannel);

            client.OnLog += HandleLog;
            client.OnNewSubscriber += HandleNewSubscriber;
            client.OnChatCommandReceived += HandleChatCommand;
            client.OnMessageReceived += HandleMessageReceived;

            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                Logging.Log("Failed to connect to twitch: {0}", e.ToString());
            }
        }

        private static void HandleLog(object sender, OnLogArgs e)
        {
            Logging.Log("Log from twitch: {0}", e.Data);
        }

        private static void HandleMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(null, e);
            }
        }

        private static void HandleChatCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (OnChatCommandReceived != null)
            {
                OnChatCommandReceived(null, e);
            }
        }

        private static void HandleNewSubscriber(object sender, OnNewSubscriberArgs args)
        {
            if (OnSubscription != null)
            {
                OnSubscription(null, new SubscribeEventArgs(args.Subscriber.UserId));
            }
        }
    }
}