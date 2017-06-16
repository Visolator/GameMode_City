//+=========================================================================================================+\\
//|			Made by..																						|\\
//|		   ____   ____  _				 __	  		  _		   												|\\
//|		  |_  _| |_  _|(_)	      		[  |		/ |_		 											|\\
//| 		\ \   / /  __   .--.   .--.  | |  ,--. `| |-' .--.   _ .--.  									|\\
//| 		 \ \ / /  [  | ( (`\]/ .'`\ \| | `'_\ : | | / .'`\ \[ `/'`\] 									|\\
//| 		  \ ' /    | |  `'.'.| \__. || | // | |,| |,| \__. | | |     									|\\
//|    		   \_/    [___][\__) )'.__.'[___]\'-;__/\__/ '.__.' [___]    									|\\
//|								BL_ID: 20490 | BL_ID: 48980													|\\
//|				Forum Profile(48980): http://forum.blockland.us/index.php?action=profile;u=144888;			|\\
//|																											|\\
//+=========================================================================================================+\\

$City::Chars = "*;!@#$%^&*()-_+='/?><,`~\|\"[]{}.\\";

function City_Debug(%function, %message)
{
	if(!$City::Debug)
		return;
	if(%function $= "")
		%function = "{0}";
	announce("\c5" @ %function @ " \c6| \c4" @ %message);
}

function City_loadFilePath(%path)
{
	if(strPos(%path,"*") <= 0)
		%path = %path @ "*";

	if(getFileCount(%path) <= 0)
	{
		warn("City_loadFilePath() - No folder exists: " @ filePath(%path));
		return -1;
	}

	for(%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path))
	{
		%fileExt = fileExt(%file);
		if(%fileExt !$= ".cs")
			continue;

		exec(%file);
	}

	return 1;
}

City_Debug("File > server.cs", "Loading assets..");

function City_LoadSupport()
{
	City_loadFilePath("Add-Ons/GameMode_City/Support/*");
	$Server::City::LoadedSupport = 1;
}

if(!$Server::City::LoadedSupport)
	City_LoadSupport();

registerOutputEvent("Player", "playSound", "dataBlock Sound", 0);
function Player::playSound(%this, %sound)
{
	if(!isObject(%sound) || %sound.getClassName() !$= "AudioProfile")
		return;
 
	if(%sound.description.isLooping || !%sound.description.is3D)
		return;
 
	serverplay3d(%sound, %this.getTransform());
}

/// <summary>
/// This crap will popup if the function is big and needs explaining
/// </summary>
/// <param name=""></param>

//Pass is CityDevPasswordSucks

if(!isObject(City_Triggers))
	new SimGroup(City_Triggers);

function City_CreateBrickgroup()
{
	$LoadingBricks_Ownership = 1;
	if(!isObject(BrickGroup_180))
	{
		%group = new SimGroup(BrickGroup_180)
		{
			client = -1;
			bl_id = 180;
			name = "The City";
			DoNotDelete = 1;
		};
		MainBrickgroup.add(%group);
	}

	if(!isObject(CityAIPlayer))
	{
		%AI = new AIPlayer(CityAIPlayer)
		{
			datablock = PlayerStandardArmor;
			position = "0 0 -1000";
			hName = "[?]";
			hBot = 1;
			name = "[?]";
			canHammerAnyone = 1;
			canArrestAnyone = 1;
			isGodPlayer = 1;
			isCityObject = 1;
		};
		MissionCleanUp.add(%AI);
	}
}
schedule(0, 0, City_CreateBrickgroup);

function fixEvents()
{
	for(%i=0;%i<clientGroup.getCount();%i++)
		serverCmdRequestEventTables(clientGroup.getObject(%i));
}

function GameConnection::City_Broadcast(%this, %message, %type)
{
	City_Broadcast(%message, %type, %this);
}

function City_Broadcast(%message, %type, %client)
{
	%str = '';
	%msgStr = "\c6@\c4City\c6: ";

	switch$(%type)
	{
		case "Normal" or "0":
			%msgStr = "\c6@\c4City\c6: ";

		case "Important" or "1":
			%msgStr = "\c7[\c3!\c7] \c6@\c4City\c6: ";

		case "Urgent" or "2":
			%msgStr = "\c7[\c0!\c7] \c6@\c4City\c6: ";

		case "Very Urgent" or "3":
			%str = 'MsgClearBricks';
			%msgStr = "\c7[\c0/!\\\c7] \c6@\c4City\c6: ";

		default:
			//Nothing
	}
	
	if(isObject(%client))
		messageClient(%client, %str, %msgStr @ %message);
	else
		messageAll(%str, %msgStr @ %message);
}

$City::AllowJets = 0;

datablock PlayerData(PlayerCityArmor : PlayerStandardArmor)
{
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = $City::AllowJets;
	jumpDelay = 20;
	minImpactSpeed = 20; //12.5

	uiName = "Blockhead";
	showEnergyBar = false;
	maxTools = 7;
	maxWeapons = 7;
};

datablock triggerData(City_InputTriggerData)
{
	tickPeriodMS = 500;
	brick = 0;
};

function City_InputTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	if(!isObject(%client = %obj.client))
		return;
	
	if(!isObject(%brick = %trigger.brick))
	{
		%trigger.delete();
		return;
	}

	if(isObject(%client.CityData["Trigger"]) && %client.CityData["Trigger"] != %trigger)
	{
		if(%obj.isInShiftMode && isFunction(GameConnection, "parseMenuData_" @ %brick.getDatablock().triggerFunction))
			%client.call("parseMenuData_" @ %brick.getDatablock().triggerFunction, "LEAVE");
		else if(isFunction(GameConnection, "parseTriggerData_" @ %brick.getDatablock().triggerFunction))
			%client.call("parseTriggerData_" @ %brick.getDatablock().triggerFunction, "LEAVE");

		%client.CityData["SelectMode"] = 0;
		%client.CityData["SelectMenuMode"] = 0;
		%client.CityData["MenuListCount"] = 0;
		%client.CityData["MenuName"] = "";
		%client.CityData["MenuList"] = "";
		%client.CityData["Trigger_Choosen"] = "";
		%client.CityData["Trigger_CanChoose"] = "";
		%client.CityData["Trigger_Mode"] = "";
		%client.CityData["Trigger"] = 0;
		%obj.isInShiftMode = 0;
		%client.centerprint("", 1);
	}

	%client.CityData["Trigger"] = %trigger;
	if(%brick.getDatablock().hasMenu && $City::CenterPrintMenu && !$Server::AvoidMenu[%client.getBLID()] && isFunction(GameConnection, "parseTriggerData_" @ %brick.getDatablock().triggerFunction))
	{
		%obj.isInShiftMode = 1;
		%client.CityData["SelectMode"] = 0;
		%client.CityData["SelectMenuMode"] = 0;
		%client.CityData["MenuListCount"] = 0;
		%client.CityData["MenuName"] = %brick.getDatablock().manuUiName;
		%client.call("parseMenuData_" @ %brick.getDatablock().triggerFunction, "ENTER");
		%client.City_initMenu();
	}
	else if(isFunction(GameConnection, "parseTriggerData_" @ %brick.getDatablock().triggerFunction))
		%client.call("parseTriggerData_" @ %brick.getDatablock().triggerFunction, "ENTER");
}

function City_InputTriggerData::onLeaveTrigger(%this, %trigger, %obj, %a)
{
	if(!isObject(%brick = %trigger.brick))
	{
		%trigger.delete();
		return;
	}

	if(!isObject(%client = %obj.client))
		return;

	if(%obj.isInShiftMode && isFunction(GameConnection, "parseMenuData_" @ %brick.getDatablock().triggerFunction))
	{
		%client.call("parseMenuData_" @ %brick.getDatablock().triggerFunction, "LEAVE");
		%client.centerprint("", 1);
	}
	else if(isFunction(GameConnection, "parseTriggerData_" @ %brick.getDatablock().triggerFunction))
		%client.call("parseTriggerData_" @ %brick.getDatablock().triggerFunction, "LEAVE");

	%client.CityData["SelectMode"] = 0;
	%client.CityData["SelectMenuMode"] = 0;
	%client.CityData["MenuListCount"] = 0;
	%client.CityData["MenuName"] = "";
	%client.CityData["MenuList"] = "";
	%client.CityData["Trigger_Choosen"] = "";
	%client.CityData["Trigger_CanChoose"] = "";
	%client.CityData["Trigger_Mode"] = "";
	%client.CityData["Trigger"] = 0;
	%obj.isInShiftMode = 0;
	%client.centerprint("", 1);
}

function fxDTSBrick::City_CreateTrigger(%brick, %bypass)
{
	if(!isObject(%brick))
		return;

	%client = %brick.getGroup().client;
	if(!isObject(%client))
		%client = %brick.client;

	%datablock = %brick.getDatablock();
	if(!isObject(%brick.trigger) && isObject(%datablock.triggerDatablock))
	{	
		%trigX = getWord(%datablock.triggerSize, 0);
		%trigY = getWord(%datablock.triggerSize, 1);
		%trigZ = getWord(%datablock.triggerSize, 2);
		
		if(mCeil(getWord(%brick.rotation, 3)) == 90)
			%scale = (%trigY / 2) SPC (%trigX / 2) SPC (%trigZ / 2);
		else
			%scale = (%trigX / 2) SPC (%trigY / 2) SPC (%trigZ / 2);
			
		
		%brick.trigger = new trigger()
		{
			datablock = %datablock.triggerDatablock;
			position = getWords(%brick.getWorldBoxCenter(), 0, 1) SPC getWord(%brick.getWorldBoxCenter(), 2) + ((getWord(%datablock.triggerSize, 2) / 4) + (%datablock.brickSizeZ * 0.1));
			rotation = "1 0 0 0";
			scale = %scale;
			polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
			brick = %brick;
		};
		City_Triggers.add(%brick.trigger);
		%trigger = %brick.trigger;

		if(%brick.getDatablock().CityBrickType $= "LotTrigger")
		{
			%brick.setName("_Lot" @ %brick.getGroup().bl_id @ "_" @ nameToID(%brick));
			%brick.doNotRename = 1;

			%trigger.ownerBL_ID = %brick.getGroup().bl_id;
			%trigger.ownerName = %brick.getGroup().name;

			if(isObject(%client) && !%bypass) //We assume that the brick fully planted.
			{
				%client.chatMessage("\c6You have a new lot. Remember, lots do not give money back; but all bricks on the lot give \c380 percent \c6brick volume back.");
				%client.chatMessage("  \c6> \c4If you are not online, you will not get refunded.");

				$City::Lots::Profile[%client.getBLID(), nameToID(%brick)] = %client.getPlayerName();
				if($City::Lots::SavePath !$= "")
					export("$City::Lots::Profile*", $City::Lots::SavePath);
			}
		}
		else if(%brick.getDatablock().CityBrickType $= "LotTrigger")
			%trigger.triggerData["NoKillZone"] = %datablock.triggerData["NoKillZone"];

		return 1;
	}
	return -1;
}

function fxDTSBrick::City_resetOwnership(%this, %bl_id, %name)
{
	if(isObject(%trigger = %this.trigger))
	{
		%trigger.ownerBL_ID = %this.getGroup().bl_id;
		%trigger.ownerName = %this.getGroup().name;
	}
}

function fxDTSBrick::City_RemoveTrigger(%brick)
{
	if(isObject(%brick.trigger))
	{
		for(%a = 0; %a < clientGroup.getCount(); %a++)
		{
			%subClient = ClientGroup.getObject(%a);
			if(isObject(%subClient.player) && %subClient.CityData["Trigger"] == %brick.trigger)
				%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, %subClient.player, true);
		}
		
		%boxSize = getWord(%brick.trigger.scale, 0) / 2.5 SPC getWord(%brick.trigger.scale, 1) / 2.5 SPC getWord(%brick.trigger.scale, 2) / 2.5;
		initContainerBoxSearch(%brick.trigger.getWorldBoxCenter(), %boxSize, $typeMasks::playerObjectType);
		while(isObject(%player = containerSearchNext()))
			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, %player);
		
		%brick.trigger.delete();
		return 1;
	}
	return -1;
}

function GameConnection::ListMainRule(%this)
{
	%this.CityData["IHaveReadTheMainRule"] = 1; //If they read this they clearly should read the other rules, but they are counted as read

	%this.chatMessage("<font:arial:20>\c6Please read the rules before playing.");
	%this.chatMessage(" -><font:arial:22> \c6If you get banned and you think it is a mistake, please feel free to talk to a Super Admin.");
	//%this.chatMessage(" -><font:arial:22> \c6If you need a tutorial here, please visit the statue!");
	%this.chatMessage(" -><font:arial:22> \c5Hint: When reading rules, press PGUP/PGDN to see them.");
	%this.chatMessage(" -><font:arial:22> \c6Issues? Contact \c4Visolator \c6on steam! (Steam user: Visolator)");
	%this.chatMessage(" -><font:arial:22> \c6Please read important notes outside, this CityRPG is \c3not \c6the same as the others.");
}
registerOutputEvent("GameConnection", "ListMainRule");

function GameConnection::ListRule(%this, %num)
{
	switch(%num)
	{
		case 1:
			%this.centerPrint("<font:arial:20>\c6Rule \c51\c6: Do not be an asshole; make sure you use common sense.", 4);

		case 2:
			%this.centerPrint("<font:arial:20>\c6Rule \c52\c6: Do not bother other players.", 4);

		case 3:
			%this.centerPrint("<font:arial:20>\c6Rule \c53\c6: Do not cause needless arguments.\n\c3This will result in a kick/ban immediately.", 3);

		case 4:
			%this.centerPrint("<font:arial:20>\c6Rule \c54\c6: Do not try to exploit/grief.\n\c3This can be a permanent ban.", 4);

		case 5:
			%this.centerPrint("<font:arial:20>\c6Rule \c55\c6: Do not spam in any kind of way.", 4);

		case 6:
			%this.centerPrint("<font:arial:20>\c6Rule \c56\c6: Do not post pornographic/trap/scare links.\n\c3This is a permanent ban.", 6);

		case 7:
			%this.centerPrint("<font:arial:20>\c6Rule \c57\c6: Do not kill in the bank. \c7[\c3Auto-ban\c7]", 4);

		default:
			%this.centerPrint("<font:arial:20>\c6(\c3No rule here\c6)", 3);
	}
}
registerOutputEvent("GameConnection", "ListRule", "int 0 12 1");

