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
using System; // Make sure to import the System namespace

namespace StoreBack.Repositories
{
    public interface IProductRepository
    {
        Task<PagedResult<GetBarcodeBalanceViewModel>> ProductBalance(int? BranchId = null,int? OrganizationId = null,int pageNumber = 1, int pageSize = 5);

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
   
            public async Task<PagedResult<GetBarcodeBalanceViewModel>> ProductBalance(int? BranchId = null, int? OrganizationId = null,  int pageNumber = 1, int pageSize = 5)
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
                cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = (object)OrganizationId ?? DBNull.Value;
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

    


        // //make products by manager

        // public async Task<PagedResult<getBalanceManagerViewModels>> BalanceManager(int OrganizationId, int? branchId, int pageNumber = 1, int pageSize = 5)

        // {
        //     // getBalanceManagerViewModels productBalance = null;
        //     List<getBalanceManagerViewModels> products = new List<getBalanceManagerViewModels>();
        //     int totalCount = 0;


        //     using (SqlConnection conn = new SqlConnection(connection))
        //     {
        //         conn.Open();
        //         SqlCommand cmd = new SqlCommand();
        //         cmd.Connection = conn;
        //         cmd.CommandType = CommandType.StoredProcedure;
        //         cmd.CommandText = "GetManagerBalance";

        //         cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = OrganizationId;
        //         cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
        //         cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
        //         if(branchId.HasValue)
        //         {
        //             cmd.Parameters.Add("@branchId", SqlDbType.Int).Value = branchId.Value;
        //         }
        //         using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        //         {
        //             while (reader.Read())
        //             {
        //                 getBalanceManagerViewModels productBalance = new getBalanceManagerViewModels
        //                 {
        //                     Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
        //                     Name = reader.GetString(reader.GetOrdinal("Name")),
        //                     Unit = reader.GetString(reader.GetOrdinal("Unit")),
        //                     Price = reader.GetFloat(reader.GetOrdinal("Price")),
        //                     Quantity = reader.GetFloat(reader.GetOrdinal("Quantity")),
        //                 };

        //                 // Check if the BrancheName column is not null before assigning it
        //                 try
        //                 {
        //                     int ordinal = reader.GetOrdinal("BranchName");

        //                     if (!reader.IsDBNull(ordinal))
        //                     {
        //                         productBalance.BrancheName = reader.GetString(ordinal);
        //                     }
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     Console.WriteLine($"Error reading BrancheName: {ex.Message}");
        //                 }

        //                 if (totalCount == 0)
        //                 {
        //                     totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
        //                 }
        //                 products.Add(productBalance);
        //             }
        //         }
        //     }

        //         return new PagedResult<getBalanceManagerViewModels> { Results = products, TotalCount = totalCount };
        // }







    }
}
