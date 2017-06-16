//This part of the mod, someone can mess with, I'm not dealing with it.



//Prefs created by the original creator of Gamemode_CityRPG, in which I have no idea.
$City::Drugs::minSellingSpeed = 1;
$City::Drugs::maxSellingSpeed = 5;
$City::Drugs::minBuyAmt = 1;
$City::Drugs::maxBuyAmt = 25;
$City::Drugs::DrugLimit = 8;
$City::Drugs::evidenceWorth = 6;
$City::Drugs::demWorth = 1.5;

$City::DrugCount = 0;
$City::Drug[$City::DrugCount++] = "Cannabis";
$City::Drug[$City::DrugCount++] = "Peyote";
$City::Drug[$City::DrugCount++] = "Shrooms";
$City::Drug[$City::DrugCount++] = "Tobacco";

//Game base code, no one should modify the prefs here unless you know what you are dong.
$City::Drugs::priceCannabis = 1800;
$City::Drugs::HarvestMinCannabis = 9;
$City::Drugs::HarvestMaxCannabis = 14;
$City::Drugs::growthTimeCannabis = 2;
$City::Drugs::MinSellCannabis = 13;
$City::Drugs::MaxSellCannabis = 14;

$City::Drugs::pricePeyote = 3000;
$City::Drugs::HarvestMinPeyote = 11;
$City::Drugs::HarvestMaxPeyote = 18;
$City::Drugs::growthTimePeyote = 2;
$City::Drugs::MinSellPeyote = 15;
$City::Drugs::MaxSellPeyote = 16;

$City::Drugs::priceShrooms = 1000;
$City::Drugs::HarvestMinShrooms = 7;
$City::Drugs::HarvestMaxShrooms = 10;
$City::Drugs::growthTimeShrooms = 1.5;
$City::Drugs::MinSellShrooms = 9;
$City::Drugs::MaxSellShrooms = 12;

$City::Drugs::priceTobacco = 1500;
$City::Drugs::HarvestMinTobacco = 8;
$City::Drugs::HarvestMaxTobacco = 16;
$City::Drugs::growthTimeTobacco = 2;
$City::Drugs::MinSellTobacco = 10;
$City::Drugs::MaxSellTobacco = 14;

datablock fxDTSBrickData(CityCannabisData)
{
	brickFile = "Add-Ons/Gamemode_City/CityItems/Drugs/Cannabis/brick.blb";
	iconName = "Add-Ons/Gamemode_City/CityItems/Drugs/Cannabis/icon";
	collisionShapeName = "Add-Ons/Gamemode_City/CityItems/Drugs/Cannabis/shape.dts";

	uiName = "Cannabis";
	category = "City";
	subCategory = "Drugs";

	CityBrickType = "Drugs";
	initialPrice = $City::Drugs::priceCannabis;
};

datablock fxDTSBrickData(CityPeyoteData)
{
	brickFile = "Add-Ons/Gamemode_City/CityItems/Drugs/Peyote/brick.blb";
	iconName = "Add-Ons/Gamemode_City/CityItems/Drugs/Peyote/icon";
	collisionShapeName = "Add-Ons/Gamemode_City/CityItems/Drugs/Peyote/shape.dts";

	uiName = "Peyote";
	category = "City";
	subCategory = "Drugs";

	CityBrickType = "Drugs";
	initialPrice = $City::Drugs::pricePeyote;
};

datablock fxDTSBrickData(CityShroomsData)
{
	brickFile = "Add-Ons/Gamemode_City/CityItems/Drugs/Shrooms/brick.blb";
	iconName = "Add-Ons/Gamemode_City/CityItems/Drugs/Shrooms/icon";

	uiName = "Shrooms";
	category = "City";
	subCategory = "Drugs";

	CityBrickType = "Drugs";
	initialPrice = $City::Drugs::priceShrooms;
};

datablock fxDTSBrickData(CityTobaccoData)
{
	brickFile = "Add-Ons/Gamemode_City/CityItems/Drugs/Tobacco/brick.blb";
	iconName = "Add-Ons/Gamemode_City/CityItems/Drugs/Tobacco/icon";
	collisionShapeName = "Add-Ons/Gamemode_City/CityItems/Drugs/Tobacco/shape.dts";

	uiName = "Tobacco";
	category = "City";
	subCategory = "Drugs";

	CityBrickType = "Drugs";
	initialPrice = $City::Drugs::priceTobacco;
};

function fxDTSBrick::startGrowing(%brick)
{
	%brickData = %brick.getDatablock();
	%brick.color = 45 + %brick.growTime;
	%drugType = $City::Drugs::growthTime[%brickData.uiName];
	%drugTime = %drugType * 7500;

	if(%brick.growTime < 8)
	{
		%brick.setColor(%brick.color);
		%brick.setEmitter(None);
		%brick.growTime++;
		%brick.schedule(%drugTime, "startGrowing", %brick);
	}
	else if(%brick.growTime == 8)
		%brick.grow();
}

function fxDTSBrick::grow(%brick)
{
	%brick.health = 0;
	%brick.grown = 1;
	%brick.setColor(61);
	%brick.drugEmitter = GunSmokeEmitter;
	%brick.setEmitter(%brick.drugEmitter);
}

