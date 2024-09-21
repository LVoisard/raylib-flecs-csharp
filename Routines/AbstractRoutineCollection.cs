using Flecs.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raylib_flecs_csharp.Routines
{
    public abstract class AbstractRoutineCollection
    {
        protected World world;
        protected AbstractRoutineCollection(World world)
        {
            this.world = world;
            InitRoutinePipeline();
            InitRoutines();
        }

        protected virtual void InitRoutinePipeline() { }

        protected abstract void InitRoutines();
    }
}
