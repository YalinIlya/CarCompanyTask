using GetValue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetValue.Code
{
    public interface IRepository : IDisposable
    {
        List<Value> GetValueList();
        Value Create(Value item);
        int Save();
    }

    public class ValueRepository : IRepository
    {
        private ValueContext db;
        public ValueRepository()
        {
            this.db = new ValueContext();
        }
        public Value Create(Value item)
        {
            Value v = db.Values.Add(item);
            return v;
        }

        public List<Value> GetValueList()
        {
            throw new NotImplementedException();
        }

        public int Save()
        {
           int res = db.SaveChanges();
            return res;
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}