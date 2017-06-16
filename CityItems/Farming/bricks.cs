$City::Farming::LargePlantingSoilCost = 200;
$City::Farming::PlantingSoilCost = 100;
$City::Farming::HeaterCost = 50;
$City::Farming::SprinklerCost = 100;
$City::Farming::BetterSprinklerCost = 300;
$City::Farming::WeederCost = 50;

datablock fxDTSBrickData(brickCFcorn1Data)
{
	brickFile ="./brickdata/corn0.blb";
	category = "";
	uiName = "corn1";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	nextStage = "brickCFcorn2Data";
	City_Cost = 70;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFcorn2Data)
{
	brickFile ="./brickdata/corn1.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn2";
	isGrownBrick = true;
	nextStage = "brickCFcorn3Data";
};

datablock fxDTSBrickData(brickCFcorn3Data)
{
	brickFile ="./brickdata/corn2.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn3";
	isGrownBrick = true;
	nextStage = "brickCFcorn4Data";
};

datablock fxDTSBrickData(brickCFcorn4Data)
{
	brickFile ="./brickdata/corn3.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn4";
	isGrownBrick = true;
	nextStage = "brickCFcorn5Data";
};

datablock fxDTSBrickData(brickCFcorn5Data)
{
	brickFile ="./brickdata/corn4.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn5";
	isGrownBrick = true;
	nextStage = "brickCFcorn6Data";
};

datablock fxDTSBrickData(brickCFcorn6Data)
{
	brickFile ="./brickdata/corn5.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn6";
	isGrownBrick = true;
	nextStage = "brickCFcorn7Data";
};

datablock fxDTSBrickData(brickCFcorn7Data)
{
	brickFile ="./brickdata/corn6.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn7";
	isGrownBrick = true;
	nextStage = "brickCFcorn8Data";
};

datablock fxDTSBrickData(brickCFcorn8Data)
{
	brickFile ="./brickdata/corn7.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "corn8";
	isGrownBrick = true;
	endBrick = true;
};

datablock fxDTSBrickData(brickCFpotato1Data)
{
	brickFile ="./brickdata/corn0.blb";
	category = "";
	uiName = "potato1";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	stages = 7;
	City_Cost = 80;
	CityBrickType = "Plant";
	nextStage = "brickCFpotato2Data";
};

datablock fxDTSBrickData(brickCFpotato2Data)
{
	brickFile ="./brickdata/corn1.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "potato2";
	isGrownBrick = true;
	nextStage = "brickCFpotato3Data";
};

datablock fxDTSBrickData(brickCFpotato3Data)
{
	brickFile ="./brickdata/potato3.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "potato3";
	isGrownBrick = true;
	nextStage = "brickCFpotato4Data";
};

datablock fxDTSBrickData(brickCFpotato4Data)
{
	brickFile ="./brickdata/potato4.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "potato4";
	isGrownBrick = true;
	nextStage = "brickCFpotato5Data";
};

datablock fxDTSBrickData(brickCFpotato5Data)
{
	brickFile ="./brickdata/potato5.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "potato5";
	isGrownBrick = true;
	nextStage = "brickCFpotato6Data";
};

datablock fxDTSBrickData(brickCFpotato6Data)
{
	brickFile ="./brickdata/potato6.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "potato6";
	isGrownBrick = true;
	nextStage = "brickCFpotato7Data";
};

datablock fxDTSBrickData(brickCFpotato7Data)
{
	brickFile ="./brickdata/potato7.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "potato7";
	isGrownBrick = true;
	endBrick = true;
};

datablock fxDTSBrickData(brickCFwheat1Data)
{
	brickFile ="./brickdata/corn0.blb";
	category = "";
	uiName = "wheat1";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	City_Cost = 60;
	CityBrickType = "Plant";
	nextStage = "brickCFwheat2Data";
};

datablock fxDTSBrickData(brickCFwheat2Data)
{
	brickFile ="./brickdata/wheat1.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat2";
	isGrownBrick = true;
	nextStage = "brickCFwheat3Data";
};

datablock fxDTSBrickData(brickCFwheat3Data)
{
	brickFile ="./brickdata/wheat2.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat3";
	isGrownBrick = true;
	nextStage = "brickCFwheat4Data";
};

datablock fxDTSBrickData(brickCFwheat4Data)
{
	brickFile ="./brickdata/wheat3.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat4";
	isGrownBrick = true;
	nextStage = "brickCFwheat5Data";
};

