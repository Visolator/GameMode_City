City_Debug("File > Survival.cs", "Loading assets..");

if(isPackage(Survival))
	deactivatePackage(Survival);

package Survival
{
	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %position)
	{
		Parent::onDeath(%client, %killerPlayer, %killer, %damageType, %position);
		if(%client.CityData["Hunger"] <= 0 || %client.CityData["Thirst"] <= 0)
		{
			%client.CityData["Hunger"] = 100;
			%client.CityData["Thirst"] = 100;

			messageAll('', "\c3" @ %client.getPlayerName() @ " \c6died of starvation/thirst.");
		}

		if(%client.CityData["Hunger"] >= 200)
		{
			%client.CityData["Hunger"] = 100;
			%client.CityData["Thirst"] = 100;
			messageAll('', "\c3" @ %client.getPlayerName() @ " \c6exploded from obesity.");
		}
	}
};
activatePackage(Survival);

function GameConnection::canDrinkMore(%this)
{
	return %this.CityData["Hunger"] < 100;
}

function GameConnection::canEatMore(%this)
{
	return %this.CityData["Hunger"] < 200;
}

function GameConnection::addHunger(%this, %amt)
{
	if(isObject(%player = %this.player) && !%this.avoidSurvival && !$City::Survival::Disabled && %player.getState() !$= "dead")
	{
		if($City::Hunger::EnableScaling)
		{
			%scale = %player.getScale();
			%sc = getWord(%scale, 2);
			if(%this.CityData["Hunger"] <= 40)
				%sc = mClampF(getWord(%scale, 2) * (%this.CityData["Hunger"] / 100), 0.6, 1);

			if(%this.CityData["Hunger"] > 100)
				%sc = mClampF(getWord(%scale, 2) * (%this.CityData["Hunger"] / 100), 1, 5);

			%sc = mFloatLength(%sc, 1);
			%player.setScale(%sc SPC %sc SPC getWord(%scale, 2));
		}
		%this.CityData["Hunger"] += %amt;
		%this.CityData["Hunger"] = mClampF(%this.CityData["Hunger"], 0, 200);
		if(%this.CityData["Hunger"] <= 0)
			%player.addHealth(-15);

		if(%this.CityData["Hunger"] >= 200)
		{
			%player.kill();
			%player.spawnExplosion(VehicleExplosionProjectile, getWord(%scale, 2));
		}
	}
	%this.City_Update();
}

function GameConnection::addThirst(%this, %amt)
{
	if(isObject(%player = %this.player) && !%this.avoidSurvival && !$City::Survival::Disabled && %player.getState() !$= "dead")
	{
		%this.CityData["Thirst"] += %amt;
		%this.CityData["Thirst"] = mClampF(%this.CityData["Thirst"], 0, 100);
		if(%this.CityData["Thirst"] <= 0)
			%player.addHealth(-5);
	}
	%this.City_Update();
}

function GameConnection::buyFood(%this, %name, %size, %mult)
{
	if($City::Survival::Disabled)
	{
		%this.chatMessage("Sorry, you cannot buy this product right now.");
		return;
	}

	if(trim(%name) $= "")
		%name = "this food";
	%mult /= 100;

	switch$(%size)
	{
		case 0:
			%cAmt = 5;
			%amount = 5;
			%portion = "Tiny";

		case 1:
			%cAmt = 12;
			%amount = 10;
			%portion = "Small";

		case 2:
			%cAmt = 18;
			%amount = 15;
			%portion = "Medium";

		case 3:
			%cAmt = 22;
			%amount = 25;
			%portion = "Large";

		case 4:
			%cAmt = 28;
			%amount = 50;
			%portion = "Huge";

		default:
			%amount = 0;
	}

	%cost = mFloatLength(%cAmt * City_getEconomyPrice() * %mult, 2);

	if(%amount <= 0)
	{
		%this.chatMessage("You did not request an amount of food.");
		return;
	}
	%title = strUpr(getSubStr(%name, 0, 1)) @ getSubStr(%name, 1, strLen(%name)-1);
	commandToClient(%this, 'MessageBoxYesNo', "Buy - " @ %title, "Would you like to buy a " @ %portion @ " portion of " @ %name @ "?<br>Costs $" @ %cost @ ".", 'confirmBuyFood');
	%this.TempData["Food"] = %name;
	%this.TempData["Cost"] = %cost;
	%this.TempData["Size"] = %size;
}

