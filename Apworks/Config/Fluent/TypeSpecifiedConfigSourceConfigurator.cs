using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public abstract class TypeSpecifiedConfigSourceConfigurator : ConfigSourceConfigurator
    {
        private readonly Type type;
        public TypeSpecifiedConfigSourceConfigurator(IConfigSourceConfigurator context, Type type)
            : base(context)
        {
            this.type = type;
        }

        protected Type Type
        {
            get { return type; }
        }
    }
}
