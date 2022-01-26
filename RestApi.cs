using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace rest_api
{
    
    class RestAPI
    {
        struct response_result
        {
            public string data { get; set; }
            public string error_message { get; set; }

        }

        struct todo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }

        }

        struct file
        {
            public int id { get; set; }
            public string filename { get; set; }
            public string size_byte { get; set; }
        }

        static string URL;

        public RestAPI()
        {
            URL = "http://164.90.174.17/";
        }
        public RestAPI(string new_url)
        {
            URL = new_url;
        }

        private void printResponseResult(response_result res)
        {
            if (res.data != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(res.data + '\n');
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(res.error_message + '\n');
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void printTodo(List<todo> list)
        {
            foreach (var s in list)
            {
                Console.Write("Id: ");
                Console.WriteLine(s.id);
                Console.Write("Title: ");
                Console.WriteLine(s.title);
                Console.Write("Description: ");
                Console.WriteLine(s.description);
                Console.WriteLine();
            }
        }

        private void printFiles(List<file> list)
        {
            foreach (var s in list)
            {
                Console.Write("Id: ");
                Console.WriteLine(s.id);
                Console.Write("Filename: ");
                Console.WriteLine(s.filename);
                Console.Write("File size in bytes: ");
                Console.WriteLine(s.size_byte);
                Console.WriteLine();
            }
        }

        public int controlUser(string username, string password, string method)
        {
            WebRequest request = WebRequest.Create(URL + "user/");
            request.Credentials = CredentialCache.DefaultCredentials;

            request.ContentType = "application/json";
            request.Method = method;

            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(username + ":" + password));
            request.Headers.Add("authorization", "Basic " + encoded);
            
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    string responseFromServer = reader.ReadToEnd();

                    response_result res = JsonConvert.DeserializeObject<response_result>(responseFromServer);
                    printResponseResult(res);
                    reader.Close();
                    response.Close();
                    return 0;
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    HttpWebResponse errorResponse = (HttpWebResponse)wex.Response;
                    StreamReader reader = new StreamReader(errorResponse.GetResponseStream());
                    string error = reader.ReadToEnd();
                    response_result res = JsonConvert.DeserializeObject<response_result>(error);
                    printResponseResult(res);
                    reader.Close();
                    errorResponse.Close();
                    return -1;
                }
            }
            return -1;
        }

        public int controlTodo(string username, string password, string method, int num = -1, string title = "", string description = "")
        {
            string tmp_url = URL;
            if (method == "GET" || method == "POST")
                tmp_url += "todo/";
            else
                tmp_url += "todo/" + num;

            WebRequest request = WebRequest.Create(tmp_url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "application/json";
            request.Method = method;

            if (method == "POST" || method == "PUT")
            {
                StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
                string json = "{\"title\":\"" + title + "\"," +
                                "\"description\":\"" + description + "\"}";

                streamWriter.Write(json);
                streamWriter.Close();
            }

            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(username + ":" + password));
            request.Headers.Add("authorization", "Basic " + encoded);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    string responseFromServer = reader.ReadToEnd();

                    if (method == "GET")
                    {
                        if (responseFromServer[responseFromServer.Length-1] != ']')
                        {
                            response_result res1 = JsonConvert.DeserializeObject<response_result>(responseFromServer);
                            printResponseResult(res1);
                            return 0;
                        }
                        List<todo> res = JsonConvert.DeserializeObject<List<todo>>(responseFromServer);

                        printTodo(res);
                    }
                    else
                    {
                        response_result res = JsonConvert.DeserializeObject<response_result>(responseFromServer);
                        printResponseResult(res);
                    }

                    reader.Close();
                    response.Close();
                    return 0;
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    HttpWebResponse errorResponse = (HttpWebResponse)wex.Response;
                    StreamReader reader = new StreamReader(errorResponse.GetResponseStream());
                    string error = reader.ReadToEnd();
                    response_result res = JsonConvert.DeserializeObject<response_result>(error);
                    printResponseResult(res);
                    return -1;
                }
            }
            return -1;
        }

        public int controlFiles(string username, string password, string method, string filename = "")
        {
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(username + ":" + password));

            WebRequest request;
            if (method == "POST")
            {
                request = WebRequest.Create(URL + "files/");
            }
            else
                request = WebRequest.Create(URL + "files/" + filename);

            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = method;
            request.Headers.Add("authorization", "Basic " + encoded);

            try
            {
                if (method == "POST")
                {
                    if (!File.Exists(filename))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File doesn't exist");
                        Console.ForegroundColor = ConsoleColor.White;
                        return -1;
                    }

                    using (var client = new WebClient())
                    {
                        client.Headers.Add("authorization", "Basic " + encoded);

                        client.UploadFile(URL + "files/", filename);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Your file has been uploaded\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return 0;
                    }
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string responseFromServer = reader.ReadToEnd();

                if (response != null)
                {
                    if (response.ContentType == "application/json")
                    {
                        if (filename == "")
                        {
                            List<file> res = JsonConvert.DeserializeObject<List<file>>(responseFromServer);
                            printFiles(res);
                            return 0;
                        }
                        else
                        {
                            response_result res = JsonConvert.DeserializeObject<response_result>(responseFromServer);
                            printResponseResult(res);
                            return 0;
                        }
                    }
                    if (method == "GET" && filename != "")
                    {
                        
                        using (var client = new WebClient())
                        {
                            client.Headers.Add("authorization", "Basic " + encoded);

                            
                            client.DownloadFile(URL + "files/" + filename, filename);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Dowload file complete\n");
                            Console.ForegroundColor = ConsoleColor.White;

                            return 0;
                        }
                    }
                    
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    HttpWebResponse errorResponse = (HttpWebResponse)wex.Response;
                    StreamReader reader = new StreamReader(errorResponse.GetResponseStream());
                    string error = reader.ReadToEnd();
                    response_result res = JsonConvert.DeserializeObject<response_result>(error);
                    printResponseResult(res);
                    return -1;
                }
            }
            return -1;
        }
    }
}