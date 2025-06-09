using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesforceIntegrationApp.Helpers
{
    public static class PaginationHelper
    {
        public static (List<T> PaginatedData, int TotalPages, int CurrentPage) ApplyPagination<T>(
        List<T> sourceList, HttpRequest request, int pageSize = 10)
        {
            int pageNumber = 1;
            if (request.Query.ContainsKey("page"))
            {
                int.TryParse(request.Query["page"], out pageNumber);
                if (pageNumber <= 0) pageNumber = 1;
            }
            int totalItems = sourceList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedData = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return (paginatedData, totalPages, pageNumber);
        }

    }
}
