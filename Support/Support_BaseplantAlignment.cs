//* Description *//
// Title: Baseplant Alignment
// Author: Boom (9740)

return; //We don't need this right now

//Modified the prefs for my server.

// Determine whether or not RTB is Enabled
if(isFile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	// Make sure the Preferences Hook is Enabled
	if(!$RTB::RTBR_ServerControl_Hook)
		exec("Add-Ons/System_ReturnToBlockland/RTBR_ServerControl_Hook.cs");

	RTB_registerPref("BP Required",	  "BP Alignment", "$Pref::Server::Baseplate::Required", 	  "bool", 	"Support_BaseplateAlignment", 1,  false, false);

	RTB_registerPref("Use Adjacency", "BP Alignment", "$Pref::Server::Baseplate::Adjacency::Enabled", "bool", 	"Support_BaseplateAlignment", 1,  false, false);

	RTB_registerPref("BP Alignment",  "BP Alignment", "$Pref::Server::Baseplate::Alignment::Enabled", "bool", 	"Support_BaseplateAlignment", 1,  false, false);
	RTB_registerPref("Auto Align",    "BP Alignment", "$Pref::Server::Baseplate::Alignment::Auto", 	  "bool", 	"Support_BaseplateAlignment", 1,  false, false);
	RTB_registerPref("Cell Width", 	  "BP Alignment", "$Pref::Server::Baseplate::Alignment::Width",   "int 0 1024", "Support_BaseplateAlignment", 16, false, false);
	RTB_registerPref("Cell Height",   "BP Alignment", "$Pref::Server::Baseplate::Alignment::Height",  "int 0 1024", "Support_BaseplateAlignment", 16, false, false);
	RTB_registerPref("Grid X Offset", "BP Alignment", "$Pref::Server::Baseplate::Alignment::XOffset", "int 0 1024", "Support_BaseplateAlignment", 14,  false, false);
	RTB_registerPref("Grid Y Offset", "BP Alignment", "$Pref::Server::Baseplate::Alignment::YOffset", "int 0 1024", "Support_BaseplateAlignment", 9,  false, false);
}
else
{
	// Use Default Preference Values
	$Pref::Server::Baseplate::Required = true;

	$Pref::Server::Baseplate::Adjacency::Enabled = true;

	$Pref::Server::Baseplate::Alignment::Enabled = true;
	$Pref::Server::Baseplate::Alignment::Auto = true;
	$Pref::Server::Baseplate::Alignment::Width = 16;
	$Pref::Server::Baseplate::Alignment::Height = 16;
	$Pref::Server::Baseplate::Alignment::XOffset = 14;
	$Pref::Server::Baseplate::Alignment::YOffset = 9;
}
$Pref::Server::Baseplate::Adjacency::Enabled = false;

// Performs a Neighbor Check on the calling Brick
function fxDTSBrick::doNeighborCheck(%brick)
{
	// Call Neighbor Check Trigger
	%success = %brick.onNeighborCheck();

	if(%success)
		%brick.onNeighborPass();
	else
		%brick.onNeighborFail();

	return %success;
}

