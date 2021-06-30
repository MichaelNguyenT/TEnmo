using RestSharp;
using RestSharp.Authenticators;
using System;
using TenmoClient.Models;
using TenmoClient.Exceptions;
using System.Collections.Generic;

namespace TenmoClient
{
    public class AuthService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string Transaction_URL = API_BASE_URL + "transaction";
        private readonly static string Account_URL = API_BASE_URL + "account";
        private readonly static string USER_URL = API_BASE_URL + "user";
        private readonly IRestClient client;

        public AuthService()
        {
            client = new RestClient();
        }

        public AuthService(IRestClient restClient)
        {
            client = restClient;
        }

        public void ProcessErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new NoResponseException("Error occurred - unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                if (response.StatusCode.ToString() == "Unauthorized")
                {
                    throw new UnauthorizedException();
                }
                else if (response.StatusCode.ToString() == "Forbidden")
                {
                    throw new ForbiddenException();
                }
                throw new NonSuccessException((int)response.StatusCode);
            }
        }

        public bool Register(LoginUser registerUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login/register");
            request.AddJsonBody(registerUser);
            IRestResponse<ApiUser> response = client.Post<ApiUser>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public ApiUser Login(LoginUser loginUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login");
            request.AddJsonBody(loginUser);
            IRestResponse<ApiUser> response = client.Post<ApiUser>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return null;
            }
            else
            {
                client.Authenticator = new JwtAuthenticator(response.Data.Token);
                return response.Data;
            }
        }

        public decimal GetBalance()
        {
            RestRequest request = new RestRequest(Account_URL);
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
                return 0;
            }
            else
            {
                return response.Data;
            }
        }

        public List<User> GetUsers()
        {
            RestRequest request = new RestRequest(USER_URL);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            return response.Data;
        }

        public decimal SendMoney(int receiverId, decimal amount)
        {
            RestRequest request = new RestRequest(Account_URL + $"/{receiverId}/{amount}");
            IRestResponse<decimal> response = client.Put<decimal>(request);
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            return response.Data;
        }
    }
}
