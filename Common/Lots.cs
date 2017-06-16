City_Debug("File > Lots.cs", "Loading assets..");

function GameConnection::addBrickVolume(%this, %volume)
{
	%this.CityData["BrickVolume"] += %volume;

	if(%this.CityData["BrickVolume"] < 0)
		%this.CityData["BrickVolume"] = 0;

	return mCeil(%this.CityData["BrickVolume"]);
}

datablock triggerData(CityLotTriggerData)
{
	tickPeriodMS = 100;
	parent = 0;
};

function fxDtsBrick::highlightToTheGround(%this)
{
	
}

//Got this from TotalRPG, thanks Pecon
function fxDtsBrick::getVolume(%this)
{
	%box = %this.getObjectBox();
	
	%x = mAbs(getWord(%box, 0)) + mAbs(getWord(%box, 3));
	%y = mAbs(getWord(%box, 1)) + mAbs(getWord(%box, 4));
	%z = mAbs(getWord(%box, 2)) + mAbs(getWord(%box, 5));
	
	%volume = %x * %y * %z;
	
	return %volume;
}

function CityLotTriggerData::onTickTrigger(%this, %trigger, %obj, %a)
{
	if(!isObject(%brick = %trigger.brick))
	{
		%trigger.delete();
		return;
	}

	if(!isObject(%client = %obj.client))
		return;

	%obj.currentLot = %trigger;
}

function CityLotTriggerData::onEnterTrigger(%this, %trigger, %obj, %a)
{
	if(!isObject(%brick = %trigger.brick))
	{
		%trigger.delete();
		return;
	}

	if(!isObject(%client = %obj.client))
		return;

	//Called when a person goes into a lot
	cancel(%client.City_UpdateSch);
	%client.City_UpdateSch = %client.schedule(200, City_Update);
}

function CityLotTriggerData::onLeaveTrigger(%this, %trigger, %obj, %a)
{
	if(!isObject(%brick = %trigger.brick))
	{
		%trigger.delete();
		return;
	}

	if(!isObject(%client = %obj.client))
		return;

	%obj.currentLot = -1;
	cancel(%client.City_UpdateSch);
	%client.City_UpdateSch = %client.schedule(200, City_Update);
}

//datablock particleData(CityLogo)
//{
//	textureName = "Add-Ons/GameMode_City/shapes/decals/CityLogo.Gad's";
	//preload = true;
//};

datablock fxDTSBrickData(CitySmallLotBrickData : brick16x16FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/16x16ZoneIcon";
	
	category = "City";
	subCategory = "Lots";
	
	uiName = "Small Lot";

	//requiresAdmin = 1;
	CityBrickType = "LotTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "16 16 1000";
	trigger = 0;

	City_Cost = 800;
};

datablock fxDTSBrickData(CityHalfSmallLotBrickData : brick16x32FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/16x32ZoneIcon";
	
	category = "City";
	subCategory = "Lots";
	
	uiName = "Half-Small Lot";
	
	//requiresAdmin = 1;
	CityBrickType = "LotTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "16 32 1000";
	trigger = 0;

	City_Cost = 1500;
};

datablock fxDTSBrickData(CityMediumLotBrickData : brick32x32FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/32x32ZoneIcon";
	
	category = "City";
	subCategory = "Lots";
	
	uiName = "Medium Lot";
	
	//requiresAdmin = 1;
	CityBrickType = "LotTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "32 32 1000";
	trigger = 0;

	City_Cost = 2250;
};

datablock fxDTSBrickData(CityLargeLotBrickData : brick64x64FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/64x64ZoneIcon";
	
	category = "City";
	subCategory = "Lots";
	
	uiName = "Large Lot";
	
	//requiresAdmin = 1;
	CityBrickType = "LotTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "64 64 1000";
	trigger = 0;

	City_Cost = 3000;
};

