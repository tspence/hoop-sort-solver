# Hoop game solver

This program demonstrates the use of [A*](https://en.wikipedia.org/wiki/A*_search_algorithm) to solve a popular genre of hoop-sorting games.
These hoop-stacking games are somewhat similar to the [Tower of Hanoi](https://en.wikipedia.org/wiki/Tower_of_Hanoi) logic puzzle that is
often used to demonstrate recursion as a programming technique.  In these games, you must rearrange a selection of colored hoops on poles
to organize them into single-colored stacks, in a form reminiscent of the [solitaire game FreeCell](https://en.wikipedia.org/wiki/FreeCell).

Although many variants of this game exist, the version I played was [Hoop Sort - Color Ring Puzzle by Longwind Studio](https://apps.apple.com/in/app/hoop-sort-color-ring-puzzle/id1615927837). 
The rules of the game are:
* All poles are stacked with four colored hoops.
* Two extra empty poles are available for use.
* You can move colored hoops onto an empty pole or onto their own color.
* Whenever you move hoops, all hoops of the same color on top of the pole move with it.

![Screenshot of the hoop sort game from my iPad](hoop-sort-screenshot.png | width=600)

The level data for this game is most likely randomly generated.  I suspect that the logic of the hoop swapping puzzles is similar to the
[Futurama Theorem](https://en.wiktionary.org/wiki/Futurama_theorem), which is a proof that any number of swaps can be undone by introducing
two new people who have not participated in the swap.  If this suspicion is correct, only two empty poles are ever required to make any
random data set solvable.

# Algorithms used

I chose to use a very basic A* algorithm to compute the moves required to solve the game.  There are lots of available moves, but fortunately
there are a couple of shortcuts we can use to simplify things:
* We only consider one empty pole at a time, even though two may exist.
* The A* distance function counts the number of 'trapped' colored hoops that are beneath other colors.
* To seek out a somewhat optimized solution, I added in the 'number of moves taken' at one fifth of the distance of the trapped hoops.

# How to solve a hoop game

The hoop logic puzzle is not especially complex, but it can sometimes require non-obvious moves, which can make it hard for a human player
to find the solution.  Here's how to use this program to solve a hoop puzzle.

First, you'll need to create a game file.  One sample file is included as [game-213.txt](game-213.txt).  The file should have one line for
each pole, using comma separated text for each color.  You can use `#` symbols for comments, and the text `empty` by itself designates
the number of empty poles:

```csv
# Each pole on a separate line.
# Each line is left to right bottom to top.
# lines that are empty just say empty; comments start with #
red, light green, brown, purple
grey, light blue, pink, orange
purple, teal, pink, pink
green, red, blue, teal
red, grey, orange, brown
light green, light blue, yellow, orange
grey, light blue, grey, light green
yellow, light green, yellow, blue
green, light blue, purple, purple
brown, teal, teal, red
brown, pink, green, blue
blue, orange, yellow, green
empty
empty
```

Once you have a text file for your level, run the program using `HoopSortSolver.exe --gamefile=path-to-file`.
