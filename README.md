# Fallout 3 ESM Reader

## Fallout 3 Quests and Dialog

Fallout 3 uses the Marrowind quest (QUST), topic (DIAL), and response (INFO) records to generate branching dialog. When the player interacts with an NPC in Marrowind, a list of topics is displayed in a dialog box. A response for the NPC is displayed when the player clicks on a topic. The list of topics is controlled by the currently active quests. Not all active quests are visible to the player in their journal though, such as the WELCOME quest that controls greeting topics. Topics and responses are further filtered by conditions (CTDA) and a display priority. In Fallout 3, only the highest priority active topic is used and the highest 4(?) responses. 

In Marrowind, choices can be displayed to the player by the NPC response (INFO) by using the Choice() function in a script attached to the INFO record. The results of the player selecting a choice are implemented by the script, which might activate a quest or set some variables. The Choice() function is not used in Fallout 3.

There are several ways quest records are activated:

- Automatical activated on game start (set by a flag in the QUST record)
- When a set of conditions (CTDA) is true
- By scripts
- Next quest field (NAM0) of a quest > stage > log entry record (not sure about this)

In Fallout 3, quest records are used for several purposes:

- In game quests ()
- Activator for sets of dialog such as WELCOME, ...
- General purpose event driven actions such as PERK effects 

Quests have an optional list of quest stages and objectives that are displayed in the HUD compass. Stages are used for multipart player quests, but they are also used to activate effects such as tag skills.

All topics (DIAL) are assocated with a quest record. For active quests, topics are optionally filtered by a set of conditions (CTDA) and an NPC ID.

## Documentation

[Base data types](https://en.uesp.net/wiki/Skyrim_Mod:File_Format_Conventions)

[xEdit Fallout 3 Record Docs](https://tes5edit.github.io/fopdoc/Fallout3/Records.html)

[Skyrim Reocrd Docs](https://en.uesp.net/wiki/Skyrim_Mod:File_Format_Conventions)

[Oblivion Record Docs](https://en.uesp.net/wiki/Oblivion_Mod:Mod_File_Format)

[Morrowind Record Docs](https://en.uesp.net/wiki/Morrowind_Mod:Mod_File_Format)

## Code Liberally Reused From

[Fallout.NET](https://github.com/CaptainSaveACode/Fallout.NET)

[MarrowindUnity](https://github.com/arycama/MorrowindUnity)

[TES3Unity](https://github.com/demonixis/TES3Unity)

[DaggerfallUnity](https://github.com/Interkarma/daggerfall-unity)


