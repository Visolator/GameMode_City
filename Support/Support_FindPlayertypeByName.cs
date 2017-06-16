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

function findPlayertypeByName(%name, %val)
{
	if(isObject(%name)) return %name.getName();
	if(!isObject(PlayerDataCache)) new ScriptObject(PlayerDataCache);
	if(PlayerDataCache.dataCount <= 0 || %val) //We don't need to cause lag everytime we try to find an item
	{
		PlayerDataCache.dataCount = 0;
		for(%i=0;%i<DatablockGroup.getCount();%i++)
		{
			%obj = DatablockGroup.getObject(%i);
			if(%obj.getClassName() $= "PlayerData" && strLen(%obj.uiName) > 0)
			{
				PlayerDataCache.data[PlayerDataCache.dataCount] = %obj;
				PlayerDataCache.dataCount++;
			}
		}
	}

	//First let's see if we find something to be exact
	if(PlayerDataCache.itemCount > 0)
	{
		for(%a=0;%a<PlayerDataCache.itemCount;%a++)
		{
			%objA = PlayerDataCache.item[%a];
			if(%objA.getClassName() $= "ItemData")
				if(%objA.uiName $= %item || %objA.getName() $= %item)
					return %objA.getName();
		}
	}

	//Okay, we found nothing, let's see if we can find it.
	if(PlayerDataCache.itemCount > 0)
	{
		for(%a=0;%a<PlayerDataCache.itemCount;%a++)
		{
			%objA = PlayerDataCache.item[%a];
			if(%objA.getClassName() $= "ItemData")
				if(striPos(%objA.uiName, %item) >= 0)
					return %objA.getName();
		}
	}
	return -1;
}

function Player::setNameDatablock(%player, %name)
{
	%client = %player.client;
	if(isObject(%name))
	{
		if(%name.getClassName() !$= "PlayerData") return false;
		%name = %name.getName();
	}
	else
		%name = findPlayertypeByName(%name);
	if(!isObject(%name)) return -1;
	%datablock = nameToID(%name);
	%player.setDatablock(%datablock);
	return true; //We didn't find a slot :(
}