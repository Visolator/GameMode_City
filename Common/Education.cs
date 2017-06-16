City_Debug("File > Education.cs", "Loading assets..");

datablock fxDTSBrickData(EducationCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Education Brick";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "Education";
	triggerSize = "2 4 1";
	trigger = 0;
};

function GameConnection::parseTriggerData_Education(%this, %message)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%amt = mFloor(%message);
	%accMoney = %this.CityData["Bank"];
	%curMoney = %this.CityData["Money"];
	if(%message $= "ENTER")
	{
		%this.chatMessage("\c6Welcome to the Education Department, how may we help you?");
		if($City::Lots::EnableBuildLicense)
		{
			if(%this.CityData["BuildingLicense"] <= 0)
			{
				if(%curMoney >= $City::License::Builder)
				{
					%list = %list SPC "1";
					%this.chatMessage(" \c31 \c6- \c6Get a builder's license");
				}
				else
					%this.chatMessage(" X \c7- \c6You do not have enough cash to get your building license \c6(Need \c2$" @ mFloatLength($City::License::Builder - %curMoney, 2) @ " \c6more)");
			}
			else if(%this.CityData["BuildingLicense"] < 3)
			{
				%cost = $City::License::Builder * (%this.CityData["BuildingLicense"] * 1.6);
				if(%curMoney >= %cost)
				{
					%list = %list SPC "1";
					%this.chatMessage(" \c31 \c7- \c6Upgrade my builder's license");
				}
				else
					%this.chatMessage(" X \c7- \c6You do not have enough cash to upgrade your building license \c6(Need \c2$" @ mFloatLength(%cost - %curMoney, 2) @ " \c6more)");
			}
			else
				%this.chatMessage(" X \c7- \c6You cannot upgrade your license");
		}
		else
			%this.chatMessage(" X \c7- \c6Building licenses are disabled");


		%list = %list @ " 2";
		%this.chatMessage(" \c32 \c7- \c6Select a job tree to educate from \c7- \c5Make sure you have cash on you");

		%list = trim(%list);
		%list = strReplace(%list, " ", ",");
		%this.CityData["Trigger_CanChoose"] = %list;
	}
	else if(%message $= "LEAVE")
	{
		%this.TempData["JobTreeSelector"] = "";
		%this.TempData["JobTree"] = "";
		%this.TempData["JobTreeSelect"] = "";
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
				%this.attemptLicenseUpgrade_Build();

			case 2:
				%this.chatMessage("\c6Please select a job tree to educate to.");
				%this.TempData["JobTree"] = "";
				for(%i=1;%i<getFieldCount($JobTrees)+1;%i++)
				{
					%jobList = %jobList SPC %i;
					%jobListField = %jobListField TAB getField($JobTrees, %i-1);
					%this.chatMessage(" \c3" @ %i @ " \c7- \c6" @ getField($JobTrees, %i-1));
				}
				%jobList = trim(%jobList);
				%jobListField = trim(%jobListField);

				%this.TempData["JobTreeSelect"] = %jobList;
				%this.TempData["JobTreeSelector"] = %jobListField;

			default:
				%this.chatMessage("Seems you somehow got here. How are you?");
		}
	}
	else if(%this.CityData["Trigger_Mode"] == 2 && containsSubstring(%this.TempData["JobTreeSelect"], %amt))
	{
		%this.TempData["JobTree"] = %amt-1;
		%this.attemptJobUpgrade_Build();
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

function GameConnection::attemptJobUpgrade_Build(%this)
{
	if(%this.CityData["Upgrade_JobWaiting"])
	{
		%this.TempData["ConfirmUpgrade_Job"] = "";
		%this.TempData["ConfirmUpgrade_JobCost"] = "";
		%this.TempData["ConfirmUpgrade_JobField"] = "";
		return;
	}

	if(%this.TempData["JobTreeSelector"] $= "")
		return;

	%job = getField(%this.TempData["JobTreeSelector"], %this.TempData["JobTree"]);
	if(%this.CityData["EDU", %job] < 8) //MaxEDU
	{
		%this.TempData["ConfirmUpgrade_Job"] = 1;
		%this.TempData["ConfirmUpgrade_JobField"] = %job;
		%this.TempData["ConfirmUpgrade_JobCost"] = mFloatLength($City::Education::JobCost + 120 * (mClampF(%this.CityData["EDU", %job], 0, 8) * 1.5), 2);
		commandToClient(%this, 'MessageBoxYesNo', "EDU(" @ mFloor(%this.CityData["EDU", %job]) @ ") - Upgrade : " @ %job, 
			"Are you sure you want to get educated in the \"" @ %job @ "\" tree?<br>Costs $" @ %this.TempData["ConfirmUpgrade_JobCost"] @ ".",
			'ConfirmUpgrade_Job');
	}
	%this.CityData["JobTreeSelector"] = "";
}

function serverCmdConfirmUpgrade_Job(%this)
{
	if(%this.TempData["ConfirmUpgrade_Job"] $= "")
	{
		%this.TempData["ConfirmUpgrade_Job"] = "";
		%this.TempData["ConfirmUpgrade_JobCost"] = "";
		%this.TempData["ConfirmUpgrade_JobField"] = "";
		return;
	}

	%job = %this.TempData["ConfirmUpgrade_JobField"];
	if(%this.CityData["EDU", %job] >= 8)
	{
		%this.TempData["ConfirmUpgrade_Job"] = "";
		%this.TempData["ConfirmUpgrade_JobCost"] = "";
		%this.TempData["ConfirmUpgrade_JobField"] = "";
		%this.chatMessage("Sorry, it seems you are already maxed out in " @ %job @ ".");
		return;
	}

	%accMoney = %this.CityData["Bank"];
	%curMoney = %this.CityData["Money"];
	%cost = %this.TempData["ConfirmUpgrade_JobCost"];
	
	if(%curMoney < %cost)
	{
		%this.TempData["ConfirmUpgrade_Job"] = "";
		%this.chatMessage("\c5Sorry, you do not have enough cash. Please go to the bank and get some cash.");
		return;
	}

	commandToClient(%this, 'MessageBoxOK', "Job - Application [" @ %job @ "]", "You are now applying for an edu application." @ 
		"<br>This is very silent, it can take up to a couple day ticks, or more.");
	%this.addMoney(-%cost);
	City_AddEconomy(%cost);
	%this.addJobEduTime(mClampF(%this.CityData["EDU", %job], 1, 8) * getRandom(10, 25));
	%this.CityData["Upgrade_JobWaiting"] = 1;
	%this.CityData["Upgrade_JobToEDU"] = %job;

	%this.TempData["ConfirmUpgrade_Job"] = "";
	%this.TempData["ConfirmUpgrade_JobCost"] = "";
	%this.TempData["ConfirmUpgrade_JobField"] = "";
}

function serverCmdIApproveJob(%this) //Cheater!
{
	if(!%this.isSuperAdmin) return;
	%this.cheatedJob = 1;
	%this.unlockAchievement("Job Approval!");
	%this.addJobEduTime(-100000);
}

function GameConnection::addJobEduTime(%this, %time)
{
	%this.CityData["Upgrade_JobTime"] += %time;
	if(%this.CityData["Upgrade_JobTime"] < 0)
	{
		%this.CityData["Upgrade_JobTime"] = 0;
		if(%this.CityData["Upgrade_JobWaiting"])
		{
			%this.CityData["Upgrade_JobWaiting"] = 0;
			if(%this.CityData["EDU", %this.CityData["Upgrade_JobToEDU"]] < 8)
			{
				%this.CityData["EDU", %this.CityData["Upgrade_JobToEDU"]]++;
				%this.chatMessage("\c6EDU in " @ %this.CityData["Upgrade_JobToEDU"] @ " is now at " @ %this.CityData["EDU", %this.CityData["Upgrade_JobToEDU"]] @ "!");
				%this.CityData["Upgrade_JobToEDU"] = "";
			}
		}
	}
	else
		%this.CityData["Upgrade_JobWaiting"] = 1;
}

City_Debug("File > Education.cs", "   -> Loading complete.");