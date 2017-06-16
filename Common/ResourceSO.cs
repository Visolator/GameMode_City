City_Debug("File > ResourceSO.cs", "Loading assets..");

datablock fxDTSBrickData(ResourceCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Resource Brick";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "Resource";
	triggerSize = "2 4 1";
	trigger = 0;
};

function GameConnection::parseTriggerData_Resource(%this, %message)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%amt = mFloor(%message);
	%money = %this.CityData["Money"];
	%ore = %this.CityData["ResourceORE"];
	%maxOre = mFloor(%this.CityData["MaxResourceORE"]);
	if(%message $= "ENTER")
	{
		%this.chatMessage("\c6Welcome to the Resource Department, how may we help you?");
		if(%ore > 0)
		{
			%list = %list SPC "1";
			%this.chatMessage(" \c31 \c7- \c6Sell ore");
		}
		else
			%this.chatMessage(" X \c7- \c6You don't have any valuable ore to sell");

		if(%maxOre < $City::Resources::MaxOreSpace)
		{
			%list = %list SPC "2";
			%this.chatMessage(" \c32 \c7- \c6Get more ore space (Current size: \c4" @ %maxOre @ "\c6/\c4" @ $City::Resources::MaxOreSpace @ "\c6) (Cost per 1 backpack space: \c3$" @ mFloatLength($City::Resources::OreSpaceCost, 2) @ "\c6)");
		}
		else
			%this.chatMessage(" X \c7- \c6Sorry, you cannot upgrade anymore ore space. (Max is \c3" @ $City::Resources::MaxOreSpace @ "\c6)");

		%list = trim(%list);
		%list = strReplace(%list, " ", ",");
		%this.CityData["Trigger_CanChoose"] = %list;
	}
	else if(%message $= "LEAVE")
	{
		%this.CityData["Trigger_Mode"] = "";
		%this.CityData["Trigger_CanChoose"] = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(containsSubstring(%this.CityData["Trigger_CanChoose"], %amt) && %this.CityData["Trigger_Mode"] $= "")
	{
		%this.CityData["Trigger_Mode"] = %amt;
		switch(%amt)
		{
			case 1:
				%cash = mFloatLength(%ore * 0.75, 2);
				%this.chatMessage("\c6All of your values of ore have been turned into cash. You have earned: \c3$" @ %cash @ "\c6.");
				%this.addBankMoney(%cash);
				%this.City_BroadCast("We have added \c4$" @ %cash @ " \c6into your bank account successfully.", "Important");
				%this.CityData["ResourceORE"] = 0;
				%this.City_SpeedFactor = 1;
				%player.City_SpeedFactor = %this.City_SpeedFactor;
				%player.setSpeedFactor(%player.City_SpeedFactor);

			case 2:
				%this.chatMessage("\c6How much do you want? Please type the number. Remember, each ore space costs \c3$" @ mFloatLength($City::Resources::OreSpaceCost, 2) @ "\c6.");
				%this.CityData["Trigger_Mode"] = 2;

			default:
				%this.chatMessage("Seems you somehow got here. How are you?");
		}
	}
	else if(%this.CityData["Trigger_Mode"] == 2)
	{
		if(%amt + %maxOre > $City::Resources::MaxOreSpace)
			%amt = $City::Resources::MaxOreSpace - %maxOre;

		if(%amt <= 0)
		{
			%this.chatMessage("\c5Looks like you can't increase your backpack space, sorry!");
			%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
			return;
		}

		%cost = mFloatLength($City::Resources::OreSpaceCost * %amt, 2);
		if(%cost > %money)
		{
			%this.chatMessage("\c5Sorry, you do not have enough cash. You need \c3$" @ %cost @ " \c5for that amount.");
			%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
			return;
		}

		%this.addMoney(-%cost);
		%this.CityData["MaxResourceORE"] += %amt;
		%this.chatMessage("\c5Added \c4" @ %amt @ " \c5more backpack space! You now have: \c3" @ mFloor(%this.CityData["MaxResourceORE"]));
		%this.City_SpeedFactor = mClampF(1.65 - (%this.CityData["ResourceORE"] / %this.CityData["MaxResourceORE"]), 0.1, 1);
		%player.City_SpeedFactor = %this.City_SpeedFactor;
		%player.setSpeedFactor(%player.City_SpeedFactor);
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

//This contains mining and the item/vehicle system.

//Create the item system
if(!isObject(CityItemSO))
{
	new ScriptGroup(CityItemSO)
	{
		class = "City_ItemSystem";
		pref["filePath"] = "config/server/City/SavedItems/";
	};
	CityItemSO.schedule(1000, Load);
}

function City_ItemSystem::find(%this, %name)
{
	if(isObject(%name))
		for(%i = 0; %i < %this.getCount(); %i++)
		{
			%obj = %this.getObject(%i);

			if(isObject(%name))
			{
				if(nameToID(%name) == nameToID(%obj))
					return %obj;
			}
		}

	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%obj = %this.getObject(%i);
		
		if(%obj.uiName $= %name)
			return %obj;
	}

	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%obj = %this.getObject(%i);
		
		if(strPos(%obj.uiName, %name) >= 0)
			return %obj;
	}
	
	return -1;
}

