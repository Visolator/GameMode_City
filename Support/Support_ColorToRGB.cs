function rgbToHex(%rgb)
{
    %r = _compToHex(255 * getWord(%rgb,0));
    %g = _compToHex(255 * getWord(%rgb,1));
    %b = _compToHex(255 * getWord(%rgb,2));
    return %r @ %g @ %b;
}
 
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

function greenToCyan(%a)
{
    %g = 1;
    %b = 1;
    if(%a >= (1/2))
        %g = mAbs(%a - 1) * 2;
    if(%a < (1/2))
        %b = %a * 2;
    return "0" SPC %g SPC %b;
}