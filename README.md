# Tic-Tac-Toe
It is Player vs AI Tic tac toe built on minimax algorithm.It starts with a spinner screen where first turn is decided wherever spinner lands. This leads to game screen where on playing 3 games winner will be decided. With every win, score of 1 gets added in the Winner's bag and if it's a tie then both the players are awarded with 0.5 each.
You can find the video here 
<div align="left">
      <a href="https://www.youtube.com/watch?v=YG30ZMTfKCo">
         <img src="https://github.com/khushboogupta17/Tic-Tac-Toe/blob/main/Assets/SS_1.png" style="width:100%;">
      </a>
</div>
<p>
<img src="https://github.com/khushboogupta17/Tic-Tac-Toe/blob/main/Assets/SS_2.png" width="20%" height="20%">
<img src="https://github.com/khushboogupta17/Tic-Tac-Toe/blob/main/Assets/SS_3.png" width="20%" height="20%">
</p>

# Technologies Used
Unity 2018.4, Visual Studio

# My take on Tic Tac Toe
Player and CPU are instances of scripatble objects PlayerData which stores their data. It further gets populated on the screen by UIVIew which handles all the commands for to UI. Game starts with GameManager which calls ChanceDecider to populate all players as cards in spinner, on clicking spin button it randomly spins for few seconds and whichever is on the front gets to play first. State of game changes to playing and if the first player is computer, GameManager playTurn function plays the turn after calculating the optimal move from minimax algorith with alpha beta pruning else it waits for player to take turn. After each move winning conditions are checked by changing state to conditionChecking, once a win is detected we change the state of game to gameEnded and if we still have valid games left then we reset the board, change state to playing and let the winner take the first chance. On every win, winner gets 1 score added and on each tie 0.5 gets added to all the players. After valid games have been ended here 3, then we check whose score is more and declare them as winner. Here state of changes to GameEnded and if player decides to restart it will again start with ChanceDecider. 

Main logic resides in Gamemanager.cs which controls various states and swapping between them.Player and CPU are taken as scriptable objects so that if we want to have their data in other scenes like shop we can easily access it. It is implementing very basic version of FSM where we have different states of game PlayerPlaying,CheckingCondition and GameEnded and switch between them to smotthly transition through game.
