using RabbitCloud.Abstractions.Utilities;
using System.Reflection;

namespace RabbitCloud.Rpc.Abstractions
{
    /// <summary>
    /// ������������
    /// </summary>
    public struct MethodDescriptor
    {
        public MethodDescriptor(MethodInfo method)
        {
            InterfaceName = method.DeclaringType.Name;
            MethodName = method.Name;
            ParamtersSignature = ReflectUtil.GetMethodParamDesc(method);
        }

        /// <summary>
        /// �������ơ�
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// ��������ǩ����
        /// </summary>
        public string ParamtersSignature { get; set; }
    }
}