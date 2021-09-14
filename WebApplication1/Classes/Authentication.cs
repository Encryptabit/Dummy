using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace WebApplication1
{
    public class Authentication
    {
        static IConfigurationManager<OpenIdConnectConfiguration> configurationManager = null;
        static OpenIdConnectConfiguration openIdConfig = null;

        // Auth0 Config
        static class Configuration
        {
            public static string Auth0_ServerHost = "https://jjones-test.us.auth0.com/";
            public static string Auth0_ApiAudience = "https://jjones-wlt.corp.stamps.com";
        }


        public static void LoadPublicKeys()
        {
            try
            {
                if (configurationManager == null)
                {
                    configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                                                $"{Configuration.Auth0_ServerHost}oauth/.well-known/openid-configuration",
                                                new OpenIdConnectConfigurationRetriever()
                                                );
                    openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
                }
            } catch (Exception Ex)
            {
                
            }
        }

        public static bool IsTokenValid(string accessToken)
        {
            // Token validation params
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidAudiences = new[] { Configuration.Auth0_ApiAudience },
                ValidIssuer = Configuration.Auth0_ServerHost,
                IssuerSigningKeys = openIdConfig.SigningKeys
            };

            string testToken = "blahblah";

            // Test to make sure it works
            //accessToken = testToken;

            // Validate the token
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = handler.ValidateToken(
                accessToken,
                validationParameters,
                out validatedToken
                );

            // Check Token expiry
            if (validatedToken.ValidTo < DateTime.Now)
                throw new AuthenticationException();

            return validatedToken.Issuer == Configuration.Auth0_ServerHost;
        }
    }
}