<Query Kind="Statements">
  <Connection>
    <ID>c5c58f2a-b12e-45e9-89a1-88c4a3cc4714</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\MSSQLServer1</Server>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

//Aggregates 
//.Count() counts the number of instances in the collection 
//.Sum( x => ....) sums (totals) a numeric field (numeric expression) in a collection
//.Min ( x => ...) finds the minimum value of a collection for a field 
//.Max ( x => ...) finds the maximum value of a collection for a field 
//.Average ( x => ...) finds the average value of a numeric field (numeric expression) in a collection

// !!IMPORTANT!! 
	//Aggregates only work on a collection of values 
	//Aggregates DO NOT work on a single instance (non-declared collection) 
	//.Sum, .Min, .Max, and .Average must have at least one record in their collection
	//.Sum and .Average MUST work on numeric fields and the field CANNOT be null 
	
//syntax: method 
//collectionset.aggregate( x => expression)
//collectionset.Select(...).aggregate()
//collectionset.Count // .Count() does not contain an expression 

	
//for Sum, Min, Max, and Average: the result is a single value 
//you can use multiple aggregates on a single column
		// .Sum(x => expression).Min(x => expression)
		
//Questions
//Find the average playing time (length) of tracks in our music collection 

		//thought process
		//average is an aggregate that needs a collection -- so what is the collection ? the tracks table is a collection 
		//what is the expression? the length of play is in milliseconds
		
		
		////Tracks.Average( x => x.Milliseconds) //for each record give me the milliseconds and give me the average
										//each x has multiple fields 
		//or you can write it like this: 
		
		////Tracks.Select( x => x.Milliseconds).Average() //a single list of numbers 
		
		//Tracks.Average() aborts because no specific field was referred to on the record 
		
//List all albums of the 60s showing the title artist and various aggregates for albums containing tracks

//For each album show the number of tracks, the total price of all tracks and the average playing length of the album tracks 

		//thought process 
		//start at albums 
		//can I get the artist name from albums? (yes from the navigational property .Artist) 
		//can I get a collection of tracks for an album? (yes from the navigational property x.Tracks)
		//can I get number of tracks in the collection (.Count()) 
		//can I get the total price of the tracks (yes using .Sum() of the UnitPrice) 
		//can I get the average of the play length (yes using .Average() of the Milliseconds ) 
		
		
		
		Albums
			.Where( x => x.Tracks.Count() > 0)//using an aggregate on a where clause 
					&& (x.ReleaseYear > 1959 && x.ReleaseYear < 1970));
					//new because we are generating fields
			.Select(x => new 
			{ 
				Title = x.Title, 
				Artist = x.ArtistName, 
				NumberofTracks = x.Tracks.Count(),
				TotalPrice = x.Tracks.Sum(tr => tr.UnitPrice),
				AverageTrackLength = x.Tracks.Select(tr => tr.Milliseconds).Average()				
			})
			;
			
			
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
