City_Debug("File > JobSO.cs", "Loading assets..");

datablock fxDTSBrickData(JobCityBrick : brick2x4FData)
{
	category = "City";
	subCategory = "Info";
	
	uiName = "Job Brick";
	
	CityBrickType = "InfoTrigger";
	requiresAdmin = 1;
	
	triggerDatablock = City_InputTriggerData;
	triggerFunction = "Job";
	triggerSize = "2 4 1";
	trigger = 0;

	hasMenu = 1;
	manuUiName = "Job menu";
};

function GameConnection::parseMenuData_Job(%this, %message, %dir)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%menuMode = %this.CityData["SelectMenuMode"];
	%amt = mFloor(%message);
	%accMoney = %this.CityData["Bank"];
	if(%message $= "ENTER")
	{
		for(%i = 0; %i < getFieldCount($JobTrees); %i++)
		{
			%list = %list SPC %i;
			%strList = %strList TAB getField($JobTrees, %i);
		}

		%this.CityData["MenuList"] = trim(%strList);
		%this.CityData["MenuListCount"] = 4;
	}
	else if(%message $= "LEAVE")
	{
		%this.tempJob = "";
		%this.currTree = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(%message $= "CHECK")
	{
		if(%menuMode == 0)
		{
			%this.CityData["MenuName"] = "Job menu";
			for(%i = 0; %i < getFieldCount($JobTrees); %i++)
			{
				%list = %list SPC %i;
				%strList = %strList TAB getField($JobTrees, %i);
			}

			%this.CityData["MenuList"] = trim(%strList);
		}
		else if(%menuMode == 1)
		{
			%this.CityData["JobSelection"] = "";
			if(%dir $= "NEXT")
			{
				%select = getField($JobTrees, %this.CityData["SelectMode"]);
				%this.TempCityData["LastJobTree"] = %select;
			}
			else
				%select = %this.TempCityData["LastJobTree"];

			%jobs = JobGroup.getJobFieldList(%select);
			%jobCanChoose = 0;
			for(%i=0;%i<getFieldCount(%jobs);%i++)
			{
				%job = getField(%jobs, %i);
				%jobObj = JobGroup.findScript(%job);
				%eduDiff = %jobObj.eduRequired - %this.CityData["EDU_" @ %jobObj.getTreeParsed()];
				%expDiff = %jobObj.eduRequired - %this.CityData["EXP_" @ %jobObj.getTreeParsed()];

				if(%eduDiff <= 2)
					%jobCanChoose++;
			}

			%this.CityData["MenuName"] = %select @ " \c6(\c4" @ %jobCanChoose @ "\c6/\c4" @ getFieldCount(%jobs) @ " job" @ (getFieldCount(%jobs) != 1 ? "s" : "") @ "\c6)";
			for(%i=0;%i<getFieldCount(%jobs);%i++)
			{
				%job = getField(%jobs, %i);
				%jobObj = JobGroup.findScript(%job);

				%eduDiff = %jobObj.eduRequired - %this.CityData["EDU_" @ %jobObj.getTreeParsed()];
				%expDiff = %jobObj.expRequired - %this.CityData["EXP_" @ %jobObj.getTreeParsed()];

				if(%this.canBeAnyJob)
					%eduDiff = 0;

				if(%eduDiff <= 2)
				{
					if((%jobObj.canArrest && %this.CityData["TotalJailTime"] > 0) || %eduDiff > 0 || %expDiff > 0)
						%col = "\c0";

					if(%jobObj == %this.getJob())
						%strList = %strList TAB "\c3" @ %jobObj.uiName;
					else
						%strList = %strList TAB %col @ %jobObj.uiName;
				}
			}

			%this.CityData["MenuList"] = trim(%strList);
		}
		else if(%menuMode == 2)
		{
			%select = %this.CityData["JobSelection"];
			if(%this.CityData["JobSelection"] $= "")
				%select = %this.CityData["SelectModeName"];

			%this.CityData["JobSelection"] = %select;
			%this.CityData["MenuName"] = %select;
			%jobObj = JobGroup.findScript(%select);

			%eduDiff = %jobObj.eduRequired - %this.CityData["EDU_" @ %jobObj.getTreeParsed()];
			%expDiff = %jobObj.expRequired - %this.CityData["EXP_" @ %jobObj.getTreeParsed()];

			if(%jobObj.canArrest && %this.CityData["TotalJailTime"] > 0)
			{
				%badGuy = "\c0You do not have a clean record\t";
				%cannotApply = 1;
			}

			if(%eduDiff > 0)
			{
				%noEdu = "\c3-> Not enough EDU\t";
				%cannotApply = 1;
			}

			if(%expDiff > 0)
			{
				%noExp = "\c3-> Not enough EDU\t";
				%cannotApply = 1;
			}

			if(%this.canBeAnyJob)
				%cannotApply = 0;

			if(%jobObj.paycheck > 0)
				%pay = "Paycheck: \c2$" @ %jobObj.paycheck;
			else
				%pay = "\c2No paycheck";

			%msg0 = (%jobObj == %this.getJob() ? "\c3You already have this job\t" : (%cannotApply ? "\c0You cannot apply for this job\t" : "\c2You can apply for this job\t")) @ %badGuy @ %noEdu @ %noExp;
			%this.CityData["MenuList"] = %msg0 TAB %pay TAB %jobObj.description TAB (%cannotApply ? "" : "\c3Confirm? \c5->");
		}
		else if(%menuMode == 3)
		{
			%job = JobGroup.findScript(%this.CityData["JobSelection"]);
			if(!isObject(%job))
			{
				%this.chatMessage("\c6Invalid job from tree.");
				%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
				return;
			}

			%eduDiff = %job.eduRequired - %this.CityData["EDU_" @ %job.getTreeParsed()];
			%expDiff = %job.expRequired - %this.CityData["EXP_" @ %job.getTreeParsed()];

			if(%job.canArrest && %this.CityData["TotalJailTime"] > 0)
			{
				%badGuy = "\c0You do not have a clean record\t";
				%cannotApply = 1;
			}

			if(%eduDiff > 0)
			{
				%noEdu = "\c3-> Not enough EDU\t";
				%cannotApply = 1;
			}

			if(%expDiff > 0)
			{
				%noExp = "\c3-> Not enough EDU\t";
				%cannotApply = 1;
			}

			if(%this.canBeAnyJob)
				%cannotApply = 0;

			if(%job == %this.getJob())
			{
				%this.chatMessage("You already have this job!");
				%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
				return;
			}

			if(%cannotApply)
			{
				%this.chatMessage("You cannot be \c3" @ %job.uiName);
				%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
			}
			else
			{
				%this.tempJob = %job.uiName;
				commandToClient(%this, 'MessageBoxYesNo', "Confirm Job - " @ %job.uiName, "Are you sure you want to pick this job?<br>Job: " @ %job.uiName, 'ConfirmJob');
			}
		}
	}
	else
	{
		%this.chatMessage("Invalid option.");
	}
}

