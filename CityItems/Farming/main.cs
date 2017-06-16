function fxDtsBrick::City_CreatePlant(%this, %bypass)
{
	if(!isObject(%this))
		return;

	%client = %this.client;
	if(!isObject(%client))
		%client = %this.getGroup().client;

	%adminLevel = %client.isAdmin + %client.isSuperAdmin + (%this.getGroup().bl_id == getNumKeyID() ? 1 : 0);

	if(%bypass)
		%adminLevel = 3;

	//Let's make it super admin only for now
	if(%adminLevel < 2 && !%bypass)
	{
		if(isObject(%client))
			%client.centerPrint("This kind of brick isn't ready to be planted.", 2);
		%this.schedule(0, delete);
		return -1;
	}

	%this.isFarmBrick = 1;
}

if(isPackage(City_Farming_Main))
	deactivatePackage(City_Farming_Main);

package City_Farming_Main
{
	function fxDTSBrick::onDeath(%brick)
	{
		if(%brick.isFarmBrick)
			$City::FarmBricks = removeWord($City::FarmBricks, %brick);
		parent::onDeath(%brick);
	}

	function fxDTSBrick::onRemove(%brick)
	{
		if(%brick.isFarmBrick)
			$City::FarmBricks = removeWord($City::FarmBricks, %brick);
		parent::onRemove(%brick);
	}

	function fxDtsBrick::setLight(%this,%light)
	{
		if(%this.dataBlock.getName() $= "brickCFheatData")
		{
			if(%this.activated)
				%light = OrangeLight;
			else
				%light = 0;
			return Parent::setLight(%this,%light);
		}
		else
			return Parent::setLight(%this,%light);
	}

	function fxDtsBrick::setColor(%this,%color)
	{
		if(!%this.isPlanted)
		{
			Parent::setColor(%this,%color);
			return;
		}

		if(%this.dataBlock.isPlantBrick || %this.dataBlock.isGrownBrick)
		{
			if(%this.dead)
				parent::setColor(%this, 41);
			else if(%this.water < 16)
				parent::setColor(%this, 54);
			else if(%this.water < 32)
				parent::setColor(%this, 55);
			else if(%this.water < 48)
				parent::setColor(%this, 56);
			else if(%this.water < 64)
				parent::setColor(%this, 57);
			else if(%this.water < 80)
				parent::setColor(%this, 58);
			else
				parent::setColor(%this, 59);
		}
		else if(%this.dataBlock $= "brickCFdirtData" || %this.dataBlock $= "brickCFfenceData" || %this.dataBlock $= "brickCFbushData")
        {
			if(%this.fert < 1)
				%this.fert = 1;
			if(%this.fert > 9)
				%this.fert = 9;
			Parent::setColor(%this,53 + %this.fert);
		}
		else if(%this.dataBlock $= "brickCFwaterData" || %this.dataBlock $= "brickCFbetterWaterData")
		{
			if(%this.activated)
				Parent::setColor(%this,7);
			else
				Parent::setColor(%this,6);
		}
		else if(%this.dataBlock $= "brickCFheatData")
		{
			Parent::setColor(%this,2);
		}
		else
			parent::setColor(%this,%color);
	}

	function fxDtsBrick::onActivate(%brick,%player,%client,%c,%d,%e,%f,%g)
	{
		Parent::onActivate(%brick,%player,%client,%c,%d,%e,%f,%g);
		if(%brick.getDatablock().isGrownBrick)
		{
			%client.chatMessage("Plant");
		}
	}
};
activatePackage(City_Farming_Main);