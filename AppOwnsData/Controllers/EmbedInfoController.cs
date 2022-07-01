// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------
using AppOwnsData.Models;
using AppOwnsData.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Dynamic;
using System.IO;
using System.Text.Json;

namespace AppOwnsData.Controllers
{
    [EnableCors("MyPolicy")]
    public class EmbedInfoController : Controller
    {
        private readonly PbiEmbedService pbiEmbedService;
        private readonly IOptions<AzureAd> azureAd;
        private readonly IOptions<PowerBI> powerBI;
        private readonly IConfiguration _configuration;

        public EmbedInfoController(PbiEmbedService pbiEmbedService, IOptions<AzureAd> azureAd, IOptions<PowerBI> powerBI,IConfiguration configuration)
        {
            this.pbiEmbedService = pbiEmbedService;
            this.azureAd = azureAd;
            this.powerBI = powerBI;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns Embed token, Embed URL, and Embed token expiry to the client
        /// </summary>
        /// <returns>JSON containing parameters for embedding</returns>
        /// 

        [HttpGet]
        public IActionResult GetKeyInformation([FromQuery] AzureAd az, [FromQuery] PowerBI Pbi)
        {
            azureAd.Value.ClientId = az.ClientId;
            azureAd.Value.ClientSecret = az.ClientSecret;
            azureAd.Value.TenantId = az.TenantId;
            
            var ClientId = azureAd.Value.ClientId;
            var ClientSecret = azureAd.Value.ClientSecret;
            var TenantId = azureAd.Value.TenantId;
            var WorkspaceId = Pbi.WorkspaceId;
            var ReportId = Pbi.ReportId;


            _configuration["AzureAd:ClientId"] = ClientId;
            _configuration["AzureAd:ClientSecret"] = ClientSecret;
            _configuration["AzureAd:TenantId"] = TenantId;
            _configuration["PowerBI:WorkspaceId"] = WorkspaceId;
            _configuration["PowerBI:ReportId"] = ReportId;

            var appSettingsValues = new
            {
                ClientId = _configuration["AzureAd:ClientId"],
                ClientSecret = _configuration["AzureAd:ClientSecret"],
                TenantId = _configuration["AzureAd:TenantId"],
                WorkspaceId = _configuration["PowerBI:WorkspaceId"],
                ReportId = _configuration["PowerBI:ReportId"]

            };

            //var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
            //var json = File.ReadAllText(appSettingsPath);

            //var jsonSettings = new JsonSerializerSettings();
            //jsonSettings.Converters.Add(new ExpandoObjectConverter());
            //jsonSettings.Converters.Add(new StringEnumConverter());

            //dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            //config.AzureAd.ClientId = az.ClientId;
            //config.AzureAd.TenantId = az.TenantId;
            //var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
            //File.WriteAllText(appSettingsPath, newJson);2

            /*GetEmBedInfoData()*/;
            azureAd.Equals(appSettingsValues);

            return Ok(GetEmBedInfoData() + "\n " + appSettingsValues);
        }


        [HttpGet]
        public string GetEmbedInfo()
        {
            try
            {

                //var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
                //var json = File.ReadAllText(appSettingsPath);

                //var jsonSettings = new JsonSerializerSettings();
                //jsonSettings.Converters.Add(new ExpandoObjectConverter());
                //jsonSettings.Converters.Add(new StringEnumConverter());

                //dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

                //config.AzureAd.ClientId = az.ClientId;
                //config.AzureAd.TenantId = az.TenantId;
                //var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
                //File.WriteAllText(appSettingsPath, newJson);2




                // Validate whether all the required configurations are provided in appsettings.json
                string configValidationResult = ConfigValidatorService.ValidateConfig(azureAd, powerBI, _configuration);
                if (configValidationResult != null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return configValidationResult;
                }

                EmbedParams embedParams = pbiEmbedService.GetEmbedParams(new Guid(powerBI.Value.WorkspaceId), new Guid(powerBI.Value.ReportId));
                return System.Text.Json.JsonSerializer.Serialize<EmbedParams>(embedParams);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return ex.Message + "\n\n" + ex.StackTrace;
            }
        }

        private string GetEmBedInfoData()
        {
            try
            {

                //var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
                //var json = File.ReadAllText(appSettingsPath);

                //var jsonSettings = new JsonSerializerSettings();
                //jsonSettings.Converters.Add(new ExpandoObjectConverter());
                //jsonSettings.Converters.Add(new StringEnumConverter());

                //dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

                //config.AzureAd.ClientId = az.ClientId;
                //config.AzureAd.TenantId = az.TenantId;
                //var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
                //File.WriteAllText(appSettingsPath, newJson);2




                // Validate whether all the required configurations are provided in appsettings.json
                string configValidationResult = ConfigValidatorService.ValidateConfig(azureAd, powerBI, _configuration);
                if (configValidationResult != null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return configValidationResult;
                }

                EmbedParams embedParams = pbiEmbedService.GetEmbedParams(new Guid(powerBI.Value.WorkspaceId), new Guid(powerBI.Value.ReportId));
                return System.Text.Json.JsonSerializer.Serialize<EmbedParams>(embedParams);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return ex.Message + "\n\n" + ex.StackTrace;
            }
        }
    }
}
