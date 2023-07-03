using Microsoft.AspNetCore.Identity;
using StoreBack.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using StoreBack.ViewModels;
using System;
using BC = BCrypt.Net.BCrypt;

namespace StoreBack.Repositories
{
    public interface IAuthRepository
    {
        Task<int> RegisterOrganizationAndUser(RegisterOrganizationViewModel model);
        Task<(User, string)> LoginUser(string email, string password);
    }
    
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; set; }
        public string connection;

        public AuthRepository(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            connection = _configuration.GetConnectionString("DefaultConnection");
        }


        //registrationrepository
        public async Task<int> RegisterOrganizationAndUser(RegisterOrganizationViewModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "RegisterOrganizationAndUser";

                    cmd.Parameters.Add("@organizationName", SqlDbType.NVarChar).Value = model.OrganizationName;
                    cmd.Parameters.Add("@address", SqlDbType.NVarChar).Value = model.Address;
                    cmd.Parameters.Add("@orgEmail", SqlDbType.NVarChar).Value = model.Email;
                    cmd.Parameters.Add("@firstName", SqlDbType.NVarChar).Value = model.FirstName;
                    cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = model.LastName;
                    cmd.Parameters.Add("@userName", SqlDbType.NVarChar).Value = model.UserName;
                    cmd.Parameters.Add("@passwordHash", SqlDbType.NVarChar).Value = model.Password;

                    var userIdParam = new SqlParameter("@userId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(userIdParam);

                    await cmd.ExecuteNonQueryAsync();

                    int userId = (int)userIdParam.Value;

                    return userId;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601) 
                {
                    throw new Exception("Email already exists.", ex);
                }
                else if (ex.Number == 515) 
                {
                    throw new Exception("OrganizationId is null.", ex);
                }
                else
                {
                    throw;
                }
            }
        }


        //login
        public async Task<(User, string)> LoginUser(string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetUserByEmail";

                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            User user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                            };

                            bool validPassword = BC.Verify(password, user.PasswordHash);
                            if (validPassword)
                            {
                                // refresh toqeni dasamtavrbelia
                                RefreshTokens refreshToken = new RefreshTokens
                                {
                                    Token = Guid.NewGuid().ToString(), 
                                    Expires = DateTime.UtcNow.AddDays(7), 
                                    UserId = user.Id,
                                };

                                _context.RefreshTokens.Add(refreshToken);
                                await _context.SaveChangesAsync();

                                return (user, refreshToken.Token);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while executing the SQL command.", ex);
            }

            return (null, null);
        }
    }
}
