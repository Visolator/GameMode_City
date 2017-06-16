City_Debug("File > CitySO.cs", "Loading assets..");

if(!isObject(City))
	new ScriptObject(City)
	{
		class = CitySO;
		memberCount = 0;
		date = 1;
	};

function City_getEconomyPrice()
{
	if(!$City::Economy::Enabled)
		return 1;
	
	%econMax = $City::Economy::Max - $City::Economy::Start;
	%econ = $City::Economy::Current;
	%price = mFloatLength(mClampF(1 / %econMax * %econ, 0.1, 2), 2);
	%price = 2 - %price;
	return %price;
}

function City_AddEconomy(%amt)
{
	if(!$City::Economy::Enabled)
	{

		return;
	}

	$City::Economy::Current += %amt;
	$City::Economy::Current = mFloatLength(mClampF($City::Economy::Current, 0, $City::Economy::Max), 2);
	%econPer = mFloatLength(100 / $City::Economy::Max * $City::Economy::Current, 2);

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);

		if(!isEventPending(%client.City_UpdateSch))
			%client.City_Update();
	}

	if(%econPer <= 1 && !City.corrupted)
	{
		City.corrupt_days = 60;
		City.corrupted = 1;
		announce("WARNING: You have 60 days to pay back to the economy (2% required to fix), or else all ONLINE/OFFLINE PROFILES will be RESET!");
	}
	else if(%econPer >= 2 && City.corrupted)
	{
		City.corrupted = 0;
		announce("City is no longer corrupted. Profiles will not be reset until city is corrupted again.");
	}
}

function resetAllProfiles()
{
	if(!City.corrupted)
	{
		announce("The city isn't even corrupted, go away.");
		return;
	}

	if(City.corrupt_days > 0)
	{
		announce("The city isn't fully corrupted, go away.");
		return;
	}

	for(%i=0;%i<ClientGroup.getCount();%i++)
		ClientGroup.getObject(%i).City_NewProfile();

	%path = $City::Profiles;
	if(strPos(%path,"*") <= 0)
		%path = %path @ "*";

	%filePath = filePath(%path);

	for(%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path))
	{
		%fileExt = fileExt(%file);
		if(%fileExt !$= ".CityBLP")
			continue;

		fileDelete(%file);
	}

	$City::Economy::Max = 100000000; //We need a max, which is $100,000,000 - This in 32-bit unsigned integer
	$City::Economy::Start = mFloor($City::Economy::Max * 0.6); //Start with $100,000,000
	$City::Economy::Current = $City::Economy::Start;

	for(%i=0;%i<ClientGroup.getCount();%i++)
		ClientGroup.getObject(%i).City_Update();
}

//Donate to the economy
function serverCmdDonate(%this, %amt)
{
	if(!isCityObject(%this))
		return;

	if(!isObject(%player = %this.player))
		return;

	if($Sim::Time - %this.lastDonate < 15 && !%this.isSuperAdmin)
	{
		%this.chatMessage("You are using the command too fast.");
		return;
	}

	%amt = mFloor(%amt);

	if(%amt <= 0)
		return;

	%currEcon = $City::Economy::Current;

	if(%amt > %this.CityData["Money"])
	{
		%amt = %this.CityData["Money"];
		%this.addMoney(-%amt);
		if(%amt <= 0)
			return;
		City_AddEconomy(%amt);
		%newEcon = $City::Economy::Current;
		%econPerDif = mFloatLength(100 / $City::Economy::Max * mFloatLength(%newEcon, 2) - 
			100 / $City::Economy::Max * mFloatLength(%currEcon, 2), 4);
		
		if(%econPerDif > 0) %econPerDifPrint = "\c2+";
		else %econPerDifPrint = "\c0";
		announce("\c4" @ %this.getPlayerName() @ "\c6 has emptied their wallet to the economy! (" @ %econPerDifPrint @ %econPerDif @ "\c7%\c6) (" @ %econPerDifPrint @ "$" @ %amt @ "\c6)");
	}
	else
	{
		%this.addMoney(-%amt);
		City_AddEconomy(%amt);
		%newEcon = $City::Economy::Current;
		%econPerDif = mFloatLength(100 / $City::Economy::Max * mFloatLength(%newEcon, 2) - 
			100 / $City::Economy::Max * mFloatLength(%currEcon, 2), 4);

		if(%econPerDif >= 0) %econPerDifPrint = "\c2+";
		else %econPerDifPrint = "\c0";
		announce("\c4" @ %this.getPlayerName() @ "\c6 has donated money to the economy! (" @ %econPerDifPrint @ %econPerDif @ "\c7%\c6) (" @ %econPerDifPrint @ "$" @ %amt @ "\c6)");
	}
	%this.lastDonate = $Sim::Time;
	if(!isEventPending(%this.City_UpdateSch))
		%this.City_Update();
}