//no kill zones
datablock fxDTSBrickData(CitySafeZoneSmallLotBrickData : brick16x16FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/16x16ZoneIcon";
	
	category = "City";
	subCategory = "Zones";
	
	uiName = "Small Safe Zone";

	CityBrickType = "ZoneTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "16 16 1000";
	trigger = 0;
	triggerData["SafeZone"] = true;
	triggerData["NoKillZone"] = true;
	triggerData["NoRobZone"] = true;

	City_Cost = 0;
	requiresAdmin = 1;
};

datablock fxDTSBrickData(CitySafeZoneHalfSmallLotBrickData : brick16x32FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/16x32ZoneIcon";
	
	category = "City";
	subCategory = "Zones";
	
	uiName = "Half-Small Safe Zone";
	
	CityBrickType = "ZoneTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "16 32 1000";
	trigger = 0;
	triggerData["NoKillZone"] = true;

	City_Cost = 0;
	requiresAdmin = 1;
};

datablock fxDTSBrickData(CitySafeZoneMediumLotBrickData : brick32x32FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/32x32ZoneIcon";
	
	category = "City";
	subCategory = "Zones";
	
	uiName = "Medium Safe Zone";
	
	CityBrickType = "ZoneTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "32 32 1000";
	trigger = 0;
	triggerData["NoKillZone"] = true;

	City_Cost = 0;
	requiresAdmin = 1;
};

datablock fxDTSBrickData(CitySafeZoneLargeLotBrickData : brick64x64FData)
{
	iconName = "Add-Ons/GameMode_City/shapes/BrickIcons/64x64ZoneIcon";
	
	category = "City";
	subCategory = "Zones";
	
	uiName = "Large Safe Zone";
	
	CityBrickType = "ZoneTrigger";
	triggerDatablock = CityLotTriggerData;
	triggerSize = "64 64 1000";
	trigger = 0;
	triggerData["NoKillZone"] = true;

	City_Cost = 0;
	requiresAdmin = 1;
};

//Data

datablock particleData(CZ_Icon_1)
{
	textureName = "Add-Ons/GameMode_City/shapes/BrickIcons/16x16ZoneIcon";
	preload = true;
};

datablock particleData(CZ_Icon_2)
{
	textureName = "Add-Ons/GameMode_City/shapes/BrickIcons/16x32ZoneIcon";
	preload = true;
};

datablock particleData(CZ_Icon_3)
{
	textureName = "Add-Ons/GameMode_City/shapes/BrickIcons/32x32ZoneIcon";
	preload = true;
};

datablock particleData(CZ_Icon_4)
{
	textureName = "Add-Ons/GameMode_City/shapes/BrickIcons/64x64ZoneIcon";
	preload = true;
};

//

datablock fxDTSBrickData(LotCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Lot Brick";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "Lot";
	triggerSize = "2 4 1";
	trigger = 0;
};

if(!isObject(Lots))
{
	new ScriptGroup(Lots)
	{
		class = LotSO;
	};
}

function LotSO::find(%this, %name)
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

