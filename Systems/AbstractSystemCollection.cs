using Flecs.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raylib_flecs_csharp.Systems
{
    public abstract class AbstractSystemCollection
    {
        protected World world;
        protected AbstractSystemCollection(World world)
        {
            this.world = world;
            InitSystemPipeline();
            InitCustomIntervals();
            InitSystems();
        }

        protected virtual void InitSystemPipeline() { }
        protected virtual void InitCustomIntervals() { }

        protected abstract void InitSystems();
    }
}
