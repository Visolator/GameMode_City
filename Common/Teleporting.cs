if(!isObject(AudioClosestQuiet3d))
	datablock AudioDescription(AudioClosestQuiet3d : AudioClosest3d)
	{
		volume = 0.75;
	};

if(!isObject(spyCloakSound))
	datablock AudioProfile(spyCloakSound)
	{
		filename = "./Sounds/playerCloak.wav";
		description = AudioClosestQuiet3d;
		preload = true;
	};

if(!isObject(spyUncloakSound))
	datablock AudioProfile(spyUncloakSound)
	{
		filename = "./Sounds/playerUncloak.wav";
		description = AudioClosestQuiet3d;
		preload = true;
	};

function Player::beginCloak(%player,%watch)
{
	if(%player.isCloaked || %player.isCloaking)
		return;
	
	if(%player.getState() $= "Dead")
		return;
	
	%player.isCloaking = 1;
	%player.cloakWatch = %watch.getID();
	
	//%player.cloakSched = %player.schedule((100 / (32 * %watch.cloakEnergyDrain))  * ((%player.getEnergyLevel()/100) / (%player.dataBlock.maxEnergy/100)) * 1000,endCloak,%player,%watch);
	//%player.setRechargeRate(-%watch.cloakEnergyDrain * (%player.dataBlock.maxEnergy/100));
	
	%player.playAudio(0,%watch.cloakSound);
	%player.doCloakAnimation(1,0);
	%player.isUncloaking = 0;
	%player.setImageAmmo(0,1);
}

function Player::endCloak(%player)
{
	if(!%player.isCloaked || %player.isCloaking)
		return;
	
	if(%player.getState() $= "Dead")
		return;
	
	%player.playAudio(0,%player.cloakWatch.uncloakSound);
	
	%player.unHideNode("ALL");
	
	if(isObject(%player.client))
	{
		%player.client.applyBodyParts();
		%player.client.applyBodyColors();
	}
	else
		applyDefaultCharacterPrefs(%player); 

	%player.setNodeColor("ALL","0 0 0 0");
	%player.doCloakAnimation(0,0);
	%player.isUncloaking = 1;
	%player.isCloaked = 0;
	%player.isCloaking = 1;
	//%player.setRechargeRate(0);
	
	%player.setImageAmmo(0,0);
	
	if(isEventPending(%player.cloakSched))
		cancel(%player.cloakSched);
}

function Player::doCloakAnimation(%player,%direction,%amt)
{
	if(%player.getState() $= "Dead")
	{
		cancel(%player.cloakAnimSched);
		return;
	}
	
	if(isObject(%player.client))
	{
		if(isObject(%player.client.minigame) && %player.client.tdmTeam != -1 && %player.client.tdmTeam !$= "")
			%col = getColorIDTable(%player.client.minigame.teamCol[%player.client.tdmTeam]);
		else
			%col = %player.client.chestColor;
	}
	else
		%col = "1 1 1 1";
	
	%amt+=0.1;
	if(%direction == 1)
	{
		%player.setNodeColor("ALL",getWords(%col,0,2) SPC (1-%amt)/2);
		if(%amt >= 1)
		{
			%player.isCloaking = 0;
			%player.isCloaked = 1;
			%player.hideNode("ALL");
			%player.setShapeNameDistance(10);
			if(!isObject(%player.tool[%player.currTool].image) || (%player.tool[%player.currTool].image.getID() != %player.cloakWatch))
				%player.endCloak();
			return;
		}
	}
	else
	{
		%player.setNodeColor("ALL",getWords(%col,0,2) SPC %amt/2);
		if(%amt >= 1)
		{
			%player.isCloaking = 0;
			%player.isCloaked = 0;
			%player.lastUnCloakTime = getSimTime();
			
			if(isObject(%player.client))
			{
				%player.client.applyBodyColors();
				if(%player.getMountedImage(0) == %player.cloakWatch)
					if(%player.currTool != -1)
						servercmdUseTool(%player.client,%player.currTool);
					else
						%player.unMountImage(0);
			}
			else
				applyDefaultCharacterPrefs(%player);
			
			if(fileName(%player.dataBlock.shapeFile) $= "m.dts")
				%player.unHideNode("headSkin");
			
			//if(%player.dataBlock.rechargeRate != PlayerStandardArmor.rechargeRate && %player.dataBlock.rechargeRate != 0)
			//   %player.setRechargeRate(%player.dataBlock.rechargeRate);
			//else
			//   %player.setRechargeRate(%player.cloakWatch.item.rechargeRate * (%player.dataBlock.maxEnergy/100));
			
			%player.setShapeNameDistance(600);
			
			return;
		}
	}
	%player.cloakAnimSched = %player.schedule(100,doCloakAnimation,%direction,%amt);
}

datablock fxDTSBrickData(TeleportCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Teleport Info";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "Teleporter";
	triggerSize = "2 4 1";
	trigger = 0;
};

datablock fxDTSBrickData(TeleporterCityBrick : brick2x2FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Teleporter";
	
	CityBrickType = "Teleporter";
	requiresAdmin = 1;
};

