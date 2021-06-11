using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebFunctions
{
    class SimpleHTTP
    {

        public Dictionary<string, string> public_cookies = null;
        private string url;

        public SimpleHTTP(string url)
        {
            this.url = url;
        }
        public Dictionary<string, string> cookies = null;
        public void setCookies(Dictionary<string, string> cookies = null)
        {
            this.public_cookies = cookies;
        }
        public string send(string postData = "")
        {
            Console.WriteLine("---start call---");
            Console.WriteLine("URL=" + url + "");
            string responseBody = "";
            string content_type = "application/json";
            if (postData.StartsWith("{"))
            {
                content_type = "application/json";
            }

            //StringContent requestContent = new StringContent(postData, Encoding.UTF8, content_type);

            HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };

            request.Headers.TryAddWithoutValidation("Content-Type", content_type);
            request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip");
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36");
            string cookie_string = "";

            if (cookies != null && cookies.Count > 0)
            {

                foreach (KeyValuePair<string, string> cookie in cookies)
                {
                    string cstring = "" + cookie.Key + "=" + cookie.Value + "";
                    cookie_string += "; " + cstring;

                }

                cookie_string = cookie_string.Substring(1).Trim();
                request.Headers.TryAddWithoutValidation("Cookie", cookie_string);
                Console.WriteLine("Cookie = " + cookie_string + "");
            }



            //MessageBox.Show(request.Headers.ToString());

            request.Content = new StringContent(postData, Encoding.UTF8, content_type);
            Console.WriteLine("POST:-----");
            Console.WriteLine(postData);

            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json; charset=UTF-8");

            HttpResponseMessage response = client.SendAsync(request).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine("request");
                Console.WriteLine(request.Headers);

                return "";
            }



            foreach (var headerName in response.Headers)
            {
                if (headerName.Key == "Set-Cookie")
                {

                    Console.WriteLine("received:" + headerName.Key + " = " + headerName.Value);

                    foreach (var values in headerName.Value)
                    {
                        string v = values.Split(';')[0];
                        string[] n = v.Split('=');
                        if (cookies.ContainsKey(n[0]))
                        {
                            cookies[n[0]] = n[1];
                        }
                        else
                        {
                            cookies.Add(n[0], n[1]);
                        }

                    }

                }
            }
            public_cookies = cookies;
            responseBody = response.Content.ReadAsStringAsync().Result;



            Console.WriteLine("Received:");
            Console.WriteLine(responseBody);
            Console.WriteLine("---end call---");

            return responseBody;

        }

        public Dictionary<string, string> getCookies()
        {
            return public_cookies;
        }

    }


}