function GameConnection::parseTriggerData_Job(%this, %message)
{
	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	if(!isObject(%player = %this.player))
		return;

	%amt = mFloor(%message);
	%accMoney = %this.CityData["Bank"];
	if(%message $= "ENTER")
	{
		%this.chatMessage("\c6Welcome to the Job selector, would you like to apply for?");
		for(%i=1;%i<getFieldCount($JobTrees)+1;%i++)
		{
			%list = %list SPC %i;
			%this.chatMessage(" \c3" @ %i @ " \c7- \c6" @ getField($JobTrees, %i-1));
		}
		%list = trim(%list);
		%list = strReplace(%list, " ", ",");
		%this.CityData["Trigger_CanChoose"] = %list;
	}
	else if(%message $= "LEAVE")
	{
		%this.CityData["Trigger_Choosen"] = "";
		%this.CityData["Trigger_CanChoose"] = "";
		%this.CityData["Trigger_Mode"] = "";
		%this.tempJob = "";
		%this.currTree = "";
		%this.chatMessage("\c6Thank you. Come again.");
	}
	else if(containsSubstring(%this.CityData["Trigger_CanChoose"], %amt) && %this.CityData["Trigger_Mode"] $= "")
	{
		%this.CityData["Trigger_Mode"] = %amt;
		%amt--;
		%jobs = JobGroup.getJobFieldList(getField($JobTrees, %amt)); //TABS
		if(getFieldCount(%jobs) > 0)
		{
			%jobCanChoose = 0;
			for(%i=0;%i<getFieldCount(%jobs);%i++)
			{
				%job = getField(%jobs, %i);
				%jobObj = JobGroup.findScript(%job);
				%eduDiff = %jobObj.eduRequired - %this.CityData["EDU_" @ %jobObj.getTreeParsed()];
				%expDiff = %jobObj.eduRequired - %this.CityData["EXP_" @ %jobObj.getTreeParsed()];

				if(%eduDiff <= 2)
					%jobCanChoose++;
			}

			%this.currTree = getField($JobTrees, %amt);
			%this.chatMessage("\c6Job tree: \c3" @ getField($JobTrees, %amt) @ " \c6(\c4" @ %jobCanChoose @ "\c7/\c4" @ getFieldCount(%jobs) @ " Job(s) Shown\c6)");
			for(%i=0;%i<getFieldCount(%jobs);%i++)
			{
				%job = getField(%jobs, %i);
				%jobObj = JobGroup.findScript(%job);

				%eduDiff = %jobObj.eduRequired - %this.CityData["EDU_" @ %jobObj.getTreeParsed()];
				%expDiff = %jobObj.expRequired - %this.CityData["EXP_" @ %jobObj.getTreeParsed()];

				if(%this.canBeAnyJob)
					%eduDiff = 0;

				if(%eduDiff <= 2)
				{
					if(%eduDiff > 0)
						%eduMsg[%i] = "\c7(\c2Not educated enough - Need " @ %eduDiff @ " more EDU\c7)";

					if(%expDiff > 0)
						%expMsg[%i] = "\c7(\c2Not experienced enough - Need " @ %expDiff @ " more EXP\c7)";

					if(%jobObj.canArrest && %this.CityData["TotalJailTime"] > 0)
						%jobMsg[%i] = "\c7(\c0You need a clean record\c7)";

					%cost[%i] = "\c7(\c5FREE\c7)";
					if(%cost[%i] > 0)
						%cost[%i] = "\c7(\c5$" @ mFloatLength(%jobObj.investment, 2) @ "\c7)";

					if(%jobObj == %this.getJob())
						%curMsg[%i] = "\c7(\c4Current job\c7)";

					%this.chatMessage(" \c6+ \c3" @ %jobObj.uiName SPC trim(%cost[%i] SPC %curMsg[%i] SPC %eduMsg[%i] SPC %expMsg[%i] SPC %jobMsg[%i]));
					%this.chatMessage("    \c7- \c6" @ %jobObj.description);
				}
			}
			if(%jobCanChoose > 5)
				%this.chatMessage("\c6You may need to page up. (PGUP/PGDN Keys)");
		}
	}
	else if(%this.CityData["Trigger_Mode"] !$= "")
	{
		%job = JobGroup.findScriptInTree(%this.currTree, %message);
		if(!isObject(%job))
		{
			%this.chatMessage("\c6Invalid job from tree.");
			%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
			return;
		}
		%this.tempJob = %job.uiName;
		commandToClient(%this, 'MessageBoxYesNo', "Confirm Job - " @ %job.uiName, "Are you sure you want to pick this job?<br>Job: " @ %job.uiName, 'ConfirmJob');
	}
	else
	{
		%this.chatMessage("\c6Invalid command. Goodbye.");
		%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
	}

	//%this.CityData["Trigger"].onLeaveTrigger(%this.player);
}