function GameConnection::ListNote(%this, %num)
{
	switch(%num)
	{
		case 1:
			%this.centerPrint("<font:arial:20>\c6Note \c5#1\c6: Just because someone is wanted, does not mean you can freely kill them!", 6);
			%this.chatMessage("<font:arial:20>\c6Note \c5#1\c6: Just because someone is wanted, does not mean you can freely kill them!");

		case 2:
			%this.centerPrint("<font:arial:20>\c6Note \c5#2\c6: Events are restricted, even if they exist, they may not be usable.", 5);
			%this.chatMessage("<font:arial:20>\c6Note \c5#2\c6: Events are restricted, even if they exist, they may not be usable.");

		case 3:
			%this.centerPrint("<font:arial:20>\c6Note \c5#3\c6: Leaving the server while wanted puts a fake player with your profile.\n\c3Don't do it.", 5);
			%this.chatMessage("<font:arial:20>\c6Note \c5#3\c6: Leaving the server while wanted puts a fake player with your profile. Don't do it.");

		case 4:
			%this.centerPrint("<font:arial:20>\c6Note \c5#4\c6: Purple names are jail immunes;\n\c6They cannot be jailed during that time.", 5);
			%this.chatMessage("<font:arial:20>\c6Note \c5#4\c6: Purple names are jail immunes, they cannot be jailed during that time.");

		case 5:
			%this.centerPrint("<font:arial:20>\c6Note \c5#5\c6: You need EDU in a job to start it. \n\c3Make sure you be careful with what you want.", 6);
			%this.chatMessage("<font:arial:20>\c6Note \c5#5\c6: You need EDU in a job to start it. Make sure you be careful with what you want.");

		case 6:
			%this.centerPrint("<font:arial:20>\c6Note \c5#6\c6: Some info bricks will require to use your shift brick keys (numpad?).\n\c6You can disable this by \c3/ToggleMenu", 5.5);
			%this.chatMessage("<font:arial:20>\c6Note \c5#6\c6: Some info bricks will require to use your shift brick keys (numpad?). You can disable this by \c3/ToggleMenu");

		default:
			%this.centerPrint("<font:arial:20>\c6(\c3Invalid note number\c6)", 3);
	}
}
registerOutputEvent("GameConnection", "ListNote", "int 0 6 1");

function City_getList(%cmd0, %cmd1, %cmd2)
{
	%msg = -1;
	switch$(%cmd1)
	{
		case "rules":
			switch$(%cmd2)
			{
				case 1:
					%this.centerPrint("<font:arial:20>\c6Rule \c51\c6: Do not be an asshole; make sure you use common sense.", 4);

				case 2:
					%this.centerPrint("<font:arial:20>\c6Rule \c52\c6: Do not bother other players.", 4);

				case 3:
					%this.centerPrint("<font:arial:20>\c6Rule \c53\c6: Do not grief.", 3);

				case 4:
					%this.centerPrint("<font:arial:20>\c6Rule \c54\c6: Do not try to exploit. <br>\c4This is a permanent ban.", 4);

				case 5:
					%this.centerPrint("<font:arial:20>\c6Rule \c55\c6: Do not spam in any kind of way.", 4);

				case 6:
					%this.centerPrint("<font:arial:20>\c6Rule \c56\c6: Do not post pornographic/trap/scare links. <br>\c4This is a permanent ban.", 6);

				case 7:
					%this.centerPrint("<font:arial:20>\c6Rule \c57\c6: Do not kill in the bank. \c7[\c3Auto-ban\c7]", 4);

				default:
					%this.centerPrint("<font:arial:20>\c6(\c3No rule here\c6)", 3);
			}

	}

	return %msg;
}

function GameConnection::City_ToggleMode(%this)
{
	if(!%this.hasSpawnedOnce)
		return;

	if(%this.CityData["Mode"] !$= "Play")
	{
		%this.CityData["Mode"] = "Play";
		%this.chatMessage("\c6You are now in \c4play \c6mode.");
		%this.chatMessage(" + \c6You can now play normally.");
		%this.instantRespawn();
	}
	else
	{
		%this.CityData["Mode"] = "Modification";
		%this.chatMessage("\c6You are now in \c4modification \c6mode.");
		%this.chatMessage(" + \c6You can now play abnormally.");
		%this.chatMessage("   - \c6EXP Earning: \c0OFF");
		%this.chatMessage("   - \c6Paychecks: \c0OFF");
		%this.chatMessage("   - \c6Jetting: \c2ON");
		%this.chatMessage("   - \c6Immunity to damage/arrest: \c2ON");
		%this.chatMessage("   - \c6Player profiling: \c2ON");
		if(isObject(%player = %this.player))
			%player.setDatablock(nameToID("PlayerStandardArmor"));
	}
}

function serverCmdToggleMode(%this)
{
	if(!%this.isAdmin)
	{
		if(%this.CityData["Mode"] $= "Play")
			return;
	}

	%bl_id = %this.getBLID();
	
	if($Sim::Time - $Server::LastMode[%bl_id] < 60 && %bl_id != getNumKeyID())
	{
		%this.chatMessage("You must wait " @ mCeil(60 - ($Sim::Time - $Server::LastMode[%bl_id])) @ " second(s) before you can toggle modes again.");
		return;
	}

	%this.City_ToggleMode();
}

function serverCmdToggleMenu(%this)
{	
	if($Sim::Time - %this.lastMenuTime < 3)
	{
		%this.chatMessage("You must wait " @ mCeil(3 - ($Sim::Time - %this.lastMenuTime)) @ " second(s) before you can toggle menu mode again.");
		return;
	}
	%this.lastMenuTime = $Sim::Time;
	%bl_id = %this.getBLID();
	
	$Server::AvoidMenu[%bl_id] = !$Server::AvoidMenu[%bl_id];
	%this.chatMessage("\c6Menu mode: " @ ($Server::AvoidMenu[%bl_id] ? "\c2Simple\c6. You will type in the chat of what you want when asked." : "\c3Advanced\c6. Use brick shift keys! (Numpad?)"));
}

function serverCmdShutDev(%this)
{
	if(!%this.isSuperAdmin)
		return;

	for(%i = ClientGroup.getCount()-1; %i >= 0; %i--)
	{
		%cl = ClientGroup.getObject(%i);
		if(!%cl.isAdmin)
			%cl.delete("Server has been shutdown for development.<br>Hope you enjoyed it!");
	}

	$Pref::Server::Password = "CityRPGDev";
	Webcom_PostServer();

	echo(%this.getPlayerName() @ " has activated the shutdown protocol.");
}

//Remove this when done - Do not forget! $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
//Add people to money list for helping
//- $1000, BL_ID 44678 - Created mini-mart and a parking lot (Cami)
//- $3000, BL_ID 14941 - Help create roads. (Jakob)
//- $5000, BL_ID 28093 - Created a fancy police department. (Georges?)
//- $10000, BL_ID 2213 - Retrieved host's name back. - Silly for this, but he's a nice guy anyways. (Carbon Zypher)

if(!isObject($BotList))
	$BotList = new ScriptGroup();

function GameConnection::getJailTimeImmunity(%this)
{
	if(getSimTime() - %this.firstCity_SpawnTime < $City::Jail::ImmunityJoinTime) //Retuns the amount of time they are immune to when ghosting
		return mClampF($City::Jail::ImmunityJoinTime * (1 + getBrickCount() / strLen(getBrickCount()) / 4 * $City::Jail::ImmunityJoinMultiplyTime), 0, $City::Jail::ImmunityMaxJoinTime);

	return $City::Jail::ImmunityTime;
}

function GameConnection::getSpawnJailImmunity(%this)
{
	return mClampF($City::Jail::ImmunityJoinTime * (1 + getBrickCount() / strLen(getBrickCount()) / 4 * $City::Jail::ImmunityJoinMultiplyTime), 0, $City::Jail::ImmunityMaxJoinTime);
}

/// <summary>
/// Checks whether an object or not contributes to the City gamemode.
/// Yeah, it looks like nothing, but this only sets on certain objects.
/// </summary>
/// <param name="obj">Object ID</param>
function isCityObject(%obj)
{
	if(!isObject(%obj))
		return false;

	if(%obj.getClassName() !$= "GameConnection" && 
		%obj.getClassName() !$= "FakeGameConnection" && 
		%obj.getClassName() $= "Player")
		return false;

	return (%obj.isCity == 1 ? true : false);
}

function GameConnection::City_addEXP(%this, %exp, %class, %hide)
{
	if(%exp == 0)
		return 1;

	%job = %this.getJob();

	if(!isObject(%job))
		return -1;

	if(%class $= "")
		%class = %job.getTreeParsed();

	%this.CityData["EXP", %class] += %exp;
	if(!%hide)
		%this.City_Broadcast((%exp < 0 ? "\c0-" : "\c2+") @ %exp @ " \c6job experience earned. You now have a total of \c4" @ %this.CityData["EXP", %class] @ " \c6experience for " @ %job.tree @ "\c6.");
}

function GameConnection::City_addEDU(%this, %edu, %class, %hide)
{
	if(%exp == 0)
		return 1;

	%job = %this.getJob();

	if(!isObject(%job))
		return -1;

	if(%class $= "")
		%class = %job.getTreeParsed();

	%this.CityData["EDU", %class] += %edu;
	if(!%hide)
		%this.City_Broadcast((%edu < 0 ? "\c0-" : "\c2+") @ %edu @ " \c6job education earned. You now have a total of \c4" @ %this.CityData["EDU", %class] @ " \c6education for " @ %job.tree @ "\c6.");
}

/// <summary>
/// Send a message to the server to everyone.
/// Packaged: Can be local, and has some spam protection and logging.
/// </summary>
/// <param name="this">Client ID</param>
/// <param name="message">Message to say to the server</param>
function serverCmdMessageSent(%this, %message)
{
	if(getSubStr(%message, 0, 1) $= "^")
	{
		%isLocal = 1;
		%message = getSubStr(%message, 1, strLen(%message));
	}

	%message = %this.stripMessage(%message);
	if(%message $= "l_spam")
	{
		messageClient(%this,'',"\c5Do not create long word messages.");
		if(!%this.isAdmin)
		{
			%this.isSpamming = true;
			%this.spamProtectStart = %time;
			%this.schedule($SPAM_PENALTY_PERIOD, spamReset);
			return;
		}
	}

	%length = strLen(%message);
	if(!%length)
		return;

	if(isObject(%trigger = %this.CityData["Trigger"]) && isObject(%brick = %trigger.brick))
	{
		if(isFunction(GameConnection, "parseMenuData_" @ %brick.getDatablock().triggerFunction) && %this.isInShiftMode && $City::CenterPrintMenu && !$Server::AvoidMenu[%this.getBLID()])
			%this.call("parseMenuData_" @ %brick.getDatablock().triggerFunction, %message);
		else if(isFunction(GameConnection, "parseTriggerData_" @ %brick.getDatablock().triggerFunction))
			%this.call("parseTriggerData_" @ %brick.getDatablock().triggerFunction, %message);

		return;
	}

	%time = getSimTime();

	if(!%this.isSpamming)
	{
		//did they repeat the same message recently?
		if(%message $= %this.lastMsg && %time - %this.lastMsgTime < $SPAM_PROTECTION_PERIOD)
		{
			messageClient(%this,'',"\c5Do not repeat yourself.");
			if(!%this.isAdmin)
			{
				%this.isSpamming = true;
				%this.spamProtectStart = %time;
				%this.schedule($SPAM_PENALTY_PERIOD,spamReset);
			}
		}

		//are they sending messages too quickly?
		if(!%this.isAdmin)
		{
			if(%this.spamMessageCount >= $SPAM_MESSAGE_THRESHOLD)
			{
				%this.isSpamming = true;
				%this.spamProtectStart = %time;
				%this.schedule($SPAM_PENALTY_PERIOD,spamReset);
			}
			else
			{
				%this.spamMessageCount ++;
				%this.schedule($SPAM_PROTECTION_PERIOD,spamMessageTimeout);
			}
		}
	}

	//tell them they're spamming and block the message
	if(%this.isSpamming && !%this.isAdmin)
	{
		spamAlert(%this);
		return;
	}

	//eTard Filter, which I hate, but have to include
	if($Pref::Server::eTardFilter)
	{
		%list = strReplace($Pref::Server::eTardList,",","\t");

		for(%i = 0; %i < getFieldCount(%list); %i ++)
		{
			%wrd = trim(getField(%list,%i));
			if(%wrd $= "")
				continue;
			if(striPos(" " @ %message @ " "," " @ %wrd @ " ") >= 0 && !%this.isAdmin)
			{
				messageClient(%this,'',"\c5This is a civilized game. Please use full words.");
				return;
			}
		}
	}

	%adminLevel = %this.isAdmin + %this.isSuperAdmin + (%this.bl_id == getNumKeyID() ? 1 : 0);

	//URLs
	for(%i = 0; %i < getWordCount(%message); %i ++)
	{
		%word = getWord(%message, %i);
		%pos = strPos(%word, "://") + 3;
		%pro = getSubStr(%word, 0, %pos);
		%url = getSubStr(%word, %pos, strLen(%word));

		if((%pro $= "http://" || %pro $= "https://") && strPos(%url, ":") == -1)
		{
			%word = "<sPush><a:" @ %url @ ">" @ %url @ "</a><sPop>";
			%message = setWord(%message, %i, %word);
		}
	}

	//if(%adminLevel >= 2 && %links > 1)
	//	%message = "<linkColor:FFA500>" @ %message;

	%name = %this.getPlayerName();
	if(%this.customName !$= "")
		%name = %this.customName;
	%pre  = %this.clanPrefix;
	%suf  = %this.clanSuffix;

	%this.lastMsg = %message;
	%this.lastMsgTime = %time;

	if(%isLocal && isObject(%player = %this.getControlObject())) //Local
	{
		initContainerRadiusSearch(%player.getPosition(), $City::LocalChatSearch, $TypeMasks::PlayerObjectType);
		while(isObject(%target = containerSearchNext()))
		{
			if(%target.getState() !$= "dead" && isObject(%colClient = %target.client))
			{
				%clients++;
				%all  = '\c7[\c5Local\c7] \c7%1%5%2\c7%3%7: %4';
				%color = "\c3";
				if(%this.chatColor !$= "")
					%color = %this.chatColor;
					
				commandToClient(%colClient, 'chatMessage', %this, '', '', %all, %pre, %name, %suf, %message, %color, "","<color:ffffff>");
			}

			if(isObject(%player = %this.player) && %player.getState() !$= "dead")
			{
				%player.playThread(3,"talk");
				%player.schedule(%length * 50, playThread, 3, "root");
			}
		}

		if(%clients > 0)
			City.LogPublicChat(%this, %message, 1);

		return;
	}

	if(%all $= "")
		%all  = '\c7%1%5%2\c7%3%7: %4';

	%color = "<color:ffff00>";
	if(%this.chatColor !$= "")
		%color = %this.chatColor;

	if(isObject(%player = %this.player) && %player.getState() !$= "dead")
	{
		%player.playThread(3, "talk");
		%player.schedule(%length * 50, playThread, 3, "root");
	}

	City.LogPublicChat(%this, %message);
	if(%this.isInJail())
	{
		%color = "<color:ffa500>";
		if(%this.chatColor !$= "")
			%color = %this.chatColor;

		if(%this.isSuperAdmin)
			%all  = '\c7[\c6Prisoner\c7] \c7%1%5%2\c7%3%7: %4';
		else
		{
			%all  = '\c7[\c6Prisoner\c7] \c7%1%5%2\c7%3%7: %4';
			for(%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%colClient = ClientGroup.getObject(%i);
				
				if(%colClient.isInJail() || %colClient.isAdmin)
					commandToClient(%colClient, 'chatMessage', %this, '', '', %all, %pre, %name, %suf, %message, %color, "","<color:ffffff>");
			}
			return;
		}
	}

	commandToAll('chatMessage',%this,'','',%all, %pre, %name, %suf, %message, %color, "", "<color:ffffff>");
}

