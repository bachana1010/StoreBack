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
    public interface IProductRepository
            {
        Task<PagedResult<GetBarcodeBalanceViewModel>> ProductBalance(
        int? BranchId = null,
            int? OrganizationId = null,
            string name = null,
            string priceOperator = null,
            decimal? priceValue = null,
            string quantityOperator = null,
            decimal? quantityValue = null,
            int pageNumber = 1, 
            int pageSize = 5);
        Task<DashboardDataViewModel> GetDashboardData(int organizationId);

            }

    
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; set; }
        public string connection;

        public ProductRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            connection = _configuration.GetConnectionString("DefaultConnection");
        }


        //get product operator
   
public async Task<PagedResult<GetBarcodeBalanceViewModel>> ProductBalance(
    int? BranchId = null,
    int? OrganizationId = null,
    string name = null,
    string priceOperator = null,
    decimal? priceValue = null,
    string quantityOperator = null,
    decimal? quantityValue = null,
    int pageNumber = 1, 
    int pageSize = 5)
{
    
    List<GetBarcodeBalanceViewModel> products = new List<GetBarcodeBalanceViewModel>();
    int totalCount = 0;

    using (SqlConnection conn = new SqlConnection(connection))
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GetBarcodeWithBalance";

        cmd.Parameters.Add("@BranchId", SqlDbType.Int).Value = (object)BranchId ?? DBNull.Value;
        // cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = (object)OrganizationId ?? DBNull.Value;
        cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = (object)OrganizationId ?? DBNull.Value;

        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = (object)name ?? DBNull.Value;
        cmd.Parameters.Add("@PriceOperator", SqlDbType.NVarChar, 1).Value = (object)priceOperator ?? DBNull.Value;
        cmd.Parameters.Add("@PriceValue", SqlDbType.Decimal).Value = (object)priceValue ?? DBNull.Value;
        cmd.Parameters.Add("@QuantityOperator", SqlDbType.NVarChar, 1).Value = (object)quantityOperator ?? DBNull.Value;
        cmd.Parameters.Add("@QuantityValue", SqlDbType.Decimal).Value = (object)quantityValue ?? DBNull.Value;
        cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = pageNumber;
        cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;

        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                GetBarcodeBalanceViewModel productBalance = new GetBarcodeBalanceViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Unit = reader.GetString(reader.GetOrdinal("Unit")),
                    Price = reader.GetFloat(reader.GetOrdinal("Price")),
                    Quantity = reader.GetFloat(reader.GetOrdinal("Quantity")),
                };

                try
                {
                    int ordinal = reader.GetOrdinal("BranchName");

                    if (!reader.IsDBNull(ordinal))
                    {
                        productBalance.BranchName = reader.GetString(ordinal);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading BranchName: {ex.Message}");
                }

                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                }
                products.Add(productBalance);
            }
        }
    }

    return new PagedResult<GetBarcodeBalanceViewModel> { Results = products, TotalCount = totalCount };
}

    


        public async Task<DashboardDataViewModel> GetDashboardData(int organizationId)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                await conn.OpenAsync();

                SqlCommand cmd = new SqlCommand("GetDashboardData", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@OrganizationId", organizationId));

                var result = new DashboardDataViewModel();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var schemaTable = reader.GetSchemaTable();

                     while (await reader.ReadAsync())
                        {
                            result.UserCount = reader.GetInt32(reader.GetOrdinal("UserCount"));
                            result.OperatorCount = reader.GetInt32(reader.GetOrdinal("OperatorCount"));
                            result.ManagerCount = reader.GetInt32(reader.GetOrdinal("ManagerCount"));
                            result.GoodsInCount = reader.GetInt32(reader.GetOrdinal("GoodsInCount"));
                            result.GoodsOutCount = reader.GetInt32(reader.GetOrdinal("GoodsOutCount"));
                            result.ProductCount = reader.GetInt32(reader.GetOrdinal("ProductCount"));
                            result.BranchCount = reader.GetInt32(reader.GetOrdinal("BranchCount"));
                        }
                }
                return result;
            }
        }






    }
}
