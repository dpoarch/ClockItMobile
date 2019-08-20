using ClockIt.Mobile.Models;
using SQLite;
using System;
using System.Threading.Tasks;

namespace ClockIt.Mobile.Assets
{
	public sealed class UserRepository
	{
		SQLiteAsyncConnection _connection;
		static UserRepository _instance;
		public static UserRepository Instance
		{
			get
			{
				if (_instance == null)
				{
					throw new Exception("You must call initialize before retrieving the singleton for the UserRepository");
				}

				return _instance;
			}
		}
		public string StatusMessage { get; set; }

		public static void Initialize(string dbPath)
		{
			if (string.IsNullOrWhiteSpace(dbPath))
			{
				throw new ArgumentNullException(nameof(dbPath));
			}

			if (_instance != null)
			{
				_instance._connection.GetConnection().Dispose();
			}

			_instance = new UserRepository(dbPath);
		}

		public UserRepository(string dbPath)
		{
			_connection = new SQLiteAsyncConnection(dbPath);
			_connection.CreateTableAsync<User>().Wait();
		}

		public async Task AddNewUser(User user)
		{
			int result = 0;
			try
			{
				if (user.UserName == null)
				{
					throw new Exception("Email address is required.");
				}

				result = await _connection.InsertAsync(user);
				StatusMessage = $"{result} record(s) added [Email {user.UserName})";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Failed to add {user.UserName}. Error {ex.Message}";
			}
		}

		public async Task<User> GetUser()
		{
			try
			{
				return await _connection.Table<User>().FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				StatusMessage = $"Failed to retrieve data. Error {ex.Message}";
			}

			return null;
		}
	}
}
