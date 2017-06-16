// ============================================================
// Project				:	CityRPG
// Author				:	Iban
// Description			:	Limited-Baton Code file
// ============================================================
// Table of Contents
// 1. Datablocks
// 2. Functions
// ============================================================

// ============================================================
// Section 1 : Datablocks
// ============================================================
datablock itemData(CityRPGLBItem : hammerItem)
{
	category = "Weapon";
	uiName = "Limited-Baton";
	image = CityRPGLBImage;
	colorShiftColor = "0.650000 0.650000 0.000000 1.000000";
	
	// CityRPG Properties
	noSpawn = true;
	canArrest = true;
};

datablock shapeBaseImageData(CityRPGLBImage : hammerImage)
{
	// SpaceCasts
	raycastWeaponRange = 6;
	raycastWeaponTargets = $TypeMasks::All;
	raycastDirectDamage = 20;
	raycastDirectDamageType = $DamageType::HammerDirect;
	raycastExplosionProjectile = hammerProjectile;
	raycastExplosionSound = hammerHitSound;
	
	item = CityRPGLBItem;
	projectile = hammerProjectile;
	colorShiftColor = "0.650000 0.650000 0.000000 1.000000";
	showBricks = 0;
};

// ============================================================
// Section 2 : Functions
// ============================================================

// Section 2.1 : Visual Functionality
function CityRPGLBImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "armAttack");
}

function CityRPGLBImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function CityRPGLBImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	if(%col.getClassName() $= "Player")
	{
		%client = %obj.client;
		if((%col.getType() & $typeMasks::playerObjectType) && isObject(%col.client))
		{
			if(%col.client.getWantedLevel())
			{
				if(%col.getDatablock().maxDamage - (%col.getDamageLevel() + 25) < %this.raycastDirectDamage)
				{
					%col.setDamageLevel(%this.raycastDirectDamage + 1);
					%col.client.arrest(%client);
				}
				else
					commandToClient(%client, 'CenterPrint', "\c3" @ %col.client.name SPC "\c6has resisted arrest!", 3);
			}
			else if(CityRPGData.getData(%col.client.bl_id).valueBounty > 0)
				commandToClient(%client, 'CenterPrint', "\c3" @ %col.client.name SPC "\c6is not wanted alive.", 3);
			else
				%doNoEvil = true;
		}
	}
	
	if(%doNoEvil) { %this.raycastDirectDamage = 0; }
	parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
	if(%doNoEvil) { %this.raycastDirectDamage = 25; }
}