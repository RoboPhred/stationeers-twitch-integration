# Twitch Integration for Stationeers

Enables logic circuit effects for twitch events.

# Installation

Requires [BepInEx 5.0.1](https://github.com/BepInEx/BepInEx/releases) or later.

1. Install BepInEx in the Stationeers steam folder.
2. Launch the game, reach the main menu, then quit back out.
3. In the steam folder, there should now be a folder BepInEx/Plugins
4. Create a folder `stationeers-twitch-integration` in the BepInEx/Plugins folder.
5. Extract the release zip file to this folder.

# Compatibility

This mod should be compatible with both standard Stationeers game installs and the Stationeers Dedicated Server.

# Usage

This mod will affect logic devices based on twitch events. The affected devices are specified using the Labeler to include commands in the device name.

## Subscription event

There are two ways to trigger a logic write on subscription.

### Set the Setting logic value to 1

To set the Setting value to 1 when a user subscribes, include `[twitch:sub]` in your device name.
For example: `Subscribe Memory [twitch:sub]`
Note that this will always set the value to 1, and will not reset it. To make use of this,
you should set the memory back to 0 when you handle the event.

### Set an arbitrary value

You can set a logic value to anything you want on subscription using `[twitch:sub:LogicType:value]`. For example: `[twitch:sub:On:1]`.

## Chat commands

You can create chat commands that allow your viewers to control logic devices. There are a few ways of doing this

### Set the Setting logic value to 1

Using `[twitch:cmd:CommandName]` will create a twitch command to set the Setting value of the target device to 1. Example: `[twitch:cmd:foobar]` will create a `!foobar` command.

### Set a given logic value to a specific value

Using `[twitch:cmd:CommandName:LogicType:Value]` will set a predetermined logic value. For example, `[twitch:cmd:foobar:On:1]` will make a `!foobar` command that turns a device on.

### Set a given logic value to a value of the user's choice.

Twitch chat can specify the value to write if you use the logic variant without a value, such as `[twitch:cmd:CommandName:LogicType]`.
For example, to make a `!setMemory` command set a memory value to a value of their choosing, include `[twitch:cmd:setMemory:Setting]` in the name of the memory unit.
The chat will be able to set a specific value by specifying it with the command, such as `!setMemory 25`.

# Configuration

Create a file called `config.json` inside the `stationeers-webapi` folder under `BepInEx/Plugins`.
This file should be a json object with the following properties.

- `enabled` (bool, required): Specify whether the mod should be enabled or disabled. Set to `true` to enable.
- `twitchUsername` (string, required): The username of your twitch bot or account.
- `twitchAcessToken` (string, required): The OAuth token of your twitch bot or account. Generate it using the [Twitch Password Generator](https://twitchapps.com/tmi/)
- `twitchChannel` (string, required): The twitch stream to integrate with. This is usually the url path, for example: `twitch.tv/foobar` uses the channel `foobar`