$City::TickSpeed = 60 * 5;

function CitySO::isMember(%this, %obj)
{
	if(!isObject(%obj)) return -1;
	if(%obj.getClassName() !$= "GameConnection") return false;
	if(%this.memberCount < 0) %this.memberCount = 0;
	for(%i=0;%i<%this.memberCount;%i++)
	{
		if(isObject(%this.member[%i]))
			if(%this.member[%i] == %obj)
				return true;
	}
	return false;
}

function CitySO::addMember(%this, %obj)
{
	if(!isObject(%obj)) return -1;
	if(%obj.getClassName() !$= "GameConnection") return false;
	if(%this.isMember(%obj)) return;
	if(%this.memberCount < 0) %this.memberCount = 0;
	for(%i=0;%i<%this.memberCount + 1;%i++)
	{
		if(!isObject(%this.member[%i]))
		{
			%this.member[%i] = %obj;
			%this.memberCount++;
			return true;
		}
	}
	return false;
}

function CitySO::removeMember(%this, %obj)
{
	if(!isObject(%obj)) return -1;
	if(%obj.getClassName() !$= "GameConnection") return false;
	if(!%this.isMember(%obj)) return;
	if(%this.memberCount < 0) %this.memberCount = 0;
	for(%i=0;%i<%this.memberCount;%i++)
	{
		if(isObject(%this.member[%i]))
			if(%this.member[%i] == %obj)
			{
				%this.member[%i] = 0;
				%this.memberCount--;
				return true;
			}
	}
	return false;
}

function CitySO::LogPublicChat(%this, %client, %message, %isLocal)
{
	if(!isObject(%this.chatLog))
		%this.chatLog = new FileObject(CitySO_ChatLog);

	if(!isObject(%this.chatLog_noIP))
		%this.chatLog_noIP = new FileObject(CitySO_ChatLogNoIP);
	
	if(%isLocal)
		%localMessage = "[LOC] ";

	%file_0 = %this.chatLog;
	%file_0.openForAppend("config/server/CityLogs/City_ChatLogs.txt");
	%file_0.writeLine(%localMessage @ "[" @ getWord(getDateTime(), 0) @ ", " @ City.getRealTime() @ "] JOB: " @ %client.getJob().uiName @ " | " @ %client.getPlayerName() @ " (" @ %client.getBLID() @ ", " @ %client.getRawIP() @ "): " @ %message);
	%file_0.close();

	%file_1 = %this.chatLog_noIP;
	%file_1.openForAppend("config/server/CityLogs/City_ChatLogs_NoIP.txt");
	%file_1.writeLine(%localMessage @ "[" @ getWord(getDateTime(), 0) @ ", " @ City.getRealTime() @ "] JOB: " @ %client.getJob().uiName @ " | " @ %client.getPlayerName() @ " (" @ %client.getBLID() @ "): " @ %message);
	%file_1.close();

	echo(%localMessage @ "(" @ %client.getJob().uiName @ ") " @ %client.getPlayerName() @ " (BL_ID: " @ %client.getBLID() @ "): " @ %message);
}

