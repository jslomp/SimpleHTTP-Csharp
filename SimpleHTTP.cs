using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CustomTools
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
            this.cookies = cookies;
        }
        List<KeyValuePair<string, string>> myHeaders = new List<KeyValuePair<string, string>>();
        public void setHeader(string key, string value)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>(key, value);
            myHeaders.Add(kv);
        }
        private string GetContentType(string fileName)

        {

            string contentType = "application/octetstream";

            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                contentType = registryKey.GetValue("Content Type").ToString();
            }

            return contentType;

        

    }

    List<Dictionary<string, string>> PostingFields = new List<Dictionary<string, string>>();
    public void addPostFile(string key, string filePath)
        {
            if (File.Exists(filePath))
            {
                Dictionary<string, string> postObj = new Dictionary<string, string>();
                postObj.Add("Key", key);
                postObj.Add("Value", filePath);
                postObj.Add("type", "file");
                PostingFields.Add(postObj);
            }
        }
        public void addPostField(string key, string value)
        {
            generated_post_data += "--"+splittoken+"\n";
            generated_post_data += "Content-Disposition: form-data; name=\""+key+"\"\n";
            generated_post_data += "\n";
            generated_post_data += value+"\n";

            Dictionary<string, string> postObj = new Dictionary<string, string>();
            postObj.Add("Key", key);
            postObj.Add("Value", value);
            postObj.Add("type", "text");
            PostingFields.Add(postObj);

        }
        string generated_post_data = "";
        string splittoken = "----WebKitFormBoundary7frcLyNMUBKMSK8z";

        public string send(string postData = "")
        {
            return sendGetBytes(postData).ToString();
        }
        public byte[] sendGetBytes(string postData = "")
        {

            if(postData == "" && generated_post_data!="")
            {
                postData = generated_post_data+"\n";
                postData += "--" + splittoken + "--";
            }
            Console.WriteLine("---start call---");
            Console.WriteLine("URL=" + url + "");
            string responseBody = "";
            
            string content_type = "multipart/form-data; boundary=" + splittoken +"";
            if (postData.StartsWith("{"))
            {
                content_type = "application/json";
            }

            if (postData.Contains("="))
            {

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

            string ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36";
            foreach(KeyValuePair<string,string> head in myHeaders)
            {
                request.Headers.TryAddWithoutValidation(head.Key, head.Value);
                Console.WriteLine("setheader: " + head.Key + " = " + head.Value + "");
                if(head.Key == "User-Agent"){
                    ua = head.Value;
                }
            }

            request.Headers.TryAddWithoutValidation("User-Agent", ua);

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

            //string content = "";

            if (content_type == "application/json")
            {
                request.Content = new StringContent(postData, Encoding.UTF8, content_type);
            }
            else
            {
                var requestContent = new MultipartFormDataContent();
                foreach (Dictionary<string, string> field in PostingFields) {
                    if (field["type"] == "text")
                    {
                        StringContent textcontent = new StringContent(field["Value"], Encoding.UTF8);
                        requestContent.Add(textcontent, field["Key"]);
                    }
                    if (field["type"] == "file")
                    {
                        var imageContent = new ByteArrayContent(File.ReadAllBytes(field["Value"]));
                        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(GetContentType(field["Value"]));
                        requestContent.Add(imageContent, field["Key"], Path.GetFileName(field["Value"]));
                    }
                }
                request.Content = requestContent;
            }
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
                Console.WriteLine("request", e);
                Console.WriteLine(request.Headers);
                Console.WriteLine(e.Data);
                
                return null;
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
            byte[] responseBytes = response.Content.ReadAsByteArrayAsync().Result;
            //responseBody = response.Content.ReadAsStringAsync().Result;



            Console.WriteLine("Received:");
            Console.WriteLine(responseBody);
            Console.WriteLine("---end call---");

            return responseBytes;

        }

        public Dictionary<string,string> getCookies()
        {
            return public_cookies;
        }

    }

    
}
