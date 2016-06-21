using Rabbit.Rpc.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Rpc.Server.Implementation.ServiceDiscovery.Implementation
{
    /// <summary>
    /// Clr������Ŀ������
    /// </summary>
    public class ClrServiceEntryFactory : IClrServiceEntryFactory
    {
        #region Field

        private readonly IServiceInstanceFactory _serviceFactory;
        private readonly IServiceIdGenerator _serviceIdGenerator;

        #endregion Field

        #region Constructor

        public ClrServiceEntryFactory(IServiceInstanceFactory serviceFactory, IServiceIdGenerator serviceIdGenerator)
        {
            _serviceFactory = serviceFactory;
            _serviceIdGenerator = serviceIdGenerator;
        }

        #endregion Constructor

        #region Implementation of IClrServiceEntryFactory

        /// <summary>
        /// ����������Ŀ��
        /// </summary>
        /// <param name="service">�������͡�</param>
        /// <param name="serviceImplementation">����ʵ�����͡�</param>
        /// <returns>������Ŀ���ϡ�</returns>
        public IEnumerable<ServiceEntry> CreateServiceEntry(Type service, Type serviceImplementation)
        {
            foreach (var methodInfo in service.GetMethods())
            {
                var implementationMethodInfo = serviceImplementation.GetMethod(methodInfo.Name,
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                yield return Create(_serviceIdGenerator.GenerateServiceId(methodInfo), implementationMethodInfo);
            }
        }

        #endregion Implementation of IClrServiceEntryFactory

        #region Private Method

        private ServiceEntry Create(string serviceId, MethodInfo method)
        {
            var type = method.DeclaringType;

            return new ServiceEntry
            {
                Descriptor = new ServiceDescriptor
                {
                    Id = serviceId
                },
                Func = parameters =>
                {
                    var instance = _serviceFactory.Create(type);

                    var list = new List<object>();
                    foreach (var parameterInfo in method.GetParameters())
                    {
                        var value = parameters[parameterInfo.Name];
                        list.Add(Convert.ChangeType(value, parameterInfo.ParameterType));
                    }

                    return method.Invoke(instance, list.ToArray());
                }
            };
        }

        #endregion Private Method
    }
}