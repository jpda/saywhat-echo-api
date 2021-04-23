using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens.Jwt;

namespace echo_func
{
    public static class EchoApi
    {
        [FunctionName("headers")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            return new OkObjectResult(GetHeadersAndParams(req));
        }

        [FunctionName("who")]
        public static IActionResult ReadAuthorizationHeader([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "who")] HttpRequest request, ILogger log)
        {
            if (!request.Headers.ContainsKey("Authorization"))
            {
                return new OkObjectResult(new { Message = "No header" });
            }

            // something wrong with value
            if (!request.Headers.TryGetValue("Authorization", out var headerValue)) return new BadRequestObjectResult(new { Message = "Authorization header value is malformed", Data = GetHeadersAndParams(request) });

            var headerParts = headerValue.ToString().Split(' ');
            if (headerParts.Length != 2 && !string.Equals(headerParts[0], "Bearer", StringComparison.OrdinalIgnoreCase))
            {
                return new BadRequestObjectResult(new { Message = "Authorization header value is malformed", Data = GetHeadersAndParams(request) });
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            if (jwtHandler.CanReadToken(headerParts[1]))
            {
                var token = jwtHandler.ReadJwtToken(headerParts[1]);
                return new OkObjectResult(token.Claims.Select(x => new { x.Type, x.Value }));
            }
            return new BadRequestObjectResult(new { Message = "Something went wrong" });
        }

        private static List<KeyValuePair<string, string>> GetHeadersAndParams(HttpRequest req)
        {
            var a = new List<KeyValuePair<string, string>>();
            foreach (var q in req.Query.Keys)
            {
                a.Add(new KeyValuePair<string, string>(q, req.Query[q]));
            }
            foreach (var q in req.Headers.Keys)
            {
                a.Add(new KeyValuePair<string, string>(q, req.Headers[q]));
            }
            return a;
        }
    }
}
