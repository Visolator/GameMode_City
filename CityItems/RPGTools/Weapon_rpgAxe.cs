datablock ExplosionData(rpgAxeExplosion)
{
   lifeTimeMS = 300;

   soundProfile = swordHitSound;

   particleEmitter = swordExplosionEmitter;
   particleDensity = 8;
   particleRadius = 0.2;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "12.0 14.0 12.0";
   camShakeAmp = "0.7 0.7 0.7";
   camShakeDuration = 0.35;
   camShakeRadius = 7.0;

   lightStartRadius = 1.5;
   lightEndRadius = 0;
   lightStartColor = "00.0 0.2 0.6";
   lightEndColor = "0 0 0";
};

datablock ItemData(rpgAxeItem : swordItem)
{
	shapeFile = "./Lil_Axe.dts";
	uiName = "RPG Axe";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	image = rpgAxeImage;
	canDrop = true;
	iconName = "./icon_rpgAxe";
};

AddDamageType("rpgAxe",   '<bitmap:add-ons/Tool_RPG/CI_rpgAxe> %1',    '%2 <bitmap:add-ons/Tool_RPG/CI_rpgAxe> %1',0.75,1);

datablock ProjectileData(rpgAxeProjectile)
{
   directDamage      = 20;
   directDamageType  = $DamageType::rpgAxe;
   radiusDamageType  = $DamageType::rpgAxe;
   explosion         = rpgAxeExplosion;

   muzzleVelocity      = 65;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 100;
   fadeDelay           = 70;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = false;
   gravityMod = 0.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";

   uiName = "RPG Axe Hit";
};

datablock ShapeBaseImageData(rpgAxeImage)
{
   shapeFile = "./Lil_Axe.dts";
   emap = true;

   mountPoint = 0;
   offset = "0 0 0";

   correctMuzzleVector = false;

   className = "WeaponImage";

   item = rpgAxeItem;
   ammo = " ";
   projectile = rpgAxeProjectile;
   projectileType = Projectile;


   melee = true;
   doRetraction = false;

   armReady = true;


   doColorShift = true;
   colorShiftColor = "0.471 0.471 0.471 1.000";

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = swordDrawSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]			= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.1;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateTimeoutValue[3]            = 0.2;
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateWaitForTimeout[3]		= true;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";

	
	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.2;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";


};

function rpgAxeImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armattack);
}

function rpgAxeImage::onStopFire(%this, %obj, %slot)
{	
	%obj.playthread(2, root);
}
function rpgAxeProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
	if(%col.getClassName() $= "fxDTSBrick")
	{
		if(%col.isCRPTree)
			%col.onHitTreeBrick(%obj.client);
	}
}