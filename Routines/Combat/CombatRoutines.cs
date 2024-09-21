using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raylib_flecs_csharp.Routines.Combat
{
    public class CombatRoutines : AbstractRoutineCollection
    {
        public CombatRoutines(World world) : base(world) {}

        protected override void InitRoutines()
        {
        }
    }
}
