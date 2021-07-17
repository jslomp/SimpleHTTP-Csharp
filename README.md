# SimpleHTTP-Csharp

Example 1

public byte[] getBlob(string query)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();

            SimpleHTTP req = new SimpleHTTP(site_domain + "/GetBlob.aspx?requestID="+ unixTimeMilliseconds.ToString()+ "&"+query+"");
            //req.setHeader("X_RequestToken", loginToken);
            req.setHeader("Origin", site_domain);
            req.setCookies(cookies);

            req.addPostField("X_RequestToken", loginToken);

            return req.sendGetBytes();

        }
        
        
        
        example 2
        
        
        SimpleHTTP req = new SimpleHTTP(site_domain + "/HandleUpload.aspx");
                    //req.setHeader("X_RequestToken", loginToken);
                    req.setHeader("Origin", site_domain);
                    req.setCookies(cookies);

                    req.addPostField("X_RequestToken", loginToken);

req.addPostField("Key", "value");

req.send();
