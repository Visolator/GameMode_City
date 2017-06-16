City_Debug("File > Bank.cs", "Loading assets..");

datablock ItemData(newDollarItem)
{
	category = "Weapon";
	className = "Weapon";
	
	shapeFile = "add-ons/GameMode_City/CityItems/Money/dollar.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	
	doColorShift = true;
	colorShiftColor = "0 0.6 0 1";
	image = newDollarImage;
	candrop = true;
	canPickup = false;
};

datablock ShapeBaseImageData(newDollarImage)
{
	shapeFile = "add-ons/GameMode_City/CityItems/Money/dollar.dts";
	emap = true;
	
	doColorShift = true;
	colorShiftColor = cashItem.colorShiftColor;
	canPickup = false;
};

datablock ItemData(cashItem)
{
	category = "Weapon";
	className = "Weapon";
	
	shapeFile = "base/data/shapes/brickWeapon.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	
	doColorShift = true;
	colorShiftColor = "0 0.6 0 1";
	image = cashImage;
	candrop = true;
	canPickup = false;
};

datablock ShapeBaseImageData(cashImage)
{
	shapeFile = "base/data/shapes/brickWeapon.dts";
	emap = true;
	
	doColorShift = true;
	colorShiftColor = cashItem.colorShiftColor;
	canPickup = false;
};

if(isPackage(Bank))
	deactivatePackage(Bank);

package Bank
{
	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %position)
	{
		if(%client.CityData["Money"] > 0)
		{
			%cash = new Item()
			{
				datablock = $City::Money::CashItem;
				canPickup = false;
				value = mFloor(%client.CityData["Money"]);
				dropTime = $Sim::Time;
				scale = $City::Money::Scale;
			};
			
			%cash.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 2));
			%cash.setVelocity(VectorScale(%client.player.getEyeVector(), 10));
			
			MissionCleanup.add(%cash);
			%cash.setShapeName("$" @ %cash.value);
			%cash.setShapeNameDistance(12);
			
			%client.CityData["Money"] = 0;
			%client.City_Update();
		}
		parent::onDeath(%client, %killerPlayer, %killer, %damageType, %position);
	}
	
	function Armor::onCollision(%this, %obj, %col, %thing, %other)
	{
		if(%col.getDatablock().getName() $= $City::Money::CashItem && %obj.getClassName() $= "Player" && %obj.getState() !$= "dead")
		{
			if($Sim::Time - %col.dropTime < 1)
				return;

			if(isObject(%client = %obj.client) && isObject(%col))
			{
				if(isObject(%client.minigame))
					%col.minigame = %client.minigame;
					
				%client.addMoney(%col.value);
				%client.chatMessage("\c6You have collected \c3$" @ %col.value @ " \c6off the ground.");

				%col.delete();
			}
			else if(%obj.getState() !$= "dead")
			{
				%col.delete();
				MissionCleanup.remove(%col);
			}
		}
		
		if(isObject(%col))
			Parent::onCollision(%this, %obj, %col, %thing, %other);
	}
	
	function CashItem::onAdd(%this, %item, %b, %c, %d, %e, %f, %g)
	{
		parent::onAdd(%this, %item, %b, %c, %d, %e, %f, %g);
		%item.schedulePop();
		//schedule($City::Money::DisappearMS, 0, "eval", "if(isObject(" @ nameToID(%item) @ ")) { " @ nameToID(%item) @ ".delete(); }");
	}
};
activatePackage(Bank);

datablock fxDTSBrickData(BankCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Bank Brick";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "Bank";
	triggerSize = "2 4 1";
	trigger = 0;
};

datablock fxDTSBrickData(ATMCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "ATM Brick";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "ATM";
	triggerSize = "2 4 1";
	trigger = 0;
};

