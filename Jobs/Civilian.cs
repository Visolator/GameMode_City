registerJob("Citizen",
	"As a citizen you have to eventually find a job that pays.", 
	"tree Citizenship" TAB
	"paycheck 0");

registerJob("Grocery Stocker",
	"Sell shop items!", 
	"tree Citizenship" TAB
	"expRequired 0" TAB
	"eduRequired 1" TAB
	"canSellFood 1" TAB
	"paycheck 10");

registerJob("Shopkeeper",
	"As shopkeeper you can buy store lots for less of a price.", 
	"tree Citizenship" TAB
	"expRequired 15" TAB
	"eduRequired 2" TAB
	"canSellFood 1" TAB
	"paycheck 25");

registerJob("Shop Manager",
	"Shop managers can sell food for less and still get more profit.", 
	"tree Citizenship" TAB
	"expRequired 25" TAB
	"eduRequired 3" TAB
	"canSellFood 1" TAB
	"paycheck 38");

registerJob("Shop Owner",
	"Shop owners can sell drinks cheaper. You can also sell alcohol beverages.", 
	"tree Citizenship" TAB
	"expRequired 40" TAB
	"eduRequired 4" TAB
	"canSellFood 1" TAB
	"paycheck 42");

registerJob("Arms Dealer",
	"Sell dangerous weapons, remember, you also have to pay for their stocks. This means you can't sell weapons without a stock.", 
	"tree Citizenship" TAB
	"expRequired 60" TAB
	"eduRequired 5" TAB
	"canSellWeapons 1" TAB
	"paycheck 26");

registerJob("Shareholder",
	"Sell food and weapons.", 
	"tree Citizenship" TAB
	"expRequired 80" TAB
	"eduRequired 6" TAB
	"canSellFood 1" TAB
	"canSellWeapons 1" TAB
	"paycheck 50");