function serverCmdConfirmBuyFood(%this)
{
	if(%this.TempData["Food"] $= "")
	{
		%this.TempData["Food"] = "";
		%this.TempData["Cost"] = "";
		%this.TempData["Size"] = "";
		return;
	}

	if($City::Survival::Disabled)
	{
		%this.chatMessage("Sorry, you cannot buy this product right now.");
		return;
	}

	%cost = %this.TempData["Cost"];

	switch$(%this.TempData["Size"])
	{
		case 0:
			%amount = 5;
			%name = "Tiny";

		case 1:
			%amount = 10;
			%name = "Small";

		case 2:
			%amount = 15;
			%name = "Medium";

		case 3:
			%amount = 25;
			%name = "Large";

		case 4:
			%amount = 50;
			%name = "Huge";

		default:
			%amount = 0;
	}

	if(%amount <= 0)
	{
		%this.chatMessage("You did not request an amount of food.");
		return;
	}

	if(%this.CityData["Money"] < %cost)
	{
		%this.chatMessage("You don't have enough cash.");
		return;
	}

	%this.addHunger(%amount);
	%this.addMoney(-%cost);
	City_AddEconomy(%cost / 2);
	%this.chatMessage("\c6Thank you for your service.");
}
registerOutputEvent("GameConnection", "buyFood", "string 50 50" TAB "list Tiny 0 Small 1 Medium 2 Large 3 Huge 4" TAB "int 30 800 100");

function GameConnection::buyDrink(%this, %name, %size, %mult)
{
	if($City::Survival::Disabled)
	{
		%this.chatMessage("Sorry, you cannot buy this product right now.");
		return;
	}

	if(trim(%name) $= "")
		%name = "this drink";
	%mult /= 100;

	switch$(%size)
	{
		case 0:
			%cAmt = 3;
			%amount = 5;
			%portion = "Tiny";

		case 1:
			%cAmt = 8;
			%amount = 10;
			%portion = "Small";

		case 2:
			%cAmt = 12;
			%amount = 15;
			%portion = "Medium";

		case 3:
			%cAmt = 16;
			%amount = 25;
			%portion = "Large";

		case 4:
			%cAmt = 20;
			%amount = 50;
			%portion = "Huge";

		default:
			%amount = 0;
	}

	%cost = mFloatLength(%cAmt * City_getEconomyPrice() * %mult, 2);

	if(%amount <= 0)
	{
		%this.chatMessage("You did not request an amount of liquid to drink.");
		return;
	}
	%title = strUpr(getSubStr(%name, 0, 1)) @ getSubStr(%name, 1, strLen(%name)-1);
	commandToClient(%this, 'MessageBoxYesNo', "Buy - " @ %title, "Would you like to buy a " @ %portion @ " portion of " @ %name @ "?<br>Costs $" @ %cost @ ".", 'confirmBuyDrink');
	%this.TempData["Drink"] = %name;
	%this.TempData["Cost"] = %cost;
	%this.TempData["Size"] = %size;
}

function serverCmdConfirmBuyDrink(%this)
{
	if(%this.TempData["Drink"] $= "")
	{
		%this.TempData["Drink"] = "";
		%this.TempData["Cost"] = "";
		%this.TempData["Size"] = "";
		return;
	}

	if($City::Survival::Disabled)
	{
		%this.chatMessage("Sorry, you cannot buy this product right now.");
		return;
	}

	%cost = %this.TempData["Cost"];

	switch$(%this.TempData["Size"])
	{
		case 0:
			%amount = 2;
			%name = "Tiny";

		case 1:
			%amount = 5;
			%name = "Small";

		case 2:
			%amount = 9;
			%name = "Medium";

		case 3:
			%amount = 13;
			%name = "Large";

		case 4:
			%amount = 18;
			%name = "Huge";

		default:
			%amount = 0;
	}

	if(%amount <= 0)
	{
		%this.chatMessage("You did not request an amount of liquid to drink.");
		return;
	}

	if(%this.CityData["Money"] < %cost)
	{
		%this.chatMessage("You don't have enough cash.");
		return;
	}

	%this.addThirst(%amount);
	%this.addMoney(-%cost);
	City_AddEconomy(%cost / 2);
	%this.chatMessage("\c6Thank you for your service.");
}
registerOutputEvent("GameConnection", "buyDrink", "string 50 50" TAB "list Tiny 0 Small 1 Medium 2 Large 3 Huge 4" TAB "int 30 800 100");

City_Debug("File > Survival.cs", "   -> Loading complete.");