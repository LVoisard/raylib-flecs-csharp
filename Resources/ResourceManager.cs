using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raylib_flecs_csharp.Resources
{
    public class ResourceManager
    {
        private static ResourceManager instance = null;

        public static ResourceManager Instance      
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourceManager();
                }

                return instance;
            }
        }
    }
}
