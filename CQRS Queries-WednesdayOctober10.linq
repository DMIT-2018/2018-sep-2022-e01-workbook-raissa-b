<Query Kind="Program">
  <Connection>
    <ID>32c7413b-06b4-4950-99ca-124171ac4815</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.\MSSQLServer1</Server>
    <Database>Chinook</Database>
    <DisplayName>Chinook-Entity</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	//Main is going to represent the webpage post method 
	//error handling 
	try
	{
			
		//code and tested the FetchTracksBy query 
		string searcharg = "Deep";
		string searchby = "Artist";
		List<TrackSelection> tracklist = Track_FetchTracksBy(searcharg, searchby); 
		//tracklist.Dump();
		
		//coded and tested the FetchPlaylist query
		string playlistname ="hansenb1";
		string username = "HansenB"; //this is a username which will come from O/S via security 
		List<PlaylistTrackInfo> playlist = PlaylistTrack_FetchPlaylist(playlistname, username);
		//playlist.Dump();
		
		//coded and tested the Add_Track transaction 
			//everything will be individual parameters
			//the command method will receive no collection but will receive individual arguments
			//arguments needed: trackid, playlistname, username 
			
			//test tracks: 
			//793 A castle full of Rascals 
			//822 A Twist in the Tail 
			//543 Burn 
			//756 Child in Time
			
			//on the webpage, the post method would have already have access 
				//to the BindProperty variables containing the input values 
			playlistname = "hansenbtest";
			int trackid = 823; 
			
			//what's needed 
				//call the service method to process the data 
			//PlaylistTrack_AddTrack(playlistname, username,  trackid); //tested
			
			//on the web page, the post method would have already have access to the 
			// BindProperty variables containing the input values 
			playlistname = "hansenbtest"
			List<PlaylistTrackTRX> tracklistinfo = new List<PlaylistTrackTRX>();
			removetracklist.Add(new PlaylistTrackTRX() 
				{SelectedTrack = true, 
				trackid = 793,
				TrackNumber = 1,
				TrackInput = 0
			})
			removetracklist.Add(new PlaylistTrackTRX()
			{
				SelectedTrack = true,
				trackid = 543,
				TrackNumber = 1,
				TrackInput = 0
			})
			removetracklist.Add(new PlaylistTrackTRX()
			{
				SelectedTrack = true,
				trackid = 822,
				TrackNumber = 1,
				TrackInput = 0
			})

			// call the service method to process the data 
			PlaylistTrack_RemoveTracks(playlistname, username, tracklistinfo); 
			
			
			
			
			//once the service method is complete, the webpage would refresh 
			playlist = PlaylistTrack_FetchPlaylist(playlistname, username);
			playlist.Dump();
			
			
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
}

// You can define other methods, fields, classes and namespaces here

#region CQRS Queries 
public class TrackSelection
{
	public int TrackId { get; set; }
	public string SongName { get; set; }
	public string AlbumTitle { get; set; }
	public string ArtistName { get; set; }
	public int Milliseconds { get; set; }
	public decimal Price { get; set; }

}
public class PlaylistTrackInfo
{
	public int TrackId { get; set; }
	public int TrackNumber { get; set; }
	public string SongName { get; set; }
	public int Milliseconds { get; set; }

}
#endregion 
//general method to drill into an exception of obtain the InnerException where you actual error is detailed
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException; 
	return ex; 
}

//pretend to be the class library project 
#region TrackServices class
public List<TrackSelection> Track_FetchTracksBy(string searcharg, string searchby)
{
	if (string.IsNullOrWhiteSpace(searcharg))
	{
		throw new ArgumentNullException ("No search value submitted");

	}
	if (string.IsNullOrWhiteSpace(searchby))
	{
		throw new ArgumentNullException("No search style submitted");

	}
	IEnumerable<TrackSelection> results = Tracks 
								.Where(x => x.Album.Artist.Name.Contains(searcharg) && searchby.Equals("Artist") ||
								 x.Album.Title.Contains(searcharg) && searchby.Equals("Album"))
								.Select(x => new TrackSelection
								{
										TrackId = x.TrackId, 
										SongName = x.Name, 
										AlbumTitle = x.Album.Title, 
										ArtistName = x.Album.Artist.Name, 
										Milliseconds = x.Milliseconds, 
										Price = x.UnitPrice 
								}
								)
								;
								
	return results.ToList();
}



