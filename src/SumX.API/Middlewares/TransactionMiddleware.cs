using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SumX.Infrastructure.Persistence.Master;

namespace SumX.API.Middlewares
{
    public sealed class TransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, MasterDbContext dbContext)
        {
            var method = context.Request.Method;
            if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method) || HttpMethods.IsPatch(method))
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    await _next(context);

                    if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                    {
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                    }
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
