datablock PlayerData(mCuff_PlayerData : PlayerStandardArmor)
{
   canJet = 0;
   uiName = "";
};

package mCuff_Package
{
   function Armor::onCollision(%data, %obj, %col, %a, %b, %c, %d, %e, %f)
   {
      if(%col.getDatablock() == nameToID(mCuff_CuffsItem) && %col.canPickup)
      {
         for(%i=0;%i<%data.maxTools;%i++)
         {
            if(!%obj.tool[%i])
            {
              %obj.pickup(%col);
               return;
            }
         }
      }
      Parent::onCollision(%data, %obj, %col, %a, %b, %c, %d, %e, %f);
   }
   function Player::mountImage(%player,%image,%slot)
   {
      if(%player.getMountedImage(%slot) == nameToID(mCuff_EquipImage))
         return;
      Parent::mountImage(%player,%image,%slot);
   }
   function serverCmdUseTool(%client,%slot)
   {
      %player = %client.player;
      if(!isObject(%player))
         return parent::serverCmdUseTool(%client,%slot);
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      parent::serverCmdUseTool(%client,%slot);
   }
   function serverCmdDropTool(%client,%slot)
   {
      %player = %client.player;
      if(!isObject(%player))
         return parent::serverCmdDropTool(%client,%slot);
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      parent::serverCmdDropTool(%client,%slot);
   }
   function servercmdRotateBrick(%client,%direction)
   {
      %player = %client.player;
      if(!isObject(%player))
         return Parent::servercmdRotateBrick(%client,%direction);
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      Parent::servercmdRotateBrick(%client,%direction);
   }
   function servercmdShiftBrick(%client,%x,%y,%z)
   {
      %player = %client.player;
      if(!isObject(%player))
         return Parent::servercmdShiftBrick(%client,%x,%y,%z);
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      Parent::servercmdShiftBrick(%client,%x,%y,%z);
   }
   function serverCmdUnUseTool(%client)
   {
      %player = %client.player;
      if(!isObject(%player))
         return Parent::serverCmdUnUseTool(%client);
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      Parent::serverCmdUnUseTool(%client);
   }
   function servercmdActivateStuff(%client)
   {
       %player = %client.player;
      if(!isObject(%player))
         return Parent::servercmdActivateStuff(%client);
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      Parent::servercmdActivateStuff(%client);
   }
   function Player::ActivateStuff(%player)
   {
      if(%player.getDatablock() == nameToID(mCuff_PlayerData))
         return;
      Parent::activateStuff(%player);
   }
};
activatePackage(mCuff_Package);
datablock ItemData(mCuff_CuffsItem:PrintGun)
{
   uiname = "Hand Cuffs";
   shapefile = "./HandCuffItem.dts";
   image=mCuff_CuffsImage;
   colorShiftColor = "0.7 0.7 0.7 1";
   iconName = "";
};
datablock ShapeBaseImageData(mCuff_CuffsImage:PrintGunImage)
{
   eyeOffset = "";
   shapeFile = "./handCuffItem.dts";
   stateEmitter[2] = "";
   stateSound[2] = "";
   showbricks=0;
};
datablock ItemData(mCuff_KeyItem:PrintGun)
{
   uiname = "Hand Cuffs Keys";
   shapefile = "./HandCuffKey.dts";
   image=mCuff_KeyImage;
   colorShiftColor = "0.7 0.7 0.7 1";
   iconName = "";
};
datablock ShapeBaseImageData(mCuff_KeyImage:PrintGunImage)
{
   eyeOffset = "";
   shapeFile = "./handCuffKey.dts";
   stateEmitter[2] = "";
   stateSound[2] = "";
   showbricks=0;
};
datablock ShapeBaseImageData(mCuff_EquipImage:printGunImage)
{
  shapeFile = "./handCuffEquip.dts";
  mountPoint = 0;
  armReady=1;
  showbricks=0;
  offset = "0.07 0 0";
  colorShiftColor = "0.7 0.7 0.7 1";
  stateSound[2]="";
  stateEmitter[2]="";
};
function mCuff_EquipImage::onFire(%data,%obj,%slot)
{
   serverPlay3d(errorSound,%obj.getTransform());
}
function mCuff_CuffsImage::onFire(%data,%obj,%slot)
{
   %obj.playThread(2,"spearThrow");
   if(%obj.mCuff_Attempt(1))
   {
      %obj.tool[%obj.currTool]=0;
      if(isObject(%obj.client))
         messageClient(%obj.client,'MsgItemPickup','',%obj.currTool,0);
      %obj.updateArm(0);
      %obj.unMountImage(0);
      serverPlay3d(BrickPlantSound,%obj.getTransform());
   }
   else
      serverPlay3d(errorSound,%obj.getTransform());
}
function mCuff_KeyImage::onFire(%data,%obj,%slot)
{
   %obj.playThread(2,"shiftLeft");
   if(%obj.mCuff_Attempt(0))
   {
      serverPlay3d(BrickPlantSound,%obj.getTransform());
   }
   else
      serverPlay3d(errorSound,%obj.getTransform());
}
function Player::mCuff_isCuffed(%obj)
{
   return %obj.getDatablock() == nameToID(mCuff_PlayerData) && %obj.getMountedImage(0) == nameToID(mCuff_EquipImage);
}
function Player::mCuff_setCuffed(%obj,%bool)
{
   if(%bool)
   {
      serverCmdUnUseTool(%obj.client);
      %obj.setDatablock(mCuff_PlayerData);
      %obj.mountImage(mCuff_EquipImage,0);
      %obj.playThread(2,"armReadyBoth");
   }
   else
   {
      %minigame = getMinigameFromObject(%obj.client);
      %data = (isObject(%minigame)?%minigame.playerDatablock:PlayerStandardArmor);
      %obj.setDatablock(%data);
      %obj.unMountImage(0);
      %obj.playThread(2,"root");
   }
}
function Player::mCuff_Attempt(%obj,%bool)
{
   %start = %obj.getEyePoint();
   %vector = getWords(%obj.getForwardVector(),0,1) SPC getWord(%obj.getEyeVector(),2);
   %end = vectorAdd(%start,vectorScale(%vector,3));
   %mask = $TypeMasks::PlayerObjectType | $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType;
   %ray = containerRaycast(%start,%end,%mask,%obj);
   %col = firstWord(%ray);
   if(isObject(%col))
   {
      if(%col.getClassName()!$="Player")
         return 0;
      if(!isObject(getMinigameFromObject(%obj.client)) || getMinigameFromObject(%obj.client) != getMinigameFromObject(%col.client))
         return 0;
      if(%col.mCuff_isCuffed()==%bool)
         return 0;
      %col.mCuff_setCuffed(%bool);
      return 1;
   }
   else
      return 0;
}