function serverCmdConfirmJob(%this)
{
	if(%this.tempJob $= "")
		return;

	if(!isObject(%player = %this.player))
		return;

	if(!isObject(%trigger = %this.CityData["Trigger"]))
		return;

	//talk(%this.tempJob);
	if(%this.getJob() == JobGroup.findScript(%this.tempJob))
		%this.chatMessage("\c6Looks like you are already this job.");
	else
		%this.setJob(%this.tempJob);
	%trigger.getDatablock().onLeaveTrigger(%trigger, %player);
}

if(!isObject(JobGroup))
{
	new ScriptGroup(JobGroup)
	{
		class = JobSO;
		filePath = "config/server/City/JobSO/";
	};
	JobGroup.schedule(1000, Load);
}
else
{
	for($i=0;$i<clientgroup.getCount();$i++)
		clientgroup.getObject($i).City_SaveProfile();

	JobGroup.schedule(1000, Load);
}

function JobSO::getJobFieldList(%this, %jobTree)
{
	for(%i=0;%i<%this.getCount();%i++)
	{
		%job = %this.getObject(%i);
		if(strReplace(%jobTree, " ", "_") $= %job.getTreeParsed())
			%list = %list TAB %job.uiName;
	}
	%list = trim(%list);
	return %list;
}

