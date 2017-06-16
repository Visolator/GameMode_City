function getDayCycleTime()
{
	if (!$EnvGuiServer::DayCycleEnabled || !isObject(DayCycle))
	{
		error("ERROR: DayCycle not enabled.");
		return -1;
	}

	%time = $Sim::Time / DayCycle.dayLength + DayCycle.dayOffset;
	return %time - mFloor(%time);
}

function getDayCycleStage(%time)
{
	return mFloor(%time / 0.25);
}

function getDayCycleStageName(%stage)
{
	switch (%stage) {
		case 0: return "dawn";
		case 1: return "day";
		case 2: return "dusk";
		case 3: return "night";

		default: return "error";
	}
}

function getMSToNextDayCycleStage(%time)
{
	%cycles = (mFloor(%time / 0.25) + 1) * 0.25 - %time;
	return mFloor(%cycles * DayCycle.dayLength * 1000);
}

function getDayCycleTimeString(%time, %mod12)
{
	%time = getTimeString(mFloor((%time * 86400) / 60));

	if (!%mod12)
	{
		if (strLen(%time) == 3)
		{
			return 0 @ %time;
		}

		return %time;
	}

	%time = strReplace(%time, ":", " ");

	%hour = getWord(%time, 0);
	%mins = getWord(%time, 1);

	if (%hour >= 13)
	{
		return %hour - 12 @ ":" @ %mins SPC "PM";
	}

	return %hour @ ":" @ %mins SPC "AM";
}

function getSunVector()
{
	%azim = mDegToRad($EnvGuiServer::SunAzimuth);

	if ($EnvGuiServer::DayCycleEnabled && isObject(DayCycle))
	{
		%time = getDayCycleTime();
		%badspotIsWeird = 0.583;

		if (%time < %badspotIsWeird)
		{
			%elev = mDegToRad((%time / %badspotIsWeird) * 180);
		}
		else
		{
			%elev = mDegToRad(180 + ((%time - %badspotIsWeird) / (1 - %badspotIsWeird)) * 180);
		}
	}
	else
	{
		%elev = mDegToRad($EnvGuiServer::SunElevation);
	}

	%h = mCos(%elev);
	return %h * mSin(%azim) SPC %h * mCos(%azim) SPC mSin(%elev);
}

function isPointInShadow(%pos, %ignore)
{
	%ray = containerRayCast(%pos,
		vectorAdd(%pos, vectorScale(getSunVector(), 250)),
		$TypeMasks::StaticShapeObjectType |
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::VehicleObjectType |
		$TypeMasks::PlayerObjectType |
		$TypeMasks::ItemObjectType,
		%ignore
	);

	return %ray !$= 0;
}

function syncDayCycle()
{
	if (!isObject(DayCycle)) {
		error("ERROR: DayCycle does not exist.");
		return;
	}

	if (DayCycle.dayLength != 86400) {
		error("ERROR: DayCycle length is not 86400.");
		return;
	}

	%all = strReplace(getWord(getDateTime(), 1), ":", " ");

	%real = getWord(%all, 0) * 3600 + getWord(%all, 1) * 60 + getWord(%all, 2);
	%curr = $Sim::Time / 86400;

	DayCycle.setDayOffset(%real - (%curr - mFloor(%curr)));
}

//Environment

