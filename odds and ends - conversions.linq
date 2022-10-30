<Query Kind="Program">
  <Connection>
    <ID>c5c58f2a-b12e-45e9-89a1-88c4a3cc4714</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\MSSQLServer1</Server>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

void Main()
{
//Conversions 
//Collection we will look at are IQueryable, IEnumerable, and List

//Display all albums and their tracks. Display the album title,
//artist name and album tracks. For each track, show the song name and 
//play time. Show only albums with 25 or more tracks. 

List<AlbumTracks> albumlist = Albums //IEnumberable<AlbumTracks> is strongly typed and you wont have to convert it 
									//List uses memory for further processing 
				.Where(a => a.Tracks.Count >= 25)
				.Select(a => new AlbumTracks
				{
					Title = a.Title, 
					Artist = a.Artist.Name, 
					Songs = a.Tracks
					.Select(tr => new SongItem //strongly typed 'SongItem' prints IEnumerable data sets  
					{
						Song = tr.Name, 
						Playtime = tr.Milliseconds /1000.0 //divided by 1000 to get seconds
					})
					.ToList()
				})
				.ToList() 
				.Dump()
				;
	//Using .FirstOrDefault()
	//first saw in CPSC1517 when checking to see if a record existed in a BLL service method 
	
	//Find the first album by Deep Purple 
	
	var artistparam = "Deep Purple";
	var resultsFOD = Albums
					.Where(a => a.Artist.Name.Equals(artistparam))
					.Select(a => a) //select the whole record
					.OrderBy(a => a.ReleaseYear) 
					.FirstOrDefault()
					//.Dump()
					;
	//if (resultsFOD != null)
	//{
	//	resultsFOD.Dump();
	//}
	//else
	//{
	//	Console.WriteLine($"No albums found for artist {artistparam}");
	//}
	
	//Distinct()
	//removes duplicate reported lines
	
	//Get a list of customer countries 
	var resultDistinct = Customers 
							.OrderBy( c => c.Country)
							.Select(c => c.Country)
							.Distinct()
							.Dump()
							;



	//.Take() and .Skip()
	// in CPSC1517 when you wanted to use the supplied Paginator 
	// the query method was to return only the needed records for the display 
	// not the entire collection 
	// what would happen: 
	// a) the query was executed returning a collection of size x 
	// b) obtained the total count (x) of return records 
	// c) you calculated the number of records to skip (pagenumber - 1) x pagesize 	
	// d) on the return method statement, you used 
	//return variablename.Skip(rowsSkipped).Take(pagesize).ToList()


	//Union
	//rules in linq are the same as sql 
	//result is the same as sql, combine separate collections into one. 
	//syntax (queryA).Union(queryB)[.Union(query...)]
	//rules: 
	// number of columns the same 
	// column data types must be the same 
	// ordering should be done as a method after the last Union 

	var resultsUnionA = (Albums //hard coded should be at the beginning of the union 
						.Where(x => x.Tracks.Count() == 0)
						.Select(x => new
						{
							title = x.Title,
							totalTracks = 0,
							totalCost = 0.00m, //letter m to make it a double data type
							averageLength = 0.00
						})
						)
				.Union(Albums
						.Where(x => x.Tracks.Count() > 0)
						.Select(x => new
						{
							title = x.Title,
							totalTracks = x.Tracks.Count(),
							totalCost = x.Tracks.Sum(tr => tr.UnitPrice),
							averageLength = x.Tracks.Average(tr => tr.Milliseconds)
						})
						.OrderBy(x => x.totalTracks)
						.Dump())
						;				
}
public class SongItem //object
{
	public string Song {get; set;}
	public double Playtime {get; set;}
}
public class AlbumTracks 
{
	public string Title {get; set;}
	public string Artist {get; set;}
	public List<SongItem> Songs {get; set;} //using the strongly typed data set 'SongItem'
}