datablock fxDTSBrickData(brickCFwheat5Data)
{
	brickFile ="./brickdata/wheat4.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat5";
	isGrownBrick = true;
	nextStage = "brickCFwheat6Data";
};

datablock fxDTSBrickData(brickCFwheat6Data)
{
	brickFile ="./brickdata/wheat5.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat6";
	isGrownBrick = true;
	nextStage = "brickCFwheat7Data";
};

datablock fxDTSBrickData(brickCFwheat7Data)
{
	brickFile ="./brickdata/wheat6.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat7";
	isGrownBrick = true;
	nextStage = "brickCFwheat8Data";
};

datablock fxDTSBrickData(brickCFwheat8Data)
{
	brickFile ="./brickdata/wheat7.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat8";
	isGrownBrick = true;
	nextStage = "brickCFwheat9Data";
};

datablock fxDTSBrickData(brickCFwheat9Data)
{
	brickFile ="./brickdata/wheat8.blb";
	category = "";
	subCategory = "Unplaceable";
	uiName = "wheat9";
	isGrownBrick = true;
	endBrick = true;
};

datablock fxDTSBrickData(brickCFtomato1Data)
{
	brickFile ="./brickdata/tomato0.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato1";
	nextStage = "brickCFtomato2Data";
	City_Cost = 50;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFtomato2Data)
{
	brickFile ="./brickdata/tomato1.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato2";
	nextStage = "brickCFtomato3Data";
};

datablock fxDTSBrickData(brickCFtomato3Data)
{
	brickFile ="./brickdata/tomato2.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato3";
	nextStage = "brickCFtomato4Data";
};

datablock fxDTSBrickData(brickCFtomato4Data)
{
	brickFile ="./brickdata/tomato3.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato4";
	nextStage = "brickCFtomato5Data";
};


datablock fxDTSBrickData(brickCFtomato5Data)
{
	brickFile ="./brickdata/tomato4.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato5";
	nextStage = "brickCFtomato6Data";
};


datablock fxDTSBrickData(brickCFtomato6Data)
{
	brickFile ="./brickdata/tomato5.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato6";
	nextStage = "brickCFtomato7Data";
};


datablock fxDTSBrickData(brickCFtomato7Data)
{
	brickFile ="./brickdata/tomato6.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato7";
	nextStage = "brickCFtomato8Data";
};


datablock fxDTSBrickData(brickCFtomato8Data)
{
	brickFile ="./brickdata/tomato7.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato8";
	nextStage = "brickCFtomato9Data";
};

datablock fxDTSBrickData(brickCFtomato9Data)
{
	brickFile ="./brickdata/tomato8.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "tomato9";
	nextStage = "brickCFtomato10Data";
};

datablock fxDTSBrickData(brickCFtomato10Data)
{
	brickFile ="./brickdata/tomato9.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	endBrick = true;
	uiName = "tomato10";
};

datablock fxDTSBrickData(brickCFgrape1Data)
{
	brickFile ="./brickdata/grape1.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape1";
	nextStage = "brickCFgrape2Data";
};

datablock fxDTSBrickData(brickCFgrape2Data)
{
	brickFile ="./brickdata/grape2.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape2";
	nextStage = "brickCFgrape3Data";
	lastStage = "brickCFgrape1Data";
};

datablock fxDTSBrickData(brickCFgrape3Data)
{
	brickFile ="./brickdata/grape3.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape3";
	nextStage = "brickCFgrape4Data";
	lastStage = "brickCFgrape2Data";
};


datablock fxDTSBrickData(brickCFgrape4Data)
{
	brickFile ="./brickdata/grape4.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape4";
	nextStage = "brickCFgrape5Data";
	lastStage = "brickCFgrape3Data";
};


datablock fxDTSBrickData(brickCFgrape5Data)
{
	brickFile ="./brickdata/grape5.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape5";
	nextStage = "brickCFgrape6Data";
	lastStage = "brickCFgrape4Data";
};

datablock fxDTSBrickData(brickCFgrape6Data)
{
	brickFile ="./brickdata/grape6.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape6";
	nextStage = "brickCFgrape7Data";
	lastStage = "brickCFgrape5Data";
};