// Returns whether or not the calling Brick has a Baseplate Neighbor
function fxDTSBrick::hasBaseplateNeighbor(%brick)
{
	//echo("\c4hasBaseplateNeighbor(\c1" @ %brick @ "\c4)");

	// Determine the Position of the Brick
	%transform = %brick.getTransform();

	%x = getWord(%transform, 0);
	%y = getWord(%transform, 1);
	%z = getWord(%transform, 2);

	//echo("\c3 -> \c9Transform\c0: " @ getWords(%transform, 0, 2));

	// Determine the Size of the Brick
	%data = %brick.getDatablock();

	%sizeX = %data.brickSizeX / 2;
	%sizeY = %data.brickSizeY / 2;

	//echo("\c3 -> \c9Brick Size\c0: " @ %sizeX SPC %sizeY);

	// Determine the Rotation of the Brick
	%rotation = mFloatLength(getWord(%brick.rotation, 3), 0);

	//echo("\c3 -> \c9Rotation\c0: " @ %rotation);

	// Determine the Raycast Length
	%lengthX = (mAbs(%rotation) == 90 ? %sizeY : %sizeX) / 2 + 0.1;
	%lengthY = (mAbs(%rotation) == 90 ? %sizeX : %sizeY) / 2 + 0.1;

	//echo("\c3 -> \c9Raycast Length\c0: " @ %lengthX SPC %lengthY);

	// Check the Non-Diagonal Directions for Baseplate Neighbors
	%startPos = %transform;

	for(%i = 0; %i < 360; %i += 90)
	{
		// Determine the End Position of the Raycast
		switch(%i)
		{
			case 0:   %endPos = vectorAdd(%startPos, %lengthX SPC "0 0");
			case 90:  %endPos = vectorAdd(%startPos, "0" SPC %lengthY SPC "0");
			case 180: %endPos = vectorSub(%startPos, %lengthX SPC "0 0");
			case 270: %endPos = vectorSub(%startPos, "0" SPC %lengthY SPC "0");
		}

		// Perform the Raycast
		//echo("\c3 -> \c0[\c2" @ %i @ "\c0]: \c9Raycasting\c0: " @ %startPos @ " to " @ %endPos);

		%object = firstWord(containerRaycast(%startPos, %endPos, $TypeMasks::FxBrickAlwaysObjectType, %brick));

		//echo("\c3 -> -> \c9Found\c0: " @ %object);

		// Determine whether or not a Neighbor was Found
		if(isObject(%object))
		{
			// Make sure the Neighbor is a Baseplate
			if(%object.getDatablock().category $= "Baseplates")
				return true;
		}		
	}

	// No Neighbors were Found
	//echo("\c3 -> \c4return \c1\"No neighbors Found\"");

	return false;
}

// Performs an Alignment Check on the calling Brick
function fxDTSBrick::doAlignmentCheck(%brick)
{
	// Call Alignment Check Trigger
	%success = %brick.onAlignmentCheck();

	if(%success)
		%brick.onAlignmentPass();
	else
		%brick.onAlignmentFail();

	return %success;
}

// Returns whether or not the calling Brick is Aligned to the Grid
function fxDTSBrick::isAligned(%brick)
{
	// Determine the Position of the Brick
	%transform = getWords(%brick.getTransform(), 0, 2);

	// Determine the Aligned Position of the Brick
	%alignment = %brick.getAlignment();

	// Check Alignment
	if(%transform !$= %alignment)
		return false;

	return true;
}

// Moves the calling Brick to the nearest Aligned Position
function fxDTSBrick::align(%brick)
{
	// Align the Brick
	%brick.setTransform(%brick.getAlignment());
}

// Returns the Nearest Alignment of the calling Brick
function fxDTSBrick::getAlignment(%brick)
{
	// Determine the Grid Attributes
	%width = $Pref::Server::Baseplate::Alignment::Width;
	%height = $Pref::Server::Baseplate::Alignment::Height;
	%xoffset = $Pref::Server::Baseplate::Alignment::XOffset;
	%yoffset = $Pref::Server::Baseplate::Alignment::YOffset;

	// Determine the Position of the Brick
	%transform = %brick.getTransform();

	%x = getWord(%transform, 0) * 2 - %xoffset;
	%y = getWord(%transform, 1) * 2 - %yoffset;
	%z = getWord(%transform, 2);

	// Determine the Size of the Brick
	%data = %brick.getDatablock();

	%sizeX = %data.brickSizeX / 2;
	%sizeY = %data.brickSizeY / 2;
	%sizeZ = %data.brickSizeZ;

	// Determine the Rotation of the Brick
	%rotation = mFloatLength(getWord(%brick.rotation, 3), 0);

	// Use the Corner of the Brick for Alignment
	%x -= mAbs(%rotation) == 90 ? %sizeY : %sizeX;
	%y -= mAbs(%rotation) == 90 ? %sizeX : %sizeY;

	// Determine Aligned Positions
	%xAlign = mFloor(%x/%width) * %width;
	%yAlign = mFloor(%y/%height) * %height;
	%zAlign = %z;

	// Reset the Alignment to the Center of the Brick
	%xAlign += mAbs(%rotation) == 90 ? %sizeY : %sizeX;
	%yAlign += mAbs(%rotation) == 90 ? %sizeX : %sizeY;

	// Add Offset Back
	%xAlign += %xoffset;
	%yAlign += %yoffset;

	// Adjust for Linear Scaling
	%xAlign /= 2;
	%yAlign /= 2;

	// Return the Alginment
	return %xAlign SPC %yAlign SPC %z;
}