public List<PlaylistTrackInfo> PlaylistTrack_FetchPlaylist(string playlistname, string username)
{
	if (string.IsNullOrWhiteSpace(playlistname))
	{
		throw new ArgumentNullException("No playlist name submitted");

	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("No username submitted");

	}
	IEnumerable<PlaylistTrackInfo> results = PlaylistTracks
								.Where(x => x.Playlist.Name.Equals(playlistname) && x.Playlist.UserName.Equals(username))
								
								.Select(x => new PlaylistTrackInfo
								{
									TrackId = x.TrackId,
									TrackNumber = x.TrackNumber,
									SongName = x.Track.Name,
									Milliseconds = x.Track.Milliseconds
								}
								)
								.OrderBy(x => x.TrackNumber);

	return results.ToList();
}
#endregion
#region Command Transaction Methods 

void PlaylistTrack_AddTrack(string playlistname, string username, int trackid)
{
	//locals 
	Tracks trackexists = null; 
	Playlists playlistexists = null; 
	PlaylistTracks playlisttrackexists = null;
	int tracknumber = 0; 
	
	if (string.IsNullOrWhiteSpace(playlistname))
	{
		throw new ArgumentNullException("No playlist name submitted");
	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("No username submitted");
	}
	trackexists = Tracks 
		.Where(x=> x.TrackId == trackid)
		.Select (x => x)
		.FirstOrDefault(); 
	if (trackexists == null )
	{
		throw new ArgumentNullException("Selected track no longer on file. Refresh track table.");
	}		
	//BR playlist names must be unique within a user 
	playlistexists = Playlists 
				.Where (x => x.Name.Equals(playlistname) && x.UserName.Equals(username))
				.Select (x => x)
				.FirstOrDefault();
				
	if (playlistexists == null)
	{
		//no (new playlist)
		playlistexists = new Playlists()
		{
			Name = playlistname, 
			UserName = username
		};
		//staging the new playlist record 
		Playlists.Add(playlistexists);
		tracknumber = 1; 
	}
	else 
	{
		//BR a track may only exist once on a playlist 
		playlisttrackexists = PlaylistTracks 
			.Where(x => (x.Playlist.Name.Equals(playlistname))
					&& x.Playlist.UserName.Equals(username)
					&& x.TrackId == trackid) 
					.Select (x => x)
					.FirstOrDefault();
		if (playlisttrackexists == null)
		{
			//generate the next track # 
			tracknumber = PlaylistTracks
					.Where(x => (x.Playlist.Name.Equals(playlistname))
					&& x.Playlist.UserName.Equals(username)
					&& x.TrackId == trackid)
					.Count();
			tracknumber++; 
		}
		else 
		{	
			var songname = Tracks 
							
							.Select(x => x.Name)
							.SingleOrDefault();
			//error
			throw new Exception($"Selected track ({songname}) already exists in the playlist.");
		}
							
	}
	//processing to stage the new track to the playlist 
	playlisttrackexists = new PlaylistTracks();
	
	//load the data into the new instance of playlist track 
	playlisttrackexists.TrackNumber = tracknumber; 
	playlisttrackexists.TrackId = trackid;

	/*************************************************************************************
	?? What about the second part of the primary key: PlaylistId
	
	If the playlist exists then we know the id: 
		playlistexists.PlaylistId; 
		
	in the situation of a NEW playlist, 
	even though we have created the playlist instance (see above)
	it is ONLY staged 
	
	this means that the actual sql record has NOT yet been created 
	this means that the IDENTITY value for the new playlist DOES NOT 
	yet exists. The value of the playlist instance (playlistexists) is zero. 
	
	Thus we have a serious problem.
	It is built into EntityFramework software and is based on using the navigational property 
	in Playlists pointing to its "child" 
	
	staging a typical Add in the past was to reference the entity and use the entity.Add(xxxxxx)
	_context.PlaylistTrack.Add(xxxxxx) [_context. is contextt instance in VS]
	
	IF you use this statement, the PlaylistId would be zero (0)
	causing your transaction to ABORT 
	
	INSTEAD do the staging using the syntax of "parent.navigationproperty.Add(xxxxxx)"
	playlistexists will be filled with either 
		scenario A) a new staged instance
		scenario B) the copy of the existing playlist instance 
	**************************************************************************************/
	
	playlistexists.PlaylistTracks.Add(playlisttrackexists);
	
	/*************************************************************************************
	Staging is complete 
	Commit the work (transaction)
	Committing the work needs a .SaveChanges()
	A transaction has ONLY ONE .SaveChanges()
	
	IF the SaveChanges() fails then all staged work being handled by the SaveChanges 
	is rolled back.
	**************************************************************************************/
	
	SaveChanges();
	
}

