# 1. Table of content
- [1. Table of content](#1-table-of-content)
- [2. What is this Plugin?](#2-what-is-this-plugin)
- [3. Setup](#3-setup)
- [4. How to use it?](#5-how-to-use-it)
- [5. Support / Feedback](#4-support--feedback)
- [6. How to contribute?](#6-how-to-contribute)
- [7. Sponsor me!](#7-how-to-sponsor)

# 2. What is this Plugin?
This Loupedeck Plugin allows you to control your home with [Home Assistant](https://homeassistant.io)

It is in a very basic state. You can only call services that need no parameters.
There is only a Windows version of it.
The code is really hacky, there are no plausibility controls etc. so expect crashing the loupedesk.exe.

# 3. Setup
Install (if available) a binary release of the plugin.

Create a homeassistant.json like this 
```json
{
  "token": "YourLongLivedToken",
  "url": "http://homeassistant.local:8123/api/",
  "entries": [
    {
      "service": "light.turn_on",
      "entities": [
        "light.light_1",
        "light.light_2",
        "light.light_4"
      ]
    },
    {
      "service": "switch.toggle",
      "entities": [
        "switch.plug_1",
        "switch.cover_3",
        "switch.light_65"
      ]
    }
  ]
}
```
Replace the fields with your values.

Place the file in `%userprofile%\.loupedeck\homeassistant\` as `homeassistant.json`

# 4. How to use it?

1. Install the Plugin
2. Create a config 
3. Add Actions to Loupedeck
4. Have fun controlling your home

In addition, see this video [youtube](https://youtu.be/9kJEw0r1UN4) or the discussion in the home assistant [community](https://community.home-assistant.io/t/use-a-loupedeck-as-a-panel-for-homeassistant/486240) 

# 5. Support / Feedback
Do you found a bug? Do you have a feature request? I would love to hear about it [here](https://github.com/lubeda/Loupedeck-HomeAssistantPlugin/issues/new/choose) or click on the “Issues” tab here on the GitHub repositories!

# 6. How to contribute?

Just fork the repository and create PR's.

# 7 How to sponsor?

[Paypal](https://www.paypal.com/donate/?hosted_button_id=FZDKSLQ46HJTU)
