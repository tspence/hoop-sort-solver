using System.Security.Cryptography;
using System.Text;

namespace HoopSortSolver {

    public class PoleState {
        public int PoleNumber { get; set; }
        public List<int> Hoops = new();
        public bool IsEmpty() 
        {
            return Hoops.Count == 0;
        }

        public bool IsFinished(GameState game)
        {
            if (Hoops.Count == 0 || Hoops.Count != game.MaxPoleHeight)
            {
                return false;
            }
            var color = Hoops[0];
            return !(from hoop in Hoops where hoop != color select hoop).Any();
        }

        public int? TopColor() 
        {
            if (Hoops.Count > 0) {
                return Hoops[Hoops.Count - 1];
            }
            return null;
        }

        public string ToHashString()
        {
            if (Hoops.Count == 0) return "Empty";
            return String.Join(" > ", Hoops);
        }
    }
}