function GameConnection::parseTriggerData_Bank(%this, %message)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%amt = mFloor(%message);
	%amtMoney = mAbs(mFloatLength(%message, 2));
	%accMoney = %this.CityData["Bank"];
	%curMoney = %this.CityData["Money"];
	if(%message $= "ENTER")
	{
		%msg = "\c2$" @ %accMoney;
		if(%accMoney <= 0)
			%msg = "\c0no money";

		if(%accMoney <= 0)
		{
			%msg1 = " X \c7- \c6Withdraw";
			%noOptions ++;
		}
		else
		{
			%msg1 = " \c31 \c7- \c6Withdraw";
			%list = %list @ " 1";
		}

		if($Sim::Time - %player.lastPickpocket > 20)
		{
			if(%curMoney <= 0)
			{
				%msg2 = " X \c7- \c6Deposit";
				%msg3 = " X \c7- \c6Deposit all money";
				%noOptions += 2;
			}
			else
			{
				%msg2 = " \c32 \c7- \c6Deposit";
				%msg3 = " \c33 \c7- \c6Deposit all money";
				%list = %list @ " 2 3";
			}
		}
		else
			%msg2 = " X \c7- \c6You cannot deposit money after pickpocketing somebody recently.";

		%optionsMsg = "Please select an option.";
		if(%noOptions >= 3)
			%optionsMsg = "There are no options that you can do here.";

		%this.chatMessage("\c6Welcome to the bank. You currently have " @ %msg @ " \c6in the bank. " @ %optionsMsg);
		%this.chatMessage(%msg1);
		%this.chatMessage(%msg2);
		if(%msg3 !$= "")
			%this.chatMessage(%msg3);

		%list = trim(%list);
		%list = strReplace(%list, " ", ",");
		%this.CityData["Trigger_CanChoose"] = %list;
		if(%noOptions >= 3)
			%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}
	else if(%message $= "LEAVE")
	{
		%this.CityData["Trigger_Mode"] = "";
		%this.CityData["Trigger_CanChoose"] = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(containsSubstring(%this.CityData["Trigger_CanChoose"], %amt) && %this.CityData["Trigger_Mode"] $= "")
	{
		%this.CityData["Trigger_Mode"] = %amt;
		switch(%amt)
		{
			case 1:
				%this.chatMessage("\c6Please say the current amount you want to withdraw from your account.");

			case 2:
				%this.chatMessage("\c6Please say the current amount you want to deposit from your wallet.");

			case 3:
				%this.CityData["Trigger_Mode"] = 2;
				serverCmdMessageSent(%this, %this.CityData["Money"]);

			default:
				%this.chatMessage("You broke something.");
		}
	}
	else if(%this.CityData["Trigger_Mode"] == 1)
	{
		if(%amtMoney > %this.CityData["Bank"])
		{
			%amtMoney = %this.CityData["Bank"];
			%this.addMoney(%this.CityData["Bank"]);
			%this.CityData["Bank"] = 0;
			%this.chatMessage("\c6Your request amount was way too high, so we emptied your account into your wallet.");
		}
		else
		{
			%this.addBankMoney(-%amtMoney);
			%this.addMoney(%amtMoney);
		}
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}
	else if(%this.CityData["Trigger_Mode"] == 2)
	{
		if(%amtMoney > %this.CityData["Money"])
		{
			%amtMoney = %this.CityData["Money"];
			%this.addBankMoney(%this.CityData["Money"]);
			%this.CityData["Money"] = 0;
			%this.chatMessage("\c6Your request amount was way too high, so we emptied your wallet into your bank.");
		}
		else
		{
			%this.addBankMoney(%amtMoney);
			%this.addMoney(-%amtMoney);
		}
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

function GameConnection::parseTriggerData_ATM(%this, %message)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%amt = mFloor(%message);
	%accMoney = %this.CityData["Bank"];
	if(%message $= "ENTER")
	{
		%msg = "\c2$" @ %accMoney;
		if(%accMoney <= 0)
			%msg = "\c0no money";

		%msg1 = " \c31 \c7- \c6Withdraw";
		if(%accMoney <= 0)
		{
			%msg1 = " X \c7- Withdraw";
			%noOptions = 1;
		}

		%optionsMsg = "Please select an option.";
		if(%noOptions)
			%optionsMsg = "There are no options that you can do here.";

		%this.chatMessage("\c6Welcome to the bank. You currently have " @ %msg @ " \c6in the bank. " @ %optionsMsg);
		%this.chatMessage(%msg1);
		if(%noOptions)
			%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}
	else if(%message $= "LEAVE")
	{
		%this.CityData["Trigger_Mode"] = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(%amt == 1 && %this.CityData["Trigger_Mode"] $= "")
	{
		%this.CityData["Trigger_Mode"] = %amt;
		switch(%amt)
		{
			case 1:
				%this.chatMessage("\c6Please say the current amount you want to withdraw from your account.");

			default:
				%this.chatMessage("You broke something.");
		}
	}
	else if(%this.CityData["Trigger_Mode"] == 1)
	{
		if(%amt > %this.CityData["Bank"])
		{
			%amt = %this.CityData["Bank"];
			%this.addMoney(%this.CityData["Bank"]);
			%this.CityData["Bank"] = 0;
			%this.chatMessage("\c6Your request amount was way too high, so we emptied your account into your wallet.");
		}
		else
		{
			%this.addBankMoney(-%amt);
			%this.addMoney(%amt);
		}
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

function GameConnection::addPaycheckMoney(%this, %amt)
{
	%this.CityData["ExtraPaycheck"] += %amt;
	if(%this.CityData["ExtraPaycheck"] > $City::Bank::MaxClientCashAmount)
		%this.CityData["ExtraPaycheck"] = $City::Bank::MaxClientCashAmount;
	%this.CityData["ExtraPaycheck"] = mFloatLength(%this.CityData["ExtraPaycheck"], 2);
}

function GameConnection::addMoney(%this, %amt)
{
	%this.CityData["Money"] = mClampF(mFloatLength(%this.CityData["Money"] += %amt, 2), 0, $City::Bank::MaxClientCashAmount);

	if(%amt < 0)
		%this.tempCityData["MoneyBlinkColor"] = "\c0";
	else
		%this.tempCityData["MoneyBlinkColor"] = "\c4";

	%this.City_Update();
}

function GameConnection::addBankMoney(%this, %amt)
{
	%this.CityData["Bank"] += %amt;
	if(%this.CityData["Bank"] > $City::Bank::MaxClientAmount)
		%this.CityData["Bank"] = $City::Bank::MaxClientAmount;
	%this.CityData["Bank"] = mFloatLength(%this.CityData["Bank"], 2);
	//%this.City_Update(); //This isn't on the print
}

function serverCmdDrop(%this, %amt)
{
	if(!isCityObject(%this))
		return;

	if(!isObject(%player = %this.player))
		return;

	if($Sim::Time - %this.lastMoneyDrop < 5)
	{
		%this.chatMessage("You are using the command too fast.");
		return;
	}

	%amt = mFloor(%amt);

	if(%amt <= 0)
		return;

	if(%amt >= $City::Money::MaxCashDrop)
		%amt = $City::Money::MaxCashDrop;

	if(%amt > %this.CityData["Money"])
	{
		%amt = %this.CityData["Money"];
		%this.CityData["Money"] = 0;
		if(%amt <= 0)
			return;
		%this.chatMessage("\c6You emptied your wallet.");
		%this.City_Update();
	}
	else
	{
		%this.addMoney(-%amt);
		%this.chatMessage("\c6You threw \c3$" @ %amt @ "\c6 on the ground.");
	}

	%this.lastMoneyDrop = $Sim::Time;

	%cash = new Item()
	{
		datablock = $City::Money::CashItem;
		canPickup = false;
		value = %amt;
		dropTime = $Sim::Time;
		scale = $City::Money::Scale;
	};
	%cash.setTransform(%player.getEyePoint() SPC %player.rotation);

	%cash.setVelocity(VectorScale(%player.getEyeVector(), 12));
			
	MissionCleanup.add(%cash);
	%cash.setShapeName("$" @ %cash.value);
	%cash.setShapeNameDistance(15);

	%this.City_Update();
}

function City_spawnCash(%loc, %val)
{
	if(%val <= 0)
		return;

	%cash = new Item()
	{
		datablock = $City::Money::CashItem;
		canPickup = false;
		value = %val;
		dropTime = $Sim::Time;
		scale = $City::Money::Scale;
		position = %loc;
	};
	MissionCleanup.add(%cash);

	%cash.setVelocity("0 0 0");
	%cash.setShapeName("$" @ %cash.value);
	%cash.setShapeNameDistance(12);
	%cash.schedulePop();
}

City_Debug("File > Bank.cs", "   -> Loading complete.");