using Dapper;
using MicroService.Shared.Response;
using Npgsql;
using System.Data;
using System.Threading.RateLimiting;

namespace Service.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;
        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            var status = await _dbConnection.ExecuteAsync($"delete from discount where id='{id}'");
            return status>0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("Discount not found",404);
        }

        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discount = await _dbConnection.QueryAsync<Models.Discount>("select * from discount");
            return Response<List<Models.Discount>>.Success(discount.ToList(), 200);
        }

        public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discount = await _dbConnection.QueryAsync<Models.Discount>($"select * from discount where userid='{userId}' and code='{code}'");
            var hasDiscount = discount.FirstOrDefault();
            if (hasDiscount == null) 
            { 
                return Response<Models.Discount>.Fail("Discount not found", 404); 
            }
            return Response<Models.Discount>.Success(hasDiscount, 200);
        }

        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>($"select * from discount where id = '{id}'")).SingleOrDefault();
            if (discount == null) 
            {
                return Response<Models.Discount>.Fail("Discount not found", 404);
            }
            return Response<Models.Discount>.Success(discount, 200);
        }

        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("Insert into discount (userid,rate,code) values(@UserId,@Rate,@Code)", discount);
            if (saveStatus > 0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("an error occurred while adding",500);
        }

        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var status = await _dbConnection.ExecuteAsync($"update discount set userid='{discount.UserId}',code='{discount.Code}',rate='{discount.Rate}' where id='{discount.Id}'");
            if (status>0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("Discount not found", 404);
        }
    }
}
