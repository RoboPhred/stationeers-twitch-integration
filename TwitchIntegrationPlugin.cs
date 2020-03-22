using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Objects.Motherboards;
using BepInEx;
using TwitchLib.Client.Events;

namespace TwitchIntegration
{
    [BepInPlugin("net.robophreddev.stationeers.TwitchIntegration", "Twitch Integration for Stationeers", "1.0.0.0")]
    public class TwitchIntegrationPlugin : BaseUnityPlugin
    {
        public static string AssemblyDirectory
        {
            get
            {
                var assemblyLocation = typeof(TwitchIntegrationPlugin).Assembly.Location;
                var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                return assemblyDir;
            }
        }

        void Awake()
        {
            TwitchIntegration.Config.LoadConfig();
            if (!TwitchIntegration.Config.Enabled)
            {
                Logging.Log("Integration not enabled.");
                return;
            }

            Logging.Log("Initializing Twitch Integration.");

            Twitch.Init();
            Twitch.OnSubscription += HandleSubscription;
            Twitch.OnChatCommandReceived += HandleChatCommandReceived;
        }

        private void HandleChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var effects = EffectGenerator.EffectsFromChatCommand(e);
            foreach (var effect in effects)
            {
                DeviceInteractor.ExecuteEffect(effect);
            }
        }

        private void HandleSubscription(object sender, SubscribeEventArgs e)
        {
            var effects = EffectGenerator.EffectsFromSubscription(e);
            foreach (var effect in effects)
            {
                DeviceInteractor.ExecuteEffect(effect);
            }
        }
    }
}