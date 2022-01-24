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
                Console.WriteLine(res.data + '\n');
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(res.error_message + '\n');
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
    }
}