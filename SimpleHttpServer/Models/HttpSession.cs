namespace SimpleHttpServer.Models
{
    using System.Collections.Generic;

    public class HttpSession
    {
        private IDictionary<string, string> parameters;

        public HttpSession(string id)
        {
            this.Id = id;
            this.parameters = new Dictionary<string, string>();
        }

        public string Id { get; private set; }

        public string this[string key]
        {
            get { return this.parameters[key]; }
        }

        public void Clear()
        {
            this.parameters = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            if (!this.parameters.ContainsKey(key))
            {
                this.parameters.Add(key, value);
            }

            this.parameters[key] = value;
        }
    }
}
