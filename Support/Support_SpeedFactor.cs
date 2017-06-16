// Support_SpeedFactor.cs
// A really simple interface to scaling the speed of a player dynamically
// For allowing easy slowdown/speedups while remaining compatible with other scripts
// By Port (http://forum.blockland.us/?action=profile;u=136812)

function Player::updateSpeedFactor(%this)
{
	if(!isObject(%this))
		return;

	%data = %this.getDataBlock();
	%factor = %this.getSpeedFactor();

	%this.setMaxForwardSpeed(%data.maxForwardSpeed * %factor);
	%this.setMaxBackwardSpeed(%data.maxBackwardSpeed * %factor);
	%this.setMaxSideSpeed(%data.maxSideSpeed * %factor);
	%this.setMaxCrouchForwardSpeed(%data.maxForwardCrouchSpeed * %factor);
	%this.setMaxCrouchBackwardSpeed(%data.maxBackwardCrouchSpeed * %factor);
	%this.setMaxCrouchSideSpeed(%data.maxSideCrouchSpeed * %factor);
	%this.setMaxUnderwaterForwardSpeed(%data.maxUnderwaterForwardSpeed * %factor);
	%this.setMaxUnderwaterBackwardSpeed(%data.maxUnderwaterBackwardSpeed * %factor);
	%this.setMaxUnderwaterSideSpeed(%data.maxUnderwaterSideSpeed * %factor);
}

function Player::setSpeedFactor(%this,%factor)
{
	if(!isObject(%this))
		return;

	if(%factor < 0) %factor = 0;
	else if(%factor > 200) %factor = 200;
	%this.speedFactor = %factor;
	%this.updateSpeedFactor();
}

function Player::getSpeedFactor(%this)
{
	if(!isObject(%this))
		return;
	
	if(%this.speedFactor $= "")
	{
		%this.speedFactor = 1;
		%this.updateSpeedFactor();
	}
	return %this.speedFactor;
}