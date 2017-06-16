registerInputEvent("fxDTSBrick", "onDoorRestricted", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame" TAB "OwnerPlayer Player" TAB "OwnerClient GameConnection", 1);

function fxDTSBrick::doorCheck(%this, %type, %string, %door, %client)
{
	if(!isObject(%client))
		return;

	%clientBL_ID = %client.getBLID();
	%trustLevel = getTrustLevel(%client, %this);

	switch(%type)
	{
		case 0: //Admin
			if(!%client.isAdmin)
				%restricted = 1;

		case 1: //Super admin
			if(!%client.isSuperAdmin)
				%restricted = 1;

		case 2: //BL_ID Allow - Whitelist
			%restricted = 1; //We will disable restriction when we find it on the list.
			if(strLen(%string) > 0)
			{
				for(%i = 0; %i < getWordCount(%string); %i++)
				{
					%bl_id = getWord(%string, %i);
					if(%bl_id == %clientBL_ID)
						%restricted = 0;
				}
			}

		case 3: //BL_ID Deny - Blacklist
			if(strLen(%string) > 0)
			{
				for(%i = 0; %i < getWordCount(%string); %i++)
				{
					%bl_id = getWord(%string, %i);
					if(%bl_id == %clientBL_ID)
						%restricted = 1; //Enable restriction because it's on the list.
				}
			}

		case 4: //Build trust
			if(%trustLevel < 1)
				%restricted = 1;

		case 5: //Full trust
			if(%trustLevel < 2)
				%restricted = 1;

		case 6: //Self trust
			if(%trustLevel != 3)
				%restricted = 1;

		default:
			%client.chatMessage("fxDTSBrick::doorCheck() - Cannot create door check.");
			return;
	}

	//They don't have the access to do so.
	if(%restricted)
	{
		$InputTarget_["Self"] = %this;
		$InputTarget_["Player"] = %client.Player;
		$InputTarget_["Client"] = %client;

		if($Server::LAN)
		{
			$InputTarget_["MiniGame"] = getMiniGameFromObject(%client);
			$InputTarget_["OwnerPlayer"] = 0;
			$InputTarget_["OwnerClient"] = 0;
		}
		else
		{
			if(getMiniGameFromObject(%this) == getMiniGameFromObject(%client))
			{
				$InputTarget_["MiniGame"] = getMiniGameFromObject(%this);
			}
			else
			{
				$InputTarget_["MiniGame"] = 0;
			}

			if(isObject(%this.getGroup().client))
			{
				$InputTarget_["OwnerPlayer"] = %this.getGroup().client.Player;
				$InputTarget_["OwnerClient"] = %this.getGroup().client;
			}
			else
			{
				$InputTarget_["OwnerPlayer"] = 0;
				$InputTarget_["OwnerClient"] = 0;
			}
		}

		%this.processInputEvent("onDoorRestricted", %client);
		return;
	}

	%this.door(%door);
}
registerOutputEvent("fxDTSBrick", "doorCheck", "list Admin 0 SuperAdmin 1 BLIDAllow 2 BLIDDeny 3 Build 4 Full 5 Self 6" TAB "string 50 75" TAB "list Toggle 0 ToggleCCW 1 OpenCW 2 OpenCCW 3 Close 4", 1);