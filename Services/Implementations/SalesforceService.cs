﻿using SalesforceIntegrationApp.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;  //Task<T> and Task classes come from
using Newtonsoft.Json;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Logging;
using System;
using SalesforceIntegrationApp.Models.DTOs;
using Microsoft.CodeAnalysis;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class SalesforceService : ISalesforceService
    {
        //public async Task<List<LeadFieldDto>> GetLeadFieldsAsync(string accessToken, string instanceUrl)
        public async Task<List<dynamic>> GetLeadFieldsAsync(string accessToken, string instanceUrl)
        {
            Logger.LogInfo("Started fetching lead metadata from Salesforce (Service Layer).");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); //Adding authorization header 
                var url = $"{instanceUrl}/services/data/v54.0/sobjects/Lead/describe";
                var response = await client.GetAsync(url);
                Logger.LogInfo($"Lead metadata fetch response: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) //if-condition in case of failing to get response throws exception
                    throw new Exception($"Error fetching lead metadata: {response.StatusCode} - {response.ReasonPhrase}");
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var parsed = JsonConvert.DeserializeObject<LeadMetadataDto>(jsonResponse);
                //Implement jsonconvert deserialize using class

                /*var fields = parsed?.fields;
                if (fields == null)
                    throw new InvalidLeadDataException("Lead metadata does not contain any fields.");
                var selectedFields = new List<dynamic>();
                
                foreach (var field in fields)
                {
                    selectedFields.Add(new
                    {
                        label = field?.label ?? "N/A", // Usage of null-conditional and null-coaleascing operator
                        name = field?.name ?? "N/A",
                        updateable = field?.updateable ?? false,
                        sortable = field?.sortable ?? false,
                        createable = field?.createable ?? false
                    });
                }*/
                /*if (parsed?.Fields == null || parsed.Fields.Count == 0)
                    throw new InvalidLeadDataException("Lead metadata does not contain any fields.");
                var dynamicList = parsed.Fields.Select(field => new
                {
                    label = field.Label ?? "N/A",
                    name = field.Name ?? "N/A",
                    updateable = field.Updateable,
                    sortable = field.Sortable,
                    createable = field.Createable
                }).Cast<dynamic>().ToList();
                return parsed.Fields;*/
                //return dynamicList;

                if (parsed?.Fields == null || parsed.Fields.Count == 0)
                    throw new InvalidLeadDataException("Lead metadata does not contain any fields.");
                //return parsed.Fields;
                var dynamicList = parsed.Fields.Select(field => new
                {
                    label = field.Label ?? "N/A",
                    name = field.Name ?? "N/A",
                    updateable = field.Updateable,
                    sortable = field.Sortable,
                    createable = field.Createable
                }).Cast<dynamic>().ToList();
                return dynamicList;
            }
        }
    }
}
