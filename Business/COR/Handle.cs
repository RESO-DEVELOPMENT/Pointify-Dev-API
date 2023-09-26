using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Chain
{
    public interface IHandler<T>
    {
        void Handle(T request);
        IHandler<T> SetNext(IHandler<T> next);
    }
    public abstract class Handler<T> : IHandler<T> where T : class
    {
        private IHandler<T> _next { get; set; }

        public virtual void Handle(T request)
        {
            _next?.Handle(request);
        }

        public IHandler<T> SetNext(IHandler<T> next)
        {
            _next = next;
            return _next;
        }
    }
}
