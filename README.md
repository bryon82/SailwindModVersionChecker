# ModVersionChecker

Checks mod versions against their release versions on either GitHub or Thunderstore websites. 
If there are any updates available for installed mods, a notification will pop up on game startup. 
In the notification there is a button to visit the mod websites to download the available updates. 
Note: This only works with BepInEx mods. 

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

### Requires

* [BepInEx 5.4.23](https://github.com/BepInEx/BepInEx/releases)

### Installation

Place the entire unzipped ModVersionChecker folder into the Sailwind/BepInEx/Plugins folder.