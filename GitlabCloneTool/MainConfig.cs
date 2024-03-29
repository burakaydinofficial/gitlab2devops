﻿using System;
using Newtonsoft.Json;

namespace GitlabCloneTool
{
    internal class MainConfig
    {
        public string CloneDirectory = " ";
        public string GitlabPrivateToken = " ";
        public string AzureUserName = " ";
        public string AzureAccessToken = " ";
        public string AzureOrganization = " ";

        public static MainConfig Parse(string json)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<MainConfig>(json);
                if (config == null || string.IsNullOrWhiteSpace(config.GitlabPrivateToken) ||
                    string.IsNullOrWhiteSpace(config.CloneDirectory))
                    return null;
                return config;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public bool IsAzureConfigured()
        {
            return !string.IsNullOrWhiteSpace(AzureAccessToken) && !string.IsNullOrWhiteSpace(AzureUserName) && !string.IsNullOrWhiteSpace(AzureOrganization);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}