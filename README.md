# KH3RandomizerWebsite
The official project for the online hosted seed generator for the Kingdom Hearts 3 Randomizer. This is also the basis for the client project that uses Electron.NET to create a desktop application. After a hiatus, there will be a run-down of the documentation either on the GitHub wiki or a video detailing an overview of the project.

# How to Play
To play the Kingdom Hearts 3 Randomizer, you will need two things: 1) A randomized seed pak made with the Seed Generator, and 2) The [Latest GoA+Randomizer Pak](https://github.com/Water-and-Critic/KH3-Rando-GoA/releases). 

## Seed Generation
This is where the randomization comes in that gives unique experiences each run, randomizing items, abilities, bonuses, even enemies, party members and bosses. You have two options on how to create a seed, 1) The hosted website, or 2) The downloadable client.
You can find the online version at the [Official Kingdom Hearts 3 Randomizer Website](https://kh3rando.com/). The hint bubbles throughout the seed creation will try and answer questions you have during the process. You will also need to check out the [Guides Section](https://kh3rando.com/guide) on the website to know what to do after you've generated a seed. 
If you prefer to have the Seed Generator locally, you can download the [Latest Official Client](https://github.com/WaterKH/KH3RandomizerClient/releases).

## GoA+Randomizer Pak
This is the UE4 logic that allows the game to process all the randomized items, abilities, bonuses and more. You can just download the [Latest GoA+Randomizer Pak](https://github.com/Water-and-Critic/KH3-Rando-GoA/releases) and drop it into your ../<path-to-KH3>/KINGDOM HEARTS III/Content/Paks/~mods folder. If you do not have a ~mods folder, simply create a new folder and call it ~mods.

# How to Contribute
**If you would like to make changes to this project, you can fork from the default branch to get started. Once you have completed working on a bug fix or feature, please submit a Pull Request back into main with your changes.**

# Credits

## Main Developers
[CriticPerfect](https://twitter.com/critic_perfect) and [WaterKH](https://twitter.com/water_kh) are the main developers for this project. CriticPerfect mainly focused and headed up the UE4 portion that allowed the game to talk with the custom actors and files generated from the website, as well as created the Station of Awakening and Garden of Assemblage levels. WaterKH created the seed generator website/client, made several different actors to communicate with the website's generated seed and reverse engineered the files needed for the seed generator to randomize everything currently supported in the KH3 Randomizer.

## Artists
There were so many talented people that contributed so much to this project artistically and they put so much hard work and effort into each of their projects. Please commission them if you are looking for work to be done, this project is a testament to the quality they can produce:

- Kimpchuu: They created the initial website's design and also created the Proof of Fantasy
- Mr. Matthews: They created the icons falling in the main menu & the logo for the KH3 Randomizer 
- NotSoMewwo: They created the 3D Render and Animation of the treasure chest for the Reveal Trailer
- Tatsoomaki: They conceptualized the Randomizer Keyblade (Pandora's Power) and created the Reveal Trailer
- dallin1016: They 3D modeled the Randomizer Keyblade (Pandora's Power) and created it in-game

## Distinguished Beta Testers & Supporters
I (waterkh) wanted to recognize these testers as they went above and beyond, testing and running back saves to diagnose, or help make it easier to diagnose, what the problem was. Theses people also showed little anger or annoyance with the development team as they were aware that bugs were likely and they always were mostly positive throughout the entire development cycle:

- PreferredWhale6: None of this would exist without her constant support. She kept me going during the most difficult time when creating this free mod for every type of KH fan to enjoy.
- Bio-Roxas: Bio was also a huge reason this reached the finish line, propping me up, promoting me early on in the development cycle and just the constant behind the scenes support when I needed to chat through problems or bugs.
- SuperSpikeGhettiBros: Mike and Jason, by far, were the best beta testers I've ever worked with. They always had positive attitudes working on and off stream helping me identify issues and find bugs. They both brought exceptional speedrunning and challenge run experience with them when testing the KH3 Randomizer, as well as just chill casual runs.
- Tatsoomaki: A veteran of running competive KH2FM randomizers, Tatsoo exceeded expectations as a beta tester, bringing with him knowledge of different strats that are utilized throughout KH3 speedruns.
- ShibuyaGato: Another veteran of running competive KH2FM randomizers, Shibuya also brought strats specifically around running randomizers, allowing her to give us valuable feedback we could improve the KH3 Rando experience with.
- RegularPat: Pat's most notable contribution was to test old patches when new ones released. On a more serious note, Pat's experience is what I expect most people's to be: a new way to experience KH3. And he helped move our development towards a variety of different opinions and views, not just for competitions or speedrunners.
- damo279: A different experience-based, analytical approach, Damo was able to identify bugs that not many people would think to try or do, leading to the discovery of a major bug. He also gave us, much like Pat, a different experience when approaching the KH3 Randomizer.
- Violin: Violin was the one that approached us to create a Discord to house the community for the KH3 Randomizer. With this server, there was feedback that helped handle several bugs.

## Discord Bug Testers & Contributors:
There were several people that helped by submitting bugs, participating in community polls and other things. We can't name them all here, but you can find them in the Discord server under #bug-reports and #development-discussion. I did want to list a couple of people that did stand out though.

- Rudabegga: They pointed us towards hosting with DigitalOcean, which has made the online seed generator the most stable it's ever been.
- Clara The Classy: They did an amazing job cross-referencing treasure chest data to verify that chests were being properly shown on the website with the correct text.
- SwiftShadow: They helped by contributing code to the hint system, allowing easier to follow hints, modular amount of hints per report and added a new category to map to.

You can also join the [Kingdom Hearts 3 Randomizer Community Discord](https://discord.gg/qf42CZfVBr) to help, find people to play the KH3 Rando with or discuss this project in general.
