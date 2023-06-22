using Microsoft.AspNetCore.Identity;
using StoreBack.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Data.SqlClient;
using StoreBack.ViewModels;
using Microsoft.Extensions.Configuration;
using BC = BCrypt.Net.BCrypt;
using System.Data;

namespace StoreBack.Repositories
{
    public interface IBranchRepository
    {
        Task addBranch(AddBranchViewModel model, User user );
        Branches GetBranch(int BranchId);

        Task DeleteBranch(int id ); 
        Task<(List<Branches> branches, int totalCount)> GetBranches(int OrganizationId, int pageNumber, int pageSize);

        Task UpdateBranch(int id, UpdateBranchViewModel model);


    }
    
    public class BranchRepository : IBranchRepository
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; set; }
        public string connection;

        public BranchRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            connection = _configuration.GetConnectionString("DefaultConnection");
        }

        public Branches GetBranch(int BranchId)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "getBranchById";

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = BranchId;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Branches branches = new Branches
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            BrancheName = reader.GetString(reader.GetOrdinal("BrancheName")),
                            OrganizationId = reader.GetInt32(reader.GetOrdinal("OrganizationId")),
                            AddedByUserId = reader.GetInt32(reader.GetOrdinal("AddedByUserId")),
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DeletedAt")))
                        {
                            branches.DeletedAt = reader.GetDateTime(reader.GetOrdinal("DeletedAt"));
                        }

                        return branches;
                    }
                    else
                    {
                        return null; 
                    }
                }
            }
        }

        public async Task addBranch(AddBranchViewModel model, User user )
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddBranch";

                cmd.Parameters.Add("@BrancheName", SqlDbType.NVarChar).Value = model.BranchName;
                cmd.Parameters.Add("@OrganizationId", SqlDbType.NVarChar).Value = user.OrganizationId;
                cmd.Parameters.Add("@AddedByUserId", SqlDbType.NVarChar).Value = user.Id;

               
                await cmd.ExecuteNonQueryAsync();

                return;
            }
        }


        //delete branch
        public async Task DeleteBranch(int id)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteBranch";

                cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
    
                await cmd.ExecuteNonQueryAsync();
                
                return;
            }
        }


            //branches list
       public async Task<(List<Branches> branches, int totalCount)> GetBranches(int OrganizationId, int pageNumber, int pageSize)

        {
            List<Branches> branches = new List<Branches>();
            int totalCount = 0;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("GetBranches", conn) 
                { 
                    CommandType = CommandType.StoredProcedure 
                })
                {
                    cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = OrganizationId;
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime? deletedAt = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("DeletedAt")))
                            {
                                deletedAt = reader.GetDateTime(reader.GetOrdinal("DeletedAt"));
                            }

                            Branches branch = new Branches
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                BrancheName = reader.GetString(reader.GetOrdinal("BrancheName")),
                                OrganizationId = reader.GetInt32(reader.GetOrdinal("OrganizationId")),
                                AddedByUserId = reader.GetInt32(reader.GetOrdinal("AddedByUserId")),
                                DeletedAt = deletedAt
                            };

                            branches.Add(branch);

                            // Get total count from the first row
                            if (totalCount == 0)
                            {
                                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                            }
                        }
                    }
                }
            }

            return (branches, totalCount);
        }



        public async Task UpdateBranch(int id,UpdateBranchViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateBranchInfo";

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@BrancheName", SqlDbType.NVarChar).Value = model.BranchName;

                await cmd.ExecuteNonQueryAsync();


            }
        }
    }
}
