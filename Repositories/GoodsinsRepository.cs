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
    public interface IGoodsinRepository
    {
        // Task<int> AddUser(AddUserViewModel model, User user);
        // User getUser(int userId);
        // Task DeleteUser(int id);

        // Task UpdateUser(int id, UpdateserViewModel model);
        // Task<List<User>> GetUsers( int OrganizationId);
        Task <int> MakeGoodsIn (MakeGoodsInViewModel model, User user);
    }
    
    public class GoodsinRepository : IGoodsinRepository
{
    private readonly ApplicationDbContext _context;
    public IConfiguration _configuration { get; set; }
    public string connection;

    public GoodsinRepository(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        connection = _configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> MakeGoodsIn(MakeGoodsInViewModel model, User user)
    {
        using (SqlConnection conn = new SqlConnection(connection))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MakeGoodsIn";


            cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar).Value = model.Barcode;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = model.Name;
            cmd.Parameters.Add("@Price", SqlDbType.Float).Value = model.Price;
            cmd.Parameters.Add("@Unit", SqlDbType.NVarChar).Value = model.Unit;
            cmd.Parameters.Add("@Quantity", SqlDbType.Float).Value = model.Quantity;
            cmd.Parameters.Add("@BranchId", SqlDbType.NVarChar).Value = user.BranchId;
            cmd.Parameters.Add("@OrganizationId", SqlDbType.NVarChar).Value = user.OrganizationId;
            cmd.Parameters.Add("@OperatorUserId", SqlDbType.NVarChar).Value = user.Id;

            // assuming the stored procedure returns the id of the created record
            var result = await cmd.ExecuteScalarAsync();
            return (int)result;
        }
    }
}

    }
