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
        Task<List<GetBarcodeBalanceViewModel>> ProductBalance( int brancheId);
        Task<List<getBalanceManagerViewModels>> BalanceManager( int organizationId, int? branchId);
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

   
        public async Task<List<GetBarcodeBalanceViewModel>> ProductBalance(int brancheId)
        {
            GetBarcodeBalanceViewModel productBalance = null;

            List<GetBarcodeBalanceViewModel> products = new List<GetBarcodeBalanceViewModel>();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetBarcodeWithBalance";
                cmd.Parameters.Add("@branchId", SqlDbType.Int).Value = brancheId;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        productBalance = new GetBarcodeBalanceViewModel
                        {
                            Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Unit = reader.GetString(reader.GetOrdinal("Unit")),
                            Price = reader.GetFloat(reader.GetOrdinal("Price")),
                            Quantity = reader.GetFloat(reader.GetOrdinal("Quantity"))

                        };

                        products.Add(productBalance);
                    }
                }
            }

            return products;
        }
    



        public async Task<List<getBalanceManagerViewModels>> BalanceManager(int OrganizationId,int? branchId)
        {
            getBalanceManagerViewModels productBalance = null;
            List<getBalanceManagerViewModels> products = new List<getBalanceManagerViewModels>();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetManagerBalance";
                cmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = OrganizationId;

                if(branchId.HasValue)
                {
                    cmd.Parameters.Add("@branchId", SqlDbType.Int).Value = branchId.Value;
                }
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        productBalance = new getBalanceManagerViewModels
                        {
                            Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Unit = reader.GetString(reader.GetOrdinal("Unit")),
                            Price = reader.GetFloat(reader.GetOrdinal("Price")),
                            Quantity = reader.GetFloat(reader.GetOrdinal("Quantity")),
                        };

                        // Check if the BrancheName column is not null before assigning it
                        try
                        {
                            int ordinal = reader.GetOrdinal("BrancheName");

                            if (!reader.IsDBNull(ordinal))
                            {
                                productBalance.BrancheName = reader.GetString(ordinal);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading BrancheName: {ex.Message}");
                        }

                        products.Add(productBalance);
                    }
                }
            }

            return products;
        }
    }
}
