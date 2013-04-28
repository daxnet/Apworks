using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Events
{
    public sealed class FuncDelegatedDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private readonly Func<TDomainEvent, bool> func;

        public FuncDelegatedDomainEventHandler(Func<TDomainEvent, bool> func)
        {
            this.func = func;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            FuncDelegatedDomainEventHandler<TDomainEvent> other = obj as FuncDelegatedDomainEventHandler<TDomainEvent>;
            if (other == null)
                return false;
            return Delegate.Equals(this.func, other.func);
        }

        public override int GetHashCode()
        {
            return Utils.GetHashCode(this.func.GetHashCode());
        }

        #region IHandler<TDomainEvent> Members

        public bool Handle(TDomainEvent message)
        {
            return func(message);
        }

        #endregion
    }
}
