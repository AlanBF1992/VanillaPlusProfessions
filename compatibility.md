# Compatibility

**Warning: This guide might assume you're already familiar with JSON format, Content Patcher packs, Game State Queries and/or C# based mods depending on the part of the guide.**

## Content Packs (Mostly CP, though)
VPP offers two custom CP tokens and one custom GSQ for compatibility purposes at the moment.
Additionally, if you need to know whether your custom X is compatible with Y feature of VPP, every talent and profession section in [``features.md``](https://github.com/KediDili/VanillaPlusProfessions/blob/main/compatibility.md) tells how to add compatibility or do they need anything at all!

The queries/tokens, and the formats are listed below:

| Token Name    | Details |
|:-------------:|:--------|
| ContentPaths  | Accepts one of ``ItemSpritesheet``, ``ProfessionIcons``, ``InsiderInfo``, ``TalentBG``, ``TalentSchema``, ``BundleIcons`` and ``SkillBars``.<br/><br/>``ItemSpritesheet`` will give you sprites of any item VPP adds, but it also contains a few misc things that don't fit anywhere else.<br/>``ProfessionIcons`` contains all and only profession icons meant to be used by VPP.<br/>``InsiderInfo`` is only meant to be used for compatibility with the InsiderInfo talent.<br/>``TalentBG`` has only the generic elements that's supposed to be in every talent tree. such as the backgrounds for icons, resetting items and talent point display.<br/>``TalentSchema`` contains an image consisting of all "lines" and an icon of all VPP base talent trees.<br/>``BundleIcons`` is only the colored smaller bundles and greyscale bundles only meant for VPP to use.<br/>``SkillBars`` are the skill bars from VPP's Color Blindness config and skill overlay.<br/> depending on the input, it'll return a path to be used in the Target field.
| HasProfession | Returns a list of the professions the farmer currently has. Best used in the When field. |

An example patch for both of them is written below:
```json
{
    "LogName": "Changing VPP's profession icons"
    "Action": "EditImage",
    "Target": "{{KediDili.VanillaPlusProfessions/ContentPaths:ProfessionIcons}}",
    "FromFile": "assets/MyIcons.png",
}
```
```json
{
    "LogName": "Changing my custom ore's price depending on if the player has Ironmonger"
    "Action": "EditImage",
    "Target": "Data/Objects",
    "Fields": {
        "ExampleAuthor.ModId_MyCustomOre": {
            "Price": 500
        }
    },
    "When": {
        "KediDili.VanillaPlusProfessions/HasProfession|contains= Ironmonger": "true",
    }
}
```

### Game State Queries
|                                Format                                  |                         Details                      |
|:----------------------------------------------------------------------:|:----------------------------------------------------:|
| ``KediDili.VanillaPlusProfessions_WasRainingHereYesterday <location>`` | Checks if it was raining the said location YESTERDAY.<br/>This can be used for compatibility with talents such as Bountiful Boletes or Renewing Mist that need this check. |

### What you can/need to add compatibility depending on what your mod adds:
| Added by mods            | VPP Feature                                     |
|:------------------------:|:-----------------------------------------------:|
| NPCs                     | Gift Of Friendship, Insider Info, Connoisseur   |
| ores/metals              | Ironmonger, Metallurgist, Alchemic Reversal     |
| Fruit Trees              | Farming-Foraging                                |
| Giant Crops              | Farming-Foraging                                |
| Wild Trees               | Exotic Tapping, Welcome to the Jungle |
| Forage                   | Ranger, Adventurer, Wayfarer, Foraging-Fishing, Bountiful Boletes  |
| Crops                    | Gleaner |
| Tappers                  | Sapper, Farming-Foraging |
| Furnaces                 | Metallurgist, Ignitor |
| Crafting recipes         | Crafter(only if its for Mining Skill) |
| Artifacts                | Archaeologist |
| Animals                  | Breed Like Rabbits, |
| Geodes                   | Matryoshka, X-ray |
| Barn/Coop maps           | Overcrowding |
| Animal Produce Machinery | Nutritionist, Pastoralism, |
| Crop Machinery           | Machinist, Cold Press |
| Trash                    | Recycler, Can It |
| Other Machinery          | Pyrolysis, Static Charge, Double Hook, Clickbait |
| Shops                    | Mate's Rates, Bookclub Bargains |
| Ore Nodes                | Crystal Cavern, Upheaval |
| Buffs                    | Healer, Survivalist |
| Cooked foods             | Survival Cooking, Sugar Rush |
| Fertilizers              | Horticulturist |
| Minerals                 | Farming-Mining |
| Fishing Tackles          | Recycler |
| Fish                     | Farming-Fishing, Vast Domain, Big Fish Small Pond |
| Mill Produce             | Fine Grind |
| Crystalariums            | Dazzle, Geometry, Synthesis |
| Obelisks                 | Monumental Discount |
| Readable Books           | Cycle of Knowledge |
| Locations                | Ranger, Adventurer, Gleaner, Wayfarer,<br/>Crystal Cavern, Upheaval, Shared Focus,<br/>Diamond of the Kitchen, Starfall, Bountiful Boletes, Trashed Treasure |


## C# Mods
VPP offers an API to do things like adding custom talent trees for SpaceCore skill mods, getting VPP's config values for Mastery Cave Changes and Color Blindness Changes, and getting VPP professions a player currently has.

If you want to use the VPP API:
1) Copy [``IVanillaPlusProfessions.cs``](https://github.com/KediDili/VanillaPlusProfessions/blob/main/Compatibility/IVanillaPlusProfessions.cs) in Compatibility folder to your project.
2) Delete the elements that you don't need, as the API may change anytime this will help your version to be compatible as long as possible.
3) Copy [``Talent.cs``](https://github.com/KediDili/VanillaPlusProfessions/blob/main/Talents/Talent.cs) in Talents folder to your project (Do this only if you are a custom skill mod author who wants to add a custom talent tree for VPP. Otherwise, ignore this step.)
4) Request it through SMAPI's ModRegistry.