function JobSO::findScriptInTree(%this, %tree, %JobName)
{
	%list = %this.getJobFieldList(%tree);
	if(getFieldCount(%list) <= 0)
		return -1;

	for(%i=0;%i<getFieldCount(%list);%i++)
	{
		%obj = %this.findScript(getField(%list, %i));
		if(%JobName == %obj)
			return %obj;
	}

	for(%i=0;%i<getFieldCount(%list);%i++)
	{
		%obj = %this.findScript(getField(%list, %i));
		if(%obj.uiName $= %JobName)
			return %obj;
	}

	for(%i=0;%i<getFieldCount(%list);%i++)
	{
		%obj = %this.findScript(getField(%list, %i));

		if(striPos(%obj.uiName, %JobName) >= 0)
			return %obj;
	}
	return -1;
}

function JobSO::findScript(%this, %JobName)
{
	if(isObject(%JobName))
		for(%i = 0; %i < %this.getCount(); %i++)
		{
			%obj = %this.getObject(%i);

			if(isObject(%JobName))
			{
				if(nameToID(%JobName) == nameToID(%obj))
					return %obj;
			}
		}

	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%obj = %this.getObject(%i);
		
		if(%obj.uiName $= %JobName)
			return %obj;
	}

	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%obj = %this.getObject(%i);
		
		if(striPos(%obj.uiName, %JobName) >= 0)
			return %obj;
	}
	return -1;
}

