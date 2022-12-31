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
    public class FilterCards
    {
        /// <summary>
        /// 
        /// </summary>
        public List<dynamic> BannedCards { get; set; } = new List<dynamic>();

        /// <summary>
        /// 
        /// </summary>
        public int MinimumAttack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaximumAttack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MinimumDefense { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaximumDefense { get; set; }
    }
}
