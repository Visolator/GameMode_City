function reqmats(%brick)
{
		%reqmats = 1;
	
		%bx = %brick.getDatablock().bricksizex;
		%by = %brick.getDatablock().bricksizey;
		%bz = %brick.getDatablock().bricksizez;

		%totalsize = %bx*%by*%bz;

		%reqmats = mCeil(%totalsize/10);

		if(getWord(%brick.position,1) > -37)
			return %reqmats;
		else
			return %reqmats*2;
}

function fxDtsBrick::onPlant_Farming(%this, %bypass)
{
	talk("called");
}