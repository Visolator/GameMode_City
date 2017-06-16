function striReplace(%source,%search,%replace)
{
	if(%search $= "")
		return %source;
	
	%lena = strLen(%source);
	%lenb = strLen(%search);
	
	for(%i = 0; %i < %lena; %i++)
	{
		%part = getSubStr(%source,%i,%lenb);
		
		if(%part !$= %search)
		{
			%clean = %clean @ getSubStr(%source,%i,1);
			continue;
		}
		
		%clean = %clean @ %replace;
		%i += %lenb - 1;
	}
	
	return %clean;
}

function mStringReplace(%source, %searchTokens, %replace)
{
	while("" !$= %searchTokens)
	{
		%searchTokens = nextToken(%searchTokens, "stripToken", ",");
		
		if(%text $= "")
			%text = striReplace(%source, %stripToken, %replace);
		else
			%text = striReplace(%text, %stripToken, %replace);

	}	
	return %text;
}

function containsSubstring(%msg, %searchTokens)
{
	while("" !$= %searchTokens)
	{
		%searchTokens = nextToken(%searchTokens, "stripToken", ",");
		%pos = strPos(%msg, %stripToken);
		
		if(%pos == -1)
			continue;
		
		if(getSubStr(%msg, %pos, strLen(%stripToken)) $= %stripToken)
			return 1;
	}
	return 0;
}

function stripRepeatingChars(%string)
{	
	%length = strLen(%string);
	
	for(%i = 0; %i < %length; %i++)
	{		
		%current = getSubStr(%string, %i, 1);
		
		if (%current !$= %previous)
		{
			%strip = %strip @ %current;
		}
		%previous = %current;
	}
	return %strip;
}

function stringNextToString(%string, %searchTokens)
{
	while("" !$= %searchTokens)
	{
		%searchTokens = nextToken(%searchTokens, "stripToken", ",");
		%string = strLwr(%string);
		%len = strLen(%stripToken);
		%pos = strPos(%string, %stripToken);
		
		if(%pos == -1)
			continue;
		
		if(%string $= getSubStr(%string, %pos, %len))
		{
			//they entire message only contains %stripToken
			return 1;
		}
		else if(getSubStr(%string, %pos + %len, 1) $= " ")
		{
			return 1;
		}
		else if(%pos - 1 != -1)
		{
			if(getSubStr(%string, %pos - 1, 1) $= " ")
			{
				return 1;
			}
		}	
		else if(getSubStr(%string, %pos, %len) $= getSubStr(%string, %pos + %len, %len))
		{
			return 1;
		}
	}
	return 0;
}