/// <summary>
/// Loads the job program.
/// </summary>
function JobSO::Load(%this)
{
	//--PARAMETERS - READ CAREFULLY--\\
	
	//Description: This will display the current job fields it can support.

	//--KEY--\\
	//	//_!	<- Required field, otherwise it will be confused
	//	//->	<- Normal
	//	//-?	<- Can depend on things
	//	-/- 	<- Comment, do not copy that part if you plan to

	//~JOB TYPES~\\
	//-> Not recommended if you pick something other than these types.
	//!-> Criminal -/- Become a criminal and tear the city apart.
	//!-> Civilian -/- Uh.. You're a civilan, lots of jobs in this field
	//!-> Police -/- Stop the criminals.
	//!-> Military -/- Work for the force, stop something worse than a criminal.
	//!-> Bounty Hunter -/- Chance of claiming bounties.
	//!-> Store clerk -/- Sells store items.
	//!-> Cityman -/- Work for the city.

	//~CRIMINAL~\\
	//->	canPickPocket	bool	- Can they pickpocket people?
	//->	canPickLock	bool	- Can they pick lock doors?
	//->	pickLockLevel	intLevel	- 0 beginner, 1 good, 2 very good, 3 best - Doors have different security levels.

	//~CIVILIAN~\\
	//-> 	canPardon	bool	- Can they pardon people from jail?

	//~MINER~\\
	//-> 	canMineResources	bool	- Can they mine?
	//-> 	mineTime	int	- 0-100%, how easy does it take to earn something?

	//~POLICE~\\
	//->	canArrest	bool	- Can they arrest people?
	//->	canBreakEnterance	bool	- Can they break entrances of enemies? If the blockhead is in another house, police will need a warrant.
	//->	minWantedRecord	int	- Tolerance demerit record to apply for the job. Ex: If you have a wanted record but want to be a cop, too bad.

	//~MILITARY~\\
	//->	canKill	bool	- Are they able to accidentally kill innocent people? This has a tolerance built into the system, otherwise they become wanted too.
	//->	armor	int	- Start with armor to protect yourself.

	//~BOUTY HUNTER~\\
	//->	canPlaceBountyOnHead	bool	- Can they place a bounty on any wanted blockhead's head?
	//->	cloakTime	int	- If more than 0, it is enabled. This is in seconds.

	//~STORE CLERK~\\
	//->	canSellFood	bool	- Can they sell food?
	//->	canSellWeapons	bool	- Can they sell weapons?

	//~CITYMAN~\\
	//->	canSellLots	bool	- Can they sell property?
	//->	canBuyLots	bool	- Can they buy property from others? Don't worry, they have to confirm for this to happen.

	//-Items-
	//_!	item[int]	string	- Item name
	//_!	itemCount	int	- Item count

	//~MAIN~\\
	//_!	chatColor	string	- Use <color:RRGGBB> format.
	//_!	paycheck	int	- How much they get on their paycheck.
	//_!	tree	string	- Where does this go to?
	//_!	description	string	- When people see the job available, this is the description shown.
	//_!	eduRequired	int	- Leave it at 0 if they can get the job without edu.
	//_!	expRequired	int	- Leave it at 0 if they can get the job without exp.
	//_!	investment	int	- How much do you need to invest to be able to get this job?
	//->	isTempJob	bool	- Is it a temporarily job?
	//->	jailTolerance	int	- How long does it take for you to get into arrest?
	//->	nextTree	string	- What is the next tree it will unlock to? Put this on all of the jobs that can.
	//->	doNotIncludeDefault	bool	- Does not include default tools.
	//->	requireExpFromTree	int	- Require exp from a different tree name.
	//->	requireEduFromTree	int	- Require education from a different tree name.

	//--YOU MAY USE FUNCTIONS, HERE IS HOW IT GOES--\\
	//	CMD function(args...);

	%this.deleteAll();
	$JobTrees = "";
	%path = $City::FilePath_Jobs @ "*";

	//This job is never removed like the others are able to
	registerJob("Loading", 
		"description This is your ordinary loading job.", 
		"weaponCount 0" TAB
		"paycheck 1");

	//announce("Loading saved files for mob classes. -> Path: " @ %path);
	%file = findFirstFile(%path);
	if(isFile(%file))
	{
		%fileExt = fileExt(%file);
		%name = fileBase(%file);
		if(%fileExt $= ".cs") //Just making sure
		{
			if(isObject(%obj = isRegisteredJob(fileBase(%path))))
				%obj.delete();

			exec(%file);
		}
	}
	else
		return;

	while(%file !$= "")
	{
		%file = findNextFile(%path);
		%fileExt = fileExt(%file);
		%name = fileBase(%file);
		if(%fileExt $= ".cs") //Just making sure
		{
			if(isObject(%obj = isRegisteredJob(fileBase(%path))))
				%obj.delete();

			exec(%file);
		}
	}

	for(%i=0;%i<clientgroup.getCount();%i++)
		clientgroup.getObject(%i).City_LoadProfile();
}

/// <summary>
/// When the job is created, we need to format the variables, so we put it into a command, see -> registerJob
/// Do not use this function.
/// </summary>
/// <param name="this">Name of the created mob.</param>
/// <param name="com">Parameters, each variable must be in a different field.</param>
function Job::onAdd(%this)
{
	JobGroup.add(%this);
	%this.parseCommand(%this.command);
}

