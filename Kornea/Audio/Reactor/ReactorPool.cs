using System.Collections.Generic;
using Kornea.Audio.Interfaces;

namespace Kornea.Audio.Reactor
{
    internal class ReactorPool
    {
        public static IDictionary<string, IReactor> Pool = new Dictionary<string, IReactor>();
    }
}