/// <summary>
/// Send a message to your team.
/// Packaged: Can be local, and has some spam protection and logging.
/// </summary>
/// <param name="this">Client ID</param>
/// <param name="message">Message to say to your team</param>
function serverCmdTeamMessageSent(%this, %message)
{
	if(getSubStr(%message, 0, 1) $= "^")
	{
		%isLocal = 1;
		%message = getSubStr(%message, 1, strLen(%message));
	}

	%message = %this.stripMessage(%message);
	%response = %this.stripMessageResponse(%message);
	if(%response $= "l_spam")
	{
		messageClient(%this,'',"\c5Do not create long word messages.");
		if(!%this.isAdmin)
		{
			%this.isSpamming = true;
			%this.spamProtectStart = %time;
			%this.schedule($SPAM_PENALTY_PERIOD, spamReset);
			return;
		}
	}

	%length = strLen(%message);
	if(!%length)
		return;

	if(isObject(%trigger = %this.CityData["Trigger"]) && isObject(%brick = %trigger.brick))
	{
		if(isFunction(GameConnection, "parseTriggerData_" @ %brick.getDatablock().triggerFunction))
			%this.call("parseTriggerData_" @ %brick.getDatablock().triggerFunction, %message);
		return;
	}

	%time = getSimTime();

	if(!%this.isSpamming)
	{
		//did they repeat the same message recently?
		if(%message $= %this.lastMsg && %time - %this.lastMsgTime < $SPAM_PROTECTION_PERIOD)
		{
			messageClient(%this,'',"\c5Do not repeat yourself.");
			if(!%this.isAdmin)
			{
				%this.isSpamming = true;
				%this.spamProtectStart = %time;
				%this.schedule($SPAM_PENALTY_PERIOD,spamReset);
			}
		}

		//are they sending messages too quickly?
		if(!%this.isAdmin)
		{
			if(%this.spamMessageCount >= $SPAM_MESSAGE_THRESHOLD)
			{
				%this.isSpamming = true;
				%this.spamProtectStart = %time;
				%this.schedule($SPAM_PENALTY_PERIOD,spamReset);
			}
			else
			{
				%this.spamMessageCount ++;
				%this.schedule($SPAM_PROTECTION_PERIOD,spamMessageTimeout);
			}
		}
	}

	//tell them they're spamming and block the message
	if(%this.isSpamming && !%this.isAdmin)
	{
		spamAlert(%this);
		return;
	}

	//eTard Filter, which I hate, but have to include
	if($Pref::Server::eTardFilter)
	{
		%list = strReplace($Pref::Server::eTardList,",","\t");
		for(%i = 0; %i < getFieldCount(%list); %i ++)
		{
			%wrd = trim(getField(%list,%i));
			if(%wrd $= "")
				continue;
			if(striPos(" " @ %message @ " "," " @ %wrd @ " ") >= 0 && !%this.isAdmin)
			{
				messageClient(%this,'',"\c5This is a civilized game. Please use full words.");
				return;
			}
		}
	}

	%adminLevel = %this.isAdmin + %this.isSuperAdmin + (%this.bl_id == getNumKeyID() ? 1 : 0);
	if(%adminLevel >= 2)
		%message = "<linkColor:FFA500>" @ %message;

	//URLs
	for(%i = 0; %i < getWordCount(%message); %i ++)
	{
		%word = getWord(%message, %i);
		%pos = strPos(%word, "://") + 3;
		%pro = getSubStr(%word, 0, %pos);
		%url = getSubStr(%word, %pos, strLen(%word));

		if((%pro $= "http://" || %pro $= "https://") && strPos(%url, ":") == -1)
		{
			%word = "<sPush><a:" @ %url @ ">" @ %url @ "</a><sPop>";
			%message = setWord(%message, %i, %word);
		}
	}

	%name = %this.getPlayerName();
	if(%this.customName !$= "")
		%name = %this.customName;
	%pre  = %this.clanPrefix;
	%suf  = %this.clanSuffix;

	%this.lastMsg = %message;
	%this.lastMsgTime = %time;

	if(%isLocal && isObject(%player = %this.getControlObject()))
	{
		initContainerRadiusSearch(%player.getPosition(), $City::LocalChatSearch, $TypeMasks::PlayerObjectType);
		while(isObject(%target = containerSearchNext()))
		{
			if(%target.getState() !$= "dead" && isObject(%colClient = %target.client))
			{
				%clients++;
				%all  = '\c7[\c5Local\c7] \c7[%5%6\c7] %1\c3%2\c7%3%7: %4';
				commandToClient(%colClient, 'chatMessage', %this, '', '', %all, %pre, %name, %suf, 
					%message, %this.getJob().chatColor, %this.getJob().uiName, "<color:ffffff>");
			}
		}

		if(%clients > 0)
			City.LogTeamChat(%this, %message, 1);

		if(isObject(%player = %this.player) && %player.getState() !$= "dead")
		{
			%player.playThread(3,"talk");
			%player.schedule(%length * 50, playThread, 3, "root");
		}

		return;
	}

	if(%all $= "")
		%all  = '\c7[%5%6\c7] %1\c3%2\c7%3%7: %4';

	%color = "<color:ffff00>";
	if(%this.chatColor !$= "")
		%color = %this.chatColor;

	if(isObject(%player = %this.player) && %player.getState() !$= "dead")
	{
		%player.playThread(3,"talk");
		%player.schedule(%length * 50, playThread, 3, "root");
	}

	City.LogTeamChat(%this, %message);
	if(%this.isInJail())
	{
		%all  = '\c7[\c6Prisoner\c7] \c7%1%5%2\c7%3%7: %4';
		%color = "<color:ffa500>";
		if(%this.chatColor !$= "")
			%color = %this.chatColor;

		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%colClient = ClientGroup.getObject(%i);

			if(%colClient.isInJail() || %colClient.isAdmin)
				commandToClient(%colClient, 'chatMessage', %this, '', '', %all, %pre, %name, %suf, %message, %color, "","<color:ffffff>");
		}
		return;
	}

	for(%i=0;%i<clientGroup.getCount();%i++)
	{
		%colClient = clientGroup.getObject(%i);
		if(%colClient.getJob() == %this.getJob())
			commandToClient(%colClient, 'chatMessage', %this, '', '', %all, %pre, %name, %suf, %message, %this.getJob().chatColor, %this.getJob().uiName, "<color:ffffff>");
		//City::LogTeamChat already does this.
		//echo("(T-" @ %this.getJob().uiName @ ") " @ %name @ ": " @ %message);
	}
}

if(isPackage(City))
	deactivatePackage(City);

