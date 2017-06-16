//Made by Visolator ID 20490

function findVehicleByName(%vehicle, %val)
{
	if(isObject(%vehicle))
		return %vehicle.getName();

	if(!isObject(VehicleCache)) new ScriptObject(VehicleCache);
	if(VehicleCache.itemCount <= 0 || %val) //We don't need to cause lag everytime we try to find an Music
	{
		VehicleCache.itemCount = 0;
		for(%i = 0; %i < DatablockGroup.getCount(); %i++)
		{
			%obj = DatablockGroup.getObject(%i);
			if(%obj.getClassName() $= "WheeledVehicleData" || %obj.getClassName() $= "FlyingVehicleData")
				if(strLen(%obj.uiName) > 0)
				{
					VehicleCache.item[VehicleCache.itemCount] = %obj;
					VehicleCache.itemCount++;
				}
		}
	}

	//First let's see if we find something to be exact
	if(VehicleCache.itemCount > 0)
	{
		for(%a = 0; %a < VehicleCache.itemCount; %a++)
		{
			%objA = VehicleCache.item[%a];
			if(%objA.getClassName() $= "WheeledVehicleData" || %objA.getClassName() $= "FlyingVehicleData")
				if(%objA.uiName $= %vehicle)
					return %objA.getName();
		}

		for(%a = 0; %a < VehicleCache.itemCount; %a++)
		{
			%objA = VehicleCache.item[%a];
			if(%objA.getClassName() $= "WheeledVehicleData" || %objA.getClassName() $= "FlyingVehicleData")
				if(strPos(%objA.uiName, %vehicle) >= 0)
					return %objA.getName();
		}
	}

	return -1;
}