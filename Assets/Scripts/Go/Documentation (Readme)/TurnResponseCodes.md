# Turn Response Code Events

TurnResponseCodeEvent fires out, and it carries and int with it. It's trivial to parse the int to tell us what error was. 

I didn't implement a string with a description of the error, because its up to the interface to choose what to do with this information. My sample just does nothing, no error codes at all. Probably not the best solution in some cases, like if the move is a suicide.

The numbers are in no order (well, the order is what order the exceptions appear in my code. No hidden meaning behind them, except I made sure 1 was success since 1 can colloquially be parsed as true.

0: Generic Failure, no error code. (shouldn't happen?)

1: Success! Move placed OR Success, turn passed (no stone placed). (Listen to the NewStonePlayedEvent to execute when to play a stone, not to this response).

Rest are failures:
2: Game is over.
3: Incorrect turn - Isn't "your" turn to play.
4: Stone already occupies the spot.
5: This play would be suicide.
6: This play violates Ko rule. (Returns game to previous game state)
7: Invalid Position, there is no point at given location.