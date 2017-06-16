City_Debug("File > JailSO.cs", "Loading assets..");

datablock fxDtsBrickData(CityRPGJailSpawnBrickData : brickSpawnPointData)
{
	category = "City";
	subCategory = "Spawning";
	
	uiName = "Jail Spawn";
	
	specialBrickType = "";
	
	CityBrickType = "Spawn";
	requiresAdmin = 1; //Regular admin level to plant
	
	spawnData = "jailSpawn";
};

///////////////////////

if(!isObject(CommunityService))
	new ScriptObject(CommunityService)
	{
		class = CommunityServiceSO;
		memberCount = 0;
	};

function CommunityServiceSO::isMember(%this, %obj)
{
	if(!isObject(%obj)) return -1;
	if(%obj.getClassName() !$= "GameConnection") return false;
	if(%this.memberCount < 0) %this.memberCount = 0;
	for(%i=0;%i<%this.memberCount;%i++)
	{
		if(isObject(%this.member[%i]))
			if(%this.member[%i] == %obj)
				return true;
	}
	return false;
}

function CommunityServiceSO::addMember(%this, %client, %time) //Time is optional, it goes by days.
{
	if(!isObject(%client)) return -1;
	if(%client.getClassName() !$= "GameConnection") return false;
	if(%this.isMember(%client)) return;
	if(%this.memberCount < 0) %this.memberCount = 0;
	%this.member[%this.memberCount] = %client;
	%this.memberCount++;
	%client.CityData["CommunityService"] = mClampF(%time, 0, $City::CommunityService::MaxDays);
	%client.CityData["Demerits"] = 0;
	serverCmdCancelbrick(%client);
	%client.instantRespawn();
	if(isObject(%player = %client.player))
	{
		%player.clearTools();
		//%player.addNewItem("Pickaxe");
	}
	return false;
}

function CommunityService::removeMember(%this, %client)
{
	if(!isObject(%client)) return -1;
	if(%client.getClassName() !$= "GameConnection") return false;
	if(!%this.isMember(%client)) return;
	if(%this.memberCount < 0) %this.memberCount = 0;
	%start = -1;
	for(%j = 0; %j < %this.memberCount; %j ++)
	{
		if(%this.member[%j] == %client) //find the member to be removed
		{
			%this.member[%j] = "";
			%start = %i;
			break;
		}
	}

	if(%start >= 0)	
	{
		for(%j = %start + 1; %j < %this.memberCount; %j ++)
			%this.member[%j - 1] = %this.member[%j];

		%this.member[%this.memberCount - 1] = "";
		%this.memberCount--;

		%client.CityData["CommunityService"] = 0;
		serverCmdCancelbrick(%client);
		%client.instantRespawn();
	}

	return false;
}

///////////////////////

if(!isObject(Jail))
	new ScriptObject(Jail)
	{
		class = JailSO;
		memberCount = 0;
	};

function JailSO::isMember(%this, %obj)
{
	if(!isObject(%obj)) return -1;
	if(%obj.getClassName() !$= "GameConnection") return false;
	if(%this.memberCount < 0) %this.memberCount = 0;
	for(%i=0;%i<%this.memberCount;%i++)
	{
		if(isObject(%this.member[%i]))
			if(%this.member[%i] == %obj)
				return true;
	}
	return false;
}

function JailSO::addMember(%this, %client, %time) //Time is optional, it goes by days.
{
	if(!isObject(%client)) return -1;
	if(%client.getClassName() !$= "GameConnection") return false;
	if(%this.isMember(%client)) return;
	if(%this.memberCount < 0) %this.memberCount = 0;
	%this.member[%this.memberCount] = %client;
	%this.memberCount++;
	%client.CityData["JailTime"] = mCeil(%client.CityData["Demerits"] / $City::Jail::MinDemerits) + %time;
	if(%client.CityData["JailTime"] < 0)
		%client.CityData["JailTime"] = 0;

	if(%client.CityData["JailTime"] > $City::Jail::MaxDays)
	{
		%client.CityData["CommunityService"] = %client.CityData["JailTime"] - $City::Jail::MaxDays;
		%client.CityData["JailTime"] = $City::Jail::MaxDays;
	}
	else
	{
		%client.CityData["CommunityService"] = %client.CityData["JailTime"];
		%client.CityData["JailTime"] = 0;
	}

	%client.CityData["Demerits"] = 0;
	if(%client.CityData["JailTime"] <= 0)
		Jail.removeMember(%client, 1);

	serverCmdCancelbrick(%client);
	%client.instantRespawn();
	if(isObject(%player = %client.player))
	{
		%player.clearTools();
		//%player.addNewItem("Pickaxe");
	}

	return false;
}

