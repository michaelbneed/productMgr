using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Business.Dto
{
	public class UserDto
	{
		//TODO Use this to perist user data - possibly
		private readonly IDbReadService _dbReadService;
		public List<User> UserData;

		public UserDto(IDbReadService dbReadService)
		{
			_dbReadService = dbReadService;
		}

		public async Task<List<User>> GetUserData()
		{
			UserData = await _dbReadService.GetAllRecordsAsync<User>();
			var loggedInUser = UserData.FirstOrDefault();

			return UserData;
		}

		
	}
}
