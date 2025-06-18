using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesforceIntegrationApp.Helpers
{
    public static class PaginationHelper
    {
        public static (List<dynamic> PaginatedData, int TotalPages, int CurrentPage) ApplyPagination<dynamic>(List<dynamic> sourceList, HttpRequest request, int pageSize = 20)// Method for pagination 
        {
            int pageNumber = 1; // Default page number set to 1
            if (request.Query.ContainsKey("page")) // Extracting the page from the query
            {
                int.TryParse(request.Query["page"], out pageNumber); // Converting the datatype of page from string to integer
                if (pageNumber <= 0) pageNumber = 1; // in-case of negative paging(Optional condition)
            }
            int totalItems = sourceList.Count; // Calculating the total items of the source list using .Count method and storing it in totalItems
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize); // calculating totalpages required to display totalItems
            var paginatedData = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); //Converts the current page items to list
            return (paginatedData, totalPages, pageNumber);
        }

    }
}
