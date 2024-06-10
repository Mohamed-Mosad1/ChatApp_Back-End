using ChatApp.Persistence.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ChatApp.Persistence.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            PaginationHeader paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers["Pagination"] = JsonSerializer.Serialize(paginationHeader, jsonOptions);
            response.Headers["Access-Control-Expose-Headers"] = "Pagination";

        }
    }
}
