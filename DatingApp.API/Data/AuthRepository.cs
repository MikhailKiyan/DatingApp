﻿namespace DatingApp.API.Data {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;

	using DatingApp.API.Models;
	using DatingApp.API.Utilities.ExtensionMethods;

	public class AuthRepository : IAuthRepository {

		private readonly DataContext context;

		public AuthRepository(DataContext context) {
			this.context = context;
		}

		public async Task<User> Login(string username, string password) {
			var user = await this.context.Users.FirstOrDefaultAsync(x => x.Username == username);

			if (user == null)
				return null;

			if(!this.VarifiPasswordHash(password, user.PasswordHash, user.PasswordSalt))
				return null;

			return user;
		}

		public async Task<User> Register(User user, string password) {
			(user.PasswordHash, user.PasswordSalt) = this.CreatePasswordHash(password);

			await this.context.Users.AddAsync(user);
			await this.context.SaveChangesAsync();

			return user;
		}

		public async Task<bool> UserExists(string username) {
			if(await this.context.Users.AnyAsync(x => x.Username == username))
				return true;
			else
				return false;
		}

		private (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password) {
			using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
				return (
					passwordHash: hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)),
					passwordSalt: hmac.Key);
			}
		}

		private bool VarifiPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
			using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.IsEqual(passwordHash);
			}
		}
	}
}
