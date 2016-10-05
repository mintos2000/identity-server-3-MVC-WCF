using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.WsFederation.Models;
using IdentityModel.Constants;


namespace MVCiServer
{
    public class RelyingParties
    {
       
        public static IEnumerable<RelyingParty> Get()
        {
            return new List<RelyingParty>
            {
                new RelyingParty
                {
                    Realm = "urn:testrp",
                    Name = "Test RP",
                    Enabled = true,
                    ReplyUrl = "https://web.local/idsrvrp/",
                    TokenType = TokenTypes.Saml2TokenProfile11,
                    TokenLifeTime = 1,

                    ClaimMappings = new Dictionary<string,string>
                    {
                        { "sub", ClaimTypes.NameIdentifier },
                        { "given_name", ClaimTypes.Name },
                        { "email",ClaimTypes.Email }
                    }
                },
                new RelyingParty
                {
                    Realm = "urn:owinrp",
                    Enabled = true,
                    ReplyUrl = "http://localhost:10313/",
                    TokenType = TokenTypes.Saml2TokenProfile11,
                    TokenLifeTime = 1,

                    ClaimMappings = new Dictionary<string, string>
                    {
                        { "sub", ClaimTypes.NameIdentifier },
                        { "name", ClaimTypes.Name },
                        { "given_name", ClaimTypes.GivenName },
                        { "email", ClaimTypes.Email }
                    }
                    // ClaimMappings = new Dictionary<string,string>
                    //{
                    //    { "sub", System.IdentityModel.Claims.ClaimTypes.NameIdentifier },
                    //    { "given_name", System.IdentityModel.Claims.ClaimTypes.Name },
                    //    { "email", System.IdentityModel.Claims.ClaimTypes.Email }
                    //}
                }
            };
        }
    }
}
