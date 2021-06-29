using TenmoClient.Models;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Exceptions;

namespace TenmoClient
{
    public static class UserService
    {
        private static ApiUser user = new ApiUser();
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string Transaction_URL = API_BASE_URL + "transaction";
        private readonly static string Account_URL = API_BASE_URL + "account";
        private readonly static IRestClient client;

        public static bool LoggedIn { get { return !string.IsNullOrWhiteSpace(user.Token); } }

        public static void SetLogin(ApiUser u)
        {
            user = u;
        }

        public static int GetUserId()
        {
            return user.UserId;
        }

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(user.Token);
        }

        public static string GetToken()
        {
            return user?.Token ?? string.Empty;
        }

        public static string GetBalance(int accountID)
        {
            RestRequest request = new RestRequest(Account_URL + "/" + accountID);
            IRestResponse<string> response =  client.Get<string>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }

            return "";
        }

        public static void ProcessErrorResponse(IRestResponse response)
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
    }
}