package City
{
	function serverCmdUseSprayCan(%this, %ID)
	{
		if(%this.getClassName() $= "GameConnection")
		{
			if(%this.isInJail() || %this.CityData["CannotPaint"])
			{
				commandToClient(%this, 'setPaintingDisabled', 1);
				return;
			}
		}

		return Parent::serverCmdUseSprayCan(%this, %ID);
	}

	function serverCmdUseInventory(%this, %slot)
	{
		if(%this.getClassName() $= "GameConnection")
		{
			if(%this.isInJail() || %this.CityData["CannotBuild"])
			{
				commandToClient(%this, 'setBuildingDisabled', 1);
				//if(isObject(%this.CityData["Trigger"]))
				//	serverCmdMessageSent(%this, %slot+1);

				return;
			}
		}

		return Parent::serverCmdUseInventory(%this, %slot);
	}

	function serverCmdInstantUseBrick(%this, %ID)
	{
		if(%this.getClassName() $= "GameConnection")
		{
			if(%this.isInJail() || %this.CityData["CannotBuild"])
			{
				commandToClient(%this, 'setBuildingDisabled', 1);
				return;
			}
		}

		return Parent::serverCmdInstantUseBrick(%this, %ID);
	}

	function servercmdBuyBrick(%this, %slot, %ID)
	{
		if(%this.getClassName() $= "GameConnection")
		{
			if(%this.isInJail() || %this.CityData["CannotBuild"])
			{
				commandToClient(%this, 'setBuildingDisabled', 1);
				return;
			}
		}

		return Parent::servercmdBuyBrick(%this, %slot, %ID);
	}

	function serverCmdLeaveMinigame(%this)
	{
		if(!%this.isSuperAdmin)
		{
			%this.chatMessage("\c5I'm afraid I cannot let you do that.. :^)");
			return;
		}
		return Parent::serverCmdLeaveMinigame(%this);
	}

	//Thanks Crown
	function HammerImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
	{
		//Check if it's a vehicle
		if((%col.getType() & $TypeMasks::VehicleObjectType))
		{
			//Determine if the owner is allowed to hammer their own vehicle, if not, return it
			if(($Pref::Server::AllowOwnerVehicleHammerPush && %obj.client != %col.spawnBrick.client) || !$Pref::Server::AllowOwnerVehicleHammerPush)
				return;
		}
		
		Parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
	}

	function WandImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
	{
		//Don't wand player objects such as players/bots
		if(%col.getType() & $TypeMasks::PlayerObjectType)
			return;
		
		Parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
	}

	function vehicle::OnActivate(%vehicle, %activatingObj, %activatingClient, %pos, %vec)
	{
		//If the they don't own the vehicle, don't click push
		if(isObject(%spawn = %vehicle.spawnBrick) && %activatingClient != %spawn.client)
			return;

		Parent::OnActivate(%vehicle, %activatingObj, %activatingClient, %pos, %vec);
	}

	function fxDTSBrick::setVehicle(%this, %vehicle, %client)
	{
		if(!%client.isSuperAdmin && isObject(%client))
			%vehicle = 0;

		Parent::setVehicle(%this, %vehicle, %client);
	}

	function fxDTSBrick::respawnVehicle(%this, %client)
	{
		if(!%client.isSuperAdmin && isObject(%client))
		{
			if(isObject(%this.vehicle))
				%this.setVehicle(0, %client);

			return;
		}

		Parent::respawnVehicle(%this, %client);
	}

	function serverCmdSuicide(%this)
	{
		if(!isObject(%pl = %this.player))
			return;

		if(%pl.getState() $= "dead")
			return;

		if(%this.isInJail())
			return;

		if($Sim::Time - %this.lastArrestAttempt < 30)
		{
			%this.centerPrint("You cannot do this action right now.");
			return;
		}

		if($Sim::Time - %this.lastTeleportTime < 5)
		{
			%this.centerPrint("You cannot do this action right now.");
			return;
		}

		%this.City_doUnstuck();
	}

	function itemData::onPickup(%this, %item, %obj)
	{
		if(isCityObject(%client = %obj.client))
			if(isObject(%job = %client.getJob()))
				if(!%job.canSpawnItems && %item.bl_id != %client.bl_id && !%client.isSuperAdmin)
				{
					%item.delete();
					return;
				}

		Parent::onPickup(%this, %item, %obj);
	}

	function fxDTSBrick::onDeath(%brick)
	{
		%brick.City_RemoveTrigger();

		switch$(%brick.getDatablock().CityBrickType)
		{
			case "Spawn":
				$City::SpawnPoints = removeWord($City::SpawnPoints, %brick);

			case "Teleporter":
				$City::Teleporters = removeWord($City::Teleporters, nameToID(%brick));

			default:
				//Nothing
		}

		parent::onDeath(%brick);
	}

	function fxDTSBrick::onRemove(%brick)
	{
		%brick.City_RemoveTrigger();

		switch$(%brick.getDatablock().CityBrickType)
		{
			case "Spawn":
				$City::SpawnPoints = removeWord($City::SpawnPoints, %brick);

			case "Teleporter":
				$City::Teleporters = removeWord($City::Teleporters, nameToID(%brick));

			default:
				//Nothing
		}

		%client = %brick.getGroup().client;
		if(!isObject(%client))
			%client = %brick.client;

		%adminLevel = %client.isAdmin + %client.isSuperAdmin + (%brick.getGroup().bl_id == getNumKeyID() ? 1 : 0);

		if(isObject(%client) && %brick.getDatablock().CityBrickType $= "" && %adminLevel <= 0 && %brick.fullPlant)
			%client.addBrickVolume(%brick.getVolume() * 8);

		parent::onRemove(%brick);
	}

	function GameConnection::createPlayer(%this, %transform)
	{
		cancel(%this.cityPrefsSch);
		%this.cityPrefsSch = %this.schedule(250, applyCityPrefs);

		return Parent::createPlayer(%this, %transform);
	}

	function GameConnection::onClientEnterGame(%this)
	{
		%this.firstCity_SpawnTime = $Sim::Time + 1000;
		%this.isCity = 1;
		%this.City_SpawnTime = $Sim::Time;

		Parent::onClientEnterGame(%this);
	}

	function GameConnection::onClientLeaveGame(%this)
	{
		if(%this.hasSpawnedOnce)
			%this.City_SaveProfile();

		if(isObject(%player = %this.getControlObject()))
		{
			if($Sim::Time - %this.lastArrestAttempt < 15 && %this.lastArrestAttempt > 0)
			{
				%levelPrint = "\c3*\c7*****";

				announce("\c6" @ %this.getPlayerName() @ " \c5has left before arrest. Their player has been left for 30 seconds. \c6Wanted level: " @ %levelPrint);
				%player = %this.player;
				%this.setName("Client");
				%player.client = new SimObject(: Client)
				{
					isCityObject = 1;
					isCityAIClient = 1;
					class = FakeGameConnection;
					minigame = %this.minigame;
					player = %player;
				};
				%player.clone = true;
				%player.isCityObject = 1;
				%player.isCityAI = 1;
				%player.client.setName("");
				%this.setName("");
				%player.schedule(30000, deleteCheck);
				$BotList.add(%player.client);
				%this.player = "";
				%player.setVelocity("0 0 0");
				%player.playThread(3, sit);
				%player.playThread(2, armReadyBoth);
				%player.playThread(1, crouch);
			}
		}

		if(isObject(%player)) //Not sure if this is broke, but to be sure
			if(isObject(%temp = %player.tempbrick))
				%temp.delete();

		return Parent::onClientLeaveGame(%this);
	}

	function Slayer_MiniGameSO::pickSpawnPoint(%mini, %client)
	{
		if(%client.isInJail() && City_FindSpawn("jailSpawn"))
			%spawn = City_FindSpawn("jailSpawn");
		//else
		//{
		//	if(City_FindSpawn("personalSpawn", %client.bl_id))
		//		%spawn = City_FindSpawn("personalSpawn", %client.bl_id);
		//	else
		//	{
		//		if(isObject(%this.getJob()) && City_FindSpawn("jobSpawn", %this.getJob().uiName))
		//			%spawn = City_FindSpawn("jobSpawn", %this.getJob().uiName);
		//		else
		//			%spawn = City_FindSpawn("jobSpawn", 1);
		//	}
		//}
		
		if(%spawn != 0)
			return %spawn;
		else
			return Parent::pickSpawnPoint(%mini, %client);
	}

	function MiniGameSO::pickSpawnPoint(%mini, %client)
	{
		if(%client.isInJail() && City_FindSpawn("jailSpawn"))
			%spawn = City_FindSpawn("jailSpawn");
		//else
		//{
		//	if(City_FindSpawn("personalSpawn", %client.bl_id))
		//		%spawn = City_FindSpawn("personalSpawn", %client.bl_id);
		//	else
		//	{
		//		if(isObject(%this.getJob()) && City_FindSpawn("jobSpawn", %this.getJob().uiName))
		//			%spawn = City_FindSpawn("jobSpawn", %this.getJob().uiName);
		//		else
		//			%spawn = City_FindSpawn("jobSpawn", 1);
		//	}
		//}
		
		if(%spawn != 0)
			return %spawn;
		else
			return Parent::pickSpawnPoint(%mini, %client);
	}

	function GameConnection::getSpawnPoint(%client)
	{
		if(%client.isInJail() && City_FindSpawn("jailSpawn"))
			%spawn = City_FindSpawn("jailSpawn");
		//else
		//{
		//	if(City_FindSpawn("personalSpawn", %client.bl_id))
		//		%spawn = City_FindSpawn("personalSpawn", %client.bl_id);
		//	else
		//	{
		//		if(isObject(%this.getJob()) && City_FindSpawn("jobSpawn", %this.getJob().uiName))
		//			%spawn = City_FindSpawn("jobSpawn", %this.getJob().uiName);
		//		else
		//			%spawn = City_FindSpawn("jobSpawn", 1);
		//	}
		//}
		
		if(%spawn != 0)
			return %spawn;
		else
			return Parent::getSpawnPoint(%client);
	}

	function FxDtsBrick::plant(%this)
    {
        if(isObject(%this.client))
        {
        	if(!%this.City_Check(0))
        		return 6;
        }
        else
        {
        	if(!%this.City_Check(1))
        		return 5;
        }

        return parent::plant(%this);
    }

	//function fxDTSBrick::onPlant(%brick)
	//{
	//	Parent::onPlant(%brick);
	//	%brick.schedule(0, City_Check);
	//}

	//function fxDTSBrick::onLoadPlant(%brick)
	//{		
	//	Parent::onLoadPlant(%brick);
	//	%brick.schedule(0, City_Check, 1);
	//}

	function serverCmdCreateMinigame(%client, %name, %color, %bool)
	{
		if(%client.isSuperAdmin)
			return Parent::serverCmdCreateMinigame(%client, %name, %color, %bool);
		else
		{
			%client.chatMessage("\c5I'm afraid I will not let you do that :^)");
			%client.play2D(errorSound);
		}
	}

	function MinigameSO::forceEquip(%this, %slot){} //This breaks everything

	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %position)
	{
		Parent::onDeath(%client, %killerPlayer, %killer, %damageType, %position);
		if(isCityObject(%killer) && %killer.getClassName() $= "GameConnection")
			%client.deadTime = $Sim::Time;
	}

	function GameConnection::AutoAdminCheck(%this)
	{
		%this.schedule(100, "applyCityStartPrefs");
		return Parent::AutoAdminCheck(%this);
	}

	function SimGroup::getTrustFailureMessage(%this)
	{
		return Parent::getTrustFailureMessage(%this);
	}

	function SimObject::onCameraEnterOrbit(%obj, %camera){}
	function SimObject::onCameraLeaveOrbit(%obj, %camera){}

	function fxDtsBrick::setName(%this, %name) //Things like lots and custom spawns should not be renamed
	{
		if(%this.doNotRename)
			return;

		Parent::setName(%this, %name);
	}

	function serverCmdSuperShiftBrick(%client, %a, %b, %c)
	{
		if(isObject(%player = %client.player) && %player.isInShiftMode)
		{
			%vec = %a SPC %b SPC %c;
			switch$(%vec)
			{
				case "0 1 0": //Left
					%client.City_onShiftKey("Left");

				case "0 -1 0": //Right
					%client.City_onShiftKey("Right");

				case "1 0 0": //Up (Farther)
					%client.City_onShiftKey("Up");

				case "-1 0 0": //Down (Closer)
					%client.City_onShiftKey("Down");
			}
			return;
		}

		Parent::serverCmdSuperShiftBrick(%client, %a, %b, %c);
	}

	function serverCmdShiftBrick(%client, %a, %b, %c)
	{
		if(isObject(%player = %client.player) && %player.isInShiftMode)
		{
			%vec = %a SPC %b SPC %c;
			switch$(%vec)
			{
				case "0 1 0": //Left
					%client.City_onShiftKey("Left");

				case "0 -1 0": //Right
					%client.City_onShiftKey("Right");

				case "1 0 0": //Up (Farther)
					%client.City_onShiftKey("Up");

				case "-1 0 0": //Down (Closer)
					%client.City_onShiftKey("Down");
			}
			return;
		}

		Parent::serverCmdShiftBrick(%client, %a, %b, %c);
	}

	function serverCmdCancelBrick(%client)
	{
		if(isObject(%player = %client.player) && %player.isInShiftMode)
		{
			%client.City_onShiftKey("Left");
			return;
		}

		Parent::serverCmdCancelBrick(%client);
	}

	function serverCmdPlantBrick(%client)
	{
		if(isObject(%player = %client.player))
		{
			if(%player.isInShiftMode)
			{
				%client.City_onShiftKey("Right");
				return;
			}

			if(!%client.isAdmin && isObject(%temp = %player.tempbrick) && $City::RestrictedBrick[%temp.getDatablock().uiName])
			{
				%client.centerprint("You are not allowed to plant this brick.", 2);
				return;
			}
		}

		Parent::serverCmdPlantBrick(%client);
	}
};
activatePackage(City);

function GameConnection::City_doUnstuck(%this)
{
	if(!isObject(%player = %this.player))
		return;

	if(isEventPending(%this.City_doUnstuckSch))
	{
		cancel(%this.City_doUnstuckSch);
		%this.chatMessage("\c6Canceling unstuck mode.");
	}
	else
	{
		%time = 60 * 5; //5 minutes
		if($Sim::Time - $Server::City::LastUnstuck[%this.getBLID()] < %time)
		{
			%this.chatMessage("\c6Sorry, you can only toggle this every 5 minutes, please wait in \c3" @ City_getDisplayTime(%time - ($Sim::Time - $Server::City::LastUnstuck[%this.getBLID()]), 0, 1) @ "\c6.");
			return;
		}

		%player.City_doUnstuckPos = %player.getPosition();
		%this.City_doUnstuckSch(10);
	}
}

function GameConnection::City_doUnstuckSch(%this, %time)
{
	if(!isObject(%player = %this.player))
		return;

	cancel(%this.City_doUnstuckSch);
	if(vectorDist(%player.City_doUnstuckPos, %player.getPosition()) > 1)
		return;

	if(%time <= 0)
	{
		City_Broadcast("\c3" @ %this.getPlayerName() @ " \c6has teleported back to the spawn.");
		City.Log(%this.getPlayerName() @ " has teleported back to the spawn.");
		%this.centerprint("\c6Teleporting back to spawn in...\n<font:impact:20>\c3NOW", 1);
		%point = %this.getSpawnPoint();
		%player.setTransform(%point);
		$Server::City::LastUnstuck[%this.getBLID()] = $Sim::Time;
	}
	else
	{
		%this.centerprint("\c6Teleporting back to spawn in...\n<font:impact:20>\c3" @ %time, 1.1);
		%this.City_doUnstuckSch = %this.schedule(1000, "City_doUnstuckSch", %time--);
	}
}

$City::CenterPrintMenu = 1;
function GameConnection::City_initMenu(%this)
{
	if(!isObject(%this.CityData["Trigger"]))
		return;

	%maxList = 3;
	%list = %this.CityData["MenuList"];

	%msg = "\c4Menu";
	if(%this.CityData["MenuName"] !$= "")
		%msg = "\c4" @ %this.CityData["MenuName"];

	%start = 0;
	if(%this.CityData["SelectMode"] > %maxList-1)
	{
		%start += %this.CityData["SelectMode"];
		%msg = %msg NL "\c5^^^^";
	}

	if(%start + %maxList > getFieldCount(%list)-1)
		%start = getFieldCount(%list) - %maxList;

	for(%i = %start; %i < %start + %maxList; %i++)
	{
		if(%i >= getFieldCount(%list))
			break;

		%field = getField(%list, %i);
		if(%i == %this.CityData["SelectMode"])
		{
			%this.CityData["SelectModeName"] = stripMLControlChars(%field);
			%msg = %msg NL "<div:1>\c6" @ %field;
		}
		else
			%msg = %msg NL "\c6" @ %field;
	}

	if(getFieldCount(%list) > %maxList && %start + %maxList < getFieldCount(%list))
		%msg = %msg NL "\c5VVVV";

	%this.centerPrint(%msg, -1);
}

function GameConnection::City_onShiftKey(%this, %mode)
{
	switch$(%mode)
	{
		case "Up":
			%this.CityData["SelectMode"] = mClampF(%this.CityData["SelectMode"]--, 0, getFieldCount(%this.CityData["MenuList"])-1);
			%this.City_initMenu();

		case "Down":
			%this.CityData["SelectMode"] = mClampF(%this.CityData["SelectMode"]++, 0, getFieldCount(%this.CityData["MenuList"])-1);
			%this.City_initMenu();

		case "Right":
			if(!isObject(%trigger = %this.CityData["Trigger"]))
				return;

			if(!isObject(%brick = %trigger.brick))
				return;
			
			%this.CityData["SelectMenuMode"] = mClampF(%this.CityData["SelectMenuMode"]++, 0, %this.CityData["MenuListCount"]-1);
			if(isFunction(GameConnection, "parseMenuData_" @ %brick.getDatablock().triggerFunction))
				%this.call("parseMenuData_" @ %brick.getDatablock().triggerFunction, "CHECK", "NEXT");

			%this.CityData["SelectMode"] = 0;
			%this.City_initMenu();

		case "Left":
			if(!isObject(%trigger = %this.CityData["Trigger"]))
				return;

			if(!isObject(%brick = %trigger.brick))
				return;

			%this.CityData["SelectMenuMode"] = mClampF(%this.CityData["SelectMenuMode"]--, 0, %this.CityData["MenuListCount"]-1);
			if(isFunction(GameConnection, "parseMenuData_" @ %brick.getDatablock().triggerFunction))
				%this.call("parseMenuData_" @ %brick.getDatablock().triggerFunction, "CHECK", "BACK");

			%this.CityData["SelectMode"] = 0;
			%this.City_initMenu();
	}
}

function GameConnection::applyCityStartPrefs(%this)
{
	%this.City_LoadProfile();
	if(%this.CityData["JailTime"] >= 1)
		Jail.addMember(%this, %this.CityData["JailTime"]);

	for(%i = 0; %i < City_Triggers.getCount(); %i++)
	{
		%trigger = City_Triggers.getObject(%i);
	}
}

