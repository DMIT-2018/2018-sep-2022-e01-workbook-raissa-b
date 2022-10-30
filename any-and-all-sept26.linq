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

//Monday September 26 
//Any and All 
//these filter tests return a true or false condition 
//they work at the complete collection level 

//Genres.Count().Dump(); 
//25

//Show genres that have tracks which are not on any playlist 
Genres	
	.Where (g => g.Tracks.Any(tr => tr.PlaylistTracks.Count() == 0))
	//take the track and find if there is any Playlist for that track and check the count of it and see if it is equal to 0
	.Select(g => g)
	//.Dump() //17 items
	;
	
	
//Show genres that have all their tracks appearing atleast once on a playlist 
		//comparing multiple collections
Genres 
	.Where(g => g.Tracks.All(tr => tr.PlaylistTracks.Count() > 0 ))
	.Select (g => g)
	//.Dump()
	;
	
//there maybe times that using a !Any() -> All(!relationship)
// 		and !All -> Any(!relationship)


//Using All and Any in comparing 2 collections 
//if your collecion is NOT a complex record there is a Linq method called .Except that can be used to solve your query 
//Dot Net Tutorials for Except method


//Compare the track collection of 2 people using All and Any 
//reoberto, almeida, and michelle brooks 

var almeida = PlaylistTracks
			.Where(x => x.Playlist.UserName.Contains("AlmeidaR"))
			.Select(x => new
			{
				song = x.Track.Name, 
				genre = x.Track.Genre.Name,
				id = x.TrackId, 
				artist = x.Track.Album.Artist.Name
				
			})
			.Distinct()
			.OrderBy (x => x.song)
			//.Dump() //110 songs
			;


var brooks = PlaylistTracks
			.Where(x => x.Playlist.UserName.Contains("BrooksM"))
			.Select(x => new
			{
				song = x.Track.Name,
				genre = x.Track.Genre.Name,
				id = x.TrackId,
				artist = x.Track.Album.Artist.Name

			})
			.Distinct()
			.OrderBy(x => x.song)
			//.Dump() //88 songs
			;

//List the tracks that both Roberto and Michelle like 

//Compare 2 datasets together, data in listA that is also in listB
//Assume listA is Roberto and listB is Michelle 
//listA is what you wish to report from 
//listB is what you wish to compare to 

//What songs does Roberto like but not Michelle

var c1 = almeida 
			.Where (rob => !brooks.Any(mic => mic.id == rob.id))
			.OrderBy(rob => rob.song)
			//.Dump() //109 songs on rob's list that are not on michelle's list 
			;

var c2 = almeida
			.Where(rob => brooks.All(mic => mic.id != rob.id))
			.OrderBy(rob => rob.song)
			//.Dump() //109 songs on rob's list that are not on michelle's list 
			;
			
var c3 = brooks 
			.Where (mic => almeida.All(rob => rob.id != mic.id))
			.OrderBy(mic => mic.song)
			.Dump() //87 
			;
			
//What songs does both michelle and roberto like 
var c4 = brooks 
	.Where(mic => almeida.Any(rob => rob.id == mic.id))
	.OrderBy(mic => mic.song)
	.Dump() //eclipse song
	;
	