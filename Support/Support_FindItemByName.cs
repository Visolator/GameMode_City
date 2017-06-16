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
//Version 2

function findItemByName(%item, %val)
{
	if(isObject(%item))
		return %item.getName();

	if(!isObject(ItemCache))
		new ScriptObject(ItemCache)
		{
			itemCount = 0;
			lastDatablockCount = DatablockGroup.getCount();
		};

	//Was going to add them into a simset but it's not like you're going to have 1000+ items.. right?
	//Should automatically create the lookup if you:
	// + Added new weapons
	// + Started the server
	if(ItemCache.itemCount <= 0 || ItemCache.lastDatablockCount != DatablockGroup.getCount() || %val) //We don't need to cause lag everytime we try to find an item
	{
		ItemCache.lastDatablockCount = DatablockGroup.getCount();
		ItemCache.itemCount = 0;
		for(%i=0;%i<DatablockGroup.getCount();%i++)
		{
			%obj = DatablockGroup.getObject(%i);
			if(%obj.getClassName() $= "ItemData" && strLen(%obj.uiName) > 0)
			{
				ItemCache.item[ItemCache.itemCount] = %obj;
				ItemCache.itemCount++;
			}
		}

		echo("findItemByName() - Created lookup database.");
	}

	//First let's see if we find something to be exact
	if(ItemCache.itemCount > 0)
	{
		%result["string"] = 0;
		%result["id"] = 0; //If this is found we are definitely giving it
		%result["string", "pos"] = 9999;
		for(%a = 0; %a < ItemCache.itemCount; %a++)
		{
			%objA = ItemCache.item[%a];
			if(%objA.getClassName() $= "ItemData")
				if(%objA.uiName $= %item || %objA.getName() $= %item)
				{
					%result["id"] = 1;
					%result["id", "item"] = %objA;
				}
				else
				{
					%pos = striPos(%objA.uiName, %item);
					if(striPos(%objA.uiName, %item) >= 0 && %pos < %result["string", "pos"])
					{
						%result["string"] = 1;
						%result["string", "item"] = %objA;
						%result["string", "pos"] = %pos;
					}						
				}
		}

		if(%result["id"] && isObject(%result["id", "item"])) //This should most likely say yes
			return %result["id", "item"].getName();

		if(%result["string"] && isObject(%result["string", "item"]))
			return %result["string", "item"].getName();
	}

	return -1;
}

function Player::hasItem(%player, %item)
{
	%client = %player.client;
	if(isObject(%item))
	{
		if(%item.getClassName() !$= "ItemData") return false;
		%item = %item.getName();
	}
	else
		%item = findItemByName(%item);
	if(!isObject(%item)) return -1;
	%item = nameToID(%item);
	for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
	{
		%tool = %player.tool[%i];
		if(isObject(%tool) && %tool == %item)
			return true;
	}

	return false;
}

function Player::addNewItem(%player, %item, %silent)
{
	%client = %player.client;
	if(isObject(%item))
	{
		if(%item.getClassName() !$= "ItemData") return false;
		%item = %item.getName();
	}
	else
		%item = findItemByName(%item);

	if(!isObject(%item))
		return -1;

	%item = nameToID(%item);
	for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
	{
		%tool = %player.tool[%i];
		if(!isObject(%tool))
		{
			%player.tool[%i] = %item;
			%player.weaponCount++;
			messageClient(%client,'MsgItemPickup','',%i,%item,%silent);
			return true;
		}
	}
	
	return false; //We didn't find a slot :(
}
registerOutputEvent("Player", "addNewItem", "datablock ItemData" TAB "bool");

function Player::removeCurrentItem(%player, %searchForAll, %silent)
{
	%client = %player.client;
	%item = %player.tool[%player.currTool];
	if(isObject(%item))
	{
		if(%item.getClassName() !$= "ItemData")
			return false;

		%item = %item.getName();
	}
	else
		%item = findItemByName(%item);

	if(!isObject(%item)) 
		return false;

	%item = nameToID(%item);
	%tool = %player.tool[%player.currTool];
	if(isObject(%tool))
	{
		%player.tool[%player.currTool] = 0;
		%player.weaponCount--;
		messageClient(%client,'MsgItemPickup','',%player.currTool,%silent);
		return true;
	}

	if(%removedItems)
		return true;

	return false;
}

function Player::removeItem(%player, %item, %searchForAll, %silent)
{
	%client = %player.client;
	if(isObject(%item))
	{
		if(%item.getClassName() !$= "ItemData")
			return false;

		%item = %item.getName();
	}
	else
		%item = findItemByName(%item);

	if(!isObject(%item))
		return -1;

	%item = nameToID(%item);
	for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
	{
		%tool = %player.tool[%i];
		if(%tool == %item)
		{
			%removedItems = 1;
			%player.tool[%i] = 0;
			%player.weaponCount--;
			messageClient(%client,'MsgItemPickup','',%i,%silent);
			serverCmdUnUseTool(%client);
			if(!%searchForAll)
				return true;
		}
	}

	if(%removedItems)
		return true;

	return false;
}
registerOutputEvent("Player", "removeItem", "datablock ItemData" TAB "bool" TAB "bool");