function GameConnection::setCanBuild(%this, %val)
{
	%this.CityData["CannotBuild"] = !%val;
	commandToClient(%this, 'setBuildingDisabled', !%val);
}

function GameConnection::setCanPaint(%this, %val)
{
	%this.CityData["CannotPaint"] = !%val;
	commandToClient(%this, 'setPaintingDisabled', !%val);
}

function GameConnection::applyCityPrefs(%this)
{
	if(!isObject(%this)) return;
	cancel(%this.cityPrefsSch);
	%this.City_SaveProfile();
	%this.City_SpawnTime = $Sim::Time;
	%this.City_Client = 0;
	%this.lastCityDataPrint = 0;

	if(%this.CityData["Mode"] $= "")
		%this.CityData["Mode"] = "Play";

	if(isObject(%player = %this.player))
	{
		if(isObject(%mini = %this.minigame))
		{
			commandToClient(%this, 'setBuildingDisabled', !%mini.EnableBuilding);
			commandToClient(%this, 'setPaintingDisabled', !%mini.EnablePainting);
		}

		if(%this.CityData["Mode"] $= "Modification")
			%player.setDatablock(nameToID(PlayerStandardArmor));

		if(%this.City_SpeedFactor <= 0)
			%this.City_SpeedFactor = 1;

		%player.City_SpeedFactor = %this.City_SpeedFactor;
		%this.isCity = 1;
		%player.isCity = 1;
		%player.setSpeedFactor(1);
		if(!%this.City_Spawned) %this.setJailImmunityTime(%this.getSpawnJailImmunity());
		else %this.setJailImmunityTime(15);
		%this.City_Spawned = 1;
		%this.lastArrestAttempt = 0;
		if(%this.isInJail())
		{
			%player.clearTools();
			%player.addNewItem("RPG Pickaxe");
			%player.addNewItem("RPG Axe");
		}
		else
		{
			if(!%this.CityData["TutorialNew"])
			{
				%this.City_Tutorial(1);
				%this.CityData["TutorialNew"] = 1;
			}

			%this.loadJob();
		}

		%this.schedule(10, City_Update);
	}
	else
		%this.isCity = 0;
}

function fxDTSBrick::City_Check(%brick, %bypass)
{
	%client = %brick.client;
	if(!isObject(%client) && !%bypass)
		%client = %brick.getGroup().client;

	%adminLevel = %client.isAdmin + %client.isSuperAdmin + (%brick.getGroup().bl_id == getNumKeyID() ? 1 : 0);
	if(%client.CityData["Mode"] !$= "")
	{
		if(%client.CityData["Mode"] $= "Play")
			%adminLevel = 0;

		if(%client.CityData["Mode"] $= "BuildModification")
			%adminLevel = 2;
	}

	if(%bypass)
		%adminLevel = 3;

	if(%adminLevel < 1 && %brick.getDatablock().isBotHole && !%bypass) //If someone decided to plant holes because why not, just disable it anyways.
	{
		if(%brick.isPlanted)
			%brick.schedule(0, delete);

		return 0;
	}

	if(%adminLevel <= 0 && !%bypass && $City::Lots::EnableBuildLicense)
	{
		if(isObject(%client))
		{
			if(!%client.canBuildBypass && %client.CityData["BuildingLicense"] <= 0)
			{
				%client.centerprint("\c5Sorry, you need a building license to do so.", 3);
				if(%brick.isPlanted)
					%brick.schedule(0, delete);

				return 0;
			}
		}
	}

	if(%brick.getDatablock() == nameToID(brickSpawnPointData) && %adminLevel < 1 && !%bypass)
	{
		if(isObject(%client) && !%bypass)
			%client.centerPrint("Sorry, you must be admin to plant this brick.", 2);

		if(%brick.isPlanted)
			%brick.schedule(0, delete);

		return 0;
	}

	if(%adminLevel < %brick.getDatablock().requiresAdmin && !%bypass)
	{
		if(isObject(%client))
			%client.centerPrint("Sorry, you must be at a higher admin level to plant this brick.", 2);

		if(%brick.isPlanted)
			%brick.schedule(0, delete);

		return 0;
	}

	%brickAmount = %client.CityData["BrickVolume"];
	if(!$City::Lots::EnableBrickVolume)
		%brickAmount = %client.CityData["Money"];

	//Decided to be lazy with lot code so I used IBan's detection
	//Find planted brick's client, position and world box size
	%client = %brick.client;
	%BrPosXYZ = %brick.getPosition();
	%BrWrldBox = %brick.getWorldBox();
		
	//Determine brick size to use for container search below
	%BrWBx = getWord(%BrWrldBox,3) - getWord(%BrWrldBox,0);
	%BrWBy = getWord(%BrWrldBox,4) - getWord(%BrWrldBox,1);
	%BrWBz = getWord(%BrWrldBox,5) - getWord(%BrWrldBox,2);
		
	//Run a container search to see if brick is within a ModTer brick
	%BrSizeXYZ = %BrWBx - 0.1 SPC %BrWBy - 0.1 SPC %BrWBz - 0.1;
	initContainerBoxSearch(%BrPosXYZ, %BrSizeXYZ, $TypeMasks::TriggerObjectType);

	//if(mCeil(getWord(%brick.rotation, 3)) == 90)
	//	%boxSize = getWord(%brick.getDatablock().brickSizeY, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeX, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;
	//else
	//	%boxSize = getWord(%brick.getDatablock().brickSizeX, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeY, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;
		
	//initContainerBoxSearch(%brick.getWorldBoxCenter(), %boxSize, $typeMasks::TriggerObjectType);
		
	while(isObject(%trigger = containerSearchNext()))
	{
		if(%trigger.getDatablock() == nameToID("CityLotTriggerData"))
		{
			%lotTrigger = %trigger;
			break;
		}
	}

	%isLotTrigger = (%brick.getDatablock().CityBrickType $= "LotTrigger" || %brick.getDatablock().CityBrickType $= "ZoneTrigger");
		
	if(%lotTrigger && !%isLotTrigger)
	{
		%lotTriggerMinX = getWord(%lotTrigger.getWorldBox(), 0);
		%lotTriggerMinY = getWord(%lotTrigger.getWorldBox(), 1);
		%lotTriggerMinZ = getWord(%lotTrigger.getWorldBox(), 2);
			
		%lotTriggerMaxX = getWord(%lotTrigger.getWorldBox(), 3);
		%lotTriggerMaxY = getWord(%lotTrigger.getWorldBox(), 4);
		%lotTriggerMaxZ = getWord(%lotTrigger.getWorldBox(), 5);
			
		%brickMinX = getWord(%brick.getWorldBox(), 0) + 0.0016;
		%brickMinY = getWord(%brick.getWorldBox(), 1) + 0.0013;
		%brickMinZ = getWord(%brick.getWorldBox(), 2) + 0.00126;
			
		%brickMaxX = getWord(%brick.getWorldBox(), 3) - 0.0016;
		%brickMaxY = getWord(%brick.getWorldBox(), 4) - 0.0013;
		%brickMaxZ = getWord(%brick.getWorldBox(), 5) - 0.00126;
			
		if(%brickMinX <= %lotTriggerMinX && %brickMinY <= %lotTriggerMinY && %brickMinZ <= %lotTriggerMinZ)
		{
			%brick.schedule(0, "delete");
			return 0;
		}

		%brickCost = mCeil(%brick.getVolume() * $City::Lots::CostPerVolume);
		if(%brick.getDataBlock().City_Cost > 0)
			%brickCost = %brick.getDataBlock().City_Cost;

		if(isObject(%client) && %adminLevel < 1 && !%bypass)
		{
			//if(%client.CityData["BrickVolume"] < %brickCost && %adminLevel < 2)
			//{
			//	%client.centerPrint("\c6Sorry, you do not have enough brick volume money to plant this brick.<br>\c4Need " 
			//		@ mCeil(%brickCost - %client.CityData["BrickVolume"]) @ " more brick volume.", 3);
			//	%brick.schedule(0, "delete");
			//	return;
			//}
			//%brickVolume = 1;

			if($City::Lots::EnableBuildLicense)
			{
				if(%client.CityData["BuildingLicense"] <= 0)
				{
					%client.centerPrint("\c6Sorry, you need a building license.", 3);
					%brick.schedule(0, "delete");
					return 0;
				}
				else
				{
					if(%client.CityData["BuildingLicense"] == 1)
					{
						if(%brick.getVolume() > 18)
						{
							%client.centerPrint("\c6Sorry, you need to upgrade your building license to plant bigger bricks.", 3);
							%brick.schedule(0, "delete");
							return 0;
						}
					}
					else if(%client.CityData["BuildingLicense"] == 2)
					{
						if(%brick.getVolume() > 24)
						{
							%client.centerPrint("\c6Sorry, you need to upgrade your building license to plant bigger bricks.", 3);
							%brick.schedule(0, "delete");
							return 0;
						}
					}
					else if(%client.CityData["BuildingLicense"] == 3)
					{
						if(%brick.getVolume() > 32)
						{
							%client.centerPrint("\c6Sorry, you need to be admin to plant bigger bricks.", 3);
							%brick.schedule(0, "delete");
							return 0;
						}
					}
				}
			}
		}
	}
	else if(%isLotTrigger && !%bypass)
	{
		if(%brick.getDataBlock().City_Cost > 0)
			%brickCost = %brick.getDataBlock().City_Cost;

		if(%brickAmount < %brickCost && %adminLevel <= 0)
		{
			%client.centerPrint("\c6Sorry, you do not have enough cash to plant this brick.<br>\c3Need " 
				@ mCeil(%brickCost - %client.CityData["Money"]) @ " more cash.", 3);

			if(%brick.isPlanted)
				%brick.schedule(0, "delete");

			return 0;
		}
		%client.addMoney(-%brickCost);
		//%brickVolume = 1;

		if(%lotTrigger)
		{
			if(isObject(%client))
				%client.centerPrint("Only a devil can plant a lot onto a lot.", 3);

			if(%brick.isPlanted)
				%brick.schedule(0, "delete");

			return 0;
		}
	}
	else if(!%bypass)
	{
		if(%adminLevel <= 0 && $City::Lots::RequireAdmin)
		{
			if(isObject(%client))
				%client.centerPrint("You cannot plant a brick outside of your lot.", 3);
			
			if(%brick.isPlanted)
				%brick.schedule(0, "delete");

			return 0;
		}
	}

	%error = "";
	if(%brick.getDatablock().CityBrickType $= "InfoTrigger" || %isLotTrigger)
		%error = %brick.City_CreateTrigger();

	if(%error == -1)
		return 0;

	if(%brick.getDatablock().CityBrickType $= "Spawn")
		$City::SpawnPoints = $City::SpawnPoints SPC nameToID(%brick);

	if(%brick.getDatablock().CityBrickType $= "Teleporter")
		$City::Teleporters = $City::Teleporters SPC nameToID(%brick);

	%error = "";
	if(%brick.getDatablock().CityBrickType $= "Plant")
		%error = %brick.City_CreatePlant(%bypass);

	if(%error == -1)
		return 0;

	%error = "";
	if(isFunction(%brick.getDatablock().getName(), "onPlant_" @ %brick.getDatablock().City_Call))
		%error = %brick.getDatablock().call("onPlant_" @ %brick.getDatablock().City_Call, %brick, %bypass);

	if(%error == -1)
		return 0;

	%error = "";
	if(isFunction(%brick.getClassName(), "onPlant_" @ %brick.getDatablock().City_Call))
		%error = %brick.call("onPlant_" @ %brick.getDatablock().City_Call, %brick, %bypass);

	if(%error == -1)
		return 0;

	if(%brickVolume && %adminLevel <= 0 && !%bypass)
	{
		if(%brickCost > 1)
		{
			if($City::Lots::EnableBrickVolume)
				%client.addBrickVolume(-%brickCost);
			else
				%client.addMoney(-%brickCost);

			%brick.fullPlant = 1;
		}
	}

	return 1;
}

function serverCmdJail(%this, %name, %days)
{
	if(!%this.isAdmin)
		return;

	if(trim(%name) $= "" || !isObject(%name = findClientByName(%name)))
		%name = %this;

	if(%name.isInJail())
	{
		%this.chatMessage("\c5They are already in jail.");
		return;
	}

	%name.addDemerits(mClampF(%days, 0, 365) * $City::Jail::MinDemerits);
	%name.arrest(%this);
	City.Log("[City] " @ %this.getPlayerName() @ " has force-jailed " @ %name.getPlayerName());
}

function SimObject::Arrest(%this, %arrester)
{
	if(isCityObject(%arrester) && !Jail.isMember(%this))
	{
		if(isFunction(%this.getClassName(), getPlayerName))
			%name = %this.getPlayerName();
		else //Must be something else
			%name = %this.netName;
		if(%this.getDaysInJail() < 1)
			%this.CityData["Demerits"] = $City::Jail::MinDemerits;

		%this.CityData["TotalJailTime"] += %this.getDaysInJail();
		%days = mCeil(%this.getDaysInJail());
		announce(%name @ " \c6has been arrested by \c3" @ %arrester.getPlayerName() @ " \c6for \c3" @ %days @ " day" @ (%days != 1 ? "s" : "") @ "\c6.");
		%pay = mClampF(mFloatLength(0.25 * %this.CityData["Demerits"], 2), 0, 5000);
		%arrester.addPaycheckMoney(%pay);
		if(%this.getJob().canArrest)
		{
			%r = getRandom(1, $City::Arrest::Chance);
			if(%r == 1)
				%this.City_addEXP(1);
		}

		if(isObject(%colPl = %this.player) && isObject(%pl = %arrester.player) && %colPl.moneyStolenFrom[%pl] > 0)
		{
			%arrester.chatMessage("You also got \c3$" @ %colPl.moneyStolenFrom[%pl] @ "\c6 back from the criminal.");
			%arrester.addMoney(%colPl.moneyStolenFrom[%pl]);
		}
	}

	if(!%this.isCityAI && !%this.isCityAIClient)
	{
		if(!CommunityService.isMember(%this))
			CommunityService.addMember(%this);

		if(!Jail.isMember(%this))
			Jail.addMember(%this);
		else
		{
			if(isFunction(%this.getClassName(), getPlayerName))
				%name = %this.getPlayerName();
			else //Must be something else
				%name = %this.netName;

			%jailTime = %this.CityData["JailTime"];

			%this.CityData["JailTime"] += %this.CityData["Demerits"] / $City::Jail::MinDemerits;
			%this.CityData["Demerits"] = 0;
			%this.instantRespawn();
			announce(%name @ " \c6has been arrested by \c3" @ %arrester.getPlayerName() @ " \c6for \c3" @ mCeil(%this.CityData["JailTime"] - %jailTime) @ " more day(s)\c6.");
		}
	}
	else
	{
		%this.CityData["JailTime"] += %this.CityData["Demerits"] / $City::Jail::MinDemerits;
		%this.CityData["Demerits"] = 0;
		%this.City_SaveProfile();
		%this.player.delete();
		%this.delete();
	}
}

