# Features
This allows other plugins to send messages using the [Telegram Bot API](https://core.telegram.org/bots).

# Configuration
```
{
  "Telegram Bot API Key": "[bot id here]:[access token here]",
  "Default ID for messages": -12345,
  "ID for error messages": -12345,
  "Try sending error messages to Telegram": true
}
```

# Getting started
![Getting your Bot API Token](https://i.imgur.com/4EqSfQt.png)

1. Talk to [BotFather on Telegram](https://t.me/BotFather) and create a bot
2. Optional: Add the bot to the chat group you want the messages in (alternatively you can have it talk directly to you)
3. Get your Bot API Token
4. Add [RawDataBot](https://t.me/RawDataBot) or another helper to the chat group you want the messages in, or talk to that bot directly (Look for message->chat->id in the JSON output)
5. Edit `Telegram.json` in the config-directory
6. Set the API key and chat ID's accordingly
7. Use `oxide.reload Telegram` in the server console or RCON to reload the plugin

# For developers
**API Methods**

`public void SendTelegramMessage(string message, long chatID = 0, bool escape = false, string parseMode = "Markdown")`

Send a message. Variables are:
* **message**: The message contents
* **chatID**: The target ID, if `0` it will use the specified default from the config
* **escape**: If set to true, the entire message will be escaped
* **parseMode**: The message parse mode, defaults to markdown but you can also use markdown V2 or HTML; see [Telegram API Documentation](https://core.telegram.org/bots/api#formatting-options) for more information

`public string Escape(string Input)`

Escape strings to make them Telegram-API friendly; you might want to do this with things like player names, but if you're not using markdown in messages you can also escape the entire message just to be safe.
