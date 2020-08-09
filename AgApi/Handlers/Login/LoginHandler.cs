using MediatR;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AppV.Models.CQRS.Handlers
{
    public class LoginHandler : IRequestHandler<LoginModel, LoginResponse>
    {
        private readonly ILogger _logger;

        public LoginHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<LoginResponse> Handle(LoginModel request, CancellationToken cancellationToken)
        {
            LoginResponse LR = new LoginResponse();
            var getTokenUrl = ConfigurationManager.AppSettings["WorkflowAddress"].ToString() + @"/token";
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpContent content;
                    if (String.IsNullOrEmpty(request.RefreshToken))
                    {
                        content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("grant_type", "password"),
                            new KeyValuePair<string, string>("username", request.UserName),
                            new KeyValuePair<string, string>("password", request.Password)
                        });
                    } else
                    {
                        content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("refresh_token", request.RefreshToken),
                            new KeyValuePair<string, string>("grant_type", "refresh_token")
                        });

                    }
                    HttpResponseMessage result = httpClient.PostAsync(getTokenUrl, content).Result;

                    string resultContent = result.Content.ReadAsStringAsync().Result;

                    var token = JsonConvert.DeserializeObject<Token>(resultContent);

                    if (token != null && token.access_token != null)
                    {
                        var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, token.userName),
                        new Claim(ClaimTypes.NameIdentifier, token.uid.ToString()),
                        new Claim(ClaimTypes.GivenName, token.Nome),
                        new Claim(ClaimTypes.Surname, String.IsNullOrEmpty(token.Sobrenome) ? "" : token.Sobrenome),
                        new Claim(ClaimTypes.Email, token.Email),
                        new Claim("GECO", token.gid.ToString()),
                        new Claim("AcessToken", string.Format("{0}", token.access_token)),
                    };

                        if (!String.IsNullOrEmpty(token.Roles))
                        {
                            String[] roles = token.Roles.Split(',');
                            foreach (string r in roles)
                                claims.Add(new Claim(ClaimTypes.Role, r));
                        }

                        var identity = new ClaimsIdentity(claims.ToArray(), "ApplicationCookie");

                        LR.Sucesso = true;
                        LR.resultData = identity;
                        LR.ExpiresIN = token.expires_in;
                    }
                    else
                    {
                        LR.Sucesso = false;
                        LR.Erros = new string[] { "Usuário ou senha incorretos!" };
                        LR.resultData = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Falha no serviço de autenticação.");
                LR.Sucesso = false;
                LR.Erros = new string[] { "Serviço de autenticação fora do ar!" };
                LR.resultData = null;
            }
            return await Task.FromResult(LR);
        }
    }
}