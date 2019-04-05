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
        public async Task<CouchResponse> InsertAsync(JToken json, bool batchMode = false)
        {
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            if (batchMode)
                request.AddQueryParameter("batch", "ok");

            request.AddParameter("application/json", json, ParameterType.RequestBody);
            return await client.http.ExecuteAsync(request);
        }

        /// <summary>
        /// http://docs.couchdb.org/en/2.2.0/api/database/common.html#post--db
        /// </summary>
        /// <param name="json"></param>
        /// <param name="batchMode">http://docs.couchdb.org/en/2.2.0/api/database/common.html#batch-mode-writes</param>
        /// <returns></returns>
        //public async Task<CouchResponse> InsertDesignDocAsync(JToken json, string designType = "_view")
        //{
        //    var request = new RestSharp.RestRequest(RestSharp.Method.POST);

        //    request.AddParameter("application/json", json, ParameterType.RequestBody);
        //    //TODO 
        //    request.Resource = "_design/" +
        //    return await client.http.ExecuteAsync(request);
        //}


        /// <summary>
        /// http://docs.couchdb.org/en/2.2.0/api/database/common.html#specifying-the-document-id
        /// Specify ID for insert
        /// </summary>
        /// <param name="json"></param>
        /// <param name="batchMode">http://docs.couchdb.org/en/2.2.0/api/database/common.html#batch-mode-writes</param>
        /// <returns></returns>
        public async Task<CouchResponse> InsertAsync(JToken json, string id, bool batchMode = false)
        {
            json["_id"] = id;
            return await this.InsertAsync(json, batchMode);
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

        /// <summary>
        /// http://docs.couchdb.org/en/2.1.2/api/database/common.html#post--db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="batchMode">http://docs.couchdb.org/en/2.1.2/api/database/common.html#batch-mode-writes</param>
        /// <returns></returns>
        public async Task<CouchResponse> InsertAsync<T>(T obj, bool batchMode = false)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return await this.InsertAsync(JToken.Parse(json), batchMode);
        }
    }
}
