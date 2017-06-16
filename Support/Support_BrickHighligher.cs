//Credit goes to Carbon Zypher for the original
$Brick::StripChars = "`~!@#^&*-=+{}\\|;:\'\",<>/?[].";

function serverCmdFindBrick(%client, %index, %name0, %name1, %name2, %name3, %name4, %name5, %name6, %name7)
{
	if(%client.isSuperAdmin)
		return;

	for(%i=0;%i<7;%i++)
		%name = %name @ "_" @ %name[%i];

	%name = trim(%name);
	%name = strReplace(%name, "-", "DASH");
	%name = strReplace(%name, " ", "_");
	%name = StripChars(%name, $Brick::StripChars);

	if(%name $= "")
		return messageClient(%client, '', "\c6You did not input the \c4brick name \c6field!");
 
	%group = getBrickGroupFromObject(%client);
	
	%index = mClampF(%index, 0, %group.NTObjectCount["_" @ %name]);
	   
	%brickID = %group.NTObject["_" @ %name, %index - 1];
	   
	if(!isObject(%brickID))
		return messageClient(%client, '', "\c6There is no brick named\c4"SPC %name @ "\c6!");
		   
	%player = %client.player;
		   
	%bricktransform = getwords(%brickID.gettransform(),0,2);
	%lengthscale = 0.1*%brickID.getdatablock().bricksizez;
		   
	%finalsend = "0 0" SPC %lengthscale;
	%finalreferral = vectoradd(%bricktransform,%finalsend);
	   
	%dirrot = dirIDtoRotation(%brickID.getAngleID());
	%finaltransform = %finalreferral SPC %dirrot;
		   
	%player.settransform(%finaltransform);
	%player.setVelocity("0 0 0");
	   
	messageClient(%client, '', "\c6Teleporting to\c4"SPC %name @ "\c6...");
	   
}
 
function serverCmdHighlightBrick(%client, %name0, %name1, %name2, %name3, %name4, %name5, %name6, %name7)
{
	if(%client.isSuperAdmin)
		return;

	for(%i=0;%i<7;%i++)
		%name = %name @ "_" @ %name[%i];

	%name = trim(%name);
	%name = strReplace(%name, "-", "DASH");
	%name = strReplace(%name, " ", "_");
	%name = StripChars(%name, $Brick::StripChars);

	if(%name $= "")
		return messageClient(%client, '', "\c6You did not input the \c4brick name \c6field!");
 
	for(%a = 0; %a < MainBrickGroup.getCount(); %a++)
	{
		%group = MainBrickGroup.getObject(%a);

		%bricktest = %group.NTObject["_" @ %name, 0];
		   
		if(!isObject(%bricktest))
			return messageClient(%client, '', "\c6There is no brick named\c4"SPC %name @ "\c6!");
		   
		for(%i = 0; %i < %group.NTObjectCount["_" @ %name]; %i++)
		{
			%brickID = %group.NTObject["_" @ %name, %i];
	 
			if(isEventPending(%brickID.resetHighlight))
				cancel(%brickID.resetHighlight);
					   
			%color = %brickID.getColorID();
			%colorFX = %brickID.getColorFxID();
			%brickID.orgColor = %color;
			%brickID.orgFX = %colorFX;
			%brickID.setColor(0);
			%brickID.setColorFX(3);
			%brickID.resetHighlight = %brickID.schedule(3000, resetColors);
		}
	}
	   
	messageClient(%client, '', "\c6Highlighting all bricks named\c4"SPC %name @ "\c6...");
}

$pi = 3.14159;
 
function dirIDtoRotation(%id) //stolen from propsys, credit to NiXiLL
{
	switch(%id)
	{
		case 0:
			%trans = %trans @ " 1 0 0 0";
		case 1:
			%trans = %trans @ " 0 0 1 " @ $pi/2;
		case 2:
			%trans = %trans @ " 0 0 1 " @ $pi;
		case 3:
			%trans = %trans @ " 0 0 -1 " @ $pi/2;
	}
	return %trans;
}
 
function FxDTSBrick::resetColors(%this)
{
	%this.setColor(%this.orgColor);
	%this.setColorFX(%this.orgFx);
}