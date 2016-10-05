using IdentityModel.Client;
using IdentityModel.Constants;
using IdentityModel.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
//using System.Net.Http;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MVCiServer.Controllers
{
    //cd C:\Program Files (x86)\IIS Express
    //IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44319/ -UseSelfSigned
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult About()
        {
            return View((User as ClaimsPrincipal).Claims);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        static TokenResponse RequestToken()
        {
            var client = new TokenClient(
                "https://localhost:44300/",
                "mvc");

            return client.RequestClientCredentialsAsync("read write").Result;
        }

        public  ActionResult ClientCredentials()
        {
            var response = GetTokenAsync();
            var result = CallApi(response.AccessToken);

            ViewBag.Json = result;
            return Content(result);
        }

        // GET: CallApi/UserCredentials
        public ActionResult UserCredentials()
        {
            var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token").Value;
            var result =  CallApi(token);

            ViewBag.Json = result;
            return Content(result);
        }

        //private static SecurityToken RequestToken()
        //{
        //    var binding = new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential);

        //    var credentials = new ClientCredentials();
        //    credentials.UserName.UserName = "username";
        //    credentials.UserName.Password = "password";

        //    return WSTrustClient.Issue(
        //           new EndpointAddress(_idsrvEndpoint),
        //           new EndpointAddress(_realm),
        //           binding,
        //           credentials);
        //}

        private  string  CallApi(string token)
        {
            //var client = new System.Net.Http.HttpClient();
            //client.SetBearerToken(token);
            var xmltoken = WrapJwt(token);
            //var json = await client.GetStringAsync("https://localhost:44321/identity");
            //return JArray.Parse(json).ToString();
            var binding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
            //binding.Security.Mode
            //var factory = new ChannelFactory<ServiceReference1.IService1>(
            //    binding,
            //    new EndpointAddress("https://localhost:44335/token"));

            //var channel = factory.CreateChannelWithIssuedToken(token);

            var factory = new ChannelFactory<ServiceReference1.IService1>(
              binding,
              new EndpointAddress("https://localhost:44302/Service1.svc"));
            factory.Credentials.SupportInteractive = false;
            factory.Credentials.UseIdentityConfiguration = true;
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            //factory.Credentials.ClientCertificate.Certificate= new System.Security.Cryptography.X509Certificates.X509Certificate2
            //    ( )
                //.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;
            // create channel with specified token
            var proxy = factory.CreateChannelWithIssuedToken(xmltoken);

            var id = proxy.GetData(1);

            return "";
        }

        static GenericXmlSecurityToken WrapJwt(string jwt)
        {
            var subject = new ClaimsIdentity("saml");
            subject.AddClaim(new Claim("jwt", jwt));

            var descriptor = new SecurityTokenDescriptor
            {
                TokenType = TokenTypes.Saml2TokenProfile11,
                TokenIssuerName = "urn:wrappedjwt",
                Subject = subject
            };

            var handler = new Saml2SecurityTokenHandler();
            var token = handler.CreateToken(descriptor);

            var xmlToken = new GenericXmlSecurityToken(
               XElement.Parse(token.ToTokenXmlString()).ToXmlElement(),
                null,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                null,
                null,
                null);

            return xmlToken;
        }

        private TokenResponse GetTokenAsync()
        {
            var client = new TokenClient(
                "https://localhost:44300/identity/connect/token",
                "mvc_service",
                "secret");

            return client.RequestClientCredentialsAsync("sampleApi").Result;
        }

        //private static SecurityToken RequestSecurityToken()
        //{
        //var binding = new WS2007FederationHttpBinding(SecurityMode.TransportWithMessageCredential);

        //var credentials = new ClientCredentials();
        //credentials.UserName.UserName = "bob";
        //credentials.UserName.Password = "abc!123";

        //return WSTrustClient.Issue(
        //    new EndpointAddress(_idsrvEndpoint),
        //    new EndpointAddress(_realm),
        //    binding,
        //    credentials);

        // set up the ws-trust channel factory
        //var factory = new WSTrustChannelFactory(
        //    new UserNameWSTrustBinding(
        //      SecurityMode.TransportWithMessageCredential),
        //      _idpAddress);
        //factory.TrustVersion = TrustVersion.WSTrust13;

        //factory.Credentials.UserName.UserName = “bob”;
        //factory.Credentials.UserName.Password = “abc!123”;

        //// create token request
        //var rst = new RequestSecurityToken
        //{
        //    RequestType = RequestTypes.Issue,
        //    KeyType = KeyTypes.Symmetric,
        //    AppliesTo = new EndpointReference("http://localhost:4684/Service1.svc")
        //};

        //// request token and return
        //return factory.CreateChannel().Issue(rst);
        //}



        private static void CallService(SecurityToken token)
        {
            var binding = new WS2007FederationHttpBinding(
              WSFederationHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.EstablishSecurityContext = false;

            //EndpointAddress address = new EndpointAddress("http://localhost:4684/Service1.svc");
            //ChannelFactory factory = new ChannelFactory<WcfService1.IService1>(binding, address);
            //WcfService1.IService1 channel = factory.CreateChannel();
            //string resturnmessage = channel.YourMethod("test");

            // set up channel factory
            var factory = new ChannelFactory<ServiceReference1.IService1>(
              binding,
              new EndpointAddress("https://localhost:44302/Service1.svc"));
            factory.Credentials.SupportInteractive = false;
            factory.Credentials.UseIdentityConfiguration = true;

            // create channel with specified token
            var proxy = factory.CreateChannelWithIssuedToken(token);

            var id = proxy.GetData(1);
        }
    }
}