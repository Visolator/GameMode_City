//+=========================================================================================================+\\
//|         Made by..                                                                                       |\\
//|        ____   ____  _                __          _                                                      |\\
//|       |_  _| |_  _|(_)              [  |        / |_                                                    |\\
//|         \ \   / /  __   .--.   .--.  | |  ,--. `| |-' .--.   _ .--.                                     |\\
//|          \ \ / /  [  | ( (`\]/ .'`\ \| | `'_\ : | | / .'`\ \[ `/'`\]                                    |\\
//|           \ ' /    | |  `'.'.| \__. || | // | |,| |,| \__. | | |                                        |\\
//|            \_/    [___][\__) )'.__.'[___]\'-;__/\__/ '.__.' [___]                                       |\\
//|                             BL_ID: 20490                                                                |\\
//|             Forum Profile: http://forum.blockland.us/index.php?action=profile;u=40877;                  |\\
//|                                                                                                         |\\
//+=========================================================================================================+\\

if(isPackage(AntiMulticlienting)) //Used for debugging
	deactivatepackage(AntiMulticlienting);

package AntiMulticlienting
{
	function GameConnection::autoAdminCheck(%this)
	{
		for(%i=0;%i<clientGroup.getCount();%i++)
		{
			%cl = clientGroup.getObject(%i);
			if(%cl != %this && %cl.getBLID() == %this.getBLID())
				if(!$Pref::Server::AllowM[%this.getBLID()])
				{
					echo(%cl.getPlayerName() @ " (ID: " @ %cl.getBLID() @ ") has been caught multiclienting.");
					%cl.delete("ERROR<br>Database has been detected you have more than 1 client on the server.<br>Please contact a super admin if you need to have more than 1 client.");
				}
		}
		return Parent::autoAdminCheck(%this);
	}
};
activatePackage(AntiMulticlienting);
