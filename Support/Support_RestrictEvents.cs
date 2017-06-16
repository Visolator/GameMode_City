if(!strLen($Server::RestrictedEvents::Save))
	$Server::RestrictedEvents::Save = "config/server/RestrictEvents.txt";

if(isFile($Server::RestrictedEvents::Save))
{
	//Let's open it.
	$Server::RestrictedEvents::FileLine = "";
	
	if(isObject($Server::RestrictedEvents::FileOBJ))
		$Server::RestrictedEvents::FileOBJ.delete();

	$Server::RestrictedEvents::FileOBJ = new FileObject();
	$Server::RestrictedEvents::FileOBJ.openForRead($Server::RestrictedEvents::Save);
	while(!$Server::RestrictedEvents::FileOBJ.isEOF())
	{
		$Server::RestrictedEvents::FileLine = $Server::RestrictedEvents::FileOBJ.readLine();

		if(getWord($Server::RestrictedEvents::FileLine, 0) $= "OUTPUT")
		{
			%event["class"] = getWord($Server::RestrictedEvents::FileLine, 1);
			%event["name"] = getWord($Server::RestrictedEvents::FileLine, 2);
			%event["level"] = getWord($Server::RestrictedEvents::FileLine, 3);
			if(!isEventOutputRestricted(%event["class"], %event["name"]))
			{
				$Server::RestrictedEvent[%event["class"], %event["name"]] = %event["name"] TAB %event["level"];
				echo("[Auto-Loaded] [Output] Restricting: " @ %event["name"] @ " on " @ %event["class"]);
			}
		}
		else if(getWord($Server::RestrictedEvents::FileLine, 0) $= "INPUT")
		{
			%event["class"] = getWord($Server::RestrictedEvents::FileLine, 1);
			%event["name"] = getWord($Server::RestrictedEvents::FileLine, 2);
			%event["level"] = getWord($Server::RestrictedEvents::FileLine, 3);
			if(!isEventInputRestricted(%event["class"], %event["name"]))
			{
				$Server::RestrictedInputEvent[%event["class"], %event["name"]] = %event["name"] TAB %event["level"];
				echo("[Auto-Loaded] [Input] Restricting: " @ %event["name"] @ " on " @ %event["class"]);
			}
		}
	}
	$Server::RestrictedEvents::FileOBJ.close();
	$Server::RestrictedEvents::FileOBJ.delete();

	$Server::RestrictedEvents::FileLine = "";
}

function restrictOutputEvent(%class, %event, %level, %save)
{
	if(%class $= "")
		%class = "fxDtsBrick";

	if(%level <= 0) //Why would you restrict it? Let's just set it to admin only anyways.
		%level = 1;

	if(!isEventOutputRestricted(%class, %event))
	{
		$Server::RestrictedEvent[%class, %event] = %event TAB %level;
		echo("Restricting an output event: " @ %class @ ", " @ %event);
	}
}

function isEventOutputRestricted(%class, %event)
{
	if($Server::RestrictedEvent[%class, %event] !$= "")
		if(firstWord($Server::RestrictedEvent[%class, %event]))
			return true;

	return false;
}

function restrictInputEvent(%class, %event, %level, %save)
{
	if(%class $= "")
		%class = "fxDtsBrick";

	if(%level <= 0) //Why would you restrict it? Let's just set it to admin only anyways.
		%level = 1;

	if(!isEventInputRestricted(%class, %event))
	{
		$Server::RestrictedInputEvent[%class, %event] = true SPC %level;
		echo("Restricting an input event: " @ %class @ ", " @ %event);
	}
}

function isEventInputRestricted(%class, %event)
{
	if($Server::RestrictedInputEvent[%class, %event] !$= "")
		if(firstWord($Server::RestrictedInputEvent[%class, %event]))
			return true;

	return false;
}

if(isPackage(Server_RestrictedEvents))
	deactivatePackage(Server_RestrictedEvents);