function Job::getTreeParsed(%this)
{
	%name = stripChars(%this.tree, $City::Chars);
	return strReplace(%name, " ", "_");
}

function City_parseJob(%job)
{
	%job = stripChars(%job, $City::Chars);
	return strReplace(%job, " ", "_");
}

/// <summary>
/// Parses job objects' commands, see -> Job::onAdd
/// Do not use this function.
/// </summary>
/// <param name="this">Name of the created mob.</param>
/// <param name="com">Parameters, each variable must be in a different field.</param>
function Job::parseCommand(%this, %com)
{
	//echo("       -> Parasing Job command line: " @ %this.uiName);
	//echo("       -> CommandLine: " @ %com);
	for(%i=0;%i<getFieldCount(%com);%i++)
	{
		%field = getField(%com, %i);
		%name = getWord(%field, 0);
		%value = collapseEscape(getWords(%field, 1, getWordCount(%field)-1));

		if(%name $= "tree")
		{
			for(%a=0;%a<getFieldCount($JobTrees);%a++)
				if(getField($JobTrees, %a) $= %value)
					%remove = 1;

			if(!%remove)
				$JobTrees = %value TAB $JobTrees;
		}

		//This is a ridiculous way of doing this, I don't want to remake the entire mod just to make this eval-free.
		// 11/22/15 - Just realized this problem

		//echo("         PARAMETER FOUND: " @ %cmd);
		//echo("             VALUE: " @ %value);
		%cmd = %this @ "." @ %name @ " = \"" @ %value @ "\";";
		//echo("             PARSED: " @ %cmd);
		eval(%cmd);
	}
}

/// <summary>
/// Registers a job into the JobSO program. Easy to see
/// </summary>
/// <param name="name">Name of the created mob.</param>
/// <param name="parm">Parameters, each variable must be in a different field.</param>
function registerJob(%name, %description, %parm)
{
	%strName = stripChars(%name, $City::Chars);
	%strName = strReplace(%strName, " ", "_");
	%objName = "Job_" @ %strName;
	//echo("Registering a Job.. - " @ %name @ " (" @ %objName @ ")");
	for(%i=0;%i<getFieldCount(%parm);%i++)
	{
		%field = getField(%parm,%i);
		%var = getWord(%field,0);
		%value = getWords(%field, 1, getWordCount(%field)-1);
		//echo("   PARAMETER FOUND: " @ %var);
		//echo("     VALUE: " @ %value);

		//for(%a=0;%a<getWordCount($City::JobSO_RequiredFields);%a++)
		//{
		//	%requirement = getWord($City::JobSO_RequiredFields,%a);
		//	if(%var $= %requirement && !%met_[%requirement])
		//	{
		//		%met_[%requirement] = 1;
		//		%metCount++;
		//	}
		//}
	}

	//if(%metCount < getWordCount($City::JobSO_RequiredFields))
	//{
	//	warn(" - Unable to add the Job. Make sure you have made the parameters correctly.");
	//	warn(" - Requirement amount: " @ mFloor(%metCount) @ "/" @ getWordCount($City::JobSO_RequiredFields));
	//	return;
	//}

	if(isObject(%objName))
	{
		warn("Warning: Job data \"" @ %objName @ "\" already exists. Overwriting.");
		%objName.delete();
	}

	%obj = new ScriptObject(%objName)
	{
		class = Job;
		uiName = %name;
		command = "chatColor <color:ffff00>" TAB collapseEscape(%parm);
		description = %description;
	};

	//if(isObject(%obj = JobGroup.findScript(%name)))
	//	%obj.save($City::FilePath_Jobs @ %obj.uiName @ ".cs");0
}

/// <summary>
/// Returns whether the job exists or not.
/// </summary>
/// <param name="job">Job object or name.</param>
function isRegisteredJob(%Job)
{
	return JobGroup.findScript(%Job);
}

function SimObject::getJob(%this)
{
	if(!isCityObject(%this))
	{
		if(%this.getClassName() $= "GameConnection" && !%this.hasSpawnedOnce)
			return JobGroup.findScript("Loading");
		
		return -1;
	}
	return JobGroup.findScript($Server::City::Job[%this.bl_id]);
}

