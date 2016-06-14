INTRODUCTION
------------

This is a Fling! game application developed in C# 5.

INSTRUCTIONS
------------
1. Open Fling.exe
2. Start entering the IDs for balls you want to place on a 8x7 grid. 

On entering values for 7 places, the cursor will move to next row.

Please note: 
Dot (.) is considered as a square in the grid that has no ball in it. 
Whereas, alphabets (a-z and A-Z) are considered as balls. 

Necessary assumptions: 
---------------------

It is assumed that the ID each ball is unique.

Output:
-------
Output will either be a list of possible solutions for the problem provided, or a message saying the puzzle has no solution.

Test Cases covered:
------------------

1. No ball has row or column common to any other ball.
2. No solution to the puzzle even if few balls have row/column common among themselves.
3. Ball being flinged is targeting a ball that has adjacent neighbour to it.
4. Only one ball in the puzzle.
5. On flinging a ball that has neighbours in its row/column, the neighbours are notified that the ball has been flinged and they need to remove it from their hit list.
6. On hitting a ball, all its neighbours having current ball in their hit list, are notified that the ball  no longer exists on the board.
7. If a ball to be hit has neighbour after it, the neighbour will get hit and will get removed from the board.

Test cases not covered:
-----------------------
1. If a ball having neighbours (not adjacent) on both the sides is flinged up or down, then the neighbour's are not notified that they can now hit each other.

eg: 
...A...
.......
.B.C.D.
.......
.......
.......
.......
.......

If C is flinged to hit A, B and D are not notified that they can now fling and hit eachother.