function CitySO::LogTeamChat(%this, %client, %message, %isLocal)
{
	if(!isObject(%this.chatLogTeam))
		%this.chatLogTeam = new FileObject(CitySO_ChatLogTeam);

	if(!isObject(%this.chatLog_noIPTeam))
		%this.chatLog_noIPTeam = new FileObject(CitySO_ChatLogNoIPTeam);

	if(%isLocal)
		%localMessage = "[LOC] ";
	
	%file_0 = %this.chatLogTeam;
	%file_0.openForAppend("config/server/CityLogs/City_ChatLogs_" @ %client.getJob().getName() @ ".txt");
	%file_0.writeLine(%localMessage @ "[" @ getWord(getDateTime(), 0) @ ", " @ City.getRealTime() @ "] JOB: " @ %client.getJob().uiName @ " | " @ %client.getPlayerName() @ " (" @ %client.getBLID() @ ", " @ %client.getRawIP() @ "): " @ %message);
	%file_0.close();

	%file_1 = %this.chatLog_noIPTeam;
	%file_1.openForAppend("config/server/CityLogs/City_ChatLogs_" @ %client.getJob().getName() @ "_NoIP.txt");
	%file_1.writeLine(%localMessage @ "[" @ getWord(getDateTime(), 0) @ ", " @ City.getRealTime() @ "] JOB: " @ %client.getJob().uiName @ " | " @ %client.getPlayerName() @ " (" @ %client.getBLID() @ "): " @ %message);
	%file_1.close();

	echo(%localMessage @ "[TEAM] (" @ %client.getJob().uiName @ ") " @ %client.getPlayerName() @ " (BL_ID: " @ %client.getBLID() @ "): " @ %message);
}

function CitySO::Log(%this, %message)
{
	%message = trim(stripMLControlChars(%message));
	if(!isObject(%this.log))
		%this.log = new FileObject(CitySO_Log);
	
	%file = %this.log;
	%file.openForAppend("config/server/CityLogs/City_Server.txt");
	%file.writeLine("[" @ getDateTime() @ "] " @ %message);
	%file.close();

	echo(%message);
}

//Tick
function CitySO::Start(%this)
{
	//announce("City has started a tick loop.");
	%this.TickLoop();
}

function CitySO::PassDay(%this)
{
	%this.time = $City::TickSpeed;
	%this.TickLoop();
}

