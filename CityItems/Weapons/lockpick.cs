// ============================================================
// Project				:	CityRPG
// Author				:	Iban
// Description			:	Lockpick Code file
// ============================================================
// Table of Contents
// 1. Datablocks
// 2. Functions
// ============================================================

// ============================================================
// Section 1 : Datablocks
// ============================================================
datablock itemData(CityRPGPicklockItem : hammerItem)
{
	category = "Weapon";
	uiName = "Lockpick";
	image = CityRPGPicklockImage;
	colorShiftColor = "0.650000 0.000000 0.000000 1.000000";
	
	// CityRPG Properties
	noSpawn = true;
};

datablock shapeBaseImageData(CityRPGPicklockImage : hammerImage)
{
	// SpaceCasts
	raycastWeaponRange = 6;
	raycastWeaponTargets = $TypeMasks::All;
	raycastDirectDamage = 0;
	raycastDirectDamageType = $DamageType::HammerDirect;
	raycastExplosionProjectile = hammerProjectile;
	raycastExplosionSound = hammerHitSound;
	
	item = CityRPGPicklockItem;
	projectile = hammerProjectile;
	colorShiftColor = "0.650000 0.000000 0.000000 1.000000";
	showBricks = 0;
};

// ============================================================
// Section 2 : Functions
// ============================================================

// Section 2.1 : Visual Functionality
function CityRPGPicklockImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "armAttack");
}

function CityRPGPicklockImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function CityRPGPicklockImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	%client = %obj.client;
	
	if(%col.getType() & $TypeMasks::FxBrickObjectType)
	{
		if($CityRPG::pref::canpicklockadmindoors != 1)
		{
			if(getrandom(0,3) == 2)
			{
				%col.JVSOpen(%client);
				
				commandToClient(%client, 'centerPrint', "\c6You have commited a crime. [\c3Breaking and Entering\c6]", 3);
				CityRPG_AddDemerits(%client.bl_id, $CityRPG::demerits::breakingAndEntering);
				return;
			}
			else
			{
				commandToclient(%client, 'centerprint', "\c6Picking Lock...", 3);
			}
		}
		else
		{
			commandToclient(%client, 'centerprint', "\c6Lock Is unpickable", 3);
		}
	}
	else if(%col.getType() & $TypeMasks::VehicleObjectType)
	{
		if(%col.locked)
		{
			%col.locked = false;
			CityRPG_AddDemerits(%obj.client.bl_id, $CityRPG::demerits::grandTheftAuto);
			commandToClient(%client, 'centerPrint', "\c6You have committed a crime. [\c3Grand Theft Auto\c6]", 5);
		}
	}
	
	parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
}

// Section 2.2 : Package
package CityRPG_LockpickPackage
{	
	function player::activateStuff(%this)
	{
		parent::activateStuff(%this);
		
		if(%this.client.getJobSO().thief)
		{
			%target = containerRayCast(%this.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%this.getEyeVector()), 2.5), %this.getEyePoint()), $TypeMasks::All);
			
			if(%this.lastPickpocket + 5 <= $sim::time && %this.client != %target.client && isObject(%target.client) && %target.getClassName() $= "Player" && getWord(CityRPGData.getData(%this.client.bl_id).valueJailData, 1) < 1 && getWord(CityRPGData.getData(%target.client.bl_id).valueJailData, 1) < 1)
			{
				if(%target.client.getJobSO().thief || %this.client.getWantedLevel())
				{
					messageClient(%this.client, '', "\c6Your target seems very aware of what you're doing...");
				}
				else
				{
					if(CityRPGData.getData(%target.client.bl_id).valueMoney > 0)
					{				
						%this.lastPickpocket = $sim::time;
						
						%money = CityRPGData.getData(%target.client.bl_id).valueMoney;
						
						if(%money >= 100)
							%maxValue = 6;
						else if(%money >= 50)
							%maxValue = 5;
						else if(%money >= 20)
							%maxValue = 4;
						else if(%money >= 10)
							%maxValue = 3;
						else if(%money >= 5)
							%maxValue = 2;
						else
							%maxValue = 1;
								
						%billStolen = mFloor(getRandom(1, %maxValue));
						
						switch(%billStolen)
						{
							case 1: %theft = 1;
							case 2: %theft = 5;
							case 3: %theft = 10;
							case 4: %theft = 20;
							case 5: %theft = 50;
							case 6: %theft = 100;
						}
						
						CityRPGData.getData(%this.client.bl_id).valueMoney += %theft;
						CityRPGData.getData(%target.client.bl_id).valueMoney -= %theft;
						
						%pRotate = getWord(%this.rotation, 3);
						%tRotate = getWord(%target.rotation, 3);
						
						messageClient(%this.client, '', "\c6You have stolen a \c3$" @ %theft @ "\c6 bill from\c3" SPC  %target.client.name @ "\c6.");
						
						if(%tRotate + 45 < %pRotate || %tRotate - 45 > %pRotate)
						{
							if(CityRPGData.getData(%this.client.bl_id).valueDemerits + $CityRPG::demerits::pickpocketing < $CityRPG::pref::demerits::wantedLevel)
							{
								%demerits = $CityRPG::pref::demerits::wantedLevel;
								serverCmdmessageSent(%target.client, "Police!" SPC %this.client.name SPC "is a thief!");
							}
							else
								%demerits = $CityRPG::demerits::pickpocketing;
							
							commandToClient(%this.client, 'centerPrint', "\c6You have commited a crime. [\c3Pickpocketing\c6]", 3);
							CityRPG_AddDemerits(%this.client.bl_id, %demerits);
							
							serverCmdAlarm(%target.client);
							messageClient(%target.client, '', "\c6You have been pick-pocketed by \c3" @ (%this.client.getWantedLevel() ? %this.client.name : "an unknown thief") @ "\c6!");
						}
						
						%this.client.SetInfo();
						%target.client.SetInfo();
					}
					else
					{
						%this.lastPickpocket = $sim::time;
						%pRotate = getWord(%this.rotation, 3);
						%tRotate = getWord(%target.rotation, 3);
						
						messageClient(%this.client, '', "\c6They have no money!");
						
						if(%tRotate + 45 < %pRotate || %tRotate - 45 > %pRotate)
						{
							if(CityRPGData.getData(%this.client.bl_id).valueDemerits + $CityRPG::demerits::pickpocketing < $CityRPG::pref::demerits::wantedLevel)
							{
								%demerits = $CityRPG::pref::demerits::wantedLevel;
								serverCmdAlarm(%target.client);
								messageClient(%target.client, '', "\c6You have been pick-pocketed by \c3" @ (%this.client.getWantedLevel() ? %this.client.name : "an unknown thief") @ "\c6!");
							}
							else
								%demerits = ($CityRPG::demerits::pickpocketing / 2);
							
							commandToClient(%this.client, 'centerPrint', "\c6You have commited a crime. [\c3Attempted Pickpocketing\c6]", 3);
							CityRPG_AddDemerits(%this.client.bl_id, %demerits);
							%this.client.SetInfo();
						}
					}
				}
			}
		}
	}
};
activatePackage(CityRPG_LockpickPackage);