function FakeGameConnection::onDeath(%this)
{
	
}

function FakeGameConnection::getTeam(%this)
{
	return -1;
}

/// <summary>
/// Parses messages, and includes security
/// </summary>
/// <param name="this">Client ID</param>
/// <param name="message">Message to parse</param>
function GameConnection::stripMessage(%this, %message, %val)
{
	serverCmdStopTalking(%this);
	%message = trim(stripMLControlChars(%message));

	if(strLen(%message) <= 0)
		return;

	return %message;
}

function GameConnection::stripMessageResponse(%this, %message, %val)
{
	serverCmdStopTalking(%this);
	%message = trim(stripMLControlChars(%message));

	if(strLen(%message) <= 0)
		return -1;

	//%message = stripRepeatingChars(%message);
	for(%i=0;%i<getWordCount(%message);%i++)
	{
		if(strLen(getWord(%message, %i)) > 20)
			return "l_spam";
	}

	return 1;
}

function SimObject::getPlayTime(%this)
{
	if(!isCityObject(%this))
		return;
	if(%this.City_SpawnTime != %this.lastCity_SpawnTime)
	{
		%this.CityData["PlayTime"] += ($Sim::Time - %this.City_SpawnTime);
		%this.lastCity_SpawnTime = $Sim::Time;
	}
	%time = %this.CityData["PlayTime"];
	return %time;
}

function Player::City_ModeUpdate(%this)
{
	
}

function GameConnection::City_Update(%this)
{
	cancel(%this.City_UpdateSch);

	if(!%this.City_Spawned)
		return;

	%econMax = $City::Economy::Max - $City::Economy::Start;
	%econ = $City::Economy::Current;
	%econPer = mFloatLength(100 / %econMax * %econ, 2);

	%time = %this.getDaysInJail();

	%this.setScore(mCeil(%this.CityData["Bank"] + %this.CityData["Money"]));
	if(isObject(%player = %this.player))
	{
		%color = "1 1 1 1";
		if($Sim::Time - %this.City_JailImmunity < %this.jailImmunity)
			%color = "1 0 1 1";
		else if(%time >= 1)
			%color = "1 0 0 1";

		if(%this.isInJail() || %this.isInCS())
			%color = "0.9 0.6 0 1";

		%player.setShapeNameColor(%color);

		if(isObject(%player.tempBrick))
			if(%this.CityData["BrickVolume"] > 0)
				%buildAmtPrint = "<just:left>\c6Volume: \c3" @ mFloatLength(%this.CityData["BrickVolume"], 2);
	}

	if(%time >= 1)
	{
		%time = mFloatLength(%time, 1); //Why is this here? I don't know.
		%dems = mCeil(%this.CityData["Demerits"]);

		if(%dems > $City::Crime::Star[$City::Crime::MaxStars] + 200)
		{
			for(%i = 1; %i <= $City::Crime::MaxStars; %i++)
			{
				if(!%yStar_hasBeenSet) //Get to the yellow star first.
				{
					%newStar = "\c0*";
					%yStar_hasBeenSet = 1;
				}
				else
					%newStar = "*";

				%stars = %stars @ %newStar;
			}
		}
		else
		{
			for(%i = 1; %i <= $City::Crime::MaxStars; %i++)
			{
				%demStar = $City::Crime::Star[%i];

				if(!%yStar_hasBeenSet) //Get to the yellow star first.
				{
					%newStar = "\c3*";
					%yStar_hasBeenSet = 1;
				}
				else if(!%gStar_hasBeenSet)
				{
					if(%dems < %demStar)
					{
						%newStar = "\c7*";
						%gStar_hasBeenSet = 1;
					}
					else
						%newStar = "*";
				}
				else
					%newStar = "*";

				%stars = %stars @ %newStar;
			}
		}

		%wantedPrint = "<just:right>\c0Wanted\c6: " @ %stars @ " "; //Dumb print gets cut off
	}

	%moneyColor = "\c2";
	if(%this.tempCityData["MoneyBlinkColor"] !$= "")
	{
		%this.tempCityData["MoneyBlinkColor"] = "";
		%moneyColor = %this.tempCityData["MoneyBlinkColor"];
		%this.tempCityData["MoneyBlinkColor"] = "";
		cancel(%this.City_UpdateSch);
		%this.City_UpdateSch = %this.schedule(200, City_Update);
	}

	%jobPrint = "\c6Job: \c3" @ %this.getJob().uiName;
	if(%this.isInJail())
		%jobPrint = "<color:ffa500>Prisoner \c4(" @ (%t = mCeil(%this.getDaysWhenInJail())) @ " day" @ (%t != 1 ? "s" : "") @ ")";
	else if(%this.isInCS())
		%jobPrint = "<color:ffa500>Community service \c4(" @ (%t = %this.CityData["CommunityService"]) @ " day" @ (%t != 1 ? "s" : "") @ ")";
	else
	{
		%paycheck = %this.getJob().paycheck;
		%moneyPrint = " \c7| \c6Money: " @ %moneyColor @ "$" @ mFloatLength(mClampF(%this.CityData["Money"], 0, $City::Bank::MaxClientCashAmount), 2);
		if(%paycheck > 0)
			%moneyPrint = %moneyPrint @ " \c7| \c6Paycheck: \c2$" @ %paycheck + %this.CityData["ExtraPayCheck"];
	}

	if($City::Economy::Enabled)
	{
		%econColor = "\c4";
		if(mCeil(%this.lastEconomyAmt) < 0)
			%this.lastEconomyAmt = %econPer;

		%econAmt = mFloatLength(%econPer, 2) - mFloatLength(%this.lastEconomyAmt, 2);
		if(%this.lastEconomyAmt != %econPer && %econAmt != 0)
		{
			if(%econAmt > 0) %econColor = "\c2";
			else %econColor = "\c0";

			%this.lastEconomyAmt = %econPer;
			cancel(%this.City_UpdateSch);
			%this.City_UpdateSch = %this.schedule(500, City_Update);
		}

		%econPrint = "\c6Economy: " @ %econColor @ %econPer @ "%";
	}

	%thirst = mCeil(mClampF(100 - %this.CityData["Thirst"], 0, 100));
	%hunger = mCeil(mClampF(100 - %this.CityData["Hunger"], -100, 100)) @ "\c6%";
	if(%hunger < 0)
		%hunger = "\c0Obese";
	if(!%this.avoidSurvival && !$City::Survival::Disabled)
		%survivalPrint = "<br>\c6Starvation: \c3" @ %hunger @ " \c7| \c6Dehydration: \c3" @ %thirst @ "\c6%";

	if(%this.CityData["Upgrade_JobTime"] > 0) %this.CityData["Upgrade_JobWaiting"] = 1;
	else %this.CityData["Upgrade_JobWaiting"] = 0;

	if(%this.CityData["Upgrade_BuilderTime"] > 0)
	{
		%this.CityData["Upgrade_BuilderWaiting"] = 1;
		%buildPrint = "<just:right>\c6Next BuildingLicense: " @ mCeil(%this.CityData["Upgrade_BuilderTime"]) @ " <just:left>";
	}
	else
		%this.CityData["Upgrade_BuilderWaiting"] = 0;

	if(isObject(%lot = %player.currentLot))
		%lotPrint = "<just:right>\c6Owner: \c4" @ %lot.ownerName @ " \c6(\c4" @ %lot.ownerBL_ID @ "\c6)";

	%br0 = "";
	if(%lotPrint @ %buildAmtPrint !$= "")
		%br0 = "<br>";

	if(%this.CityData["Upgrade_JobWaiting"])
		%etcPrint = "<br>\c6Next EDU: \c4" @ mCeil(%this.CityData["Upgrade_JobTime"]);

	%msg = "<font:" @ $City::PrintFont @ ":18>" @ %jobPrint @ %moneyPrint @
		" <just:right>\c6 " @ City.getDate() @ ", " @ City.getTime() @ 
		" <br><just:left>" @ %econPrint @ %buildPrint @
		%survivalPrint @ %wantedPrint @ %br0 @ %buildAmtPrint @ %lotPrint @ %etcPrint;


	if(%this.lastCityDataPrint !$= %msg)
	{
		%this.bottomPrint(%msg, -1, 1);
		%this.lastCityDataPrint = %msg;
	}
}

function GameConnection::setJailImmunityTime(%this, %time)
{
	%this.City_JailImmunity = $Sim::Time;
	%this.jailImmunity = %time;
}

function Player::deleteCheck(%this)
{
	if(isObject(%this))
		%this.delete();
}

function serverCmdStuck(%this)
{
	if(!isObject(%player = %this.player))
		return;

	if(%player.getState() !$= "dead" && getWord(%player.getPosition(), 2) < 6 && $Sim::Time - %player.lastStuck > 3)
	{
		%player.lastStuck = $Sim::Time;
		%player.setVelocity("0 0 20");
	}
	else
	{
		%this.chatMessage("Sorry, you either aren't stuck in a hole or you are typing this command too fast.");
	}
}

function GameConnection::City_SaveProfile(%this)
{
	if(!isCityObject(%this))
		return -1;
	%path = $City::Profiles @ %this.bl_id @ ".CityBLP";
	if(!strLen(%this.CityData["CityPN"]))
		%this.CityData["CityPN"] = %this.bl_id @ getRandom(1000,10000);

	%file = new FileObject();
	%file.openForWrite(%path);
	//See line 8 for the args
	%file.writeLine("CityPN" TAB %this.CityData["CityPN"]);
	%file.writeLine("PlayTime" TAB %this.getPlayTime());
	%file.writeLine("Job" TAB %this.getJob().uiName);
	%file.writeLine("JailTime" TAB %this.getDaysWhenInJail());
	%file.writeLine("Money" TAB %this.CityData["Money"]);
	%file.writeLine("Bank" TAB %this.CityData["Bank"]);
	%file.writeLine("Hunger" TAB %this.CityData["Hunger"]);
	%file.writeLine("Thirst" TAB %this.CityData["Thirst"]);
	%file.writeLine("BuildingLicense" TAB %this.CityData["BuildingLicense"]);
	%file.writeLine("ResourceORE" TAB %this.CityData["ResourceORE"]);
	%file.writeLine("MaxResourceORE" TAB %this.CityData["MaxResourceORE"]);
	%file.writeLine("ExtraPayCheck" TAB %this.CityData["ExtraPayCheck"]);
	%tempList = %this.getJob().getTreeParsed();
	%file.writeLine("EDU_" @ %this.getJob().getTreeParsed() TAB %this.CityData["EDU", %this.getJob().getTreeParsed()]);
	%file.writeLine("EXP_" @ %this.getJob().getTreeParsed() TAB %this.CityData["EXP", %this.getJob().getTreeParsed()]);
	for(%i=0;%i<JobGroup.getCount();%i++)
	{
		%job = JobGroup.getObject(%i);
		%parse = %job.getTreeParsed();
		%duplicate = 0;
		for(%j=0;%j<getWordCount(%tempList);%j++)
		{
			%job_j = getWord(%tempList, %j);
			if(%job_j $= %parse)
				%duplicate = 1;
		}

		if(!%duplicate && %parse !$= "")
		{
			%tempList = %tempList SPC %parse;
			%file.writeLine("EDU_" @ %parse TAB mCeil(%this.CityData["EDU", %parse]));
			%file.writeLine("EXP_" @ %parse TAB mCeil(%this.CityData["EXP", %parse]));
		}
	}
	%file.writeLine("Upgrade_BuilderTime" TAB %this.CityData["Upgrade_BuilderTime"]);
	%file.writeLine("Upgrade_JobToEDU" TAB %this.CityData["Upgrade_JobToEDU"]);
	%file.writeLine("Upgrade_JobTime" TAB %this.CityData["Upgrade_JobTime"]);
	echo("\'" @ %this.getPlayerName() @ "\' profile has been saved.");
	%file.close();
	%file.delete();
	return 1;
}

