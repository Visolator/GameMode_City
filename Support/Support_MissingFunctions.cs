registerOutputEvent(Minigame,playSound,"dataBlock Sound",0);
function MinigameSO::playSound(%this, %sound)
{
	if(!isObject(%sound) || %sound.getClassName() !$= "AudioProfile")
		return;
	 
	if(%sound.description.isLooping)
		return;
	 
	for(%i = 0; %i < %this.numMembers; %i++)
	{
		%cl = %this.member[%i];
		//check for hacky dedicated minigame mods or AIConnections
		if(isObject(%cl) && %cl.getClassName() $= "GameConnection")
			%cl.play2D(%sound);
	}
}

function MinigameSO::play2D(%this, %sound)
{
	%this.playSound(%sound);
}

//Score

function GameConnection::getScore(%this)
{
	return %this.score;
}

//Server uptime

function getServerUpTime()
{
	return getTimeString(mFloor(getSimTime()/60000));
}

function getStringTime(%stringTime)
{
	%upTimeStringFields = strReplace(%stringTime,":","" TAB "");
	if(getFieldCount(%upTimeStringFields) >= 3)
	{
		%RTime = getField(%upTimeStringFields,1) + (24 * getField(%upTimeStringFields,0));
		%rRTime = mFloor(getField(%upTimeStringFields,2));
	}
	else
	{
		%RTime = getField(%upTimeStringFields,0);
		%rRTime = mFloor(getField(%upTimeStringFields,1));
	}
	if(%Rtime >= 24) %time["day"] = mFloor(%Rtime / 24);
	if(%Rtime > 0) %time["hour"] = %Rtime;
	if(%time["day"] > 0) %string = %string @ %time["day"] @ " day" @ (%time["day"] != 1 ? "s" @ (%time["hour"] > 0 ? "," : "") : (%time["hour"] > 0 ? "," : ""));
	if(%time["hour"] > 0)
	{
		if(%time["day"] > 0)
			%time["hour"] = %time["hour"] - %time["day"] * 24;
		%string = %string @ (%string !$= "" ? " " : "") @ %time["hour"] @ " hour" @ (%time["hour"] != 1 ? "s" @ (%rRTime > 0 ? "," : "") : (%rRTime > 0 ? "," : ""));
	}
	if(%rRTime > 0) %string = %string @ (%time["hour"] !$= "" ? " and " : " ") @ %rRTime @ " minute" @ (%rRTime != 1 ? "s" : "");
	return trim(%string);
}

function getServerUpTimeString()
{
	%upTimeString = getServerUpTime();
	%upTimeStringFields = strReplace(%upTimeString,":","" TAB "");
	if(getFieldCount(%upTimeStringFields) >= 3)
	{
		%RTime = getField(%upTimeStringFields,1) + (24 * getField(%upTimeStringFields,0));
		%rRTime = mFloor(getField(%upTimeStringFields,2));
	}
	else
	{
		%RTime = getField(%upTimeStringFields,0);
		%rRTime = mFloor(getField(%upTimeStringFields,1));
	}

	%upTimeStringFields = strReplace(%upTimeString,":","" TAB "");
	if(%Rtime >= 24) %time["day"] = mFloor(%Rtime / 24);
	if(%Rtime > 0) %time["hour"] = %Rtime;
	if(%time["day"] > 0) %string = %string @ %time["day"] @ " day" @ (%time["day"] != 1 ? "s" @ (%time["hour"] > 0 ? "," : "") : (%time["hour"] > 0 ? "," : ""));
	if(%time["hour"] > 0)
	{
		if(%time["day"] > 0)
			%time["hour"] = %time["hour"] - %time["day"] * 24;
		%string = %string @ (%string !$= "" ? " " : "") @ %time["hour"] @ " hour" @ (%time["hour"] != 1 ? "s" @ (%rRTime > 0 ? "," : "") : (%rRTime > 0 ? "," : ""));
	}
	if(%rRTime > 0) %string = %string @ (%time["hour"] !$= "" ? " and " : " ") @ %rRTime @ " minute" @ (%rRTime != 1 ? "s" : "");
	return trim(%string);
}

function serverCmdUpTime(%this)
{
	if(getSimTime() - %this.lastUpCheck < 500 && %this.lastUpCheck > 0) return;
	%this.lastUpCheck = getSimTime();
	%this.chatMessage("The server has been up for \c4" @ getServerUpTimeString() @ ".");
}

//Hex

function hexToRgb(%rgb)
{
	 %r = _hexToComp(getSubStr(%rgb,0,2)) / 255;
	 %g = _hexToComp(getSubStr(%rgb,2,2)) / 255;
	 %b = _hexToComp(getSubStr(%rgb,4,2)) / 255;
	 return %r SPC %g SPC %b;
}
 
function _compToHex(%comp)
{
	 %left = mFloor(%comp / 16);
	 %comp = mFloor(%comp - %left * 16);
	 %left = getSubStr("0123456789ABCDEF",%left,1);
	 %comp = getSubStr("0123456789ABCDEF",%comp,1);
	 return %left @ %comp;
}
 
function _hexToComp(%hex)
{
	 %left = getSubStr(%hex,0);
	 %comp = getSubStr(%hex,1);
	 %left = striPos("0123456789ABCDEF",%left);
	 %comp = striPos("0123456789ABCDEF",%comp);
	 if(%left < 0 || %comp < 0)
		  return 0;
	 return %left * 16 + %comp;
}

function rgbToHex(%rgb)
{
	 %r = _compToHex(255 * getWord(%rgb,0));
	 %g = _compToHex(255 * getWord(%rgb,1));
	 %b = _compToHex(255 * getWord(%rgb,2));
	 return %r @ %g @ %b;
}

function redToGreen(%a)
{
	%r = 1;
	%g = 1;
	if(%a >= (1/2))
		%g = mAbs(%a - 1) * 2;
	if(%a < (1/2))
		%r = %a * 2;
	return %r SPC %g SPC "0";
}

function greenToRed(%a)
{
	%r = 1;
	%g = 1;
	if(%a >= (1/2))
		%r = mAbs(%a - 1) * 2;
	if(%a < (1/2))
		%g = %a * 2;
	return %r SPC %g SPC "0";
}

function hasItemOnList(%list, %item)
{
	if(getWordCount(%list) <= 0)
		return false;

	for(%i = 0; %i < getWordCount(%list); %i++)
	{
		%word = getWord(%list, %i);
		if(%word $= %item)
			return true;
	}

	return false;
}

function addItemToList(%list, %item)
{
	if(hasItemOnList(%list, %item))
		return %list;

	%list = trim(%list SPC %item);
	return %list;
}

function removeItemFromList(%list, %item)
{
	for(%i = 0; %i < getWordCount(%list); %i++)
	{
		%word = getWord(%list, %i);
		if(%word $= %item)
		{
			%list = removeWord(%list, %i);
			return %list;
		}
	}

	return %list;
}

/// Returns a random value within two limits.
/// %lim0: One of the two limits.
/// %lim1: One of the two limits.
function getRandomF(%lim0, %lim1)
{
	%diff = %lim1 - %lim0;
	return getRandom() * %diff + %lim0;
}