datablock fxDTSBrickData(brickCFgrape7Data)
{
	brickFile ="./brickdata/grape7.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape7";
	nextStage = "brickCFgrape8Data";
	lastStage = "brickCFgrape6Data";
};

datablock fxDTSBrickData(brickCFgrape8Data)
{
	brickFile ="./brickdata/grape8.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "Grape8";
	endBrick = true;
	lastStage = "brickCFgrape7Data";
};


datablock fxDTSBrickData(brickCFweed1Data)
{
	brickFile ="./brickdata/weed1.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed1";
	nextStage = "brickCFweed2Data";
	City_Cost = 50;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFweed2Data)
{
	brickFile ="./brickdata/weed2.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed2";
	nextStage = "brickCFweed3Data";
	lastStage = "brickCFweed1Data";
};

datablock fxDTSBrickData(brickCFweed3Data)
{
	brickFile ="./brickdata/weed3.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed3";
	nextStage = "brickCFweed4Data";
	lastStage = "brickCFweed2Data";
};

datablock fxDTSBrickData(brickCFweed4Data)
{
	brickFile ="./brickdata/weed4.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed4";
	nextStage = "brickCFweed5Data";
	lastStage = "brickCFweed3Data";
};


datablock fxDTSBrickData(brickCFweed5Data)
{
	brickFile ="./brickdata/weed5.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed5";
	nextStage = "brickCFweed6Data";
	lastStage = "brickCFweed4Data";
};


datablock fxDTSBrickData(brickCFweed6Data)
{
	brickFile ="./brickdata/weed6.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed6";
	nextStage = "brickCFweed7Data";
	lastStage = "brickCFweed5Data";
};


datablock fxDTSBrickData(brickCFweed7Data)
{
	brickFile ="./brickdata/weed7.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed7";
	nextStage = "brickCFweed8Data";
	lastStage = "brickCFweed6Data";
};


datablock fxDTSBrickData(brickCFweed8Data)
{
	brickFile ="./brickdata/weed8.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed8";
	nextStage = "brickCFweed9Data";
	lastStage = "brickCFweed7Data";
};

datablock fxDTSBrickData(brickCFweed9Data)
{
	brickFile ="./brickdata/weed9.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "weed9";
	nextStage = "brickCFweed10Data";
	lastStage = "brickCFweed8Data";
};

datablock fxDTSBrickData(brickCFweed10Data)
{
	brickFile ="./brickdata/weed10.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	endBrick = true;
	uiName = "weed10";
	lastStage = "brickCFweed9Data";
};

datablock fxDTSBrickData(brickCFblueberries1Data)
{
	brickFile ="./brickdata/blueberries1.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries1";
	nextStage = "brickCFblueberries2Data";
	City_Cost = 50;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFblueberries2Data)
{
	brickFile ="./brickdata/blueberries2.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries2";
	nextStage = "brickCFblueberries3Data";
	lastStage = "brickCFblueberries1Data";
};

datablock fxDTSBrickData(brickCFblueberries3Data)
{
	brickFile ="./brickdata/blueberries3.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries3";
	nextStage = "brickCFblueberries4Data";
	lastStage = "brickCFblueberries2Data";
};

datablock fxDTSBrickData(brickCFblueberries4Data)
{
	brickFile ="./brickdata/blueberries4.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries4";
	nextStage = "brickCFblueberries5Data";
	lastStage = "brickCFblueberries3Data";
};


datablock fxDTSBrickData(brickCFblueberries5Data)
{
	brickFile ="./brickdata/blueberries5.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries5";
	nextStage = "brickCFblueberries6Data";
	lastStage = "brickCFblueberries4Data";
};


datablock fxDTSBrickData(brickCFblueberries6Data)
{
	brickFile ="./brickdata/blueberries6.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries6";
	nextStage = "brickCFblueberries7Data";
	lastStage = "brickCFblueberries5Data";
};


datablock fxDTSBrickData(brickCFblueberries7Data)
{
	brickFile ="./brickdata/blueberries7.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries7";
	nextStage = "brickCFblueberries8Data";
	lastStage = "brickCFblueberries6Data";
};


datablock fxDTSBrickData(brickCFblueberries8Data)
{
	brickFile ="./brickdata/blueberries8.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries8";
	nextStage = "brickCFblueberries9Data";
	lastStage = "brickCFblueberries7Data";
};

