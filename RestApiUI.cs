using System;

namespace rest_api
{
    class RestApiUI
    {
        static string username;
        static string password;
        static string filename;
        static string title;
        static string description;
        static int id;

        public static void start()
        {
            string input;
            RestAPI api = new RestAPI();

            while (true)
            {
                Console.WriteLine("Enter a number of action\n" +
                                "1. Sign in\n" +
                                "2. Log in\n");
                input = Console.ReadLine();
                int result;
                if (input == "1" || input == "2")
                {
                    Console.Write("Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    password = Console.ReadLine();
                }
                switch (input)
                {
                    case "1":
                        result = api.controlUser(username, password, "POST");
                        break;
                    case "2":
                        result = api.controlUser(username, password, "GET");
                        break;
                    default:
                        result = -1;
                        break;
                }
                if (result == 0)
                {
                    while (true)
                    {
                        Console.WriteLine("Enter a number of action\n" +
                                "1. Get all todos\n" +
                                "2. Create todo\n" +
                                "3. Change todo\n" +
                                "4. Delete todo\n" +
                                "5. Upload file\n" +
                                "6. See all files\n" +
                                "7. Download file\n" +
                                "8. Delete file\n" +
                                "10. Log out\n" +
                                "11. Exit\n");

                        input = Console.ReadLine();
                        if (input == "11")
                        {
                            return;
                        }
                        if (input == "10")
                        {
                            break;
                        }
                        if (input.Length == 1 && input[0] > '1' && input[0] < '5')
                        {
                            if (input == "3" || input == "4")
                            {
                                while (true)
                                {
                                    try
                                    {
                                        Console.Write("Enter todo id what you wanna select for action: ");
                                        string st_id = Console.ReadLine();
                                        Convert.ToInt32(st_id);

                                        id = Convert.ToInt32(st_id);
                                        break;
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Can't read this id. Please, write again");
                                    }

                                }

                            }
                            if (input != "4")
                            {
                                Console.Write("Enter new title of todo: ");
                                title = Console.ReadLine();
                                Console.Write("Enter new description of todo: ");
                                description = Console.ReadLine();
                            }
                            
                        }
                        else if (input.Length == 1 && input[0] > '4' && input[0] < '9')
                        {
                            if (input != "6")
                            {
                                Console.Write("Enter filename what you wanna select for action: ");
                                filename = Console.ReadLine();
                            }
                        }
                        switch (input)
                        {
                            case "1":
                                api.controlTodo(username, password, "GET");
                                break;
                            case "2":
                                api.controlTodo(username, password, "POST", -1, title, description);
                                break;
                            case "3":
                                api.controlTodo(username, password, "PUT", id, title, description);
                                break;
                            case "4":
                                api.controlTodo(username, password, "DELETE", id);
                                break;
                            case "5":
                                api.controlFiles(username, password, "POST", filename);
                                break;
                            case "6":
                                api.controlFiles(username, password, "GET", "");
                                break;
                            case "7":
                                api.controlFiles(username, password, "GET", filename);
                                break;
                            case "8":
                                api.controlFiles(username, password, "DELETE", filename);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}