function JailSO::removeMember(%this, %client, %noRespawn)
{
	if(!isObject(%client)) return -1;
	if(%client.getClassName() !$= "GameConnection") return false;
	if(!%this.isMember(%client)) return;
	if(%this.memberCount < 0) %this.memberCount = 0;
	%start = -1;
	for(%j = 0; %j < %this.memberCount; %j ++)
	{
		if(%this.member[%j] == %client) //find the member to be removed
		{
			%this.member[%j] = "";
			%start = %i;
			break;
		}
	}

	if(%start >= 0)	
	{
		for(%j = %start + 1; %j < %this.memberCount; %j ++)
			%this.member[%j - 1] = %this.member[%j];

		%this.member[%this.memberCount - 1] = "";
		%this.memberCount --;

		%client.CityData["JailTime"] = 0;
		if(!%noRespawn)
		{
			serverCmdCancelbrick(%client);
			%client.instantRespawn();
		}
	}

	return false;
}

function SimObject::getDaysWhenInJail(%this)
{
	if(!isCityObject(%this))
		return 0;

	return %this.CityData["JailTime"];
}

function SimObject::getDaysInJail(%this)
{
	if(!isCityObject(%this))
		return 0;

	return (%this.CityData["Demerits"] / $City::Jail::MinDemerits);
}

function GameConnection::isInCS(%this)
{
	return CommunityService.isMember(%this);
}

function GameConnection::isInJail(%this)
{
	return Jail.isMember(%this);
}

function GameConnection::addDemerits(%this, %dems)
{
	%this.CityData["Demerits"] += %dems;
	if(%this.CityData["Demerits"] < 0)
		%this.CityData["Demerits"] = 0;

	%this.City_Update();
}

function serverCmdClearRecord(%this, %name)
{
	if(!%this.getJob().canClearRecords && !%this.isAdmin)
	{
		%this.chatMessage("\c6You are not allowed to clear records.");
		return;
	}

	if(!isObject(%target = findClientByName(%name)))
	{
		%this.chatMessage("\c6There is no \c4\"" @ %name @ "\"\c6.");
		return;
	}

	if(%target == %this && !%this.isSuperAdmin)
	{
		%this.chatMessage("\c5The extent of your legal corruption only goes so far. You cannot clear your own record.");
		return;
	}

	if(%this != %target)
	{
		%cost = 2000;
		if(%cost > %this.CityData["Money"] && !%this.isAdmin)
		{
			%this.chatMessage("\c6You are too poor to clean \"" @ %target.getPlayerName() @ "\". You need \c3$" @ %cost @ "\c6.");
			return;
		}

		City.Log(%target.getPlayerName() @ "(" @ %target.getBLID() @ "," @ %target.getRawIP() @ ")" 
			@ " has their record cleared by " @ %this.getPlayerName() @ "(" @ %this.getBLID() @ "," @ %this.getRawIP() @ ")");
		%this.chatMessage("\c6You have cleaned " @ %target.getPlayerName() @ ".");
		%target.chatMessage("\c6You have been cleaned by " @ %this.getPlayerName() @ ".");
	}
	else
	{
		City.Log(%this.getPlayerName() @ "(" @ %this.getBLID() @ "," @ %this.getRawIP() @ ") has cleaned themself.");
		%this.chatMessage("\c5You have cleaned yourself.. Cheater.");
	}

	%target.CityData["TotalJailTime"] = 0;
	if(Jail.isMember(%target))
		Jail.removeMember(%target);
}