datablock fxDTSBrickData(brickCFblueberries9Data)
{
	brickFile ="./brickdata/blueberries9.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "blueberries9";
	nextStage = "brickCFblueberries10Data";
	lastStage = "brickCFblueberries8Data";
};

datablock fxDTSBrickData(brickCFblueberries10Data)
{
	brickFile ="./brickdata/blueberries10.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	endBrick = true;
	uiName = "blueberries10";
	lastStage = "brickCFblueberries9Data";
};

datablock fxDTSBrickData(brickCFfenceData)
{
	brickFile ="./brickdata/fence.blb";
	category = "";
	subCategory = "City";
	uiName = "Plant Fence";
};

datablock fxDTSBrickData(brickCFheatData : brick2x2Data)
{
	uiName = "Heater";
	category = "City";
	subCategory = "Technology";
	City_Cost = $City::Farming::HeaterCost;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFdirtData : brick2x2Data)
{
	uiName = "Planting Soil";
	category = "City";
	subCategory = "Planting";
	City_Cost = $City::Farming::PlantingSoilCost;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFbushData : brick4x4Data)
{
	uiName = "Large Planting Soil";
	category = "City";
	subCategory = "Planting";
	City_Cost = $City::Farming::LargePlantingSoilCost;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFwaterData : brick2x2Data)
{
	uiName = "Sprinkler";
	category = "City";
	subCategory = "Technology";
	City_Cost = $City::Farming::SprinklerCost;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFbetterWaterData : brick2x2Data)
{
	uiName = "Better Sprinkler";
	category = "City";
	subCategory = "Technology";
	City_Cost = $City::Farming::BetterSprinklerCost;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFweederData : brick2x2Data)
{
	uiName = "Weed killer";
	category = "City";
	subCategory = "Technology";
	City_Cost = $City::Farming::WeederCost;
	CityBrickType = "Plant";
};


datablock fxDTSBrickData(brickCFstrawberries1Data)
{
	brickFile ="./brickdata/strawberry1.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries1";
	nextStage = "brickCFstrawberries2Data";
	City_Cost = 50;
	CityBrickType = "Plant";
};

datablock fxDTSBrickData(brickCFstrawberries2Data)
{
	brickFile ="./brickdata/strawberry2.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries2";
	nextStage = "brickCFstrawberries3Data";
	lastStage = "brickCFstrawberries1Data";
};

datablock fxDTSBrickData(brickCFstrawberries3Data)
{
	brickFile ="./brickdata/strawberry3.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries3";
	nextStage = "brickCFstrawberries4Data";
	lastStage = "brickCFstrawberries2Data";
};

datablock fxDTSBrickData(brickCFstrawberries4Data)
{
	brickFile ="./brickdata/strawberry4.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries4";
	nextStage = "brickCFstrawberries5Data";
	lastStage = "brickCFstrawberries3Data";
};


datablock fxDTSBrickData(brickCFstrawberries5Data)
{
	brickFile ="./brickdata/strawberry5.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries5";
	nextStage = "brickCFstrawberries6Data";
	lastStage = "brickCFstrawberries4Data";
};


datablock fxDTSBrickData(brickCFstrawberries6Data)
{
	brickFile ="./brickdata/strawberry6.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries6";
	nextStage = "brickCFstrawberries7Data";
	lastStage = "brickCFstrawberries5Data";
};


datablock fxDTSBrickData(brickCFstrawberries7Data)
{
	brickFile ="./brickdata/strawberry7.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries7";
	nextStage = "brickCFstrawberries8Data";
	lastStage = "brickCFstrawberries6Data";
};


datablock fxDTSBrickData(brickCFstrawberries8Data)
{
	brickFile ="./brickdata/strawberry8.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries8";
	nextStage = "brickCFstrawberries9Data";
	lastStage = "brickCFstrawberries7Data";
};

datablock fxDTSBrickData(brickCFstrawberries9Data)
{
	brickFile ="./brickdata/strawberry9.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries9";
	nextStage = "brickCFstrawberries10Data";
	lastStage = "brickCFstrawberries8Data";
};

datablock fxDTSBrickData(brickCFstrawberries10Data)
{
	brickFile ="./brickdata/strawberry10.blb";
	category = "";
	isGrownBrick = true;
	subCategory = "Unplaceable";
	uiName = "strawberries10";
	lastStage = "brickCFstrawberries9Data";
};
