datablock projectileData(ShovelProjectile : hammerProjectile)
{
	lifeTime = 250;
	explodeOnDeath = false;
};

if(isPackage(City_Farming_ShovelProjectile))
	deactivatePackage(City_Farming_ShovelProjectile);

package City_Farming_ShovelProjectile
{
	function ShovelProjectile::onCollision(%this, %obj, %brick, %fade, %pos, %normal)
	{
		Parent::onCollision(%this, %obj, %brick, %fade, %pos, %normal);
		//Code
	}
};
activatePackage(City_Farming_ShovelProjectile);

datablock itemData(ShovelItem : hammerItem)
{
	shapeFile = "./models/shovel.dts";
	uiName = "Till Soil";
	image = ShovelImage;
	iconName = "./models/shovel";
	colorShiftColor = "0.3 0.3 0.3 1";
};

datablock shapeBaseImageData(ShovelImage : hammerImage)
{
	shapeFile = "./models/shovel.dts";
	item = ShovelItem;
	projectile = ShovelProjectile;
	colorShiftColor = "0.3 0.3 0.3 1";
};

function ShovelImage::onFire(%this, %obj, %slot)
{
	parent::onFire(%this, %obj, %slot);
	%obj.playThread(2, "armAttack");
}

function ShovelImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

datablock projectileData(PlantingCanProjectile : hammerProjectile)
{
	lifeTime = 250;
	explodeOnDeath = false;
};

if(isPackage(City_Farming_PlantingCanProjectile))
	deactivatePackage(City_Farming_PlantingCanProjectile);

package City_Farming_PlantingCanProjectile
{
	function PlantingCanProjectile::onCollision(%this, %obj, %brick, %fade, %pos, %normal)
	{
		Parent::onCollision(%this, %obj, %brick, %fade, %pos, %normal);
		//Code
	}
};
activatePackage(City_Farming_PlantingCanProjectile);

datablock itemData(PlantingCanItem : hammerItem)
{
	shapeFile = "./models/hoe.dts";
	uiName = "Plant Crops";
	image = PlantingCanImage;
	iconName = "./models/sack";
	colorShiftColor = "0.3 0.3 0.3 1";
};

datablock shapeBaseImageData(PlantingCanImage : hammerImage)
{
	shapeFile = "./models/hoe.dts";
	item = PlantingCanItem;
	projectile = PlantingCanProjectile;
	colorShiftColor = "0.3 0.3 0.3 1";
};

function PlantingCanImage::onFire(%this, %obj, %slot)
{
	parent::onFire(%this, %obj, %slot);
	%obj.playThread(2, "shiftLeft");
}

function PlantingCanImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

datablock projectileData(WateringCanProjectile : hammerProjectile)
{
	lifeTime = 250;
	explodeOnDeath = false;
};

if(isPackage(City_Farming_WateringCanProjectile))
	deactivatePackage(City_Farming_WateringCanProjectile);

package City_Farming_WateringCanProjectile
{
	function WateringCanProjectile::onCollision(%this, %obj, %brick, %fade, %pos, %normal)
	{
		Parent::onCollision(%this, %obj, %brick, %fade, %pos, %normal);
		//Code
	}
};
activatePackage(City_Farming_WateringCanProjectile);

datablock itemData(WateringCanItem : hammerItem)
{
	shapeFile = "./models/Wateringcan.dts";
	uiName = "Water Crops";
	image = WateringCanImage;
	iconName = "./models/Wateringcan";
	colorShiftColor = "0.3 0.3 0.3 1";
};

datablock shapeBaseImageData(WateringCanImage : hammerImage)
{
	shapeFile = "./models/Wateringcan.dts";
	item = WateringCanItem;
	projectile = WateringCanProjectile;
	colorShiftColor = "0.3 0.3 0.3 1";
};

function WateringCanImage::onFire(%this, %obj, %slot)
{
	parent::onFire(%this, %obj, %slot);
	%obj.playThread(2, "armAttack");
}

function WateringCanImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}


datablock projectileData(HarvestProjectile : hammerProjectile)
{
	lifeTime = 250;
	explodeOnDeath = false;
};

if(isPackage(City_Farming_HarvestProjectile))
	deactivatePackage(City_Farming_HarvestProjectile);

package City_Farming_HarvestProjectile
{
	function HarvestProjectile::onCollision(%this, %obj, %brick, %fade, %pos, %normal)
	{
		Parent::onCollision(%this, %obj, %brick, %fade, %pos, %normal);
		//Code
	}
};
activatePackage(City_Farming_HarvestProjectile);

datablock itemData(HarvestItem : hammerItem)
{
	shapeFile = "./models/shears.dts";
	uiName = "Harvest Tool";
	image = HarvestImage;
	colorShiftColor = "0 0 0 1";
};


datablock shapeBaseImageData(HarvestImage : hammerImage)
{
	shapeFile = "./models/shears.dts";
	item = HarvestItem;
	projectile = HarvestProjectile;
	colorShiftColor = "0 0 0 1";
};


function HarvestImage::onFire(%this, %obj, %slot)
{
	parent::onFire(%this, %obj, %slot);
	%obj.playThread(2, "armAttack");
}


function HarvestImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}


datablock projectileData(FertCanProjectile : hammerProjectile)
{
	lifeTime = 250;
	explodeOnDeath = false;
};

if(isPackage(City_Farming_FertCanProjectile))
	deactivatePackage(City_Farming_FertCanProjectile);

package City_Farming_FertCanProjectile
{
	function FertCanProjectile::onCollision(%this, %obj, %brick, %fade, %pos, %normal)
	{
		Parent::onCollision(%this, %obj, %brick, %fade, %pos, %normal);
		//Code
	}
};
activatePackage(City_Farming_FertCanProjectile);

datablock itemData(FertCanItem : hammerItem)
{
	shapeFile = "./models/hoe.dts";
	uiName = "Fertilizer";
	image = FertCanImage;
	iconName = "./models/sack";
	colorShiftColor = "1 0.6 0.3 1";
};

datablock shapeBaseImageData(FertCanImage : hammerImage)
{
	shapeFile = "./models/hoe.dts";
	item = FertCanItem;
	projectile = FertCanProjectile;
	colorShiftColor = "1 0.6 0.3 1";
};

function FertCanImage::onFire(%this, %obj, %slot)
{
	parent::onFire(%this, %obj, %slot);
	%obj.playThread(2, "armAttack");
}

function FertCanImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}