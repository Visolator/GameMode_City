package City_Farming
{
	function serverCmdsetwrenchdata(%client,%data)
	{
		%brick = %client.wrenchbrick;
		if(%client.isAdmin)
			return parent::serverCmdsetwrenchdata(%client,%data);
		else
		{
			if(%brick.getDataBlock().getName() $= "brickDDRPdirtData" || %brick.getDataBlock().isGrownBrick)
				return;

			if(%brick.getDataBlock().getName() $= "BrickMusicData" || %brick.getDataBlock().getName() $= "BrickVehicleSpawnData")
				return Parent::servercmdsetwrenchdata(%client,%data);
			else
			{
				%numsettings = getWordCount(%data);
				%render = getword(%data,%numsettings-1);
				%collide = getword(%data,%numsettings-3);
				%raycast = getword(%data,%numsettings-5);
				%itemrespawntime = getword(%data,%numsettings-7);
				%itemdir = getword(%data,%numsettings-9);
				%itempos = getword(%data,%numsettings-11);
				%item = getword(%data,%numsettings-13);
				%emitterdir = getword(%data,%numsettings-15);
				%emitter = getword(%data,%numsettings-17);
				%light = getword(%data,%numsettings-19);
				
				%name = "";
				if(getword(%data,1) !$= "" &&  getword(%data,2) !$= "")
				{
					for(%a = 0; %a<%numsettings-21; %a++)
						%name = %name SPC getword(%data,%a+1);
				}

				%settingstring = "N";
				if(%name $= "")
					%settingstring = %settingstring SPC "" SPC "" TAB "";
				else
					%settingstring = %settingstring SPC %name TAB "";
		
				%settingstring = %settingstring @ "LDB" SPC %light TAB "EDB" SPC %emitter TAB "EDIR" SPC %emitterdir;
				%settingstring = %settingstring TAB "IDB" SPC %item TAB "IPOS" SPC %itempos TAB "IDIR" SPC %itemdir;
				%settingstring = %settingstring TAB "IRT" SPC %itemrespawntime TAB "RC" SPC %raycast TAB "C" SPC %collide;
				%settingstring = %settingstring TAB "R" SPC %render;
				Parent::serverCmdsetwrenchdata(%client,%settingstring);
			}
		}
	}
};
activatepackage(City_Farming);