function SimObject::City_SaveProfile(%this)
{
	if(!isCityObject(%this))
		return -1;
	%path = $City::Profiles @ %this.bl_id @ ".CityBLP";
	if(!strLen(%this.CityData["CityPN"]))
		%this.CityData["CityPN"] = %this.bl_id @ getRandom(1000,10000);

	%file = new FileObject();
	%file.openForWrite(%path);
	//See line 8 for the args
	%file.writeLine("CityPN" TAB %this.CityData["CityPN"]);
	%file.writeLine("PlayTime" TAB %this.getPlayTime());
	%file.writeLine("Job" TAB %this.getJob().uiName);
	%file.writeLine("JailTime" TAB %this.getDaysWhenInJail());
	%file.writeLine("Money" TAB %this.CityData["Money"]);
	%file.writeLine("Bank" TAB %this.CityData["Bank"]);
	%file.writeLine("Hunger" TAB %this.CityData["Hunger"]);
	%file.writeLine("Thirst" TAB %this.CityData["Thirst"]);
	%file.writeLine("BuildingLicense" TAB %this.CityData["BuildingLicense"]);
	%file.writeLine("ResourceORE" TAB %this.CityData["ResourceORE"]);
	%file.writeLine("MaxResourceORE" TAB %this.CityData["MaxResourceORE"]);
	%file.writeLine("ExtraPayCheck" TAB %this.CityData["ExtraPayCheck"]);
	%job = %this.getJob();
	%tempList = %job.getTreeParsed();
	$Server::City::Job[%this.getBLID()] = %job.uiName;
	%file.writeLine("EDU_" @ %job.getTreeParsed() TAB %this.CityData["EDU", %job.getTreeParsed()]);
	%file.writeLine("EXP_" @ %job.getTreeParsed() TAB %this.CityData["EXP", %job.getTreeParsed()]);
	for(%i=0;%i<JobGroup.getCount();%i++)
	{
		%job = JobGroup.getObject(%i);
		%parse = %job.getTreeParsed();
		%duplicate = 0;
		for(%j=0;%j<getWordCount(%tempList);%j++)
		{
			%job_j = getWord(%tempList, %j);
			if(%job_j $= %parse)
				%duplicate = 1;
		}

		if(!%duplicate && %parse !$= "")
		{
			%tempList = %tempList SPC %parse;
			%file.writeLine("EDU_" @ %parse TAB mCeil(%this.CityData["EDU", %parse]));
			%file.writeLine("EXP_" @ %parse TAB mCeil(%this.CityData["EXP", %parse]));
		}
	}
	%file.writeLine("Upgrade_BuilderTime" TAB %this.CityData["Upgrade_BuilderTime"]);
	%file.writeLine("Upgrade_JobToEDU" TAB %this.CityData["Upgrade_JobToEDU"]);
	%file.writeLine("Upgrade_JobTime" TAB %this.CityData["Upgrade_JobTime"]);
	echo("\'" @ %this.getPlayerName() @ "\' profile has been saved.");
	%file.close();
	%file.delete();
	return 1;
}

function GameConnection::City_LoadProfile(%this)
{
	if(!isCityObject(%this))
		return;
	%path = $City::Profiles @ %this.getBLID() @ ".CityBLP";
	if(!isFile(%path))
		return %this.City_NewProfile();
	%file = new FileObject();
	%file.openForRead(%path);
	//See line 8 for the args
	//echo("\'" @ %this.name @ "\' profile has been loaded.");
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		//echo(%line);
		%fLine = strReplace(getField(%line,0)," ","_");
		if(%fLine $= "Job")
		{
			$Server::City::Job[%this.getBLID()] = JobGroup.findScript(getField(%line, 1));
			%this.loadJob();
		}
		else
		{
			//echo("Setting data " @ getWord(getField(%fLine,0),0) @ " to " @ getField(%line, 1));
			%this.CityData[getWord(getField(%fLine, 0), 0)] = getField(%line, 1);
		}
	}
	%file.close();
	%file.delete();
	%this.City_Update();
}

function GameConnection::City_getProfileNumber(%this)
{
	if(!isCityObject(%this))
		return -1;
	if(!strLen(%this.CityData["CityPN"]))
		%this.CityData["CityPN"] = %this.bl_id @ getRandom(1000,10000);
	return %this.CityData["CityPN"];
}

function GameConnection::City_NewProfile(%this, %brokenProfile)
{
	if(!isCityObject(%this))
		return -1;
	%this.setJob("Citizen");
	%path = $City::Profiles @ %this.bl_id @ ".CityBLP"; //Not sure why I put this here
	if(!strLen(%this.CityData["CityPN"]))
		%this.CityData["CityPN"] = %this.bl_id @ getRandom(1000,10000);

	%this.CityData["Hunger"] = 101;
	%this.CityData["Thirst"] = 101;
	%this.CityData["BuildingLicense"] = 0;
	%this.CityData["Job"] = %this.getJob().uiName;
	%this.CityData["Bank"] = 0;
	%this.CityData["Money"] = 0;
	%this.CityData["ExtraPayCheck"] = 0;
	%this.CityData["MaxResourceORE"] = $City::Resources::StartMaxOreSpace;
	%this.CityData["ResourceORE"] = 0;
	%this.addBankMoney(1000);
	City_AddEconomy(-2500);
	//rip
	for(%i = 0; %i < JobGroup.getCount(); %i++)
	{
		%job = JobGroup.getObject(%i);
		%parse = %job.getTreeParsed();
		
		%this.CityData["EDU", %parse] = 0;
		%this.CityData["EXP", %parse] = 0;
	}

	echo("\'" @ %this.getPlayerName() @ "\' profile has been created.");
	City.log(%this.getPlayerName() @ "(" @ %this.bl_id @ ", " @ %this.getRawIP() @ ") - Profile has been created.");
	%this.City_SaveProfile();
	if(!%brokenProfile)
		%this.chatMessage("<font:arial bold:25>\c6Welcome \c3" @ %this.getPlayerName() @ "\c6! We see you are new here. Hope you enjoy the server!");

	%this.City_Update();
	return 1;
}

function City_CanArrest(%attacker, %target)
{
	if(isCityObject(%attacker) && isCityObject(%target))
	{
		if(%attacker.getClassName() !$= "GameConnection" && !%attacker.isCityAI && !%attacker.isCityAIClient)
			%attacker = %attacker.getControllingClient();

		if(%attacker.isCityAI)
			%attacker = %attacker.client;

		if(%target.getClassName() !$= "GameConnection" && !%target.isCityAI && !%target.isCityAIClient)
			%target = %target.getControllingClient();

		if(%target.isCityAI)
			%target = %target.client;

		if((%attacker.getJob().canArrest || %attacker.canArrestAnyone) && (%target.getDaysInJail() >= 1 || %attacker.canArrestAnyone))
			if($Sim::Time - %target.City_JailImmunity > %target.jailImmunity)
				return true;
	}
	return false;
}

function GameConnection::City_CanArrest(%this, %target)
{
	return City_CanArrest(%this, %target);
}

function City_CanLegallyDamage(%attacker, %target)
{
	return 0;
}

function GameConnection::City_CanLegallyDamage(%this, %target)
{
	if(!isObject(%this))
		return 0;

	return City_CanLegallyDamage(%this, %target);
}

//Stolen from IBan and then recoated
function City_FindSpawn(%search, %id)
{
	%search = strlwr(%search);
	%fullSearch = %search @ (%id ? " " @ %id : "");
	
	for(%a = 0; %a < getWordCount($City::SpawnPoints); %a++)
	{
		%brick = getWord($City::SpawnPoints, %a);
		
		if(isObject(%brick))
		{
			%spawnData = strLwr(%brick.getDatablock().spawnData);
			if(%fullSearch $= %spawnData)
				%possibleSpawns = (%possibleSpawns $= "") ? %brick : %possibleSpawns SPC %brick;
		}
		else
			$City::SpawnPoints = strreplace($City::SpawnPoints, %brick, "");
	}
	
	if(%possibleSpawns !$= "")
	{
		%spawnBrick = getWord(%possibleSpawns, getRandom(0, getWordCount(%possibleSpawns) - 1));
		%cords = vectorSub(%spawnBrick.getWorldBoxCenter(), "0 0" SPC (%spawnBrick.getDatablock().brickSizeZ - 3) * 0.1) SPC getWords(%spawnBrick.getTransform(), 3, 6);
		if(getWordCount(%cords) == 7)
			return %cords;
		return false;
	}
	else
		return false;	
}

function findclosestcolor(%x)
{
	for(%a=0; %a<64; %a++)
	{
		%match = mabs(getword(getcoloridtable(%a),0) - getword(%x,0)) + mabs(getword(getcoloridtable(%a),1) - getword(%x,1)) + mabs(getword(getcoloridtable(%a),2) - getword(%x,2)) + mabs(getword(getcoloridtable(%a),3) - getword(%x,3))*4;

		if(%match < %bestmatch || %bestmatch $= "")
		{
			%bestmatch = %match;
			%bestid = %a;
		}
	}
	return %bestid;
}

function GameConnection::addDemeritsIfWitnessed(%this, %dems, %range, %target)
{
	if(!isObject(%target))
		return;

	if(%this.canArrest)
	{
		%this.addDemerits(%dems);
		return true;
	}

	%range = mClampF(%range, 10, 100);
	if(%this.whoSeesMeInRange(%range, %target) > 0)
	{
		%this.addDemerits(%dems);
		return true;
	}

	return false;
}

function GameConnection::whoSeesMeInRange(%this, %range, %opp)
{
	if(!isObject(%player = %this.player) || %player.getState() $= "dead")
		return 0;

	%range = mClampF(%range, 1, 100);

	initContainerRadiusSearch(%player.getPosition(), %range, $TypeMasks::PlayerObjectType);
	while((%target = containerSearchNext()) != 0)
	{
		if(%player != %target && %target != %opp)
		{
			if(%target.canSeeObject(%player) && $City::Witness::SeePlayers)
				%amt++;
			else if(!$City::Witness::SeePlayers)
				%amt++;
		}
	}

	return mCeil(%amt);
}

function fxDtsBrick::canSeeObject(%this, %object){}

function Player::canSeeObject(%this, %object)
{    
	%ev = %this.getEyeVector();  // get the player's eye vector
	%pos = %this.getPosition();  // check where he is
	if(!isObject(%object)) // does the object exist?
		return 0;  // if it doesn't then fuck trying to find it
	%ep = isFunction(%object.getClassname(), getEyePoint) ? %object.getEyePoint() : %object.getPosition(); // check if the object's eyepoint can be got and if so get it's eye point and pos
	%vd = vectorDist(%pos, %ep);  // find how far away they are from each other
	if(%vd > 200)  // if they're further than reasonable distance,
		return 0; // sod it
	%cast = containerRaycast(%pos, %ep, $TypeMasks::FxBrickObjectType); // check if there are any bricks blocking the way
	if(isObject(%cast)) // if you find anything blocking the view of the object,
		return 0;  // it must be unviewed.
	%adjp = vectorSub(%pos, %ep);  // do some maths
	%angle = mATan(getWord(%adjp,1), getWord(%adjp,0));
	%ea = mATan(getWord(%ev,1), getWord(%ev,0));
	%cansee = %ea - %angle;
	%canSee = mAbs(%canSee);
	if(%cansee > 3.92689 || %cansee < 2.35629)
		return 0;
	return 1; // conclude it can be seen if we got this far
}

function GameConnection::centerprint_addLine(%this, %message, %timeS)
{
	%timeS = mClampF(%timeS, 0.1, 10);
	%this.centerPrint = trim(%this.centerPrint TAB %message);

	if(getFieldCount(%this.centerPrint) > 3) %print = getFields(%this.centerPrint, 0, 2);
	else if(getFieldCount(%this.centerPrint) == 2) %print = getFields(%this.centerPrint, 0, 1);
	else %print = %this.centerPrint;

	if(getFieldCount(%print) != 3)
	{
		if(getFieldCount(%print) == 2)
			%print = " \t" @ %print;
		else
			%print = " \t \t" @ %print;
	}
	%print = strReplace(%print, "\t", "<br>\cr");
	%this.centerPrint(%print, -1);
	%this.schedule(1000 * %timeS, centerprint_removeLine, %time);
}

function GameConnection::centerprint_removeLine(%this, %time)
{
	cancel(%this.centerPrintSch);
	
	if(getFieldCount(%this.centerPrint) == 1)
	{
		%this.centerPrint = "";
		%this.centerPrint(" ", 0.1);
		return;
	}

	if(getFieldCount(%this.centerPrint) > 1)
	{
		%this.centerPrint = removeField(%this.centerPrint, getFieldCount(%this.centerPrint)-1);
		%ye = 1;
	}
	else if(getFieldCount(%this.centerPrint) == 1)
	{
		%this.centerPrint = getField(%this.centerPrint, 0);
		%this.centerPrintSch = %this.schedule(1000 * %time, centerprint_removeLine);
		%ye = 1;
	}

	if(%ye)
	{
		if(getFieldCount(%this.centerPrint) >= 3) %print = removeField(%this.centerPrint, 2);
		else if(getFieldCount(%this.centerPrint) == 2) %print = removeField(%this.centerPrint, 1);
		else %print = %this.centerPrint;

		if(getFieldCount(%print) != 3)
		{
			if(getFieldCount(%print) == 2)
				%print = " \t" @ %print;
			else
				%print = " \t \t" @ %print;
		}

		%print = strReplace(%print, "\t", "<br>\cr");
		%this.centerPrint(%print, -1);
	}
	else
		%this.centerPrint("", 0.1);
}

function GameConnection::testCenterPrintLines(%this)
{
	%this.centerprint_addLine("\c6You have been caught commiting a crime. [\c3Attempted Murder\c6]", 2);
	%this.schedule(1500, centerprint_addLine, "\c6You have been caught commiting a crime. [\c3Murder\c6]", 3);
	%this.schedule(2500, centerprint_addLine, "\c6You have been caught commiting a crime. [\c3DedMurder\c6]", 4);
	%this.schedule(2500, centerprint_addLine, "\c6You have been caught commiting a crime. [\c3ComMurder\c6]", 6);
	//Adding things at the same time will go at the same time, so different times at the end
}

function servercmdReapTempbricks(%cl)
{
	if(!%cl.isAdmin && !%cl.isSuperAdmin) return;
	reapTempBricks(%cl);
}

function reapTempBricks(%cl)
{
	%temps = 0; %groups = MainBrickgroup.getCount();
	for(%i=0;%i<%groups;%i++)
	{
		%group = MainBrickgroup.getObject(%i);
		%bricks = %group.getCount();
		for(%j=0;%j<%bricks;%j++)
		{
			%brick = %group.getObject(%j);
			if(!%brick.isPlanted) %temp[-1+%temps++] = %brick;
		}
	}
	%count = ClientGroup.getCount();
	for(%i=0;%i<%count;%i++)
	{
		%this = ClientGroup.getObject(%i);
		if(!isObject(%pl = %this.player)) continue;
		if(!isObject(%temp = %pl.tempbrick)) continue;
		for(%j=0;%j<%temps;%j++)
			if(%temp[%j] == %temp) %shift++;
			else %temp[%j - %shift] = %temp[%j];
		%temps -= %shift;
	}
	for(%i=0;%i<%temps;%i++) %temp[%i].delete();
	%name = %cl.name;
	if(%name $= "")
		%name = "Server";
	if(%temps != 0) messageAll('MsgClearBricks', "\c3" @ %name @ "\c2 has reaped \c3" @ %temps @ "\c2 disowned tempbrick" @ (%temps == 1 ? "": "s") @ ".");
}