function CitySO::TickLoop(%this)
{
	cancel(%this.tickSch);
	%this.time++;
	if(%this.time > $City::TickSpeed)
	{
		if(%this.corrupted)
			%this.corrupt_days--;
		%this.time = 0;
		%this.date++;
		%this.getDateMessage();
		if($EnvGuiServer::DayCycleEnabled)
		{
			setEnvironment("DayLength", $City::TickSpeed);
			setEnvironment("DayOffset", 0.5);
			schedule(0, setEnvironment, "DayOffset", 0.1);
		}

		//Do client stuff here.
		for(%i=0;%i<ClientGroup.getCount();%i++)
		{
			%client = ClientGroup.getObject(%i);
			%job = %client.getJob();

			%paycheck = 0;
			%doNotPay = 0;
			%extraPay = 0;
			%totalPay = 0;
			%jTime = 0;
			%extraPayMsg = "";
			%reason = "Unknown";

			if(%client.City_Spawned)
			{
				if(!isObject(%player = %client.player) || %player.getState() $= "dead")
					if($Sim::Time - %client.lastDead > 15)
					{
						%doNotPay = 1;
						%reason = "Attempting to stay dead to recieve money";
					}

				if(%client.CityData["Mode"] $= "Modification")
				{
					%doNotPay = 1;
					%reason = "Modification Mode";
				}

				if(!%doNotPay)
				{
					if(%client.isInJail())
					{
						%client.CityData["JailTime"] -= 1;
						%jTime = mFloor(%client.CityData["JailTime"]);
						if(%jTime <= 0)
						{
							%client.CityData["JailTime"] = 0;
							%client.chatMessage(" \c6- \c4You have been released from jail.");
							Jail.removeMember(%client);
							%client.instantRespawn();
						}
						else if(%jTime == 1)
							%client.chatMessage(" \c6- \c0Last day in prison.");
						else
							%client.chatMessage(" \c6- \c0You have " @ %jTime @ " days left in jail.");
					}
					else if(%client.isInCS())
					{
						%client.CityData["CommunityService"] -= 1;
						%jTime = mFloor(%client.CityData["CommunityService"]);
						if(%jTime <= 0)
						{
							%client.CityData["CommunityService"] = 0;
							%client.chatMessage(" \c6- \c4You have been released from community service.");
							CommunityService.removeMember(%client);
						}
						else if(%jTime == 1)
							%client.chatMessage(" \c6- \c0Last day of community service.");
						else
							%client.chatMessage(" \c6- \c0You have " @ %jTime @ " days left in community service.");
					}
					else if(isObject(%job))
					{
						%paycheck = %job.paycheck;
						%extraPay = %client.CityData["ExtraPayCheck"];
						%totalPay = %paycheck + %extraPay;

						if(%client.CityData["Demerits"] > $City::Jail::MinDemerits)
						{
							%client.addDemerits(-$City::Jail::ReduceDemeritsPerDay);
							%client.chatMessage(" \c6 - You have had your demerits reduced to \c3" @ %client.CityData["Demerits"]
								@ "\c6 due to <a:en.wikipedia.org/wiki/Statute_of_limitations>Statute of Limitations</a>\c6.");
							%client.chatMessage(" \c6 - \c5Sorry, you don't get your paycheck because you are wanted.");
						}
						else
						{
							if(%client.CityData["Demerits"] > 0)
							{
								%client.addDemerits(-$City::Jail::ReduceDemeritsPerDay);
								%client.chatMessage(" \c6 - You have had your demerits reduced to \c3" @ %client.CityData["Demerits"]
									@ "\c6 due to <a:en.wikipedia.org/wiki/Statute_of_limitations>Statute of Limitations</a>\c6.");
							}

							if(%job.paycheck > 0)
							{
								if(%extraPay > 0)
									%extraPayMsg = " \c6(\c2+$" @ %extraPay @ "\c6)";
								%client.chatMessage(" \c6 - \c6You have received your paycheck of \c2$" @ %job.paycheck @ %extraPayMsg @ "\c6.");
								%client.addBankMoney(%totalPay);
								City_AddEconomy(-%totalPay);
								%client.CityData["ExtraPayCheck"] = 0;
							}

							if(%this.corrupted)
							{
								if(%this.corrupt_days <= 0)
								{
									%client.chatMessage(" \c6 - \c0YOU'RE PROFILE HAS BEEN RESET BECAUSE THE ECONOMY WAS TOO LOW TO PAY EVERYONE.");
								}
								else if(%this.corrupt_days == 1)
									%client.chatMessage(" \c6 - \c6This is your last day to pay to the economy, or else your profile will be reset.");
								else
									%client.chatMessage(" \c6 - \c6You have \c5" @ %this.corrupt_days @ " \c6more day(s) to pay to the economy, or else your profile will be reset.");
							}
						}
					}
					else
						%client.chatMessage(" \c6 - \c5Sorry, it looks like you missed out on your paycheck. Your job is too poor to give you any money.");
				}
				else
					%client.chatMessage(" \c6 - \c5Sorry, it looks like you missed out on your paycheck. Reason: \c4" @ %reason);

				%client.City_SaveProfile();
			}
			else
				%client.chatMessage(" \c6 - \c5Sorry, it looks like you missed out on your paycheck. You have to spawn first.");
		}
	}

	if(%this.corrupted && %this.corrupt_days <= 0)
		resetAllProfiles();

	for(%i=0;%i<ClientGroup.getCount();%i++)
	{
		%client = ClientGroup.getObject(%i);
		if(!isEventPending(%client.City_UpdateSch)) //If their ui is getting updated anyways what is the point.
		{
			%client.addHunger(-$City::Survival::HungerDepletePerTick);
			%client.addThirst(-$City::Survival::ThirstDepletePerTick);
			%client.City_Update();
			%client.addBuildEduTime(-(24 / $City::TickSpeed)*$City::BuildEDUMult);
			%client.addJobEduTime(-(24 / $City::TickSpeed)*$City::BuildJobMult);
		}
	}
	%this.tickSch = %this.schedule(1000, TickLoop);
}