package Server_RestrictedEvents
{
	function serverCmdAddEvent(%client,%enabled,%inputEventIdx,%delay,%targetIdx,%namedTargetNameIdx,%outputEventIdx,%par1,%par2,%par3,%par4)
	{
		if(isObject(%client) && %client.getClassName() $= "GameConnection")
		{
			%target = inputEvent_getTargetClass("fxDtsBrick",%inputEventIdx,%targetIdx);
			if(%target $= "")
				%target = "fxDtsBrick";
			%outputEvent = outputEvent_getOutputName(%target, %outputEventIdx);
			%inputEvent = $InputEvent_Name["fxDtsBrick", %inputEventIdx];

			if(!%client.canUseRestrictedEvent(%target, %outputEvent))
			{
				%client.chatMessage("Restricted output: " @ %outputEvent);
				return;
			}

			if(!%client.canUseRestrictedInput(%target, %inputEvent))
			{
				%client.chatMessage("Restricted input: " @ %inputEvent);
				return;
			}
		}

		Parent::serverCmdAddEvent(%client,%enabled,%inputEventIdx,%delay,%targetIdx,%NamedTargetNameIdx,%outputEventIdx,%par1,%par2,%par3,%par4,%par5,%par6,%par7,%par8,%par9,%par10,%par11,%par12,%par13);
	}
};
activatePackage(Server_RestrictedEvents);

function GameConnection::canUseRestrictedEvent(%this, %class, %event)
{
	%adminLevel = %this.isAdmin + %this.isSuperAdmin + (%this.getBLID() == getNumKeyID() ? 1 : 0);
	if(%adminLevel < getWord($Server::RestrictedEvent[%class, %event], 1))
		if(!%this.canUseRestrictedEvents())
			return false;

	return true;
}

function GameConnection::canUseRestrictedInput(%this, %class, %event)
{
	%adminLevel = %this.isAdmin + %this.isSuperAdmin + (%this.getBLID() == getNumKeyID() ? 1 : 0);
	if(%adminLevel < getWord($Server::RestrictedInputEvent[%class, %event], 1))
		if(!%this.canUseRestrictedEvents())
			return false;

	return true;
}

function GameConnection::canUseRestrictedEvents(%this)
{
	if(%this.isAdmin)
		return true;

	if(%this.unRestrictEvents)
		return true;

	if($Server::RestrictedEvent[%this.getBLID()])
		return true;

	return false;
}

function serverCmdToggleREs(%this, %target)
{
	if(!%this.isSuperAdmin)
		return;

	if(!isObject(%targ = findClientByName(%target)))
	{
		if($Server::RestrictedEvent[%target])
		{
			%this.chatMessage("\c6BL_ID: \c4" @ %target @ "\c6 can no longer use restricted events.");
			$Server::RestrictedEvent[%target] = false;
			export("$Server::RestrictedEvent*", $Server::RestrictedEvents::Save, false);
		}
		return;
	}
	%bl_id = %targ.getBLID();
	if($Server::RestrictedEvent[%bl_id])
	{
		%this.chatMessage("\c6BL_ID: \c4" @ %bl_id @ " (" @ %targ.getSimpleName() @ ")\c6 can no longer use restricted events.");
		$Server::RestrictedEvent[%bl_id] = false;
	}
	else
	{
		%this.chatMessage("\c6BL_ID: \c4" @ %bl_id @ " (" @ %targ.getSimpleName() @ ")\c6 can now use restricted events.");
		$Server::RestrictedEvent[%bl_id] = true;
	}
	export("$Server::RestrictedEvent*", $Server::RestrictedEvents::Save, false);
}

function serverCmdREs(%this)
{
	if(!%this.isAdmin)
		return;

	for(%i=0;%i<clientGroup.getCount();%i++)
	{
		%cl = clientGroup.getObject(%i);
		if($Server::RestrictedEvent[%this.getBLID()] || %cl.isAdmin)
			%this.chatMessage("\c4" @ %cl.getSimpleName() @ "\c6 can use \c0Restricted Events\c6.");
	}
}