using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CouchDB.Client
{
    public partial class CouchDatabase
    {
        /// <summary>
        /// http://docs.couchdb.org/en/2.2.0/api/database/common.html#post--db
        /// </summary>
        /// <param name="json"></param>
        /// <param name="batchMode">http://docs.couchdb.org/en/2.2.0/api/database/common.html#batch-mode-writes</param>
        /// <returns></returns>
        public async Task<CouchResponse> CreateIndexAsync(string field, string indexName = null)
        {
            var request = new RestSharp.RestRequest("_index", RestSharp.Method.POST);

            JObject obj = new JObject
            {
                { "type", "json" },
                {
                    "index", new JObject
                    {
                        { "fields", new JArray { field } }
                    }
                }
            };

            if (!string.IsNullOrEmpty(indexName))
                obj.Add("name", indexName);

            request.AddParameter("application/json", obj, ParameterType.RequestBody);
            return await client.http.ExecuteAsync(request);
        }

        public async Task<CouchResponse> InsertDesignDocumentAsync(string name, object json)
        {
            var request = new RestSharp.RestRequest(RestSharp.Method.PUT);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            request.Resource = "_design/" + name;
            var res = await client.http.ExecuteAsync(request);
            if (res.ErrorException != null)
                throw res.ErrorException;
            return res;
        }
    }
}
