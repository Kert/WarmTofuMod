# WarmTofuMod
This is a mod for Initial Drift Online. The goal is to fix various issues and add some quality of life improvements and even new features

# List of changes
- Added suspension spring force and damper customization. Check garage for options
- Transmission mode no longer resets when you restart the game
- Camera mode no longer resets when you change maps/restart the game
- Added realtime tofu delivery timer
- Huge performance optimizations
<details>
  <summary>Screenshot comparisons</summary>
  Before
  <img src="https://github.com/Kert/WarmTofuMod/assets/5028587/0b9eee6e-5726-4617-a347-8b8939899f3c">
  After
  <img src="https://github.com/Kert/WarmTofuMod/assets/5028587/5709990f-edb9-4e56-b9fa-0d59df03a4d9">
</details>

# How to install
First you need to prepare the game for using mods
## BepInEx 5 Install
Grab an x64 zip with [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21)

Read [BepInEx 5 Installing](https://docs.bepinex.dev/articles/user_guide/installation/index.html) for more info

Then Open ```%game folder%/BepInEx/config/BepInEx.cfg``` and change entry point type under ```[Preloader.Entrypoint]``` to ```MonoBehaviour``` like this:
```
[Preloader.Entrypoint]
Type = MonoBehaviour
```
Then grab ```MMHOOK.zip``` from [here](https://github.com/Kert/WarmTofuMod/releases/tag/MMHOOK) and extract archive into your game folder

It's pre-compiled libraries that were made using [MonoMod HookGen](https://github.com/MonoMod/MonoMod/blob/master/README-RuntimeDetour.md) to make modding easier

## WarmTofuMod Install (or any other mod)
Take mod's *.dll file and put it into ```%game folder%/BepInEx/plugins/```

Compiled WarmTofuMod *.dll files are [here](https://github.com/Kert/WarmTofuMod/releases)

# Compiling mod yourself
Copy original game's ```Assembly-CSharp.dll``` from ```%game folder%\Initial Drift_Data\Managed``` to ```%game folder%/BepInEx/utils/``` and rename it to ```PUBLIC-Assembly-CSharp.dll```

You need a compiler that can target **netstandard2.0**

After that you just run **build.bat**

The project expects that your game folder is ```C:\Program Files (x86)\Steam\steamapps\common\Initial Drift Online\```

You should change all instances of that in **.csproj** and **build.bat** if yours is different 