function serverCmdPardon(%this, %name)
{
	if(!%this.getJob().canPardon && !%this.isAdmin)
	{
		%this.chatMessage("\c6You are not allowed to pardon.");
		return;
	}

	if(!isObject(%target = findClientByName(%name)))
	{
		%this.chatMessage("\c6There is no \c4\"" @ %name @ "\"\c6.");
		return;
	}

	if(%target == %this && !%this.isSuperAdmin)
	{
		%this.chatMessage("\c5The extent of your legal corruption only goes so far. You cannot pardon yourself.");
		return;
	}

	if(%this != %target)
	{
		if(%target.CityData["JailTime"] * $City::Jail::CostPerLevel > %this.CityData["Money"] && !%this.isAdmin)
		{
			%this.chatMessage("\c6You are too poor to pardon \"" @ %target.getPlayerName() @ "\". You need \c3$" @ 
				%target.CityData["JailTime"] * $City::Jail::CostPerLevel @ "\c6.");
			return;
		}
		City.Log(%target.getPlayerName() @ "(" @ %target.getBLID() @ "," @ %target.getRawIP() @ ")" 
			@ " has been pardoned by " @ %this.getPlayerName() @ "(" @ %this.getBLID() @ "," @ %this.getRawIP() @ ")");
		%this.chatMessage("\c6You have pardoned " @ %target.getPlayerName() @ ".");
		%target.chatMessage("\c6You have been pardoned by " @ %this.getPlayerName() @ ".");
	}
	else
	{
		City.Log(%this.getPlayerName() @ "(" @ %this.getBLID() @ "," @ %this.getRawIP() @ ") has pardoned themself.");
		%this.chatMessage("\c5You have pardoned yourself.. Cheater.");
	}

	Jail.removeMember(%target);
	CommunityService.removeMember(%target);
}

$City::Demerits::VehicleDamage = 20;
$City::Demerits::AttemptedMurder = 3.5;
$City::Demerits::Murder = 40;
$City::Witness::Range = 300;
$City::Witness::Enabled = 1;

if($City::Witness::Enabled)
	if(!$City::Witness::HideKills)
	{
		addSpecialDamageMsg(HideKill, "","");
		$City::Witness::HideKills = 1;
	}

//For later use??
function isSpecialKill_HideKill()
{
	return $City::Witness::Enabled;
}

if(isPackage(SpecialKills))
	deactivatePackage(SpecialKills);

if(isPackage(City_Jail))
	deactivatePackage(City_Jail);