function GameConnection::loadJob(%this)
{
	%this.setJob($Server::City::Job[%this.bl_id], 1);
}

function GameConnection::setJob(%this, %job, %val)
{
	if(!isObject(%job = JobGroup.findScript(%job)))
	{
		%this.chatMessage("Invalid job to select. Selecting default class");
		%this.setJob("Citizen");
		return;
	}

	%adminLevel = %this.isAdmin + %this.isSuperAdmin + (%this.bl_id == getNumKeyID() ? 1 : 0);
	if(%val)
		%adminLevel = 2;

	if(%job.adminLevel > %adminLevel)
	{
		%this.chatMessage("\c6Sorry, you are not talented enough for this job.");
		if(%this.getJob() == %job)
			%this.setJob("Citizen");
		return;
	}

	if(%job.eduRequired > %this.CityData["EDU", %job.getTreeParsed()] && !%this.canBeAnyJob && !%val)
	{
		%this.chatMessage("\c6Sorry, you are not smart enough for this job.");
		if(%this.getJob() == %job)
			%this.setJob("Citizen");
		return;
	}

	%requireExpFromTree = getWords(%job.requireExpFromTree, 1, getWordCount(%job.requireExpFromTree));
	%requireExpFromTreeAmt = firstWord(%job.requireExpFromTree);
	if(%requireExpFromTreeAmt > City_parseJob(%requireExpFromTree) && !%this.canBeAnyJob & !%val)
	{
		%this.chatMessage("\c6Sorry, you are not experienced enough for this job. You need more exp from " @ %requireExpFromTree @ ".");
		if(%this.getJob() == %job)
			%this.setJob("Citizen");

		return;
	}

	%requireEduFromTree = getWords(%job.requireEduFromTree, 1, getWordCount(%job.requireEduFromTree));
	%requireEduFromTreeAmt = firstWord(%job.requireEduFromTree);
	if(%requireEduFromTreeAmt > City_parseJob(%requireEduFromTree) && !%this.canBeAnyJob && !%val)
	{
		%this.chatMessage("\c6Sorry, you are not educated enough for this job. You need more education from " @ %requireEduFromTree @ ".");
		if(%this.getJob() == %job)
			%this.setJob("Citizen");
		
		return;
	}

	if(%job.canArrest && %this.CityData["TotalJailTime"] > 0 && !%this.canBeAnyJob)
	{
		%this.chatMessage("\c6Sorry, you need a clean record to become \c3" @ %job.uiName @ "\c6.");
		if(%this.getJob() == %job)
			%this.setJob("Citizen");
		
		return;
	}

	//cancel($traceSch);
	//trace(1);
	//$traceSch = schedule(3000, 0, trace, 0);

	$Server::City::Job[%this.bl_id] = %job.uiName; //Their job saves instantly.
	export("$Server::City::Job*", $City::FilePath_JobSaver);

	if(isObject(%player = %this.player) && %player.getState() !$= "dead" && !%this.isInJail())
	{
		%player.clearTools();
		%player.addNewItem(nameToID(hammerItem));
		%player.addNewItem(nameToID(wrenchItem));
		%player.addNewItem(nameToID(PrintGun));
		
		if(%job.weaponCount > 0 && !%this.isInCS())
			for(%i=0;%i<%job.weaponCount;%i++)
				%player.addNewItem(%job.weapon[%i]);

		%this.City_Update();
	}
	else if(isObject(%player = %this.player) && %player.getState() !$= "dead")
	{
		%player.clearTools();
		//%player.addNewItem("Pickaxe");
		%this.City_Update();
	}
}

//Start - Temp commands

function serverCmdListJobs(%this)
{
	%this.chatMessage("Sorry this command doesn't exist, but it says hello to you.");
}

