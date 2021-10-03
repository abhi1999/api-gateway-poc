using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OzNet
{
    public class Destination
    {
        public string Path { get; set; }
        public bool SendCookiesAndAuth { get; set; }

        static HttpClientHandler handler = new HttpClientHandler();
        public bool RequiresAuthentication { get; set; }
        static HttpClient client = new HttpClient(handler, false);
        public Destination(string uri, bool requiresAuthentication)
        {
            Path = uri;
            RequiresAuthentication = requiresAuthentication;
        }

        public Destination(string path)
            :this(path, false)
        {
        }

        private Destination()
        {
            Path = "/";
            RequiresAuthentication = false;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequest request)
        {
            string requestContent;
            using (Stream receiveStream = request.Body)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    requestContent = readStream.ReadToEnd();
                }
            }

            using (var newRequest = new HttpRequestMessage(new HttpMethod(request.Method), CreateDestinationUri(request)))
            {
                newRequest.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);

                if (this.SendCookiesAndAuth)
                {


                    foreach (var c in request.Cookies)
                    {
                        newRequest.Headers.Add("Cookie", c.Key + "=" + c.Value);
                    }
                    /*if (request.Headers["Authorization"].ToString().Length > 0 && request.Headers["Authorization"].ToString().IndexOf("null") == -1){
                        newRequest.Headers.Add("Authorization", request.Headers["Authorization"].ToString());
                    }
                    if (request.Headers["ClientKey"].ToString().Length > 0 && request.Headers["ClientKey"].ToString().IndexOf("null") == -1)
                    {
                        newRequest.Headers.Add("ClientKey", request.Headers["ClientKey"].ToString());
                    }*/
                    addCustomHeader("Authorization", request, newRequest);
                    addCustomHeader("ClientKey", request, newRequest);

                }
                HttpResponseMessage response = await client.SendAsync(newRequest);

                return response;
                 
                /*using (HttpResponseMessage response = await client.SendAsync(newRequest))
                {
                    //var responseString = await response.Content.ReadAsStringAsync();
                    // return responseString;
                    return response;
                }*/
            }
        }
        private static void addCustomHeader(string headerName, HttpRequest request, HttpRequestMessage newRequest)
        {
            if (request.Headers[headerName].ToString().Length > 0 && request.Headers[headerName].ToString().IndexOf("null") == -1)
            {
                newRequest.Headers.Add(headerName, request.Headers[headerName].ToString());
            }
        }

        private string CreateDestinationUri(HttpRequest request)
        {
            string requestPath = request.Path.ToString();
            string queryString = request.QueryString.ToString();

            string endpoint = "";
          //  string[] endpointSplit = requestPath.Substring(1).Split('/');

         //   if (endpointSplit.Length > 1)
         //       endpoint = endpointSplit[1];


            return Path + endpoint + queryString;
        }

    }
}
