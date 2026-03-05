## how it looks
![alt text](image.png)
![alt text](image-1.png)

## what is it

unity mono injection cheat for REPO - made for fun, not maintained

inject with SharpMonoInjector after loading into a level (not main menu)

features:
- god mode (invincible + health stays at 100)
- speed multiplier (1x - 5x, uses game's own speed system)
- no ragdoll (blocks damage/enemy ragdoll, voluntary tumble still works)
- no break (valuables cant take damage)
- player esp (name, hp, distance through walls)
- enemy esp (name, hp, distance through walls)
- loot esp (item name + value)
- extraction esp (green = open, red = locked)
- infinite stamina
- tp to extraction
- upgrades menu (health/stamina/speed/strength/jump/range, takes effect next level)
- troll chat (flashbang, big text, invisible messages, spam - multiplayer only)

how to build and use:
1. build the dll (release, .net 4.8, class library)
2. update project refs from `REPO_Data/Managed/`
3. launch game, load into a level
4. inject with [SMI](https://github.com/warbler/SharpMonoInjector) or any other mono injector
5. press insert in game

my non chatgpt note: written in an afternoon, updated the esp later, upgrade changes only kick in after level transition, probably wont be updating this shit for too long, enjoy updating its feats lads

## credits:
- me
- [DarkForm's ESP in his cheat](https://github.com/Dark-Form/REPO-Internal)

---

# IMPORTANT

since there is a lot of people on github that report absolutely everything, i have to write notes like this one

1. this is NOT a paid cheat, please dont resell it
2. this is for edu purposes only and im not responsible for what u do with this shit
3. its meant for friend lobbies for fun

---

note to myself:
```
smi.exe inject -p REPO -a "C:\git-repos\repo-hax\cheat\bin\Release\cheat.dll" -n cheat -c Loader -m Load
```