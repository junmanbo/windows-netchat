using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Net.Sockets;

namespace NetChat
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmSvr());
        }
    }
    public static class ControlExtensions
    {
        public static T Clone<T>(this T ControlToClone)
            where T : Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            T instance = Activator.CreateInstance<T>();
            foreach(PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(ControlToClone, null), null);
                }
            }
            return instance;
        }
    }
    public class StateObject
    {
        // Size of receive buffer
        public const int BufferSize = 1024;

        // Receive buffer
        public byte[] buffer = new byte[BufferSize];

        //Client socket
        public Socket workSocket = null;
    }
}
