using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Telegram", "NoxiousPluK", "0.1.0")]
    [Description("Send messages using the Telegram Bot API")]
    public class Telegram : CovalencePlugin
    {
        #region Configuration
        private Configuration config;

        private class Configuration
        {
            [JsonProperty("Telegram Bot API Key")]
            public string TelegramBotAPIKey = "id:key";

            [JsonProperty("Default ID for messages")]
            public long DefaultID = -12345;

            [JsonProperty("ID for error messages")]
            public long ErrorID = -12345;

            [JsonProperty("Try sending error messages to Telegram")]
            public bool SendErrors = true;

            public string ToJson() => JsonConvert.SerializeObject(this);
            public Dictionary<string, object> ToDictionary() => JsonConvert.DeserializeObject<Dictionary<string, object>>(ToJson());
        }

        protected override void LoadDefaultConfig() => config = new Configuration();

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null)
                {
                    throw new JsonException();
                }

                if (!config.ToDictionary().Keys.SequenceEqual(Config.ToDictionary(x => x.Key, x => x.Value).Keys))
                {
                    LogWarning("Configuration appears to be outdated; updating and saving");
                    SaveConfig();
                }
            }
            catch
            {
                LogWarning($"Configuration file {Name}.json is invalid; using defaults");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig()
        {
            LogWarning($"Configuration changes saved to {Name}.json");
            Config.WriteObject(config, true);
        }
        #endregion Configuration

        void OnServerInitialized(bool initial)
        {
            if (initial)
            {
                SendTelegramMessage($"ℹ Plugin loaded: *Telegram*");
            }
            else
            {
                SendTelegramMessage($"ℹ Plugin reloaded: *Telegram*");
            }
        }

        public void SendTelegramMessage(string message, long chatID = 0, bool escape = false, string parseMode = "Markdown")
        {
            if (config.TelegramBotAPIKey == "id:key")
            {
                LogWarning("Telegram plug-in not configured.");
                return;
            }

            if (escape)
                message = Escape(message);

            if (chatID == 0)
                chatID = config.DefaultID;

            string url = $"https://api.telegram.org/bot{config.TelegramBotAPIKey}/sendMessage?chat_id={chatID}&text={message}&parse_mode={parseMode}&disable_web_page_preview=true";
            webrequest.Enqueue(url, null, (code, response) =>
            {
                if (code != 200 || response == null)
                {
                    LogWarning($"Telegram bot error ({code}): {response} on request {url}");
                    if (config.SendErrors)
                        SendTelegramError($"Telegram error: {code}\r\n\r\nResponse: {Escape(response)}\r\n\r\nRequest: {Escape(url)}");
                    return;
                }
            }, this);
        }

        public string Escape(string Input)
        {
            Input = Input.Replace("_", "\\_");
            Input = Input.Replace("*", "\\*");
            Input = Input.Replace("`", "\\`");
            Input = Input.Replace("[", "\\[");
            Input = Input.Replace("&", "%26");
            Input = Input.Replace("?", "%3F");
            Input = Input.Replace("#", "%23");
            return Input;
        }

        private void SendTelegramError(string message)
        {
            string url = $"https://api.telegram.org/bot{config.TelegramBotAPIKey}/sendMessage?chat_id={config.ErrorID}&text={message}&parse_mode=Markdown&disable_web_page_preview=true";
            webrequest.Enqueue(url, null, (code, response) =>
            {
                if (code != 200 || response == null)
                {
                    LogWarning($"Telegram bot error ({code}): {response} on request {url}");
                    return;
                }
            }, this);
        }
    }
}
