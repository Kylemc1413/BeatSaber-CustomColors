#### Allows you to customize your colors in Beat Saber
#### Requires BeatSaberCustomUI
### Colors can be changed in game in the settings menu, specifically the color settings tab. All the different settings are described below

#### Changelog 1.11.2
- Hopefully fixed lighting breaking after applying settings
#### Changelog 1.11.1
- Fixed note/saber colors being reversed
#### Changelog 1.11.0
- Settings File has moved to UserData/CustomColors.ini **Your Existing settings WILL Be Reset**
- User Color Preset Colors are now chosen with a color picker in the user colors tab! You can still adjust them in your config file like normal if you prefer as well
- Does not change logo color currently

#### Changelog 1.10.11
- New 'Pink' preset
- Removed some dead code that was causing performance issues when using platforms such as the Synthwave or Spectrogram platforms
- Minor fixes

#### Changelog 1.10.9
- Plugin will hopefully properly disable it's functionality if Chroma is installed, as chroma has more comprehensive options and it would be best to avoid unnecessary conflicts
- New Color Preset: Dark Blue
#### Changelog 1.10.7
- Optimized rainbow walls more
#### Changelog 1.10.6
- Minor fixes
- Overhauled rainbow walls to be much prettier (May cause a tiny performance hit if turned on)
#### Changelog 1.10.2
- Added basic events that are triggered after the plugin reads preferences and after the plugin applies the colors, doesn't cover rainbow override or custom saber override since those occur separately 
#### Changelog 1.10.0
- Thanks to brian, changing wall colors no longer lags the game!
- Improved Rainbow Walls!
#### Changelog 1.9.1
- New Color Preset
- Fixed issue with user colors not applying to walls properly

#### Changelog 1.9.0
- Improved Override settings to correctly apply based on how they are set

#### Changelog 1.8.0
- Added an option to allow environments to override the colors, such as the new OST in 0.12.2

##### Hoftix 1.8.1
- Added presets for the Colors in the new OST Song
#### Changelog 1.7.0
- Added an option to disable the plugin to the "Custom Colors" Settings tab, this should allow people to fully experience the new OST map if they wish, or if they simply want a way to go back to the default colors
#### Changelog 1.6.8
- Fixes, which do cause the logo to be entirely the color of the right light now 
- Setting for Rainbow Wall Override

Can customize your colors in UserData/CustomColors.ini. Below is an example of what the custom colors section of it will look like, it is generated after running the game once.
```ini
[Core]
OverrideCustomSabers = True
allowEnvironmentColors = True
disablePlugin = False
Brightness = 1

[Presets]
leftNoteColorPreset = 1
rightNoteColorPreset = 0
wallColorPreset = 0
leftLightPreset = 1
rightLightPreset = 2
rainbowWalls = False

[User Preset Colors]
Left User Preset R = 255
Left User Preset G = 0
Left User Preset B = 0
Right User Preset R = 175
Right User Preset G = 120
Right User Preset B = 255

```
- Left Note Color Preset: Controls the left color for sabers, and notes, refer to bottom for usage
- Right Note Color Preset: Controls the right color for sabers and notes, refer to bottom for usage
- Wall Color Preset: Controls the color of walls in song, refer to bottom for usage
- Left Light Preset: Controls the Left color of lighting, refer to bottom for usage
- Right Light Preset: Controls the Right color of lighting, refer to bottom for usage

- User Preset Colors - Adjust the r,g, and b values for the left and right user presets on a 0-255 scale, going above 255 will make the colors brighter than normal
- OverrideCustomSabers - Whether custom colors should also affect the color of custom sabers, 1 for on, 0 for off
- Brightness - Max 1, minimum 0 : The lower the number, the dimmer the lights are

### PRESETS
- There are two types of presets, one for Left/Right Colors, and one for Walls & Left/Right LIGHT Colors, simply put the number corresponding to the option you want for that setting
- Changes to colors while the game is open are applied once the game changes scenes, i.e. entering or exiting a song

#### Left/Right Note Colors
```ini
0 - User Left/Right, based on which setting it is being set for
1 - Default red
2 - Default blue
3 - Electric blue
4 - Green
5 - Purple
6 - Orange
7 - Yellow
8 - Black
9 - OST Orange
10 - OST Purple
11 - Klouder Blue
12 - Miku
```
### Wall & Left/Right LIGHT Colors
```ini
0 - Default, based on setting 
1- Left Color Preset
2 - Right Color Preset
3 - User Left
4 - User Right
5 - Electric blue
6 - Green
7 - Purple
8 - Orange
9 - Yellow
10 - Black
11 - Pure Black
12- OST Orange
13 - OST Purple
14 - Klouder Blue
15 - Miku
```
