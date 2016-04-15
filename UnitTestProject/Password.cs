using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    public class Password
    {
        [TestMethod]
        public void TestMethodMD5()
        {
            
            string temp;
            temp = "1x?SK_2q/p ";

            string passwordMD5;
            passwordMD5 = MyManagerCSharp.SecurityManager.getMD5Hash(temp);

            Debug.WriteLine(String.Format("MyManagerCSharp.SecurityManager.getMD5Hash  {0} : {1} ", temp, passwordMD5));
        }
    }
}
