using TenmoClient.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace TenmoClient
{
    public static class UserService
    {
        private static ApiUser user = new ApiUser();
        //private readonly static string API_BASE_URL = "https://localhost:44315/";
        //private readonly static IRestClient client;

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
    }
}
