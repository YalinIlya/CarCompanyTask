using GetValue.Code;
using GetValue.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace GetValue.Controllers
{
    public class ValuesController : ApiController
    {
        //ValueContext db = new ValueContext();
        ValueRepository repo = new ValueRepository();

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string GetValue(int currentValue)
        {
            CalculateValue CalcVal = new CalculateValue();
            currentValue = CalcVal.Calc(currentValue);

            ExternalApi ExtApi = new ExternalApi();
            currentValue = ExtApi.GetValue(currentValue);

            Value vl = new Value();
            vl.value = currentValue;
            repo.Create(vl);
            repo.Save();
            //db.Values.Add(vl);
            //db.SaveChanges();
            return currentValue.ToString();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
