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
     Task<PagedResult<GetGoodsOutViewModel>> getGoodsOut(int organizationId, int? branchId, int pageNumber = 1, int pageSize = 5, string quantityOperator = null, float? quantityValue = null, DateTime? dateFrom = null, DateTime? dateTo = null);

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



            //goodsOut list for manager



                    public async Task<PagedResult<GetGoodsOutViewModel>> getGoodsOut(int OrganizationId, int? branchId, int pageNumber = 1, int pageSize = 5, string quantityOperator = null, float? quantityValue = null, DateTime? dateFrom = null, DateTime? dateTo = null)
            {
                List<GetGoodsOutViewModel> goodsOutList = new List<GetGoodsOutViewModel>();
                int totalCount = 0;

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetGoodsOut";

                    cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = OrganizationId;
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                    if(branchId.HasValue)
                    {
                        cmd.Parameters.Add("@BranchId", SqlDbType.Int).Value = branchId.Value;
                    }
                    
                    if(!string.IsNullOrEmpty(quantityOperator) && quantityValue.HasValue)
                    {
                        cmd.Parameters.Add("@QuantityOperator", SqlDbType.NVarChar, 1).Value = quantityOperator;
                        cmd.Parameters.Add("@QuantityValue", SqlDbType.Float).Value = quantityValue.Value;
                    }
                    
                    if(dateFrom.HasValue)
                    {
                        cmd.Parameters.Add("@DateFrom", SqlDbType.DateTime2).Value = dateFrom.Value;
                    }
                    
                    if(dateTo.HasValue)
                    {
                        cmd.Parameters.Add("@DateTo", SqlDbType.DateTime2).Value = dateTo.Value;
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GetGoodsOutViewModel goodsOutVM = new GetGoodsOutViewModel
                            {
                                Quantity = reader.GetFloat(reader.GetOrdinal("Quantity")),
                                OutDate = reader.GetDateTime(reader.GetOrdinal("OutDate")),
                            BranchName = reader.GetString(reader.GetOrdinal("BranchName")),
                                OperatorUserName = reader.GetString(reader.GetOrdinal("OperatorUserName")),
                                BarcodeName = reader.GetString(reader.GetOrdinal("BarcodeName"))
                            };
                            if (totalCount == 0)
                            {
                                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                            }

                            goodsOutList.Add(goodsOutVM);
                        }
                    }
                }
                
                return new PagedResult<GetGoodsOutViewModel> { Results = goodsOutList, TotalCount = totalCount };
            }

                
 }
            
}




