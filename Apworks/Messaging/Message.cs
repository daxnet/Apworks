using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public class Message
    {

        public Message() { }
        
        private Guid id;

        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        private byte[] body;

        public byte[] Body
        {
            get { return body; }
            set { body = value; }
        }

        private string typeName;

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        private string assemblyQualifiedName;

        public string AssemblyQualifiedName
        {
            get { return assemblyQualifiedName; }
            set { assemblyQualifiedName = value; }
        }

        public static Message FromObject(object obj, IMessageSerializer serializer)
        {
            return new Message
            {
                AssemblyQualifiedName = obj.GetType().AssemblyQualifiedName,
                ID = Guid.NewGuid(),
                TypeName = obj.GetType().Name,
                Body = serializer.Serialize(obj)
            };
        }
    }
}
