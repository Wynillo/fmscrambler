using FMLib.Utility;

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