$City::BuildEDUMult = 5;
$City::BuildJobMult = 5;

function CitySO::getTime(%this)
{
	%time = mFloor(24 / $City::TickSpeed * %this.time);

	%s = "AM";
	%timeS = %time;
	if(%timeS == 0)
		%timeS = 1;
	
	if(%time >= 12)
	{
		%timeS = %time - 12;
		%s = "PM";
		if(%timeS == 12)
		{
			%timeS = 12;
			%s = "AM";
		}

		if(%timeS == 0)
			%timeS = 12;
	}
	
	%timeS = %timeS @ ":00 " @ %s;
	return %timeS;
}

function CitySO::loadCalendar(%this)
{
	// Counters
	%this.numOfMonths = 12;
	%this.zbNumMonths = %this.numOfMonths - 1;
	
	// Names
	%this.nameOfMonth[0] = "January";
	%this.nameOfMonth[1] = "February";
	%this.nameOfMonth[2] = "March";
	%this.nameOfMonth[3] = "April";
	%this.nameOfMonth[4] = "May";
	%this.nameOfMonth[5] = "June";
	%this.nameOfMonth[6] = "July";
	%this.nameOfMonth[7] = "August";
	%this.nameOfMonth[8] = "September";
	%this.nameOfMonth[9] = "October";
	%this.nameOfMonth[10] = "November";
	%this.nameOfMonth[11] = "December";
	
	// Days
	%this.daysInMonth[0] = 31;
	%this.daysInMonth[1] = 28;
	%this.daysInMonth[2] = 31;
	%this.daysInMonth[3] = 30;
	%this.daysInMonth[4] = 31;
	%this.daysInMonth[5] = 30;
	%this.daysInMonth[6] = 31;
	%this.daysInMonth[7] = 31;
	%this.daysInMonth[8] = 30;
	%this.daysInMonth[9] = 31;
	%this.daysInMonth[10] = 30;
	%this.daysInMonth[11] = 31;
	
	// Holidays
	%this.holiday[1] = "\c2Happy New Year!";
	%this.holiday[91] = "\c2A\c1p\c2r\c1i\c2l \c0F\c3o\c0o\c3l\c0s \c7D\c6a\c7y\c6!";
	%this.holiday[350] = "\c0H\c3a\c2p\c1p\c5y\c6 Holidays\c7!";
}

function CitySO::getDate(%this)
{
	%ticks = %this.date;
	if(%ticks < 0)
		%ticks = 0;
	
	for(%a = 0; %ticks > %this.daysInMonth[%a % %this.numOfMonths]; %a++)
		%ticks -= %this.daysInMonth[%a % %this.numOfMonths];
	
	%year = mFloor(%a / %this.numOfMonths);
	
	// If the second number from last is a "1" (e.g. 12 or 516), the suffix will always be "th"
	if(strlen(%ticks) > 1 && getSubStr(%ticks, (strlen(%ticks) - 2), 1) $= "1")
	{
		%suffix = "th";
	}
	// If not, it can either be "st," "nd," "rd," or "th," depending on the last numeral.
	else
	{
		switch(getSubStr(%ticks, (strlen(%ticks) - 1), 1))
		{
			case 1: %suffix = "st";
			case 2: %suffix = "nd";
			case 3: %suffix = "rd";
			default: %suffix = "th";
		}
	}

	return %this.nameOfMonth[%this.getMonth()] SPC %ticks @ %suffix;
}