function servercmdForceReapTempbricks(%cl)
{
	if(!%cl.isSuperAdmin) return;
	forceReapTempBricks(%cl);
}

function forceReapTempBricks(%cl)
{
	(%max = new SimObject()).delete();
	for(%i=0;%i<%max;%i++)
	{
		if(!isObject(%i) || %i.getClassName() !$= "fxDtsBrick" || %i.isPlanted())
			continue;

		%temp[-1+%temps++] = %i;
	}
	%count = ClientGroup.getCount();
	for(%i=0;%i<%count;%i++)
	{
		%this = ClientGroup.getObject(%i);
		if(!isObject(%pl = %this.player)) continue;
		if(!isObject(%temp = %pl.tempbrick)) continue;
		%dontReap[%temp] = 1;
	}
	for(%i=0;%i<%temps;%i++) if(!%dontReap[%obj = %temp[%i]]) { %obj.delete(); %reaped++; }
	%name = %cl.name;
	if(%name $= "")
		%name = "Server";
	if(%reaped != 0) messageAll('MsgClearBricks', "\c3" @ %name @ "\c2 force reaped \c3" @ %reaped @ "\c2 disowned tempbrick" @ (%reaped == 1 ? "": "s") @ ".");
}

function City_ClientLoop()
{
	cancel($City::Schedule);

	if((%count = ClientGroup.getCount()) > 0)
	{
		for(%i = 0; %i < %count; %i++)
		{
			%client = ClientGroup.getObject(%i);

			if(isObject(%player = %client.player) && %player.getState() !$= "dead")
			{

			}
		}
	}

	$City::Schedule = schedule(100, 0, City_ClientLoop);
}
schedule(0, 0, City_ClientLoop);

City_Debug("File > server.cs", "   -> Loading complete. Loading other assets.");
City_Debug(" ");

function City_LoadCore()
{
	exec("Add-Ons/GameMode_City/server.cs");
	$Server::City::LoadedCore = 1;
}

function City_LoadCommon()
{
	City_loadFilePath("add-ons/GameMode_City/Common/*");

	$Server::City::LoadedCommon = 1;
}

if(!$Server::City::LoadedCommon)
	City_LoadCommon();

function City_LoadMisc()
{
	City_loadFilePath("add-ons/GameMode_City/CityItems/RPGTools/*");
	City_loadFilePath("add-ons/GameMode_City/CityItems/Farming/*");
	exec("add-ons/GameMode_City/CityItems/Bricks/roads.cs");
	exec("add-ons/GameMode_City/CityItems/weapons/baton.cs");

	$Server::City::LoadedMisc = 1;
}

if(!$Server::City::LoadedMisc)
	City_LoadMisc();

function ApplyCityStuff(%val)
{
	if(!$Server::City::LoadedStuff || %val > 0)
	{
		$City::RestrictedBrick["ModTer"] = 1;

		if(!isFile("config/server/CityPrefs.cs") || %val > 1)
		{
			//Other scripts
			//-------------

			//Teleporting.cs
			$City::Teleport::Enabled = 1;
			$City::Teleport::MinCostDistance = 20;
			$City::Teleport::CostPerDistance = 0.05;

			//Tutorial.cs
			//Always enable the tutorial. - Enables the tutorial
			$City::Tutorial::Enabled = 1;
			//Relies on - $City::Tutorial::Enabled : Forces new players to use the tutorial, but have an option to opt out.
			$City::Tutorial::NewPlayers = 1;
			//-------------

			//JailSO.cs
			$City::Jail::ReduceDemeritsPerDay = 30;
			$City::Jail::CostPerLevel = 100;

			$City::Jail::ImmunityTime = 10;
			$City::Jail::ImmunityJoinTime = 10; //When they join the game, if they are still wanted, give them a bit of time to ghost. This is also changed by brickcount.
			$City::Jail::ImmunityJoinMultiplyTime = 0.0002; //Based on brickcount, multiply it
			$City::Jail::ImmunityMaxJoinTime = 60;
			$City::Jail::MaxDays = 2;

			$City::Jail::MinDemerits = 100;
			$City::Crime::Star[1] = $City::Jail::MinDemerits;
			$City::Crime::Star[2] = $City::Crime::Star[1] + 140;
			$City::Crime::Star[3] = $City::Crime::Star[2] + 210;
			$City::Crime::Star[4] = $City::Crime::Star[3] + 310;
			$City::Crime::Star[5] = $City::Crime::Star[4] + 420;
			$City::Crime::Star[6] = $City::Crime::Star[5] + 550;
			$City::Crime::MaxStars = 6;
			//-------------

			//JailSO.cs - CommunityService
			$City::CommunityService::MaxDays = 30;
			$City::CommunityService::MinDemerits = 30;
			//-------------

			//CitySO.cs
			$City::Economy::Enabled = 0; //Because this is a feature that won't work too well, we will just leave it off
			$City::Economy::Max = 100000000; //We need a max, which is $100,000,000 - This in 32-bit unsigned integer
			$City::Economy::Start = mCeil($City::Economy::Max * 0.6); //Start with 60% of $100,000,000
			$City::Economy::Current = $City::Economy::Start;
			//-------------

			//Bank.cs
			$City::Bank::MaxClientCashAmount = 100000; //100 thousand
			$City::Bank::MaxClientAmount = 10000000; //10 million
			$City::Money::DisappearMS = 30000;
			$City::Money::MaxCashDrop = 10000; //10 thousand
			$City::Money::Scale = "1 1 1";

			//"add-ons/GameMode_City/CityItems/Money/dollar.dts"
			$City::Money::CashItem = "cashItem";
			//-------------

			//ResourceSO.cs
			$City::Resources::StartMaxOreSpace = 60;
			$City::Resources::MaxOreSpace = 1028;
			$City::Resources::OreSpaceCost = 25;
			$City::Resources::NoOreColorID = findclosestcolor(getColorF("20 20 18 255"));
			$City::Resources::OreColorID = findclosestcolor(getColorF("255 255 255 255")); //218 165 31 255
			//-------------

			//Survival.cs
			$City::Survival::HungerDepletePerTick = 0.1;
			$City::Survival::ThirstDepletePerTick = 0.04;
			$City::Hunger::EnableScaling = 0;
			$City::Survival::Disabled = 1;
			//-------------

			//Lots.cs
			$City::Lots::EnableBuildLicense = 0;
			$City::License::Builder = 500;
			$City::Lots::CostPerStorageVolume = 1.4;
			$City::Lots::CostPerVolume = 12;
			$City::Lots::LotMultiplier = 10;
			//-------------

			//Education.cs
			$City::Education::JobCost = 250;
			//-------------

			//JobSO.cs
			$City::FilePath_Jobs = "Add-Ons/Gamemode_City/Jobs/";
			$City::FilePath_JobSaver = "config/server/JobData.cs";
			//-------------

			//server.cs (main)
			$City::LocalChatSearch = 40;
			$City::Minigame::Debug = 1; //Remove when done
			$City::Profiles = "config/server/City/Profiles/";
			$City::Debug = 0;

			$City::CanPayDemerits = 1;
			$City::Lots::RequireAdmin = 1;
			$City::Arrest::Chance = 3;
			$City::PrintFont = "Arial";

			export("$City::*", "config/server/CityPrefs.cs");
		}
		else
			exec("config/server/CityPrefs.cs");

		restrictInputEvent("fxDTSBrick", "onRelay");

		restrictOutputEvent("fxDTSBrick", "RadiusImpulse");
		restrictOutputEvent("fxDTSBrick", "SetItem");
		restrictOutputEvent("fxDTSBrick", "SetItemDirection");
		restrictOutputEvent("fxDTSBrick", "SetItemPosition");
		restrictOutputEvent("fxDTSBrick", "SetVehicle");
		restrictOutputEvent("fxDTSBrick", "RespawnVehicle");
		restrictOutputEvent("fxDTSBrick", "SpawnExplosion");
		restrictOutputEvent("fxDTSBrick", "SpawnItem");
		restrictOutputEvent("fxDTSBrick", "SpawnProjectile");

		restrictOutputEvent("Player", "AddHealth");
		restrictOutputEvent("Player", "AddVelocity");
		restrictOutputEvent("Player", "BurnPlayer");
		restrictOutputEvent("Player", "ChangeDatablock");
		restrictOutputEvent("Player", "ClearBurn");
		restrictOutputEvent("Player", "ClearTools");
		restrictOutputEvent("Player", "Dismount");
		restrictOutputEvent("Player", "InstantRespawn");
		restrictOutputEvent("Player", "Kill");
		restrictOutputEvent("Player", "SetHealth");
		restrictOutputEvent("Player", "SetPlayerScale");
		restrictOutputEvent("Player", "SetVelocity");
		restrictOutputEvent("Player", "SpawnExplosion");
		restrictOutputEvent("Player", "SpawnProjectile");

		restrictOutputEvent("Player", "playSound");

		restrictOutputEvent("GameConnection", "IncScore");

		restrictOutputEvent("MiniGame", "BottomPrintAll");
		restrictOutputEvent("MiniGame", "CenterPrintAll");
		restrictOutputEvent("MiniGame", "ChatMsgAll");
		restrictOutputEvent("MiniGame", "Reset");
		restrictOutputEvent("MiniGame", "RespawnAll");

		//For future events:

		//VCE
		restrictOutputEvent("fxDtsBrick","VCE_modVariable");
		restrictOutputEvent("fxDtsBrick","VCE_ifValue");
		restrictOutputEvent("fxDtsBrick","VCE_retroCheck");
		restrictOutputEvent("fxDtsBrick","VCE_ifVariable");
		restrictOutputEvent("fxDtsBrick","VCE_stateFunction");
		restrictOutputEvent("fxDtsBrick","VCE_callFunction");
		restrictOutputEvent("fxDtsBrick","VCE_relayCallFunction");
		restrictOutputEvent("fxDtsBrick","VCE_saveVariable");
		restrictOutputEvent("fxDtsBrick","VCE_loadVariable");

		restrictInputEvent("fxDtsBrick","onVariableTrue");
		restrictInputEvent("fxDtsBrick","onVariableFalse");
		restrictInputEvent("fxDtsBrick","onVariableFunction");
		restrictInputEvent("fxDtsBrick","onVariableUpdate");

		restrictOutputEvent("Player","VCE_ifVariable");
		restrictOutputEvent("Player","VCE_modVariable");
		restrictOutputEvent("GameConnection","VCE_ifVariable");
		restrictOutputEvent("GameConnection","VCE_modVariable");
		restrictOutputEvent("Minigame","VCE_ifVariable");
		restrictOutputEvent("Minigame","VCE_modVariable");
		restrictOutputEvent("Vehicle","VCE_ifVariable");
		restrictOutputEvent("Vehicle","VCE_modVariable");

		//Player transform
		//restrictOutputEvent("fxDtsBrick", "setPlayerTransform");
	}
	$GameModeDisplayName = "CityRPG v0.8";
	$Server::City::LoadedStuff = 1;
}
schedule(0, 0, ApplyCityStuff);

//Stolen from RTB

//- addItemToList (adds an item to a space delimited list)
function addItemToList(%string,%item)
{
	if(hasItemOnList(%string,%item))
		return %string;

	if(%string $= "")
		return %item;
	else
		return %string SPC %item;
}

//- hasItemOnList (checks for an item in a list)
function hasItemOnList(%string,%item)
{
	for(%i=0;%i<getWordCount(%string);%i++)
	{
		if(getWord(%string,%i) $= %item)
			return 1;
	}
	return 0;
}

//- removeItemFromList (removes an item from a space-delimited list)
function removeItemFromList(%string,%item)
{
	if(!hasItemOnList(%string,%item))
		return %string;

	for(%i=0;%i<getWordCount(%string);%i++)
	{
		if(getWord(%string,%i) $= %item)
		{
			if(%i $= 0)
				return getWords(%string,1,getWordCount(%string));
			else if(%i $= getWordCount(%string)-1)
				return getWords(%string,0,%i-1);
			else
				return getWords(%string,0,%i-1) SPC getWords(%string,%i+1,getWordCount(%string));
		}
	}
}

//End of stolen functions from RTB

function City_getDisplayTime(%time, %ignoreSeconds, %timestring)
{
	%days = mFloor(%time / 86400);
	%hours = mFloor(%time / 3600);
	%minutes = mFloor((%time % 3600) / 60);
	%seconds = mFloor(%time % 3600 % 60);

	if(%timestring)
	{
		if(%days > 0)
			%nDays = %days @ " day" @ (%days != 1 ? "s" : "");

		if(%hours > 0)
			%nHours = %hours @ " hour" @ (%hours != 1 ? "s" : "");

		if(%minutes > 0)
			%nMinutes = %minutes @ " minute" @ (%minutes != 1 ? "s" : "");

		if(%seconds > 0 && !%ignoreSeconds)
			%nSeconds = %seconds @ " second" @ (%seconds != 1 ? "s" : "");

		%nTimeString = trim(%nDays TAB %nHours TAB %nMinutes TAB %nSeconds);
		%nTimeStringCount = getFieldCount(trim(%nDays TAB %nHours TAB %nMinutes TAB %nSeconds));

		if(%nTimeStringCount <= 0)
			return "0 seconds";

		if(%nTimeStringCount > 1)
		{
			%nTimeStringLast = getField(%nTimeString, %nTimeStringCount-1);
			%nTimeString = getFields(%nTimeString, 0, %nTimeStringCount-2);
		}
		else
			%nTimeString = getField(%nTimeString, 0);

		%nTimeString = strReplace(%nTimeString, "" TAB "", ", ");
		%nTimeString = %nTimeString @ (%nTimeStringLast !$= "" ? " and " @ %nTimeStringLast : "");

		return %nTimeString;
	}

	return %days TAB %hours TAB %minutes TAB %seconds;
}