// Disable Existing Package
if(isPackage(BaseplateAlignment_Package))
	deactivatePackage(BaseplateAlignment_Package);

//* Package *//
package BaseplateAlignment_Package
{
	//////////////////////////
	//* Placement Triggers *//
	//////////////////////////
	// Triggered when a Client plants a Brick
	function serverCmdPlantBrick(%client)
	{
		// Determine the Brick being Planted
		%brick = %client.player.tempBrick;

		// Determine whether or not Baseplates are Required
		if($Pref::Server::Baseplate::Required)
		{
			// Make sure the Brick is a Grounded Baseplate
			if(%brick.getDistanceFromGround() == 0 && %brick.getDatablock().category !$= "Baseplates")
			{
				commandToClient(%client, 'MessageBoxOK', "Invalid Brick", "You must start with a Baseplate!<bitmap:base/client/ui/brickIcons/16x16 Base>");
				return;
			}

			// Check for Baseplate Alignment
			if($Pref::Server::Baseplate::Alignment::Enabled)
			{
				// Make sure the Brick is Aligned
				if(!%brick.doAlignmentCheck())
				{
					commandToClient(%client, 'MessageBoxOK', "Unaligned Baseplate", "You must align the Baseplate to the existing Grid!");
					return;
				}
			}

			// Check for Baseplate Adjacency
			if($Pref::Server::Baseplate::Adjacency::Enabled)
			{
				// Make sure the Brick has a Baseplate Neighbor
				if(!%brick.doNeighborCheck())
				{
					commandToClient(%client, 'MessageBoxOK', "No Baseplate Neighbor", "You must place the Baseplate near another Baseplate!");
					return;
				}
			}
		}

		// Call the Parent Method
		Parent::serverCmdPlantBrick(%client);
	}

	//////////////////////////
	//* Alignment Triggers *//
	//////////////////////////
	// Triggered when a Brick is Checked for Alignment
	function fxDTSBrick::onAlignmentCheck(%brick)
	{
		return %brick.isAligned();
	}

	// Triggered when a Baseplate Passes an Alignment Check
	function fxDTSBrick::onAlignmentPass(%brick)
	{
		// Override if Necessary
	}

	// Triggered when a Baseplate Fails an Alignment Check
	function fxDTSBrick::onAlignmentFail(%brick)
	{
		// Check for Auto-Align
		if($Pref::Server::Baseplate::Alignment::Auto)
			%brick.align();
	}

	//////////////////////////
	//* Adjacency Triggers *//
	//////////////////////////
	// Triggered when a Brick is Checked for Baseplate Neighbors
	function fxDTSBrick::onNeighborCheck(%brick)
	{
		// Override for Admins
		return %brick.hasBaseplateNeighbor();
	}

	// Triggered when a Baseplate Passes a Neighbor Check
	function fxDTSBrick::onNeighborPass(%brick)
	{
		// Override if Necessary
	}

	// Triggered when a Baseplate Fails a Neighbor Check
	function fxDTSBrick::onNeighborFail(%brick)
	{
		// Override if Necessary
	}
};

activatePackage(BaseplateAlignment_Package);

