using System;

namespace Ryujinx.HLE.HOS.Services
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class ServiceAttribute : Attribute
    {
        public readonly string Name;
        public readonly Func<IpcService> ObjectFactory;

        public ServiceAttribute(string name, object objFactory = null)
        {
            Name          = name;
            ObjectFactory = objFactory as Func<IpcService>;
        }
    }
}