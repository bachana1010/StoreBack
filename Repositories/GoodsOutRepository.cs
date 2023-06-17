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
    public interface IGoodsOutRepository
    {
        // Task<int> AddUser(AddUserViewModel model, User user);
        // User getUser(int userId);
        // Task DeleteUser(int id);

        // Task UpdateUser(int id, UpdateserViewModel model);
        // Task<List<User>> GetUsers( int OrganizationId);
        Task <int> MakeGoodsOut (MakeGoodsOutViewModel model, User user);
        // GetBarcodeViewModel getBarcode(string  barcodetext);

    }
    
    public class GoodsOutRepository : IGoodsOutRepository
{
    private readonly ApplicationDbContext _context;
    public IConfiguration _configuration { get; set; }
    public string connection;

    public GoodsOutRepository(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        connection = _configuration.GetConnectionString("DefaultConnection");
    }


    //makegoodsout product
         public async Task<int> MakeGoodsOut(MakeGoodsOutViewModel model, User user)
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Check quantity
                    cmd.CommandText = "CheckQuantity";
                    cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar).Value = model.Barcode;
                    cmd.Parameters.Add("@RequiredQuantity", SqlDbType.Float).Value = model.Quantity;

                    int isAvailable = 0;
                    float remainingQuantity = 0;

                    using(SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            isAvailable = reader.GetInt32(reader.GetOrdinal("IsAvailable"));
                            remainingQuantity = (float)reader.GetDouble(reader.GetOrdinal("RemainingQuantity"));
                        }
                    }

                    // If quantity is available
                    if (isAvailable == 1)
                    {
                        cmd.Parameters.Clear();  // Clear parameters for next command
                        cmd.CommandText = "MakeGoodsOut";
                        cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar).Value = model.Barcode;
                        cmd.Parameters.Add("@OutQuantity", SqlDbType.Float).Value = model.Quantity;
                        cmd.Parameters.Add("@BranchId", SqlDbType.Int).Value = user.BranchId;
                        cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = user.OrganizationId;
                        cmd.Parameters.Add("@OperatorUserId", SqlDbType.Int).Value = user.Id;

                        var makeGoodsOutResult = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(makeGoodsOutResult);
                    }
                    else  // If quantity is not available
                    {
                        throw new Exception($"Insufficient quantity available. Remaining quantity: {remainingQuantity}");
                    }
                }
            }


                
            }
            
        }




