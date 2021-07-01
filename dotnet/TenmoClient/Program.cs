using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        ApiUser user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    try
                    {
                        Console.WriteLine("Your current account balance is: $" + authService.GetBalance());
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                }
                else if (menuSelection == 2)
                {
                    //Needs to check both "to" and "from" transfers
                    //Implement getting the transfer details for now.
                    //Method to get the transfer details at the specific Id
                    //Auth, Transactiondao, transactionController
                    Console.WriteLine("---------");
                    Console.WriteLine("Transfers\nID    From/To         Amount");
                    Console.WriteLine("---------");
                    List<Transaction> transactions = authService.GetTransactions();
                    foreach (Transaction item in transactions)
                    {
                        if (item.FromId == UserService.GetUserId())
                        {
                            Console.WriteLine($"{item.Id}    From: {item.ToId}     ${item.Amount}");
                        }
                        else
                        {
                            Console.WriteLine($"{item.Id}    To: {item.FromId}     ${item.Amount}");
                        }
                    }
                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    try
                    {
                        List<User> listOfUsers = authService.GetUsers();
                        foreach (User user in listOfUsers)
                        {
                            if (user.UserId != UserService.GetUserId())
                            {
                                Console.WriteLine();
                                Console.WriteLine($"{user.UserId} , {user.Username}");
                            }  
                        }
                        Console.WriteLine();
                        Console.WriteLine("---------");
                        Console.WriteLine("Who would you like to send money to? ");
                        string receiverIdString = Console.ReadLine();
                        int receiverId = int.Parse(receiverIdString);

                        //TODO fine tune selecting an ID thats not yourself
                        if (receiverId == UserService.GetUserId())
                        {
                            Console.WriteLine("You are unable to send money to yourself, please select a valid Id.");
                        }
                        else
                        {
                            //Display error message if amount sent is more than balance
                            Console.WriteLine("How much money would you like to send? ");
                            string amountAsString = Console.ReadLine();
                            decimal amount = decimal.Parse(amountAsString);
                            Console.WriteLine("---------");
                            Console.WriteLine("Your remaining balance is $" + authService.SendMoney(receiverId, amount));
                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new ApiUser()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
