#### Allows you to customize your colors in Beat Saber
### Colors can be changed in game in the settings menu, specifically the color settings tab. All the different settings are described below
## READ HOW TO CHANGE YOUR COLORS BELOW, MAKE SURE TO READ THE *_ENTIRETY_* OF IT
Can customize your colors in UserData/Modprefs.ini. Below is an example of what the custom colors section of it will look like, it is generated after running the game once.
```ini
[CustomColorsEdit]
userIncrement=10
leftColorPreset=4
rightColorPreset=5
wallColorPreset=0
leftLightPreset=5
rightLightPreset=2
LeftRed=0
LeftGreen=0
LeftBlue=0
RightRed=0
RightGreen=0
RightBlue=0
OverrideCustomSabers=1
Brightness=1

```
- userIncrement- Ignore this
- Left color Preset: Controls the left color for sabers, and notes, refer to bottom for usage
- Right Color Preset: Controls the right color for sabers and notes, refer to bottom for usage

- Wall Color Preset: Controls the color of walls in song, refer to bottom for usage
- Left Light Preset: Controls the Left color of lighting, refer to bottom for usage
- Right Light Preset: Controls the Right color of lighting, refer to bottom for usage

- LeftRed, LeftGreen, LeftBlue: Controls the color of the left user preset, RGB Color, 0-255 for each, going ABOVE 255 will make the blocks brighter
- RightRed, RightGreen, Rightlue: Controls the color of the right user preset, RGB Color, 0-255 for each, going ABOVE 255 will make the blocks brighter
- OverrideCustomSabers - Whether custom colors should also affect the color of custom sabers, 1 for on, 0 for off
- Brightness - Max 1, minimum 0 : The lower the number, the dimmer the lights are

### PRESETS
- There are two types of presets, one for Left/Right Colors, and one for Walls & Left/Right LIGHT Colors, simply put the number corresponding to the option you want for that setting
- Changes to colors while the game is open are applied once the game changes scenes, i.e. entering or exiting a song

#### Left/Right Colors
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
```
### Wall & Left/Right LIGHT Colors
```ini
0 - Default, based on setting 
1 - User Left
2 - User Right
3 - Electric blue
4 - Green
5 - Purple
6 - Orange
7 - Yellow
8 - Black
9 - Pure Black
```

