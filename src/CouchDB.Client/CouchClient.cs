using EnsureThat;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace CouchDB.Client
{
    public class ResponseException: Exception
    {
        public HttpStatusCode Status;
        public string StatusDescription;
        public ResponseException(string message, HttpStatusCode status, string statusDescription, Exception innerException = null) : base(message, innerException)
        {
            Status = status;
            StatusDescription = statusDescription;
        }
    }
    public partial class CouchClient
    {
        internal string originalConnectionString;
        internal RestClientWrapper http;
        internal string connectionString;
        internal Uri baseUrl;
        
        public CouchClient(string connectionString = null)
        {
            this.originalConnectionString = connectionString;
            
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "127.0.0.1:5984";
            }

            connectionString = connectionString.EndsWith("/", StringComparison.InvariantCultureIgnoreCase) ? connectionString : connectionString + "/";
            connectionString = connectionString.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ? connectionString : "http://" + connectionString;
            Uri uri = new Uri(connectionString);

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                connectionString = connectionString.Replace(uri.UserInfo + "@", string.Empty);
            }

            this.connectionString = connectionString;
            var client = new RestClient(connectionString);
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                client.Authenticator = new HttpBasicAuthenticator(
                    uri.UserInfo.Split(':')[0],
                    uri.UserInfo.Split(':')[1]
                );
            }

            http = new RestClientWrapper(client);
            http.client.AddDefaultHeader("Content-Type", "application/json");
            baseUrl = http.client.BaseUrl;
        }

        public void ResetBaseUrl()
        {
            http.client.BaseUrl = baseUrl;
        }

        public async Task<CouchDatabase> GetDatabaseAsync(string database, bool throwOnError = true)
        {
            var db = await this.GetDatabaseInfoAsync(database);
            if (db.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new CouchDatabase(this, database);
            }
            else
            {
                if (throwOnError)
                    throw new ResponseException(db.ErrorMessage, db.StatusCode, db.StatusDescription, db.ErrorException);
                else
                    return null;
            }
        }

        public async Task<CouchResponse> InsertManually(string path, object content)
        {
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddParameter("application/json", content, ParameterType.RequestBody);
            request.Resource = path;
            return await http.ExecuteAsync(request);
        }
    }
}