public void PlaylistTrack_RemoveTracks(string playlistname, string username, List<PlaylistTrackTRX> tracklistinfo)
{
	//local variables 
	Playlists playlistexists = null;  
	
	if (string.IsNullOrWhiteSpace(playlistname))
	{
		throw new ArgumentNullException("No playlist name submitted");

	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("No username submitted");
	}
	
	var count = tracklistinfo.Count();
	if (count == 0)
	{
		throw new ArgumentNullException("No list of tracks were submitted");
	}
	playlistexists = Playlists
				.Where(x => x.Name.Equals(playlistname) && x.UserName.Equals(username))
				.Select(x => x)
				.FirstOrDefault();	
	if (playlistexists == null)
	{
		throw new ArgumentNullException($"Playlist {playlistname} does not exist for this user.");
		
	}
	else
	{
		
	}
}

//sort the command model data list on the re-org value 
// in ascending order comparing x to y 
//a descending order comparing y to x 

tracklistinfo.Sort((x,y) => x.TrackInput.CompareTo(y.TrackInput));

//b) unique new track numbers 
// the collection has been sorted in ascending order therefore the next number must be equal to or greater than the previous number 
//one could check to see if the next number is +1 of the previous unmber 
//but the re-org loop which does the actual resquence of numbers 
//will have that situation 
//Therefore "holes" in this loop does not matter (logically)

for (int i = 0; ie < tracklistinfo.Count - 1; i++)
{
	var songname1 = Tracks 
				.Where (x => x.TrackId == tracklistinfo[i].TrackId)
				.Select(x => x.Name)
				.SingleOrDefault();
	var songname2 = Tracks
				.Where(x => x.TrackId == tracklistinfo[i + 1].TrackId)
				.Select(x => x.Name)
				.SingleOrDefault();
		if (tracklistinfo[i].TrackInput == tracklistinfo[i + 1].TrackInput)
	

}

//validation loop to check that the data is indeed a positive number 
//use int.TryParse to check that the value to be tested in a number 
//check the result of tryparse against the value 1


int tempnum = 0;
foreach (var track in tracklistinfo)
{
	var songname = Tracks 
				.Where (x => x.TrackId == track.TrackId)
				.Select (x => x.Name)
				.SingleOrDefault();
	if (int.TryParse(Tracks.TrackInput.ToString(), out tempnum))	
	{
		if (tempnum < 1)
		{
			errorlist.Add(new Exception($"The track ({songname} re-sequence value needs to be greater than 0 )"))
		}
	}
	else
	{
		errorlist.Add(new Exception($"The track ({songname}) resequence value needs to"))
	}
}


#endregion 