If your add any new things of these following things, you might need to use the API for compatibility with these features:
| Mod Feature     | VPP Feature                   |
|:---------------:|:-----------------------------:|
| Fishing Tackles | Recycler                      |
| Monsters        | Slimeshot, Monster Specialist |
| Fertilizers     | Horticulturist                |
| Trinkets        | Accessorise, Hidden Benefits  |
| Lost Books      | Lost And Found                |

### Skill Mods by SpaceCore
VPP will recognize your custom skill for features like the skill overlay, but will NOT try to manage your professions or add new ones for levels 15 and 20.
Creating a talent tree, expanding your skill's levelling and adding profesions for levels 15 and/or 20 is optional, VPP will be compatible with your skill mod even if you do none of these.

***If you DO want to add professions to your skill for levels 15 and 20***
- Increase the ``ExperienceLimits``'s length through SpaceCore API
- Add your extra professions as if you're adding regular level 5 or 10 professions, with the only difference being that the Level field needs to be 15 or 20.
- Relax because we all need rest and this isn't a bad thing.

There are also some guidelines to keep in mind that aren't necessarily technical, if you'd like to follow VPP's convention on your new professions:
- Level 15 professions are always a continuation of level 5 and 10 professions
- Level 20 professions are "combo" professions, which are special professions that do not care what you chose at level 15 and that are always a crossover of another skill and your skill. (or in VPP's case, two different vanilla skills).

***If you DO want to create a talent tree***
- [Add the VPP API to your project.](https://github.com/KediDili/VanillaPlusProfessions/blob/main/compatibility.md#c-mods)
- Create all of your talents as ``Talent`` instances. (You can do this however you may like, so you can read them from a .json file -which is the way VPP does it- or hardcode them in C# if that's your way.)
- Register them by using the ``RegisterCustomSkillTree`` method.
<br/><br/>

There are also a couple of guidelines to keep in mind that aren't necessarily technical:
- VPP awards talent points for every level up, including SpaceCore skills. This means there's at least ``[insert your level limit]`` talent points the player may gain just from your skill so it's highly recommended your talent tree has at least a few more than the limit, otherwise a player can max your talent tree and still be left with extra points and have nothing more to experience your tree, which reduces replayability. (For exp: VPP adds 25 talents for every vanilla skill, since the max limit is now 20 and not 10)
- Talents are much more powerful and accessible than professions, due to talents being immediately purchasable but professions have level limits. This means having a talent do an exact same thing as your professions will lead the player to prefer talents instead of your professions because they're just easier and basically kill your professions or enable min-maxing by trying to benefit from both at once.<br/><br/>TLDR; Do not make your talents do the same thing as your professions or your skill's other extra features.


### Skill mods that don't use SpaceCore's API to add their skills
Unfortunately I haven't got any feature that is going to help your mod as of yet, I am very sorry for any inconvenience this may cause.
