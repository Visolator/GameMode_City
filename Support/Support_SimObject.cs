//Thanks Greek2Me

$Support::ChainBatchSize = 1000;
$Support::ChainTimeOut = 100;

// +-----------+
// | SimObject |
// +-----------+
function SimObject::call(%this,%method,%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16,%v17)
{
	%lastNull = -1;
	for(%i = 0; %i < 18; %i ++)
	{
		%a = %v[%i];
		if(%a $= "")
		{
			if(%lastNull < 0)
				%lastNull = %i;
			continue;
		}
		else
		{
			if(%lastNull >= 0)
			{
				for(%e = %lastNull; %e < %i; %e ++)
				{
					if(%args !$= "")
						%args = %args @ ",";
					%args = %args @ "\"\"";
				}
				%lastNull = -1;
			}
			if(%args !$= "")
				%args = %args @ ",";
			%args = %args @ "\"" @ %a @ "\"";
		}
	}

	eval(%this @ "." @ %method @ "(" @ %args @ ");");
}

function SimObject::getGroupID(%this)
{
	%parent = %this.getGroup();
	if(!isObject(%parent))
		return -1;

	for(%i=0; %i < %parent.getCount(); %i++)
	{
		%obj = %parent.getObject(%i);
		if(%obj == %this)
			return %i;
	}

	return -1;
}

// +----------+
// | SimGroup |
// +----------+
function SimGroup::chainMethodCall(%this,%method,%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16,%v17)
{
	cancel(%this.chain_schedule);

	%batch = (%this.chain_batchSize $= "" ? $Support::ChainBatchSize : %this.chain_batchSize);
	%count = %this.getCount();
	%index = (%this.chain_index $= "" ? %count - 1 : %this.chain_index);
	%endIndex = (%index - %batch < 0 ? 0 : %index - %batch);

	for(%i = %index; %i >= %endIndex; %i --)
	{
		%obj = %this.getObject(%i);
		%obj.call(%method,%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16,%v17);
	}
	%this.chain_index = %endIndex - 1;
	if(%this.chain_index <= 0)
	{
		if(isFunction(%this,%this.chain_callback))
			%this.call(%this.chain_callback);
		%this.chain_index = "";
		%this.chain_batchSize = "";
		%this.chain_timeOut = "";
		%this.chain_callback = "";
	}
	else
	{
		%time = (%this.chain_timeOut $= "" ? $Support::ChainTimeOut : %this.chain_timeOut);
		%this.chain_schedule = %this.schedule(%time,"chainMethodCall",%method,%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16,%v17);
	}
}