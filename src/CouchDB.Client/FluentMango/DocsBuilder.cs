﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace CouchDB.Client.FluentMango
{
    /// <summary>
    /// http://docs.couchdb.org/en/2.2.0/api/database/bulk-api.html#db-bulk-docs
    /// </summary>
    public class DocsBuilder
    {
        private JArray itens = new JArray();

        public static implicit operator string(DocsBuilder builder)
        {
            return builder.ToString();
        }

        public override string ToString()
        {
            JObject obj = new JObject();
            obj.Add("docs", itens);

            return obj.ToString();
        }

        public void Add(JArray jarray)
        {
            this.itens = jarray;
        }

        public void Add(dynamic[] itensArr)
        {
            foreach (var item in itensArr)
            {
                var token = JToken.FromObject(item);
                this.itens.Add(token);
            }
        }

        public void Add(string id, string rev = null, bool? deleted = null)
        {
            JObject obj = new JObject();
            obj.Add("_id", id);

            if (!string.IsNullOrEmpty(rev))
                obj.Add("rev", rev);

            if (deleted.HasValue)
                obj.Add("deleted", deleted.Value.ToString().ToLower());

            itens.Add(obj);
        }
    }
}