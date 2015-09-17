using System;

namespace RasterizeCsharp.AppUtils
{
    public class CustomExceptionHandler
    {
        public CustomExceptionHandler(string message, Exception ex)
        {
           Console.WriteLine(message+" ==> "+ex.Message);
        }
    }
}