function GameConnection::parseTriggerData_Lot(%this, %message)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%amt = mFloor(%message);
	%accMoney = %this.CityData["Bank"];
	%curMoney = %this.CityData["Money"];
	if(%message $= "ENTER")
	{
		%this.chatMessage("\c6Welcome to the Real Estate Department. How can we help you?");
		
		if(!$City::Lots::EnableBrickVolume)
			%this.chatMessage(" X \c7- \c6Brick volume is disabled");
		else
		{
			if(%curMoney > $City::Lots::CostPerStorageVolume)
			{
				%list = %list SPC "1";
				%this.chatMessage(" \c31 \c7- \c6Get brick volume (\c3$" @ mFloatLength($City::Lots::CostPerStorageVolume, 2) @ " \c6per brick volume)");
			}
			else
				%this.chatMessage(" X \c7- \c6You do not have enough money for brick volume (\c3$" @ mFloatLength($City::Lots::CostPerStorageVolume, 2) @ " \c6per brick volume)");
		}


		%list = trim(%list);
		%list = strReplace(%list, " ", ",");
		%this.CityData["Trigger_CanChoose"] = %list;
	}
	else if(%message $= "LEAVE")
	{
		%this.CityData["Trigger_Choosen"] = "";
		%this.CityData["Trigger_CanChoose"] = "";
		%this.CityData["Trigger_Mode"] = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(containsSubstring(%this.CityData["Trigger_CanChoose"], %amt) && %this.CityData["Trigger_Mode"] $= "")
	{
		%this.CityData["Trigger_Mode"] = %amt;
		switch(%amt)
		{
			case 1:
				%this.chatMessage("\c6How much brick volume do you want? (\c3$" @ mFloatLength($City::Lots::CostPerStorageVolume, 2) @ " \c6per brick volume)");
				%this.chatMessage("\c6When you are ready, please say the amount.");

			default:
				%this.chatMessage("\c6You have an invalid command, you must have choosen something that you cannot select.");
				%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
		}
	}
	else if(%this.CityData["Trigger_Mode"] == 1)
	{
		if($City::Lots::CostPerStorageVolume > %curMoney)
		{
			%this.chatMessage("\c6You're too poor you penniless hobo.");
			%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
			return;
		}
		%this.attemptBuyBrickVolume(%amt);
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

function GameConnection::attemptBuyBrickVolume(%this, %amt)
{
	%amt = mClampF(%amt, 1, 5000);
	if(%this.CityData["BrickVolume"] + %amt > 5000)
		%amt = 5000 - %this.CityData["BrickVolume"];

	if(%amt <= 0)
	{
		%this.chatMessage("\c6Sorry, you either didn't put an amount or you are at full capacity.");
		return;
	}

	%cost = mFloatLength($City::Lots::CostPerStorageVolume * %amt, 2);
	if(%this.CityData["BrickVolume"] < 5000)
	{
		%this.TempData["ConfirmPurchase_Build"] = 1;
		%this.TempData["ConfirmPurchase_BuildAmt"] = %amt;
		%this.TempData["ConfirmPurchase_BuildCost"] = %cost;
		commandToClient(%this, 'MessageBoxYesNo', "Buy - Brick Volume", 
			"Are you sure you want to buy " @ %amt @ " brick volume?<br>Costs $" @ %this.TempData["ConfirmPurchase_BuildCost"] @ ".",
			'ConfirmPurchase_Build');
	}
	else
		%this.chatMessage("\c6Sorry, you already have enough brick volume. Come back later.");
}

function serverCmdConfirmPurchase_Build(%this)
{
	if(%this.TempData["ConfirmPurchase_Build"] $= "")
	{
		%this.TempData["ConfirmPurchase_Build"] = "";
		%this.TempData["ConfirmPurchase_BuildCost"] = "";
		%this.TempData["ConfirmPurchase_BuildAmt"] = "";
		return;
	}

	%accMoney = %this.CityData["Bank"];
	%curMoney = %this.CityData["Money"];
	%cost = %this.TempData["ConfirmPurchase_BuildCost"];
	
	if(%curMoney < %cost)
	{
		%this.TempData["ConfirmPurchase_Build"] = "";
		%this.chatMessage("\c5Sorry, you do not have enough cash.");
		return;
	}

	%this.addMoney(-%cost);
	City_AddEconomy(%cost);
	%this.addBrickVolume(%this.TempData["ConfirmPurchase_BuildAmt"]);
	if(isObject(brickPlantSound))
		%this.play2D(brickPlantSound);

	%this.TempData["ConfirmPurchase_Build"] = "";
	%this.TempData["ConfirmPurchase_BuildCost"] = "";
	%this.TempData["ConfirmPurchase_BuildAmt"] = "";
	if(isObject(%pl = %this.player))
		if(isObject(%trig = %this.CityData["Trigger"]))
			%trig.getDatablock().onLeaveTrigger(%trig, %pl);
}

function GameConnection::attemptLicenseUpgrade_Build(%this)
{
	if(%this.CityData["Upgrade_BuilderWaiting"])
	{
		%this.TempData["ConfirmUpgrade_Build"] = "";
		%this.TempData["ConfirmUpgrade_BuildCost"] = "";
		return;
	}
	
	if(%this.CityData["BuildingLicense"] <= 0)
	{
		%this.TempData["ConfirmUpgrade_Build"] = 1;
		%this.TempData["ConfirmUpgrade_BuildCost"] = $City::License::Builder;
		commandToClient(%this, 'MessageBoxYesNo', "Upgrade - Building License", 
			"Are you sure you want to buy this license?<br>Costs $" @ %this.TempData["ConfirmUpgrade_BuildCost"] @ ".",
			'ConfirmUpgrade_Build');
	}
	else if(%this.CityData["BuildingLicense"] < 3)
	{
		%this.TempData["ConfirmUpgrade_Build"] = 1;
		%this.TempData["ConfirmUpgrade_BuildCost"] = $City::License::Builder * (%this.CityData["BuildingLicense"] * 1.6);
		commandToClient(%this, 'MessageBoxYesNo', "Upgrade - Building License", 
			"Are you sure you want to upgrade this license?<br>Costs $" @ %this.TempData["ConfirmUpgrade_BuildCost"] @ ".",
			'ConfirmUpgrade_Build');
	}
	else
		%this.chatMessage("\c6Sorry, you cannot go any farther into your builder's license.");
}

function serverCmdConfirmUpgrade_Build(%this)
{
	if(%this.TempData["ConfirmUpgrade_Build"] $= "")
	{
		%this.TempData["ConfirmUpgrade_Build"] = "";
		%this.TempData["ConfirmUpgrade_BuildCost"] = "";
		return;
	}

	%accMoney = %this.CityData["Bank"];
	%curMoney = %this.CityData["Money"];
	%cost = %this.TempData["ConfirmUpgrade_BuildCost"];
	
	if(%curMoney < %cost)
	{
		%this.TempData["ConfirmUpgrade_Build"] = "";
		%this.chatMessage("\c5Sorry, you do not have enough cash.");
		return;
	}

	%this.addMoney(-%cost);
	City_AddEconomy(%cost);
	%this.addBuildEduTime(mClampF(%this.CityData["BuildingLicense"], 1, 3) * getRandom(30, 60));
	%this.CityData["Upgrade_BuilderWaiting"] = 1;
}

function GameConnection::addBuildEduTime(%this, %time)
{
	%this.CityData["Upgrade_BuilderTime"] += %time;
	if(%this.CityData["Upgrade_BuilderTime"] < 0)
	{
		%this.CityData["Upgrade_BuilderTime"] = 0;
		if(%this.CityData["Upgrade_BuilderWaiting"])
		{
			%this.CityData["Upgrade_BuilderWaiting"] = 0;
			if(%this.CityData["BuildingLicense"] < 3)
			{
				%this.chatMessage("\c6Building license upgraded.");
				%this.CityData["BuildingLicense"]++;
			}
		}
	}
}

if(isPackage("City_Lots"))
	deactivatePackage("City_Lots");

package City_Lots
{
	function serverCmdPlantBrick(%client)
	{
		if(!%client.isAdmin && $City::Lots::EnableBuildLicense)
			if(!%client.CityData["BuildingLicense"])
			{
				%client.centerprint("\c5Sorry, you need a building license to do so.", 3);
				return;
			}
		
		return Parent::serverCmdPlantBrick(%client);
	}
};
activatePackage("City_Lots");

if(isFile($City::Lots::SavePath))
	exec($City::Lots::SavePath);

City_Debug("File > Lots.cs", "   -> Loading complete.");