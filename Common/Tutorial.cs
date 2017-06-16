City_Debug("File > Tutoral.cs", "Loading assets..");

if(isPackage(City_Tutorial))
	deactivatePackage(City_Tutorial);

package City_Tutorial
{
	function serverCmdMessageBoxNo(%this)
	{
		if(%this.TempCityData["TutorialHelp"])
		{
			%this.TempCityData["TutorialHelp"] = 0;

			if(isObject(%player = %this.player))
			{
				%this.TempCityData["TutorialRedo"] = 1;
				commandToClient(%this, 'MessageBoxYesNo', "Uh oh!", "Are you sure you know how to play?<br>You can always do /tutorial.", 'ConfirmTutorialDone');
			}
		}	
		else if(%this.TempCityData["TutorialRedo"])
		{
			%this.TempCityData["TutorialRedo"] = 0;
			%this.TempCityData["TutorialHelp"] = 0;
			%this.City_TutorialRedo();
		}
	}
};
activatePackage(City_Tutorial);

function GameConnection::Tutorial_Help(%this)
{
	%this.centerPrint("Sorry, there is no tutorial right now :(<br>Please check again sometime soon.", 5);
}
registerOutputEvent("GameConnection", "Tutorial_Help");

function serverCmdJob(%this)
{
	%this.chatMessage("Sorry, we do not use this. Please go to a job/edu department and apply for a job when you see the names by typing the job normally in the chat.");
}

function serverCmdConfirmTutorialDone(%this)
{
	%this.TempCityData["TutorialRedo"] = 0;
	%this.TempCityData["TutorialHelp"] = 0;
}

function serverCmdConfirmTutorial(%this)
{
	if(!isObject(%player = %this.player))
	{
		%this.chatMessage("You must exist!");
		%this.TempCityData["TutorialHelp"] = 0;
		return;
	}

	if(!%this.TempCityData["TutorialHelp"])
		return;

	%this.TempCityData["TutorialHelp"] = 0;

	%this.chatMessage("There is no tutorial yet, sorry :(");
}

function GameConnection::City_TutorialRedo(%this, %bypass)
{
	if(isObject(%player = %this.player))
	{
		%this.TempCityData["TutorialRedo"] = 0;
		%this.TempCityData["TutorialHelp"] = 1;
		commandToClient(%this, 'MessageBoxYesNo', "Welcome back, " @ %this.getPlayerName() @ "!", "Would you like to see the basic tutorials on how to play?", 'ConfirmTutorial');
	}
}

function GameConnection::City_Tutorial(%this, %bypass)
{
	if(isObject(%player = %this.player))
	{
		%this.TempCityData["TutorialRedo"] = 0;
		%this.TempCityData["TutorialHelp"] = 1;
		commandToClient(%this, 'MessageBoxYesNo', "Welcome, " @ %this.getPlayerName() @ "!", "This is Visolator's CityRPG Mod." @ 
			"<br>Would you like to see the basic tutorials on how to play?", 'ConfirmTutorial');
	}
}

function serverCmdTutorial(%this)
{
	%this.City_Tutorial(1);
}

function serverCmdSetCityAddon(%this)
{
	if(%this.getBLID() != getNumKeyID())
	{
		%this.chatMessage("You are not the host.");
		return;
	}

	if(isFile(%file = "Add-Ons/GameMode_City/Special/NewAddOnList.cs"))
	{
		%this.chatMessage(%file @ " \c7- \c6New add-on list has been created, please restart the game completely but quitting it.");
		exec(%file);
		schedule(50, 0, export, "$AddOn__*", "config/server/ADD_ON_LIST.cs");
	}
}

function serverCmdHelp(%this, %type, %cmd1, %cmd2)
{
	switch$(%type)
	{
		case "commands":
			%this.chatMessage("\c6/tutorial - A WIP tutorial, check as much as you want!");
			if(%this.isAdmin)
			{
				%this.chatMessage("\c6======== \c3Admin commands \c6========");
				%this.chatMessage("\c6/setJob \c3job name \c7- \c6Sets a person's job, forced. Do not use this unless their profile was broken.");
				%this.chatMessage("\c6/Jail \c3name time \c7- \c6If you think they deserved it, jail them.");
				%this.chatMessage("\c6/Pardon \c3name \c7- \c6Releases someone from jail.");
				%this.chatMessage("\c6/ToggleMode \c7- \c6Play mode/Modification mode");
				%this.chatMessage("\c6/ClearRecord \c3name \c7- \c6Clears their record");
				%this.chatMessage("\c6======================================");
			}
			if(%this.getBLID() == getNumKeyID())
				%this.chatMessage("\c6/SetCityAddon - Sets your add-on list to the correct bricks for the CityRPG build downloaded.");

		case "list":
			switch$(%cmd1)
			{
				case "jobs":
					%this.chatMessage("\c5Sorry, please see the EDU/Job department to see the job list. You will only see what is available to you, or close to being available.");

				default:
					%this.chatMessage("\c6Categories of \c4/help list");
					%this.chatMessage("  - \c6jobs \c7> \c6A broken command that will not work here, please go to the edu/job department.");
			}

		case "cost":
			switch$(%cmd1)
			{
				case "edu" or "education":
					%job = %this.getJob();
					%treeUIName = %job.tree;
					%treeName = %job.getTreeParsed();
					%edu = mClampF(%this.CityData["EDU", %treeName], 0, 8);

					if(%edu < 8)
						%this.chatMessage("\c5Next education cost for \c6" @ %job.uiName @ " \c5(\c6" @ %treeUIName @ "\c5)"
							@ ": \c2$" @ mFloatLength($City::Education::Job + 120 * (mClampF(%this.CityData["EDU", %treeName], 0, 8) * 1.5), 2));
					else
						%this.chatMessage("\c5You have maxed out your education.");

				case "job":
					%this.chatMessage("Sorry this does not work yet.");
					return;

					%job = %this.getJob();
					%treeUIName = %job.tree;
					%treeName = %job.getTreeParsed();
					%exp = mClampF(%this.CityData["EDU", %treeName], 0, 8);


					%this.chatMessage("\c5Next job for \c6" @ %treeName @ " \c5is " @ %newJob.uiName @ "(EDU: " @ %newJob.requiredEdu @ " | EXP: " @ %newJob.requiredExp @ ")");

				default:
					%this.chatMessage("\c6Categories of \c4/help cost");
					%this.chatMessage("  - \c6edu, education \c7> \c6Tells you your next education.");
					%this.chatMessage("  - \c6job, nextJob \c7> \c6See what you can do for your next job in your field.");
			}

		case "tutorial":
			switch$(%cmd1)
			{
				case "cash":
					%this.chatMessage("\c5hi");

				default:
					serverCmdTutorial(%this);
			}

		default:
			%this.chatMessage("\c6Categories of /help - Currently WIP!");
			%this.chatMessage("  - \c6commands \c7> \c6See a list of commands you are able to use.");
			%this.chatMessage("  - \c6cost \c7> \c6See the costs of stuff. Next job? Next education?");
			%this.chatMessage("  - \c6list \c7> \c6See a list of.. Stuff.");
			%this.chatMessage("  - \c6tutorial \c7> \c6A way to use the tutorial.");
	}
}

City_Debug("File > Tutoral.cs", "   -> Loading complete.");