function GameConnection::parseTriggerData_Teleporter(%this, %message)
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
		%this.chatMessage("\c6Welcome to the Teleporter Department. What can we do for you?");

		for(%i = 0; %i < getWordCount($City::Teleporters); %i++)
		{
			%tele = getWord($City::Teleporters, %i);
			if(isObject(%tele))
				if(%tele.getName() !$= "" && %tele.getName() !$= "_")
					%teles = %teles SPC %tele;
		}
		%availableTeleporters = trim(%teles);

		if(getWordCount(%availableTeleporters) <= 0)
			%this.chatMessage(" X \c7- \c6Sorry, there are no teleporters available right now.");
		else
		{
			%this.chatMessage(" \c31 \c7- \c6Teleport");
			%list = %list @ " 1";
		}


		%list = trim(%list);
		%list = strReplace(%list, " ", ",");
		%this.CityData["Trigger_CanChoose"] = %list;
	}
	else if(%message $= "LEAVE")
	{
		%this.CityData["Trigger_CanChoose"] = "";
		%this.CityData["Trigger_Mode"] = "";
		%this.CityData["Trigger_Cost"] = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(containsSubstring(%this.CityData["Trigger_CanChoose"], %amt) && %this.CityData["Trigger_Mode"] $= "")
	{
		%this.CityData["Trigger_Mode"] = %amt;
		switch(%amt)
		{
			case 1:
				%this.chatMessage("\c6Where to? (Must be exact name)");
				for(%i = 0; %i < getWordCount($City::Teleporters); %i++)
				{
					%tele = getWord($City::Teleporters, %i);
					if(isObject(%tele))
						if(%tele.getName() !$= "" && %tele.getName() !$= "_")
						{
							%dist = vectorDist(%tele.getPosition(), %player.getPosition());

							%newName = strReplace(%tele.getName(), "_", " ");
							%newName = strReplace(%newName, "DASH", "-");
							%msg = " \c6(\c2$" @ mFloatLength($City::Teleport::MinCostDistance * %dist, 2) @ "\c6)";
							if(%dist < $City::Teleport::MinCostDistance)
								%msg = "\c6(\c2FREE\c6)";

							%this.chatMessage("\c6 -\c3" @ %newName @ %msg);
						}
				}

			default:
				%this.chatMessage("\c6You have an invalid command, you must have choosen something that you cannot select.");
				%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
		}
	}
	else if(%this.CityData["Trigger_Mode"] == 1)
	{
		%oldMessage = %message;
		%message = getSafeVariableName("_" @ %message);
		for(%i = 0; %i < getWordCount($City::Teleporters); %i++)
		{
			%tele = getWord($City::Teleporters, %i);
			if(isObject(%tele))
				if(%tele.getName() !$= "" && %tele.getName() !$= "_")
				{
					if(%tele.getName() $= %message)
					{
						%dist = vectorDist(%tele.getPosition(), %player.getPosition());
						%cost = $City::Teleport::MinCostDistance * %dist;

						if(%cost > %accMoney)
						{
							%this.chatMessage("Sorry, you do not have enough to teleport to this location.");
							return;
						}

						%this.addBankMoney(%cost);
						%player.CloakToLocation(%tele);
					}
				}
		}
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

function Player::CloakToLocation(%this, %transform)
{
	if(!isObject(%this))
		return;

	if(%this.getState() $= "Dead")
		return;

	if(!isObject(%client = %this.client))
		return;

	%client.lastTeleportTime = $Sim::Time;
	%client.chatMessage("\c6Teleporting...");
	%myTransform = %this.gettransform();

	if(getWordCount(%transform) == 1 && isObject(%transform))
		%teleTransform = %transform.gettransform();
	else if(getWordCount(%transform) == 3 || getWordCount(%transform) == 7)
		%teleTransform = %transform;
	else
		return;

	%prot = getwords(%myTransform, 3, 6);
	%or = getwords(%teleTransform, 0, 2);
	%lscale = 0.1;
	if(getWordCount(%transform) == 1 && isObject(%transform))
		%lscale = 0.1 * %transform.getdatablock().bricksizez;

	%finalsend = "0 0" SPC %lscale;
	%fr = vectoradd(%or, %finalsend);
						
	%finaltransform = %fr SPC %prot;

	%this.mountImage(PlayerTeleportImage, 3);
	%this.doCloakAnimation(1);
	%client.play3D("spyCloakSound", %myTransform);
	%this.schedule(700, CloakToLocation2, %finaltransform);
}

function Player::CloakToLocation2(%this, %transform)
{
	if(!isObject(%this))
		return;

	if(%this.getState() $= "Dead")
		return;

	if(!isObject(%client = %this.client))
		return;

	if(getWordCount(%transform) == 1 && isObject(%transform))
		%teleTransform = %transform.gettransform();
	else if(getWordCount(%transform) == 3 || getWordCount(%transform) == 7)
		%teleTransform = %transform;
	else
		return;

	City.Log(%client.getPlayerName() @ " has teleported to " @ %oldMessage @ ". Position: " @ getWords(%teleTransform, 0, 2));
						
	%this.schedule(50, hideNode, "ALL");
	%this.schedule(100, setTransform, %teleTransform);
}