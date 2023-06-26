using Microsoft.AspNetCore.Identity;
using StoreBack.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Data.SqlClient;
using StoreBack.ViewModels;
using Microsoft.Extensions.Configuration;
using BC = BCrypt.Net.BCrypt;
using System.Data;
using System.Collections.Generic;
using System;

namespace StoreBack.Repositories
{
    public interface IUserRepository
    {
        Task<int> AddUser(AddUserViewModel model, User user);
        User getUser(int userId);
        Task DeleteUser(int id);
        Task UpdateUser(int id, UpdateserViewModel model);
        Task<PagedResult<User>> GetUsers(UserFilterViewModel filter , int OrganizationId, int pageNumber = 1, int pageSize = 5);
    }
    
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; set; }
        public string connection;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            connection = _configuration.GetConnectionString("DefaultConnection");
        }

        //getUser

        public User getUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetUserById";

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            OrganizationId = reader.GetInt32(reader.GetOrdinal("OrganizationId")),
                            BranchId = reader.IsDBNull(reader.GetOrdinal("BranchId")) ? 
                                (int?)null : 
                                reader.GetInt32(reader.GetOrdinal("BranchId"))

                            // fill out other properties if necessary
                        };

                        return user;
                    }
                    else
                    {
                        return null; // or throw an exception, depending on your needs
                    }
                }
            }
        }


        //addUser
        public async Task<int> AddUser(AddUserViewModel model, User user)
        {
            Console.WriteLine(model.Password);
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CreateUser";

                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = model.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = model.LastName;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = model.UserName;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = BC.HashPassword(model.Password);
                cmd.Parameters.Add("@Role", SqlDbType.NVarChar).Value = model.Role;
                cmd.Parameters.Add("@OrganizationId", SqlDbType.NVarChar).Value = user.OrganizationId;
                cmd.Parameters.Add("@BranchId", SqlDbType.NVarChar).Value = model.BranchId;

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


        //update
       public async Task UpdateUser(int id, UpdateserViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateUserInfo";

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = model.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = model.LastName;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = model.UserName;
                
                if (!string.IsNullOrEmpty(model.Password))
                {
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = BC.HashPassword(model.Password);
                }
                

                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task DeleteUser(int id)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteUser";

                cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
    
                await cmd.ExecuteNonQueryAsync();
                
                return;
            }
        }

        public async Task<PagedResult<User>> GetUsers(UserFilterViewModel filter, int OrganizationId, int pageNumber = 1, int pageSize = 5)
            {
                List<User> users = new List<User>();
                int totalCount = 0;

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("GetUsers", conn) 
                    { 
                        CommandType = CommandType.StoredProcedure 
                    })
                    {
                        cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = OrganizationId;
                        cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = (object)filter.Username ?? DBNull.Value;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = (object)filter.Email ?? DBNull.Value;
                        cmd.Parameters.Add("@Role", SqlDbType.NVarChar).Value = (object)filter.Role ?? DBNull.Value;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                User user = new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Role = reader.GetString(reader.GetOrdinal("Role")),
                                };

                                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                                
                                users.Add(user);
                            }
                        }
                    }
                }

                return new PagedResult<User> { Results = users, TotalCount = totalCount };
            }
                }
            }
