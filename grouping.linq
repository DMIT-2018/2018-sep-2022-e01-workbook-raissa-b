<Query Kind="Expression">
  <Connection>
    <ID>c5c58f2a-b12e-45e9-89a1-88c4a3cc4714</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\MSSQLServer1</Server>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

//Grouping 

//when you create a group it builds two (2) components 
//a) Key Component (deciding criteria value(s)) defining the group
//		reference this component using the groupname.Key[.propertyname]
//		1 value for key: groupname.Key
//		n values for key: groupname.Key.propertyname
// (property < - > field < - > attribute < - > value)
//b) data of the group (raw instances of the collection) 

//Ways to Group: 
//a) by a single column (field, attribute, property) 
		//groupname.Key
//b) by a set of columns (anonymous dataset)		
		//groupname.Key.property
//c) by using an entity (entity name/navproperty)
		//groupname.Key.property
		
		
//Concept Processing 
//start with a "pile" of data (original collection prior to grouping)
//specify the grouping property or properties 
//result of the group operation will be to "place the data into smaller piles"
		// the piles are dependent on the grouping propert(ies) value(s)
		// the grouping propert(ies) become the Key 
		// the individual instances are the data in the smaller piles 
		// the entire individual instance of the original collection is place in the smaller pile 
	//manipulate each of the "smaller piles" using your linq commands 
	
//grouping is different than Ordering -- 
//Ordering is the final re-sequencing of a collection for display 
//Grouping re-organizes a collection into separate, usually smaller collections for further processing
		//ie aggregates 
		
//Grouping is an excellent way to organize your data especially if:
		// you need to process data on a property that is "NOT" a relative key 
		// such as a foreign key which form a "natural" group using the navigational properties 
		
		
//Display albums by release year 
 //this request does not need grouping 
 //this request is an ordering of output: OrderBy 
 //this ordering affects only display 
 
 Albums
 	.OrderBy(a => a.ReleaseYear)
	
//Display albums grouped by ReleaseYear 
	//explicit requesst to break up the display into desired "piles"
Albums
	.GroupBy(a => a.ReleaseYear)

//Processing on the groups created by the Group command 

//Display the number of albums produced each year 
Albums
	.GroupBy(a => a.ReleaseYear)
	.Select(eachgroupPile => new 
	{
		Year = eachgroupPile.Key,
		NumOfAlbums = eachgroupPile.Count()
	})

//Display the number of albums produced each year and list only the years which have more than 10 albums

Albums
	.GroupBy(a => a.ReleaseYear)
	.Where(egP => egP.Count() > 10) //egP = each group pile //filtering against each group pile
	.Select(eachgroupPile => new
	{
		Year = eachgroupPile.Key,
		NumOfAlbums = eachgroupPile.Count()
	})

//or 

Albums
	.GroupBy(a => a.ReleaseYear)
	//.Where(egP => egP.Count() > 10) //egP = each group pile //filtering against each group pile
	.Select(eachgroupPile => new
	{
		Year = eachgroupPile.Key,
		NumOfAlbums = eachgroupPile.Count()
	})
	.Where (x => x.NumOfAlbums > 10 ) //filtering against the opt of the .Select() command




//Use a multiple set of properties to form the group 
//Include a nested query to report on the small pile group 


//Display albums grouped by ReleaseLabel, ReleaseYear. 
//Display the Release year and number of albums. 
//List only the years with 10 or more albums released.
//For each album, display the title and release year. 

Albums
	.GroupBy(a => new { a.ReleaseLabel, a.ReleaseYear})
	.Where(egP => egP.Count() > 2) //filtering against each group pile
	.Select(eachgroupPile => new
	{
		Label = eachgroupPile.Key.ReleaseLabel, 
		Year = eachgroupPile.Key.ReleaseYear,
		NumOfAlbums = eachgroupPile.Count(),
		AlbumItems = eachgroupPile
							.Select(egPInstance => new {
							title = egPInstance.Title, 
							YearofAlbum = egPInstance.ReleaseYear
							})//to display year and title
	})


//

Albums
	.GroupBy(a => new { a.ReleaseLabel, a.ReleaseYear })
	.Where(egP => egP.Count() > 2) //filtering against each group pile
	.Select(eachgroupPile => new
	{
		Label = eachgroupPile.Key.ReleaseLabel,
		Year = eachgroupPile.Key.ReleaseYear,
		NumOfAlbums = eachgroupPile.Count(),
		AlbumItems = eachgroupPile
							.Select(egPInstance => new
							{
								title = egPInstance.Title,
								artist= egPInstance.Artist.Name,
								trackcount = egPInstance.Tracks
												.Select(x => x), //a navigational property Tracks
								YearofAlbum = egPInstance.ReleaseYear
							})//to display year and title
	})
Albums
	.GroupBy(a => new { a.ReleaseLabel, a.ReleaseYear })
	.Where(egP => egP.Count() > 2) //filtering against each group pile
	.ToList() // this forces collection into local memory for further processing trackcountA
	.Select(eachgroupPile => new
	{
		Label = eachgroupPile.Key.ReleaseLabel,
		Year = eachgroupPile.Key.ReleaseYear,
		NumOfAlbums = eachgroupPile.Count(),
		AlbumItems = eachgroupPile
							.Select(egPInstance => new
							{
								title = egPInstance.Title,
								artist = egPInstance.Artist.Name,
								trackcountA = egPInstance.Tracks.Count(),
								trackcountB = egPInstance.Tracks.Count(), //a navigational property Tracks
								YearofAlbum = egPInstance.ReleaseYear
							})//to display year and title
	})




