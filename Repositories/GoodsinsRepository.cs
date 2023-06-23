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
        Task<int> MakeGoodsIn(MakeGoodsInViewModel model, User user);
        GetBarcodeViewModel getBarcode(string  barcodetext);
        Task<PagedResult<GetGoodsinViewModel>> GetGoodsIn(int organizationId, int? branchId, int pageNumber = 1, int pageSize = 5);

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


    //add product
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

        //getbarcode

        public GetBarcodeViewModel getBarcode(string  barcodetext)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "getBarcode";

                cmd.Parameters.Add("@barcodetext", SqlDbType.NVarChar).Value = barcodetext;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        GetBarcodeViewModel barcodeVM = new GetBarcodeViewModel

                       {
                        Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Price = reader.GetFloat(reader.GetOrdinal("Price")),
                        Unit = reader.GetString(reader.GetOrdinal("Unit")),
                        BranchId = reader.GetInt32(reader.GetOrdinal("BranchId")),
                        OrganizationId = reader.GetInt32(reader.GetOrdinal("OrganizationId")),
                        Quantity = reader.GetFloat(reader.GetOrdinal("Quantity")) 

                        };

                        return barcodeVM;

                    }
                    else
                    {
                        return null; 
                    }
                }

                
            }
            
        }


        //goodsin list for manager
            public async Task<PagedResult<GetGoodsinViewModel>> GetGoodsIn(int OrganizationId, int? branchId, int pageNumber = 1, int pageSize = 5)
        {
            List<GetGoodsinViewModel> goodsinList = new List<GetGoodsinViewModel>();
            int totalCount = 0;
            
            Console.WriteLine(OrganizationId);
            Console.WriteLine(pageNumber);
            Console.WriteLine(pageSize);
            
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetGoodsIn";

                cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = OrganizationId;
                cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                if(branchId.HasValue) // Only add the BranchId parameter if it has a value.
                {
                cmd.Parameters.Add("@BranchId", SqlDbType.Int).Value = branchId.Value;
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GetGoodsinViewModel goodsinVM = new GetGoodsinViewModel
                        {
                            Quantity = reader.GetFloat(reader.GetOrdinal("Quantity")),
                            EntryDate = reader.GetDateTime(reader.GetOrdinal("EntryDate")),
                            BranchName = reader.GetString(reader.GetOrdinal("BranchName")),
                            OperatorUserName = reader.GetString(reader.GetOrdinal("OperatorUserName")),
                            BarcodeName = reader.GetString(reader.GetOrdinal("BarcodeName"))
                        };

                        // Get the total count from the first row
                        if (totalCount == 0)
                        {
                            totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                        }

                        goodsinList.Add(goodsinVM);
                    }
                }
            }
            
            return new PagedResult<GetGoodsinViewModel> { Results = goodsinList, TotalCount = totalCount };
        }


}
                    
}


