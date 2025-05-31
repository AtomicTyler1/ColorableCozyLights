## Links

[![TDPC - Join the discord](https://img.shields.io/badge/TDPC-Join_my_discord!-424db8?style=plastic&logo=discord&logoColor=%23ffffff&labelColor=%235865F2)](https://www.discord.gg/38hGSYrPyj)
[![Static Badge](https://img.shields.io/badge/Lethal_Company_Modding-I_have_a_modding_thread!-830000?style=plastic&logo=discord&logoColor=%23ffffff&labelColor=%23bc1d00)](https://discord.com/channels/1168655651455639582/1297711493151719515)
[![Kofi - Support me](https://img.shields.io/badge/Kofi-Support_me_on_kofi!-cf4922?style=plastic&logo=kofi&logoColor=%23ffffff&labelColor=%23FF6433)](https://ko-fi.com/atomictyler)

[![Lethal Company](https://img.shields.io/badge/AtomicStudio-Lethal_Company_Mods!-2381bf?style=plastic&logo=thunderstore&logoColor=%23ffffff&labelColor=35afff)](https://thunderstore.io/c/lethal-company/p/AtomicStudio/)
[![Content Warning](https://img.shields.io/badge/AtomicStudio-Content_Warning_Mods!-2381bf?style=plastic&logo=thunderstore&logoColor=%23ffffff&labelColor=35afff)](https://thunderstore.io/c/content-warning/p/AtomicStudio/)

[![Nexus](https://img.shields.io/badge/NexusMods-I_have_mods_there_too!-e26a00?style=plastic&logo=nexusmods&logoColor=%23ffffff&labelColor=%23E6832B)](https://next.nexusmods.com/profile/atomictyler1/mods)

## What does it do?

Credit to @Nelots on discord for the idea! (nelot.s)

This includes 8 config options. 
- R Config (Red)
- G Config (Green)
- B Config (Blue)
- Brightnes Config (Please don't go above 1000)
- Rainbow Config (Cycles colors)
- RainbowSpeed Config (The speed, default 0.05 but 1 is good aswell)
- Enable Animations (If it should look to use animations)
- Animations (The animations https://atomictyler.dev/tools/lights)

# Animations go to: https://atomictyler.dev/

**THE WEBSITE DOES NOT SAVE ANIMATIONS ON REFRESH OR LEAVE!! PLEASE SAVE YOUR ANIMATION SOMEWHERE!!**

*When entering the website, please scroll down until you stumble across the custom tools section. It is then the second tool. You should get the hang of it relitively quickly. Although if you want to import codes, please make sure they arnt too big otherwise the website may become frozen for you.*

Here, try importing this code, it may look complicated but all I did was use the builder and it took 1 minute!

```
{'255,0,0,1.00','100'},{'0,17,255,1.00','100'},{'0,255,85,1.00','100'},{'0,76,148,1.00','100'},{'255,187,0,1.00','100'},{'153,0,255,1.00','100'},{'255,255,255,1.00','100'}
```

# Making manual animations

The animation process consists of the following: 

```
{'0,0,0,0','100'}
```

That may look confusing, so lets do a deeper dive.

| Red Value       | Green Value | Blue Value       | Alpha Value | Time (ms) Value |
| --------------- | ----------- | ---------------- | ----------- | --------------- |
|      0          | 0           |      0           | 0           |      100        | 

Thats basically what it is, but thats just a solid color, why not cycle between red and blue every 1000ms?
To do this, we do the same thing as before, but now to introduce a new step we take a comma and do the same **{},{}**

```
{'0,0,255,1','1000'},{'255,0,0,1','1000'}
```

And this is all very similar! You can go infinitely, but it may be better to use the website to see what each step looks like.

| Red Value       | Green Value | Blue Value       | Alpha Value | Time (ms) Value |
| --------------- | ----------- | ---------------- | ----------- | --------------- |
|      0          | 0           |      255         | 1           |      1000       | 
|      255        | 0           |      0           | 1           |      1000       | 

After changing the colors in the config. You dont need a restart.
"But its not going back". Turn the lights on and off 2 - 3 times.

Helpful website for RGB values (This website is RGBA, no worries. Skip out the last number box):
- Box 1 = R
- Box 2 = G
- Box 3 = B
- Box 4 = Miss out, the mod auto does it

https://rgbacolorpicker.com

# Join the discord!

Join my discord or the lethal company modding discord to view my other mods

My discord:
https://discord.gg/38hGSYrPyj

Lethal Company modding:
https://discord.gg/XeyYqRdRGC


