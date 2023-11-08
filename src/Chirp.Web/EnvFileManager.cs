using DotNetEnv;
using System;

public class EnvFileManager
{
    public static string GetConnectionString(bool isDevelopment)
    {
        DotNetEnv.Env.Load();

        if (isDevelopment)
        {
            var azureServer = DotNetEnv.Env.GetString("AZURE_SQL_SERVER");
            var azureInitialCatalog = DotNetEnv.Env.GetString("AZURE_SQL_INITIAL_CATALOG");
            var azureUser = DotNetEnv.Env.GetString("AZURE_SQL_USER");
            var azurePassword = DotNetEnv.Env.GetString("AZURE_SQL_PASSWORD");

            return $"Server={azureServer};Initial Catalog={azureInitialCatalog};Persist Security Info=False;User ID={azureUser};Password={azurePassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }
        else
        {
            // Provide an alternative connection string for non-development environments if needed.
            throw new NotImplementedException("Non-development connection string not implemented.");
        }

    }

    //Don't know where to use it
    public static void GetAzureADB2C()
    {
        DotNetEnv.Env.Load();
        string instance = DotNetEnv.Env.GetString("AZUREAD_B2C_INSTANCE");
        string clientId = DotNetEnv.Env.GetString("AZUREAD_B2C_CLIENTID");
        string domain = DotNetEnv.Env.GetString("AZUREAD_B2C_DOMAIN");
        string signedOutCallbackPath = DotNetEnv.Env.GetString("AZUREAD_B2C_SIGNEDOUTCALLBACKPATH");
        string signUpSignInPolicyId = DotNetEnv.Env.GetString("AZUREAD_B2C_SIGNUPSIGNINPOLICYID");
        string clientSecret = DotNetEnv.Env.GetString("AZUREAD_B2C_CLIENTSECRET");
        string callbackPath = DotNetEnv.Env.GetString("AZUREAD_B2C_CALLBACKPATH");

    }
}
