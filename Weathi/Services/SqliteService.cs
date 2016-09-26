using SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Weathi.Model;

namespace Weathi.Services
{
	public class SqliteService
	{
		public string Path;

		public SqliteService ()
		{
			// create DB path
			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			Path = System.IO.Path.Combine(docsFolder, "db_weathi.db");
		}

		public string CreateDatabase()
		{		
			//bool isExists=false;
			try
			{
				//isExists= System.IO.Directory.Exists(path);
				var connection = new SQLiteConnection(Path);
				connection.CreateTable<Weather>();
				return "Database created";				
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		public string InsertData(Weather data)
		{
			try
			{
				var db = new SQLiteConnection(Path);

				db.Insert(data);

				return "OK";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		public string UpdateData(Weather data)
		{
			try
			{
				var db = new SQLiteConnection(Path);

				db.Update(data);

				return "OK";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		public string InsertUpdateAllData(IEnumerable data)
		{
			try
			{
				var db = new SQLiteConnection(Path);
				if (db.InsertAll(data) != 0)
					db.UpdateAll(data);
				return "List of data inserted or updated";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		public int FindNumberRecordWeathers()
		{
			try
			{
				var db = new SQLiteConnection(Path);
				// this counts all records in the database, it can be slow depending on the size of the database
				var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Weather");

				return count;
			}
			catch (SQLiteException)
			{
				return -1;
			}
		}

		//retrieve a specific user by querying against their first name
		public Weather GetWeather(int id)
		{
			using (var database = new SQLiteConnection(Path))
			{
				return database.Table<Weather>().FirstOrDefault(u => u.Id == id);				
			}
		}

		public IList<Weather> GetWeathers()
		{
			using (var database = new SQLiteConnection(Path))
			{
				return database.Table<Weather>().ToList();
			}
		}


		public string DeleteEvent(Weather data)
		{
			using (var database = new SQLiteConnection(Path))
			{
				try
				{
					database.Delete(data);
					return "Item Deleted";
				}
				catch (SQLiteException ex)
				{
					return ex.Message;
				}
			}
		}
	}
}

