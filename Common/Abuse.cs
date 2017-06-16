City_Debug("File > Abuse.cs", "Loading assets..");

function serverCmdRainMoney(%this, %val)
{
	if(!%this.isSuperAdmin)
		return;

	if(!isObject(%player = %this.player))
		return;

	announce("It's raining money near " @ %this.getPlayerName());
	echo("[City] It's raining money near " @ %this.getPlayerName());

	for(%i = 0; %i < 32; %i++)
		schedule(100 * %i, 0, "City_spawnCash", vectorAdd(%player.getPosition(), getRandom(-5, 5) SPC getRandom(-5, 5) SPC 50), %val);
}

City_Debug("File > Abuse.cs", "   -> Loading complete.");