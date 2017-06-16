registerJob("Pickpocket",
	"Become a pickpocket because you like to have other people's hard earned cash.", 
	"tree Criminal" TAB
	"eduRequired 1" TAB
	"jailTolerance 0.7" TAB
	"canPickpocket 1" TAB
	"paycheck 0");

registerJob("Thief",
	"The Thief is a step above Pickpocket, thieving valuable items from other people.",
	"tree Criminal" TAB
	"eduRequired 2" TAB
	"expRequired 10" TAB
	"jailTolerance 3" TAB
	"canPickpocket 1" TAB
	"paycheck 0");
	
//criminal is kind of placeholder name, looking for a better on that sounds low tiered
registerJob("Criminal",
	"tree Criminal" TAB
	"The Criminal is a step up from the normal thief in higher pay and bigger jobs." TAB
	"eduRequired 3" TAB
	"expRequired 30" TAB
	"jailTolerance 3" TAB
	"canPickpocket 1" TAB
	"canPickLock 1" TAB
	"pickLockLevel 0" TAB
	"paycheck 0");
	
//registerJob("Established Crminal",
//	"A Criminal who has established name for himself.", 
//	"tree Criminal" TAB
//	"jailTolerance 3" TAB
//	"canPickpocket 1" TAB
//	"canPickLock 1" TAB
//	"pickLockLevel 0" TAB
//	"paycheck 100");
	
//registerJob("Mafia Henchmen",
//	"This Henchmen has made it into the league of big boys. He does a lot of dirty work but for a big pay.", 
//	"tree Criminal" TAB
//	"jailTolerance 3" TAB
//	"canPickpocket 1" TAB
//	"canPickLock 1" TAB
//	"pickLockLevel 1" TAB
//	"paycheck 200");
	
//registerJob("Heist Runner",
//	"The Heist Runner is notorious for hits on police, enemy mobs, and big corporations. From these runs, the Heist Runner is tougher than other criminals.", 
//	"tree Criminal" TAB
//	"jailTolerance 4" TAB
//	"canPickpocket 1" TAB
//	"canPickLock 1" TAB
//	"pickLockLevel 2" TAB
//	"paycheck 400");
	
//registerJob("Mafia Advisor",
//	"The Mafia Advisor advises the highest of criminals and is one of the most respected criminal positions.", 
//	"tree Criminal" TAB
//	"jailTolerance 4" TAB
//	"canPickpocket 1" TAB
//	"canPickLock 1" TAB
//	"pickLockLevel 2" TAB
//	"paycheck 800");
	
//registerJob("Kingpin",
//	"The Kingpin is the top criminal. Everyone fears him.", 
//	"tree Criminal" TAB
//	"jailTolerance 8" TAB
//	"canPickpocket 1" TAB
//	"canPickLock 1" TAB
//	"pickLockLevel 3" TAB
//	"paycheck 2000");
	
//registerJob("Public Enemy #1",
//	"Notoriously wrapped in mystery, this man is the bane of the city himself.", 
//	"tree Criminal" TAB
//	"jailTolerance 15" TAB
//	"canPickPocket 1" TAB
//	"canPickLock 1" TAB
//	"pickLockLevel 3" TAB
//	"paycheck 200");

registerJob("Burgular",
	"The Burgular is the first of the intrusion class. He can break into people's homes to steal valuables!", 
	"tree Criminal" TAB
	"eduRequired 2" TAB
	"expRequired 15" TAB
	"jailTolerance 2" TAB
	"canPickpocket 1" TAB
	"canPickLock 1" TAB
	"pickLockLevel 0" TAB
	"paycheck 0");
	
registerJob("Pick Master",
	"The Pick Master has mastered the technique of getting through entrances and exits. He is better then any burgular.", 
	"tree Criminal" TAB
	"eduRequired 3" TAB
	"expRequired 35" TAB
	"jailTolerance 3" TAB
	"canPickpocket 1" TAB
	"canPickLock 1" TAB
	"pickLockLevel 1" TAB
	"paycheck 0");


registerJob("Rhino",
	"The Rhino is the brute force cracker for doors. He has the power to get through almost any locked door.", 
	"tree Criminal" TAB
	"eduRequired 5" TAB
	"expRequired 70" TAB
	"jailTolerance 4" TAB
	"canPickpocket 1" TAB
	"canPickLock 1" TAB
	"pickLockLevel 2" TAB
	"paycheck 0");


registerJob("DoorDevil",
	"Doordevil has heightened senses and can penetrate even the most secure doors. He is a force to be reckoned with.", 
	"tree Criminal" TAB
	"eduRequired 7" TAB
	"expRequired 120" TAB
	"jailTolerance 4" TAB
	"canPickpocket 1" TAB
	"canPickLock 1" TAB
	"pickLockLevel 3" TAB
	"paycheck 0");
	
registerJob("The Skeleton",
	"The Skeleton is barely even human anymore. No door is a match for him and he can take the wildest of beatings. Doors crumble to the sound of the Skeleton's name!", 
	"tree Criminal" TAB
	"eduRequired 8" TAB
	//no exp, quest annointed after doordevil reach
	"jailTolerance 5" TAB
	"canPickpocket 1" TAB
	"canPickLock 1" TAB
	"pickLockLevel 4" TAB
	"paycheck 0");
