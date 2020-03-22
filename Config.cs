
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TwitchIntegration
{
    class ConfigBody
    {
        public bool enabled { get; set; }
        public bool debug { get; set; }
        public string twitchUsername { get; set; }
        public string twitchAcessToken { get; set; }
        public string twitchChannel { get; set; }

        public ConfigBody()
        {
            this.enabled = true;
            this.debug = false;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(this.twitchUsername))
            {
                throw new Exception("Twitch username must be specified.");
            }
            if (string.IsNullOrEmpty(this.twitchAcessToken))
            {
                throw new Exception("Twitch access token must be specified.");
            }
            if (string.IsNullOrEmpty(this.twitchChannel))
            {
                throw new Exception("Twitch channel must be specified.");
            }
        }
    }

    public static class Config
    {
        private static ConfigBody _instance;

        public static bool Enabled { get { return Config._instance.enabled; } }
        public static bool Debug { get { return Config._instance.debug; } }
        public static string TwitchUsername { get { return Config._instance.twitchUsername; } }
        public static string TwitchAccessToken { get { return Config._instance.twitchAcessToken; } }
        public static string TwitchChannel { get { return Config._instance.twitchChannel; } }

        public static void LoadConfig()
        {
            var assemblyDir = TwitchIntegrationPlugin.AssemblyDirectory;
            var path = Path.Combine(assemblyDir, "config.json");

            string configText;
            try
            {
                configText = File.ReadAllText(path);
            }
            catch (FileNotFoundException)
            {
                Logging.Log("No config file present.  Disabling twitch integration.");
                _instance = new ConfigBody();
                return;
            }

            try
            {
                Config._instance = JsonConvert.DeserializeObject<ConfigBody>(configText);
                Logging.Log(new Dictionary<string, string>() { { "ConfigPath", path } }, "Loaded config file.");
            }
            catch (Exception e)
            {
                Logging.Log(new Dictionary<string, string>() { { "ConfigPath", path } }, "Failed to load config file: " + e.ToString());
                _instance = new ConfigBody()
                {
                    enabled = false
                };
                return;
            }

            try
            {
                Config._instance.Validate();
            }
            catch (Exception e)
            {
                Logging.Log(
                new Dictionary<string, string>() {
                    {"ConfigPath", path}
                },
                "Invalid configuration: " + e.Message
                );
                Config._instance.enabled = false;
            }
        }
    }
}