function City_ItemSystem::Load(%this)
{
	//--PARAMETERS - READ CAREFULLY--\\
	
	//Description: This will display the current job fields it can support.

	//--KEY--\\
	//	//_!	<- Required field, otherwise it will be confused
	//	//->	<- Normal
	//	//-?	<- Can depend on things
	//	-/- 	<- Comment, do not copy that part if you plan to

	//~MAIN~\\
	//-> Description	string	Describe the object.
	//-> AdminLevel	int	Required level to buy and equip the item.
	//-> JobRequired	string	Required jobs seperated by fields (tabs) to aquire the item.
	//-> Cost	int	Cost of the item, remember, this will auto-adjust by the tax rates and the economy and the seller.

	//--YOU MAY USE FUNCTIONS, HERE IS HOW IT GOES--\\
	//	CMD function(args[, ...]);

	%this.deleteAll();
	%path = %this.pref["filePath"] @ "*";

	//Add the default tools.
	registerJob("Hammer",
		"description Building tool to hammer bricks. Witnessed to be used as a weapon for self-defense.", 
		"cost 0");

	registerJob("Wrench",
		"description Building tool to wrench events on bricks. Witnessed to be used as a mass weapon of destruction.", 
		"cost 0");

	registerJob("Printer",
		"description Building tool to print on bricks.", 
		"cost 0");

	//announce("Loading saved files for mob classes. -> Path: " @ %path);
	%file = findFirstFile(%path);
	if(isFile(%file))
	{
		%fileExt = fileExt(%file);
		%name = fileBase(%file);
		if(%fileExt $= ".cs") //Just making sure
		{
			if(isObject(%obj = isRegisteredCityItem(fileBase(%path))))
				%obj.delete();

			exec(%file);
		}
	}
	else
		return;

	while(%file !$= "")
	{
		%file = findNextFile(%path);
		%fileExt = fileExt(%file);
		%name = fileBase(%file);
		if(%fileExt $= ".cs") //Just making sure
		{
			if(isObject(%obj = isRegisteredCityItem(fileBase(%path))))
				%obj.delete();

			exec(%file);
		}
	}
}

/// <summary>
/// When the job is created, we need to format the variables, so we put it into a command, see -> registerJob
/// Do not use this function.
/// </summary>
/// <param name="this">Name of the created mob.</param>
/// <param name="com">Parameters, each variable must be in a different field.</param>
function ItemSO::onAdd(%this)
{
	CityItemSO.add(%this);
	%this.parseCommand(%this.command);
}

function ItemSO::getTreeParsed(%this)
{
	return strReplace(%this.tree, " ", "_");
}

/// <summary>
/// Parses job objects' commands, see -> ItemSO::onAdd
/// Do not use this function.
/// </summary>
/// <param name="this">Name of the created mob.</param>
/// <param name="com">Parameters, each variable must be in a different field.</param>
function ItemSO::parseCommand(%this, %com)
{
	//echo("       -> Parasing Job command line: " @ %this.uiName);
	//echo("       -> CommandLine: " @ %com);
	for(%i=0;%i<getFieldCount(%com);%i++)
	{
		%field = getField(%com, %i);
		%name = getWord(%field, 0);
		%value = collapseEscape(getWords(%field, 1, getWordCount(%field)-1));

		if(%name $= "tree")
		{
			for(%a=0;%a<getFieldCount($JobTrees);%a++)
				if(getField($JobTrees, %a) $= %value)
					%remove = 1;

			if(!%remove)
				$JobTrees = %value TAB $JobTrees;
		}

		//echo("         PARAMETER FOUND: " @ %cmd);
		//echo("             VALUE: " @ %value);
		%cmd = %this @ "." @ %name @ " = \"" @ %value @ "\";";
		//echo("             PARSED: " @ %cmd);
		eval(%cmd);
	}

	%this.command = ""; //Make sure we don't create duplicates.
	%this.save(CityItemSO.pref["filePath"] @ %this.uiName @ ".cs");
}

function registerCityItem(%name, %description, %parm)
{
	%strName = stripChars(%name, $City::Chars);
	%strName = strReplace(%strName, " ", "_");
	%objName = "CityItem_" @ %strName;

	for(%i=0;%i<getFieldCount(%parm);%i++)
	{
		%field = getField(%parm,%i);
		%var = getWord(%field,0);
		%value = getWords(%field, 1, getWordCount(%field)-1);
		//echo("   PARAMETER FOUND: " @ %var);
		//echo("     VALUE: " @ %value);

		//for(%a=0;%a<getWordCount($City::ItemSO_RequiredFields);%a++)
		//{
		//	%requirement = getWord($City::ItemSO_RequiredFields,%a);
		//	if(%var $= %requirement && !%met_[%requirement])
		//	{
		//		%met_[%requirement] = 1;
		//		%metCount++;
		//	}
		//}
	}

	//if(%metCount < getWordCount($City::ItemSO_RequiredFields))
	//{
	//	warn(" - Unable to add the Job. Make sure you have made the parameters correctly.");
	//	warn(" - Requirement amount: " @ mFloor(%metCount) @ "/" @ getWordCount($City::ItemSO_RequiredFields));
	//	return;
	//}

	if(isObject(%objName))
	{
		warn("Warning: Job data \"" @ %objName @ "\" already exists. Overwriting.");
		%objName.delete();
	}

	%obj = new ScriptObject(%objName)
	{
		class = "ItemSO";
		uiName = %name;
		command = collapseEscape(%parm);
		description = %description;
	};
}

