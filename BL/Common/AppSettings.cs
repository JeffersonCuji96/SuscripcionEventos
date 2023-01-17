using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Common
{
    public class AppSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int DaysExpireToken { get; set; } = 0;
    }
}
