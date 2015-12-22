using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImDiskManager
{
    class Objects
    {
        public class ProgramSettings
        {
            public string DriveLetter;
            public string ImagePath;
            public Boolean ExitFlag = false;
            public Boolean ReadyToMap = false;
            public Int16 Timeout = 10; 
        }
    }
}