function fxDTSBrick::harvest(%brick, %client)
{
	%brickData = %brick.getDatablock();

	if(!%brick.grown)
		return false;

	if(%brick.health < %brick.random)
	{
		%brick.health++;
		%percentage = mFloor((%brick.health / %brick.random) * 100);
			
		// cool color effect
		if(%percentage >= 0 && %percentage < 10)
			%color = "<color:ff0000>";
		else if(%percentage >= 10 && %percentage < 20)
			%color = "<color:ff2200>";
		else if(%percentage >= 10 && %percentage < 30)
			%color = "<color:ff4400>";
		else if(%percentage >= 10 && %percentage < 40)
			%color = "<color:ff6600>";
		else if(%percentage >= 10 && %percentage < 50)
			%color = "<color:ff8800>";
		else if(%percentage >= 10 && %percentage < 60)
			%color = "<color:ffff00>";
		else if(%percentage >= 10 && %percentage < 70)
			%color = "<color:88ff00>";
		else if(%percentage >= 10 && %percentage < 80)
			%color = "<color:66ff00>";
		else if(%percentage >= 10 && %percentage < 90)
			%color = "<color:44ff00>";
		else if(%percentage >= 10 && %percentage < 100)
			%color = "<color:22ff00>";
		else if(%percentage == 100)
			%color = "<color:00ff00>";

		commandToClient(%client,'centerPrint',"\c3" @ %brickData.uiName @ " \c6harvested: %" @ %color @ "" @ %percentage,3);
		return;
	}

	%harvestAmt = getRandom($City::Drugs::HarvestMin[%brickData.uiName], $City::Drugs::HarvestMax[%brickData.uiName]);
	CityData.getData(%client.bl_id).value[%brickData.uiName] += %harvestAmt;
	%client.centerPrint("\c6You have harvested\c3" SPC %harvestAmt SPC "\c6grams of\c3" SPC %brickData.uiName @ "\c6.", 3);

	%brick.currentColor = 45;
	%brick.setColor(%brick.currentColor);
	%brick.grown = false;
	%brick.health = 0;
	%brick.growTime = 0;
	%brick.watered = 0;
	%brick.random = getRandom($City::Drugs::HarvestMin[%brickData.uiName], $City::Drugs::HarvestMax[%brickData.uiName]);
	%brick.drugEmitter = "None";
	%brick.setEmitter(None);
	%client.setInfo();
}

function gameConnection::startSelling(%client, %drug)
{
	%amount = CityData.getData(%client.bl_id).value[%drug];

	if(%amount <= 0)
	{
		messageClient(%client, '', "\c6You're all out!");
		return 0;
	}
	else
		%client.drugTick = %client.schedule((getRandom($City::Drugs::minSellingSpeed, $City::Drugs::maxSellingSpeed) * 1000), "startSelling", %drug);
	

	%grams = getRandom($City::Drugs::minBuyAmt, $City::Drugs::maxBuyAmt);

	if(%grams > %amount)
		%grams = %amount;

	%profit = getRandom($City::Drugs::minSell[%drug], $City::Drugs::maxSell[%drug]);

	%cash = %grams * %profit;

	%random = getRandom(1, 2);

	if(%random == 1)
		%cash -= getRandom(0.75, 1);
	else
		%cash += getRandom(1, 1.25);

	%cash = mFloor(%cash);

	CityData.getData(%client.bl_id).valueMoney += %cash;
	%client.setInfo();

	%slang = %grams;
	switch(%slang)
	{
		case 1:
			%slang = "a \c3gram\c6 of";
		case 2:
			%slang = "a \c3dimebag\c6 of";
		case 3:
			%slang = "\c3three grams\c6 of";
		case 4:
			%slang = "a \c3dub\c6 of";
		case 5:
			%slang = "\c3five grams\c6 of";
		default:
			%slang = "some";
	}

	messageClient(%client, '', "\c6You sold " @ %slang SPC %drug SPC "to a stranger for \c3$" @ %cash @"\c6.");

	CityData.getData(%client.bl_id).value[%drug] -= %grams;
}

function servercmdmyDrugs(%client, %time)
{
	if(%time $= "")
		%time = 5;

	%cent = "<just:left>\c3Cannabis\c6:" SPC CityData.getData(%client.bl_id).valueCannabis;
	%cent = %cent @ "<br>\c3Peyote\c6:" SPC CityData.getData(%client.bl_id).valuePeyote;
	%cent = %cent @ "<br>\c3Shrooms\c6:" SPC CityData.getData(%client.bl_id).valueShrooms;
	%cent = %cent @ "<br>\c3Tobacco\c6:" SPC CityData.getData(%client.bl_id).valueTobacco;
	%client.centerPrint(%cent, %time);
}

function fxDTSBrick::bagPlant(%brick, %client)
{
	%brickData = %brick.getDatablock();

	%harvestAmt = getRandom($City::Drugs::HarvestMin[%brickData.uiName], $City::Drugs::HarvestMax[%brickData.uiName]);
	CityData.getData(%client.bl_id).valueEvidence += %harvestAmt;
	messageClient(%client, '', "\c6You take" SPC %harvestAmt SPC "grams of" SPC %brickData.uiName SPC "as evidence.");
	%brick.schedule(0, "delete");
}