datablock itemData(batonItem : hammerItem)
{
	shapeFile = "Add-Ons/GameMode_City/CityItems/weapons/policeBaton.dts";
	category = "Weapon";
	uiName = "Baton";
	image = batonImage;
	//colorShiftColor = "0.898039 0.898039 0.000000 1.000000";
	
	// CityRPG Properties
	noSpawn = true;
	canArrest = true;
	jobVariableRequired = "canArrest";
};

datablock ProjectileData(batonProjectile)
{
	directDamage		  = 20;
	directDamageType	 = $DamageType::Baton;
	radiusDamageType	 = $DamageType::Baton;
	
	explosion = hammerExplosion;
	explosionSound = hammerHitSound;

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 10;
	brickExplosionMaxVolume = 1;
	brickExplosionMaxVolumeFloating = 2;
	
	impactImpulse		= 0;
	verticalImpulse		= 0;
	particleEmitter		= "";
	
	muzzleVelocity		= 60;
	velInheritFactor	= 1;
	
	armingDelay			= 0;
	lifetime			= 100;
	fadeDelay			= 80;
	bounceElasticity	= 0.5;
	bounceFriction		= 0.20;
	isBallistic			= false;
	gravityMod = 0.0;
	
	hasLight	 = false;
	lightRadius = 1.0;
	lightColor  = "1.0 1.0 0.5";
};

datablock shapeBaseImageData(batonImage : hammerImage)
{
	shapeFile 			= "Add-Ons/GameMode_City/CityItems/weapons/policeBaton.dts";	
	eyeOffset 			= "0 0 0";
	item 				= batonItem;
	melee 				= true;
	projectile			= batonProjectile;
	projectileType		= Projectile;
	//colorShiftColor 	= "0.898039 0.898039 0.000000 1.000000";
	showBricks 			= 0;

	stateTimeoutValue[0] = 0.2;
	stateTimeoutValue[1] = 0.0;
	stateTimeoutValue[2] = 0.01;
	stateTimeoutValue[3] = 0.2;
	stateTimeoutValue[4] = 0;
	stateTimeoutValue[5] = 0.2;
};

function batonImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "armAttack");
}

function batonImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

if(isPackage(City_Weapon_Baton))
	deactivatePackage(City_Weapon_Baton);

//Since the Parent exists we have to make an overwrite
package City_Weapon_Baton
{
	function batonProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
	{
		parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
		if(%col.getClassName() $= "fxDTSBrick")
		{
			%client = %obj.client;
			%brickData = %col.getDatablock();
			if(%brickData.isDoor)
			{
				if(isCityObject(%client))
				{
					%job = %client.getJob();
					if(!%job.canArrest)
					{
						if(%client.addDemeritsIfWitnessed(50, $City::Witness::Range, %this))
							%client.centerprint_addLine("\c6You have been caught commiting a crime. [\c3Breaking in\c6]", 2);
					}

				}

				if(isObject(%group = getBrickgroupFromObject(%col)) && %group.bl_id != 180)
					%col.fakeKillBrick("0 0 0", 3);
				else if(!isObject(%group))
					%col.fakeKillBrick("0 0 0", 3);
			}
		}
	}
};
activatePackage(City_Weapon_Baton);

function batonProjectile::damage(%this, %obj, %col, %fade, %pos, %normal)
{
	if(%this.directDamage <= 0)
		return;

	%damageType = $DamageType::Direct;
	if(%this.DirectDamageType)
		%damageType = %this.DirectDamageType;

	%scale = getWord(%obj.getScale(), 2);
	%directDamage = mClampF(%this.directDamage, 1, 100) * %scale;

	if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		if(isCityObject(%colClient = %col.client) && isCityObject(%client = %obj.client))
		{
			if(%client.City_CanArrest(%colClient))
			{
				if(%col.getHealth() - %directDamage <= 1)
					%colClient.schedule(0, arrest, %client); //avoid console errors
				else
				{
					if(isFunction(%colClient.getClassName(), getPlayerName))
						%name = %colClient.getPlayerName();
					else //Must be something else
						%name = %colClient.netName;

					%client.centerPrint("\c3" @ %name @ " \c6has resisted arrest!", 2);
					%colClient.lastArrestAttempt = $Sim::Time;
					%col.damage(%col, %pos, %directDamage, %damageType);
					cancel(%col.slowRunCancelSch);
					%col.setSpeedFactor(0.4);
					%col.slowRunCancelSch = %col.schedule(1000, setSpeedFactor, %col.City_SpeedFactor);
				}
			}
		}
		else
			%col.damage(%obj, %pos, %directDamage, %damageType);
	}
}