# Twitch Integration for Stationeers

Enables logic circuit effects for twitch events.
Download the mod [here](https://github.com/RoboPhred/stationeers-twitch-integration/releases).

# Installation

Requires [BepInEx 5.0.1](https://github.com/BepInEx/BepInEx/releases) or later.

1. Install BepInEx in the Stationeers steam folder.
2. Launch the game, reach the main menu, then quit back out.
3. In the steam folder, there should now be a folder BepInEx/Plugins
4. Create a folder `stationeers-twitch-integration` in the BepInEx/Plugins folder.
5. Extract the release zip file to this folder.

# Compatibility

This mod should be compatible with both standard Stationeers game installs and the Stationeers Dedicated Server.

# Configuration

Before using this mod, you must configure it.

Create a file called `config.json` inside the `stationeers-twitch-integration` folder under `BepInEx/Plugins`.
This file should be a json object with the following properties.

- `enabled` (bool, required): Specify whether the mod should be enabled or disabled. Set to `true` to enable.
- `twitchUsername` (string, required): The username of your twitch bot or account.
- `twitchAcessToken` (string, required): The OAuth token of your twitch bot or account. Generate it using the [Twitch Password Generator](https://twitchapps.com/tmi/)
- `twitchChannel` (string, required): The twitch stream to integrate with. This is usually the url path, for example: `twitch.tv/foobar` uses the channel `foobar`
- `debug` (bool, optional): Set to `true` to record to the logs whenever a device is changed as a result of this mod.

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

You can set a logic value to anything you want on subscription using `[twitch:sub:LogicType:value]`. For example, naming a Flashng Light `Subscriber Light [twitch:sub:On:1]` will turn the light on when someone subscribes.

## Chat commands

You can create chat commands that allow your viewers to control logic devices. There are a few ways of doing this

### Set the Setting logic value to 1

Using `[twitch:cmd:CommandName]` will create a twitch command to set the Setting value of the target device to 1. For example, naming a logic memory to `Foobar Memory [twitch:cmd:foobar]` will create a `!foobar` command, and the memory will be set to 1 whenever someone uses that command.

### Set a given logic value to a specific value

Using `[twitch:cmd:CommandName:LogicType:Value]` will set a predetermined logic value. For example, a volume pump named `Air Pump [twitch:cmd:pumpOn:On:1]` will make a `!pumpOn` command that turns the pump on.

### Set a given logic value to a value of the user's choice.

Twitch chat can specify the value to write if you use the logic variant without a value, such as `[twitch:cmd:CommandName:LogicType]`.
For example, naming a logic memory `[twitch:cmd:setMemory:Setting]` will create a "!setMemory <value>" command.  This command will set the logic memory to the specified value; `!setMemory 25` will set the memory to 25.

# Responding to Events

Since the effect of many events in this mod is to set a memory value to 1, you will need to reset the value when you handle an event.

For example, the following IC script makes a good detection mechanism:

```mips
alias EventMemory d0
alias FlashingLight d1

awaitEvent:
yield
 # Check the event memory for an event
 l r0 EventMemory Setting
 beq r0 1 handleEvent # Got an event, handle it
 j awaitEvent # No event, keep looking

handleEvent:
 # Clear the memory so we are ready to detect another event
 # Do this before handling the event, so we can capture another event
 # while we are doing other things
 s EventMemory Setting 0

 # Turn on the flashing light
 s FlashingLight On 1

 # Leave the light on for 5 seconds
 sleep 5

 # Turn the light back off
 s FlashingLight On 0

 # Start looking for new events
 j awaitEvent

```

# TODO
- Events for bits, both presense of and amount.
- Find a way to pull in subscriber names.  Perhaps renaming a device to the last subscriber name?