package City_Jail
{
	function Armor::Damage(%armor, %this, %sourceObject, %position, %damage, %damageType)
	{
		%attacker = %sourceObject;

		if(isObject(%sourceObject))
		{
			if(%sourceObject.getClassName() $= "Projectile")
				%attacker = %sourceObject.sourceObject;

			if(%sourceObject.getClassName() $= "WheeledVehicle")
				return;

			if(!isObject(%attacker))
				return Parent::Damage(%armor, %this, %sourceObject, %position, %damage, %damageType);

			if(%attacker.getClassName() $= "GameConnection")
				%colClient = %attacker;
			else
			{
				if(%attacker.getClassName() $= "Player" || %attacker.getClassName() $= "Projectile")
					%colClient = %attacker.client;
				else if(isFunction(%attacker.getClassName(), getMountedObject))
					%colClient = %attacker.getMountedObject(0).client;
			}

		}

		if(%attacker.hBot)
			return;

		if(%this.client.invincible)
			return;

		if(%attacker.client.damageRatio > 1)
			%damage *= %attacker.client.damageRatio;

		if(%damageType == $DamageType::HammerDirect && !%attacker.canHammer[%this])
			if(!%attacker.canHammerAnyone)
			{
				%attacker.hammered[%this]++;
				if(%attacker.hammered[%this] > 6)
					%this.canHammer[%attacker] = 1;
				else
				{
					if(%attacker.hammered[%this] == 4)
					{
						if(isObject(%colClient) && !%colClient.TempCityData["HammerAnnoy"])
						{
							%colClient.TempCityData["HammerAnnoy"] = 1;
							commandToClient(%colClient, 'MessageBoxOK', "Watch out!", "You can get hammered back!<br>This warning will no longer appear.");
							serverCmdUnUseTool(%colClient);
						}
					}

					%this.resetHamerSch[%object] = %this.schedule(5000, "ResetHammerSch");
				}

				return;
			}

		if(%damageType == $DamageType::HammerDirect)
		{
			cancel(%this.slowRunCancelSch);
			%this.setSpeedFactor(0.6);
			%this.slowRunCancelSch = %this.schedule(2000, setSpeedFactor, %this.City_SpeedFactor);
		}

		if(isObject(%lot = %this.currentLot))
			if(%lot.triggerData["NoKillZone"])
				return;

		if(isObject(%lot = %attacker.currentLot))
			if(%lot.triggerData["NoKillZone"])
				return;

		//%this should exist.. (%object)
		if(%this.getClassName() $= "GameConnection")
			%client = %this;
		else
			%client = %this.client;

		//announce(%damageType);
		if(%damageType == $DamageType::Vehicle)
		{
			//announce(%colClient.getClassName());
			//announce(%client.getClassName());
			if(isCityObject(%colClient) && isCityObject(%client))
				if(!%colClient.City_CanLegallyDamage(%client))
				{
					%this.canHammer[%attacker] = 1;
					//%colClient.chatMessage("Step: Hurting people with vehicles");
					if(%colClient.addDemeritsIfWitnessed($City::Demerits::VehicleDamage, $City::Witness::Range, %this))
						%colClient.centerprint_addLine("\c6You have been caught commiting a crime. [\c3Running over a blockhead\c6]", 2);
				}
		}
		else if(isCityObject(%colClient) && isCityObject(%client) && !%colClient.City_CanLegallyDamage(%client) && %client != %colClient && %damageType != $DamageType::HammerDirect)
		{
			%colClient.setJailImmunityTime(0);
			%this.canHammer[%attacker] = 1;
			//%colClient.chatMessage("Step: Rampage");
			if(%colClient.addDemeritsIfWitnessed($City::Demerits::AttemptedMurder * %damage, $City::Witness::Range, %this))
				%colClient.centerprint_addLine("\c6You have been caught commiting a crime. [\c3Attempted Murder\c6]", 2);
		}

		return Parent::Damage(%armor, %this, %sourceObject, %position, %damage, %damageType);
	}

	function gameConnection::onDeath(%client, %killerObject, %killerClient, %damageType, %position)
	{
		%lastDamageType = %damageType;
		if($City::Witness::Enabled && isObject(%player = %client.player) && %client != %killerClient)
		{
			initContainerRadiusSearch(%player.getPosition(), $City::Witness::Range, $TypeMasks::PlayerObjectType);
			while((%target = containerSearchNext()) != 0)
			{
				if(%player != %target && %target != %opp)
				{
					if(isObject(%cl = %target.client) && %target.canSeeObject(%player))
					{
						%cl.sendKillMessage(%client, %killerClient);
						%amt++;
					}
					else if(isObject(%cl = %target.client))
					{
						%cl.sendKillMessage();
						%amt++;
					}
				}
			}

			%damageType = 0;
			%client.player.lastDirectDamageType = 0;
		}

		Parent::onDeath(%client, %killerObject, %killerClient, %damageType, %position);
		if(isCityObject(%killerClient) && %killerClient.getClassName() $= "GameConnection" && %client != %killerClient)
			if(%amt > 0 && %lastDamageType != $DamageType::HammerDirect)
			{
				%killerClient.addDemerits($City::Demerits::Murder);
				%killerClient.centerprint_addLine("\c6You have been caught commiting a crime. [\c3Murder\c6]", 2);
			}
	}
};
activatePackage(City_Jail);

function GameConnection::sendKillMessage(%this, %victimClient, %suspectClient)
{
	if(%victimClient == %this || %suspectClient == %this || %victimClient == %suspectClient)
		return;

	if(isObject(%victimClient))
	{
		if(%victimClient.getClassName() $= "GameConnection")
			%victimName = %victimClient.getPlayerName();
		else
			%victimName = %victimClient.name;
	}

	if(!isObject(%victimClient))
		%victimName = "Someone";

	%msg = "\c6 near your location.";
	if(isObject(%suspectClient))
	{
		if(%suspectClient.getClassName() $= "GameConnection")
		{
			%msg = "\c6 in front of your eyes.";
			%suspectName = %suspectClient.getPlayerName();
		}
		else
		{
			%msg = "\c6 in front of your eyes...";
			%suspectName = %suspectClient.name;
		}
	}

	if(!isObject(%suspectClient))
		%suspectName = "Somebody";

	%mMsg = %suspectName @ " \c6killed \c3" @ %victimName @ %msg;
	if(!isObject(%victimClient) && !isObject(%suspectClient))
		%mMsg = "\c6You hear a deadly scream nearby.";

	messageClient(%this, '', %mMsg);
}

function Player::ResetHammerSch(%this, %object)
{
	if(!isObject(%object))
		return;

	cancel(%this.resetHamerSch[%object]);
	%this.hammered[%object] = 0;
	//%this.resetHamerSch[%object] = %this.schedule(5000, "ResetHammerSch");
}

City_Debug("File > JailSO.cs", "   -> Loading complete.");