function isRegisteredCityItem(%name)
{
	return CityItemSO.find(%name);
}

datablock fxDTSBrickData(City_OreData)
{
	brickFile = "Add-Ons/GameMode_City/CityItems/Bricks/4x Cube.blb";
	iconName = "Add-Ons/GameMode_City/CityItems/BrickIcons/4x Cube";
	
	category = "City";
	subCategory = "Resources";
	
	uiName = "Ore";
	
	isRock = true;
	hasOre = true;
	resources = 15;
	
	CityBrickType = "Ore";
	requiresAdmin = 1;

	City_Call = "OreBrick";
};

function fxDTSBrick::onPlant_OreBrick(%this, %bypass)
{
	if(!%this.hasOre)
	{
		%this.hasOre = true;
		%this.isRock = 1;
	}

	%this.setColor($City::Resources::OreColorID);

	%this.disappear(0); //Make sure the ore is not hidden
	%this.oreMaxHealth = 60;
	%this.oreHealth = 60;
}

function fxDTSBrick::onHitOreBrick(%this, %client)
{
	if(!isObject(%client))
		return;

	if(!isObject(%player = %client.player))
		return;

	if(%this.hasOre)
	{
		%this.oreHealth -= 1;
		if(%this.oreHealth < 0)
		{
			%client.centerPrint("<font:Arial:18>\c6Ore mined!<br><just:left>\c6Backpack: \c5" @ mFloor(%client.CityData["ResourceORE"] / %client.CityData["MaxResourceORE"] * 100) @ "\c6%"
				@ "<just:right>\c6Current EXP: \c5" @ mFloor(%client.CityData["EXP", %client.getJob().getTreeParsed()]) @ " ", 3);
			%this.hasOre = 0;
			%this.setColor($City::Resources::NoOreColorID);
			%this.schedule(30000, onPlant_OreBrick, 1);
		}
		else
		{
			%expPrint = "\c6Cannot earn EXP: \c0Backpack too full";

			if(%client.CityData["ResourceORE"] < %client.CityData["MaxResourceORE"])
			{
				%expPrint = "\c6Current EXP: \c5" @ mFloor(%client.CityData["EXP", %client.getJob().getTreeParsed()]);
				%r = getRandom(1, 4);
				if(%r == 1)
					%client.City_addEXP(0.0125, "", 1);
				%client.CityData["ResourceORE"] = mClampF(%client.CityData["ResourceORE"] + 0.2, 0, %client.CityData["MaxResourceORE"]);
			}

			%client.centerPrint("<font:Arial:20><just:left>\c6Ore: \c4Quartz<just:right>\c6Ore: \c4" @ mFloor(%this.oreHealth / %this.oreMaxHealth * 100) @ "\c6% " @ 
				"<br><just:left>\c6Backpack: \c5" @ mFloor(%client.CityData["ResourceORE"] / %client.CityData["MaxResourceORE"] * 100) @ "\c6%"
				@ "<just:right>" @ %expPrint @ " ", 1);

			%client.City_SpeedFactor = mClampF(1.65 - (%client.CityData["ResourceORE"] / %client.CityData["MaxResourceORE"]), 0.1, 1);
			%player.City_SpeedFactor = %client.City_SpeedFactor;
			%player.setSpeedFactor(%player.City_SpeedFactor);
		}
	}
}

if(isPackage(City_Ore))
	deactivatePackage(City_Ore);

package City_Ore
{	
	function fxDTSBrick::setColor(%this, %color)
	{
		if(%this.getDataBlock().isRock && %this.isPlanted)
		{
			if(!%this.hasOre)
				parent::setColor(%this, $City::Resources::NoOreColorID);
			else
				parent::setColor(%this, $City::Resources::OreColorID);
		}
		else
			parent::setColor(%this, %color);
	}
	
	function fxDTSBrick::setColorFX(%this, %FX)
	{
		if(!%this.getDataBlock().isRock)
			parent::setColorFX(%this, %FX);
	}
	
	function fxDTSBrick::setShapeFX(%this, %FX)
	{
		if(!%this.getDataBlock().isRock)
			parent::setShapeFX(%this, %FX);
	}
	
	function fxDTSBrick::setEmitter(%this, %emitter)
	{
		if(!%this.getDataBlock().isRock)
			parent::setEmitter(%this, %emitter);
	}
};
activatePackage(City_Ore);

City_Debug("File > ResourceSO.cs", "   -> Loading complete.");