// Change an environment setting without needing a fake client. Implements
// the default functionality - could be written more succintly, but this was
// intended to be as much like the default system as possible.
function setEnvironment(%varName, %value)
{
	if ($EnvGuiServer["::" @ %varName] $= %value)
		return;

	switch$ (%varName)
	{
	case "SimpleMode":
		$EnvGuiServer::SimpleMode = mClamp(%value, 0, 1);
		EnvGuiServer::setSimpleMode();

		if (!$EnvGuiServer::SimpleMode)
		{
			if (!$EnvGuiServer::HasSetAdvancedOnce)
				EnvGuiServer::readAdvancedVarsFromSimple();

			EnvGuiServer::setAdvancedMode();
		}

	case "SkyIdx":
		$EnvGuiServer::SkyIdx = mClamp(%value, 0, $EnvGuiServer::SkyCount);
		setSkyBox($EnvGuiServer::Sky[$EnvGuiServer::SkyIdx]);

	case "WaterIdx":
		$EnvGuiServer::WaterIdx = mClamp(%value, 0, $EnvGuiServer::WaterCount);
		setWater($EnvGuiServer::Water[$EnvGuiServer::WaterIdx]);

	case "GroundIdx":
		$EnvGuiServer::GroundIdx = mClamp(%value, 0, $EnvGuiServer::GroundCount);
		setGround($EnvGuiServer::Ground[$EnvGuiServer::GroundIdx]);

	case "SunFlareTopIdx":
		$EnvGuiServer::SunFlareTopIdx = mClamp(%value, 0, $EnvGuiServer::SunFlareCount);
		%top = $EnvGuiServer::SunFlareTop[$EnvGuiServer::SunFlareTopIdx];
		%bottom = $EnvGuiServer::SunFlareBottom[$EnvGuiServer::SunFlareBottomIdx];
		SunLight.setFlareBitmaps(%top, %bottom);

	case "SunFlareBottomIdx":
		$EnvGuiServer::SunFlareBottomIdx = mClamp(%value, 0, $EnvGuiServer::SunFlareCount);
		%top = $EnvGuiServer::SunFlareTop[$EnvGuiServer::SunFlareTopIdx];
		%bottom = $EnvGuiServer::SunFlareBottom[$EnvGuiServer::SunFlareBottomIdx];
		SunLight.setFlareBitmaps(%top, %bottom);

	case "DayOffset":
		$EnvGuiServer::DayOffset = mClampF(%value, 0, 1);
		DayCycle.setDayOffset($EnvGuiServer::DayOffset);

	case "DayLength":
		$EnvGuiServer::DayLength = mClamp(%value, 0, 86400);
		DayCycle.setDayLength($EnvGuiServer::DayLength);

	case "DayCycleEnabled":
		$EnvGuiServer::DayCycleEnabled = mClamp(%value, 0, 1);
		DayCycle.setEnabled($EnvGuiServer::DayCycleEnabled);

	case "DayCycleIdx":
		$EnvGuiServer::DayCycleIdx = mClamp(%value, 0, $EnvGuiServer::DayCycleCount);
		%dayCycle = $EnvGuiServer::DayCycle[$EnvGuiServer::DayCycleIdx];
		echo("server setting daycycle to " @ %dayCycle);
		loadDayCycle(%dayCycle);

	case "SunAzimuth":
		$EnvGuiServer::SunAzimuth = mClampF(%value, 0, 360);
		Sun.azimuth = $EnvGuiServer::SunAzimuth;
		Sun.sendUpdate();

	case "SunElevation":
		$EnvGuiServer::SunElevation = mClampF(%value, -10, 190);
		Sun.elevation = $EnvGuiServer::SunElevation;
		Sun.sendUpdate();

	case "DirectLightColor":
		$EnvGuiServer::DirectLightColor = getColorF(%value);
		Sun.color = $EnvGuiServer::DirectLightColor;
		Sun.sendUpdate();

	case "AmbientLightColor":
		$EnvGuiServer::AmbientLightColor = getColorF(%value);
		Sun.ambient = $EnvGuiServer::AmbientLightColor;
		Sun.sendUpdate();

	case "ShadowColor":
		$EnvGuiServer::ShadowColor = getColorF(%value);
		Sun.shadowColor = $EnvGuiServer::DirectLightColor;
		Sun.sendUpdate();

	case "SunFlareColor":
		$EnvGuiServer::SunFlareColor = getColorF(%value);
		SunLight.color = $EnvGuiServer::SunFlareColor;
		SunLight.sendUpdate();

	case "SunFlareSize":
		$EnvGuiServer::SunFlareSize = mClampF(%value, 0, 10);
		// Badspot, why not just SunLight.setFlareSize(...);?
		SunLight.flareSize = $EnvGuiServer::SunFlareSize;
		SunLight.sendUpdate();

	case "SunFlareIdx":
		$EnvGuiServer::SunFlareIdx = mClamp(%value, 0, $EnvGuiServer::SunFlareCount);
		// ... what? Why does this exist? There is no "SunFlareIdx" system.
		// Get outta' here.

	case "VisibleDistance":
		$EnvGuiServer::VisibleDistance = mClampF(%value, 0, 1000);
		Sky.visibleDistance = $EnvGuiServer::VisibleDistance;
		Sky.sendUpdate();

	case "FogDistance":
		$EnvGuiServer::FogDistance = mClampF(%value, 0, 1000);
		Sky.fogDistance = $EnvGuiServer::FogDistance;
		Sky.sendUpdate();

	case "FogHeight":
		$EnvGuiServer::FogHeight = mClampF(%value, 0, 1000);
		// Nothing to see here...

	case "FogColor":
		$EnvGuiServer::FogColor = getColorF(%value);
		Sky.fogColor = $EnvGuiServer::FogColor;
		Sky.sendUpdate();

	case "WaterColor":
		$EnvGuiServer::WaterColor = getColorF(%value);

		if (isObject(WaterPlane))
		{
			WaterPlane.color = getColorI($EnvGuiServer::WaterColor);
			WaterPlane.blend = getWord(WaterPlane.color, 3) < 255;
			WaterPlane.sendUpdate();
		}

		updateWaterFog();

	case "WaterHeight":
		$EnvGuiServer::WaterHeight = mClampF(%value, 0, 100);

		if (isObject(WaterPlane))
		{
			%pos = getWords(GroundPlane.getTransform(), 0, 2);
			%pos = vectorAdd(%pos, "0 0" SPC $EnvGuiServer::WaterHeight);

			WaterPlane.setTransform(%pos SPC "0 0 1 0");
			WaterPlane.sendUpdate();

			updateWaterFog();

			if (isObject(WaterZone))
			{
				%pos = vectorSub(%pos, "0 0 99.5");
				%pos = vectorSub(%pos, "500000 -500000 0");
				WaterZone.setTransform(%pos SPC "0 0 1 0");
			}
		}

	case "UnderWaterColor":
		$EnvGuiServer::UnderWaterColor = getColorF(%value);

		if (isObject(WaterZone))
		{
			WaterZone.setWaterColor($EnvGuiServer::UnderWaterColor);
		}

	case "SkyColor":
		$EnvGuiServer::SkyColor = getColorF(%value);
		// Something is off about this one...
		Sky.skyColor = $EnvGuiServer::SkyColor;
		Sky.sendUpdate();

	// Should probably combine this into one
	// case "WaterScrollX" or "WaterScrollY":
	case "WaterScrollX":
		$EnvGuiServer::WaterScrollX = mClampF(%value, -10, 10);
		$EnvGuiServer::WaterScrollY = mClampF($EnvGuiServer::WaterScrollY, -10, 10);

		if (isObject(WaterPlane))
		{
			WaterPlane.scrollSpeed = $EnvGuiServer::WaterScrollX SPC $EnvGuiServer::WaterScrollY;
			WaterPlane.sendUpdate();
		}

		if (isObject(WaterZone))
		{
			%fx = $EnvGuiServer::WaterScrollX * 414;
			%fy = $EnvGuiServer::WaterScrollY * -414;

			WaterZone.appliedForce = %fx SPC %fy SPC 0;
			WaterZone.sendUpdate();
		}

	case "WaterScrollY":
		$EnvGuiServer::WaterScrollY = mClampF(%value, -10, 10);
		$EnvGuiServer::WaterScrollX = mClampF($EnvGuiServer::WaterScrollX, -10, 10);

		if (isObject(WaterPlane))
		{
			WaterPlane.scrollSpeed = $EnvGuiServer::WaterScrollX SPC $EnvGuiServer::WaterScrollY;
			WaterPlane.sendUpdate();
		}

		if (isObject(WaterZone))
		{
			%fx = $EnvGuiServer::WaterScrollX * 414;
			%fy = $EnvGuiServer::WaterScrollY * -414;

			WaterZone.appliedForce = %fx SPC %fy SPC 0;
			WaterZone.sendUpdate();
		}

	case "GroundColor":
		$EnvGuiServer::GroundColor = getColorF(%value);

		if (isObject(GroundPlane))
		{
			GroundPlane.color = getColorI($EnvGuiServer::GroundColor);
			GroundPlane.blend = getWord(GroundPlane.color, 3) < 255;
			GroundPlane.sendUpdate();

			Sky.renderBottomTexture = getWord(GroundPlane.color, 3) <= 0;
			Sky.noRenderBans = Sky.renderBottomTexture;
			Sky.sendUpdate();
		}

	case "GroundScrollX":
		$EnvGuiServer::GroundScrollX = mClampF(%value, -10, 10);
		$EnvGuiServer::GroundScrollY = mClampF($EnvGuiServer::GroundScrollY, -10, 10);

		if (isObject(GroundPlane))
		{
			GroundPlane.scrollSpeed = $EnvGuiServer::GroundScrollX SPC $EnvGuiServer::GroundScrollY;
			GroundPlane.sendUpdate();
		}

	case "GroundScrollY":
		$EnvGuiServer::GroundScrollY = mClampF(%value, -10, 10);
		$EnvGuiServer::GroundScrollX = mClampF($EnvGuiServer::GroundScrollX, -10, 10);

		if (isObject(GroundPlane))
		{
			GroundPlane.scrollSpeed = $EnvGuiServer::GroundScrollX SPC $EnvGuiServer::GroundScrollY;
			GroundPlane.sendUpdate();
		}

	case "VignetteMultiply":
		$EnvGuiServer::VignetteMultiply = mClamp(%value, 0, 1);
		sendVignetteAll();

	case "VignetteColor":
		$EnvGuiServer::VignetteColor = getColorF(%value);
		sendVignetteAll();

	default:
		return 0;
	}

	return 1;
}