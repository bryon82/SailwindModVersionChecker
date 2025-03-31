# ModVersionChecker

Checks mod versions against their release versions on either GitHub or Thunderstore websites. 
If there are any updates available for installed mods, a notification will pop up on game startup. 
In the notification there is a button to visit the mod websites to download the available updates. 

![Screenshot of Updates Available Notification](https://github.com/bryon82/SailwindModVersionChecker/blob/main/Screenshots/ModVersionChecker.png)  

## For Mod Authors

This works by comparing the version obtained from the BepInEx ChainLoader against the GitHub release 
tag or Thunderstore package version. The GitHub release tag can have any characters in it as long as 
there is some form of `int.int.int` in the tag. If you want your mod to be checked by ModVersionChecker, 
a folder named About with a file named mvc.json must exist in the root of your mod folder structure. 
mvc.json has fields for repo and website. The repo field should be your `"AuthorName/RepoName"` for 
checking GitHub releases or `"TeamName/PackageName"` for checking Thunderstore releases. The website 
to check for releases can be either `"github"` or `"thunderstore"`.

ExampleMod folder structure:
```
ExampleMod
|
|---About
|   |
|   |---mvc.json
|
|---ExampleMod.dll

```

ExampleMod mvc.json
```json

{
	"repo": "ExampleAuthor/ExampleMod",
	"website": "github"
}

```

## Configurable

* Enable/disable notification.

### Requires

* [BepInEx 5.4.23](https://github.com/BepInEx/BepInEx/releases)

### Installation

Place the entire unzipped ModVersionChecker folder into the Sailwind/BepInEx/Plugins folder.