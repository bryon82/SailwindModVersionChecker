# ModVersionChecker

Checks the version of BepInEx mods against their release versions on either GitHub or Thunderstore websites. 
If there are any updates available for installed mods, a notification will pop up on game startup. 
In the notification there is a button to visit the mod websites to download the available updates.  
<br>
Mod release versions are checked hourly at 20 minutes past the hour so there may be a lag from when a mod author publishes a release.  

![Screenshot of Updates Available Notification](https://github.com/bryon82/SailwindModVersionChecker/blob/main/Screenshots/ModVersionChecker.png)  

## For Mod Authors

This works by comparing the version obtained from the BepInEx ChainLoader against the GitHub release 
tag or Thunderstore package version. The GitHub release tag can have any characters in it as long as 
there is some form of `int.int.int` in the tag. If you want your mod to be checked by 
ModVersionChecker your mod will need to be listed in [ModList.json](https://github.com/bryon82/SailwindModVersionChecker/blob/main/ModList.json). 
If you want your mod listed or you need to update your mod entry send a pull request with the necessary changes. 
Here is an example entry:

ExampleMod entry
```json
{
    "guid": "com.exampleauthor.examplemod",
    "repo": "https://github.com/exampleAuthor/exampleMod"
}
```

## Configurable

* Enable/disable notification.
* Enable/disable checking for updates.

### Requires

* [BepInEx 5.4.23](https://github.com/BepInEx/BepInEx/releases)

### Installation

If updating, remove ModVersionChecker folders and/or ModVersionChecker.dll files from previous installations.  
<br>
Extract the downloaded zip. Inside the extracted ModVersionChecker-\<version\> folder copy the ModVersionChecker folder and paste it into the Sailwind/BepInEx/Plugins folder.  

#### Consider supporting me 🤗

<a href='https://www.paypal.com/donate/?business=WKY25BB3TSH6E&no_recurring=0&item_name=Thank+you+for+your+support%21+I%27m+glad+you+are+enjoying+my+mods%21&currency_code=USD' target='_blank'><img src="https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif" border="0" alt="Donate with PayPal button" />
<a href='https://ko-fi.com/S6S11DDLMC' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi6.png?v=6' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>