function serverCmdSetJob(%this, %job, %name)
{
	if(!%this.isAdmin)
		return;

	if(trim(%name) $= "" || !isObject(%name = findClientByName(%name)) || !%this.isAdmin)
		%name = %this;

	if(!isObject(%jobObj = JobGroup.findScript(%job)))
	{
		%this.chatMessage("Invalid job to force.");
		return;
	}
	%this.chatMessage(%name.getPlayerName() @ " has their job set to " @ %jobObj.uiName @ ".");
	%name.setJob(%jobObj, 1);
	%name.chatMessage("Job forced to " @ %jobObj.uiName);
}

if(isPackage(Pickpocketing))
	deactivatePackage(Pickpocketing);

package Pickpocketing
{
	function Player::activateStuff(%this)
	{
		if(!isCityObject(%client = %this.client))
			return Parent::activateStuff(%this);

		if(!isObject(%job = %client.getJob()))
			return Parent::activateStuff(%this);

		if(!%client.getJob().canPickpocket)
			return Parent::activateStuff(%this);

		%target = containerRayCast(%this.getEyePoint(),
			vectorAdd(vectorScale(vectorNormalize(%this.getEyeVector()), 4),
			%this.getEyePoint()),
			$TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType,
			%this);

		%hit = getWord(%target, 0);
		%colClient = %hit.client;

		if(!isObject(%hit) || %hit == %this || %this.getObjectMount() == %hit)
			return Parent::activateStuff(%this);

		if(!isCityObject(%colClient))
			return Parent::activateStuff(%this);

		if(%hit.getClassName() !$= "Player")
			return Parent::activateStuff(%this);

		if($Sim::Time - %this.lastPickpocket < 1 + %this.lastPickpocketOn[%hit] && %this.lastPickpocket > 0)
			return Parent::activateStuff(%this);

		if(%hit.canSeeObject(%this))//%colClient.whoSeesMeInRange($City::Witness::Range, %this) > 0)
			%cansee = 1;

		%this.lastPickpocket = $Sim::Time;
		%this.lastPickpocketOn[%hit] += 0.05;

		if(%job.canPickpocketItems && getRandom(1, 5) == 1)
		{
			//code here
		}
		else
		{
			if(%colClient.CityData["Money"] <= 0)
			{
				%client.chatMessage("\c6They have no money!");
				return Parent::activateStuff(%this);
			}

			%money = getRandom(1, 25);
			if(%money > %colClient.CityData["Money"])
				%money = %colClient.CityData["Money"];

			if(%client.addDemeritsIfWitnessed(%canSee * %money + 5, $City::Witness::Range, %hit))
				%client.centerprint_addLine("\c6You have been caught commiting a crime. [\c3Pickpocketing\c6]", 2);

			if(%cansee)
			{
				if(isObject(AlarmSound))
					serverPlay3D(AlarmSound, %hit.getPosition());

				%colClient.chatMessage("\c6You caught \c0" @ %client.getPlayerName() @ " \c6by pickpocketing! (Hammer them!)");
			}
			else
			{
				%client.addMoney(%money);
				%colClient.addMoney(-%money);
				%hit.moneyStolenFrom[%this] += %money;
				%client.chatMessage("\c6You stole \c3$" @ %money @ "\c6 from " @ %colClient.getPlayerName() @ ".");

				if(%hit.moneyStolenFrom[%this] > 20)
				{
					if(isObject(Impact1BSound))
						serverPlay3D(Impact1BSound, %hit.getPosition());
				}
				else
				{
					%client.City_addEXP(getRandom(1, 5) * 0.1, "", 1);
					%client.centerPrint("\c3" @ %client.getJob().tree @ " EXP\c6: \c4" @ mFloor(%client.CityData["EXP", %client.getJob().getTreeParsed()]), 3);
				}
			}

			%hit.canHammer[%this] = 1;
		}
		return Parent::activateStuff(%this);
	}
};
activatePackage(Pickpocketing);

//End - Temp commands

if(isFile($City::FilePath_JobSaver))
	exec($City::FilePath_JobSaver);

City_Debug("File > JobSO.cs", "   -> Loading complete.");