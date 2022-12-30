using FMLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLib.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class StarterDeck
    {
        /// <summary>
        /// 
        /// </summary>
        public int Dropped { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int[] Cards { get; set; } = new int[Static.MaxCards];
    }
}
