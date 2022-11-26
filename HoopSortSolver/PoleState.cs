using System.Security.Cryptography;
using System.Text;

namespace HoopSortSolver {

    public class PoleState {
        public List<string> Hoops = new();
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

        public string? TopColor() 
        {
            if (Hoops.Count > 0) {
                return Hoops[Hoops.Count - 1];
            }
            return null;
        }

        public string ToGameString()
        {
            if (Hoops.Count == 0) return "Empty";
            return String.Join(" > ", Hoops);
        }
    }
}