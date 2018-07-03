using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Common
{
    public class UnityContainerHelp
    {
        private readonly IUnityContainer _container;
        public UnityContainerHelp()
        {
            _container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            _container.LoadConfiguration(section, "unity");
        }

        public T GetServer<T>()
        {
            return _container.Resolve<T>();
        }

        public T GetServer<T>(string name)
        {
            return _container.Resolve<T>(name);
        }
    }
}