function CitySO::getRealTime(%this)
{
	%time = mFloatLength(getSubStr(getWord(getDateTime(), 1), 0, 2), 0);
	%s = "AM";
	%timeS = %time;
	if(%timeS == 0)
		%timeS = 1;
	if(%time > 12)
	{
		%timeS = %time - 12;
		%s = "PM";
		if(%timeS == 12)
		{
			%timeS = 12;
			%s = "AM";
		}
	}
	%timeS = %timeS @ getSubStr(getWord(getDateTime(), 1), 2, 3) @ %s;
	return %timeS;
}

function CitySO::getDateMessage(%this)
{
	%ticks = %this.date;
	if(%this.resetBricks > 7)
	{
		reapTempBricks();
		%this.resetBricks = 0;
	}
	else
		%this.resetBricks = 0;
		
	if(%ticks < 0)
		%ticks = 0;
	
	for(%a = 0; %ticks > %this.daysInMonth[%a % %this.numOfMonths]; %a++)
		%ticks -= %this.daysInMonth[%a % %this.numOfMonths];
	
	%year = mFloor(%a / %this.numOfMonths) + 50;
	
	// If the second number from last is a "1" (e.g. 12 or 516), the suffix will always be "th"
	if(strlen(%ticks) > 1 && getSubStr(%ticks, (strlen(%ticks) - 2), 1) $= "1")
	{
		%suffix = "th";
	}
	// If not, it can either be "st," "nd," "rd," or "th," depending on the last numeral.
	else
	{
		switch(getSubStr(%ticks, (strlen(%ticks) - 1), 1))
		{
			case 1: %suffix = "st";
			case 2: %suffix = "nd";
			case 3: %suffix = "rd";
			default: %suffix = "th";
		}
	}
	
	messageAll('', '\c6Good morning! It is now of the \c4%2\c6%4 of \c4%1\c6, \c4%3 \c6years...', %this.nameOfMonth[%this.getMonth()], %ticks, %year, %suffix);
	
	if(%this.holiday[%this.getCurrentDay()] !$= "")
		messageAll('', "\c6 -" SPC %this.holiday[%this.getCurrentDay()]);
}

function CitySO::getMonth(%this)
{
	%ticks = %this.date;
	
	for(%a = 0; %ticks > %this.daysInMonth[%a % %this.numOfMonths]; %a++)
		%ticks -= %this.daysInMonth[%a % %this.numOfMonths];
	
	%month = %a % %this.numOfMonths;
	return %month;
}

function CitySO::dumpCalendar(%this)
{
	for(%a = 0; %this.daysInMonth[%a] !$= ""; %a++)
	{
		echo(%this.nameOfMonth[%a] @ " has " @ %this.daysInMonth[%a] @ " day(s).");
	}
}

function CitySO::getYearLength(%this)
{
	for(%a = 0; %this.daysInMonth[%a] > 0; %a++)
	{
		%totalLength += %this.daysInMonth[%a];
	}
	
	return %totalLength;
}

function CitySO::getCurrentDay(%this)
{
	return (%this.date % %this.getYearLength());
}

function CitySO::loadData(%this)
{
	if(isFile("config/server/City/Calendar.cs"))
	{
		exec("config/server/City/Calendar.cs");
		%this.date = $City::Date;
	}
	else
		%this.date = 0;
	
	%this.loadCalendar();
}

function CalendarSO::saveData(%this)
{
	$City::Date1	= %this.date;
	export("$City::Calendar*", "config/server/CityRPG/CityRPG/Calendar.cs");
}

//End of tick

City.schedule(1000, loadCalendar);
City.schedule(5000, Start);

City_Debug("File > CitySO.cs", "   -> Loading complete.");