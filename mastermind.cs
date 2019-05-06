using System;

/*
Create a C# console application that is a simple version of Mastermind.  
The randomly generated answer should be four (4) digits in length, with each digit between the numbers 1 and 6.  
After the player enters a combination, a minus (-) sign should be printed for every digit that is correct 
but in the wrong position, and a plus (+) sign should be printed for every digit that is both correct 
and in the correct position.  Nothing should be printed for incorrect digits.  
The player has ten (10) attempts to guess the number correctly before receiving a message that they have lost.


NOTE: did not specify what if anything to be done if guess is totally correct
I choose to print "win" message 

We do the best we can, even if the input is incomplete or outside of expected range.  
Whatever subset of input conforms to range between 1 and 6 inclusive is checked against the 'answer'.  
Excess input is ignored although there can be optional feed backs such as too long, 
too short, improper input


To consider ... on a loss, wouldn't it make sense to print out what the target or answer should have been?
This will be done if the optional printFeedback is set to true



*/

class Program
{
    
    const int num_elements = 4; // based on problem statement
    // indices will be zero to above num_elements less 1
    
    const int max_value = 6;    // based on problem statement
    // values will be 1 to the above max_value
    
    const int max_attempts = 10;  // based on problem statement
    
    static Random _random = new Random();  // to get random numbers

    static int[] targets = new int[num_elements]; // the answer
    
    static int[] guesses = new int[num_elements]; // the players combination

    static int[] count_avail_each_input = new int[1 + max_value];

    // following three are for optional feedback
    static bool isTooShort; // the input was too short but we evaulate it anway
    static bool isTooLong;  // the input was too long, we ignore extra
    static bool isImproper; // the input had improper values or characters
    static bool printFeedback = true; // turn on the above optional feedback messages


    static bool isWon;     // if the game has been won
    static bool isLost;    // if the game has been lost
    
    static bool printDebug = false; // turn on debug printing


    static void Main()
    {
        // a loop could be placed here if desired ....
            PlayGame();
    }
    
    // Main game 
    static void PlayGame()
    {
        
        int idx = 0;
        int attempts = 0;
        int guess_idx = 0;
        int int_from_char = 0;
        
        int result = 0;
        
        string my_guess_input;
        

        isWon = false;
        isLost = false;

        
        // Here we initialize the targets (answer) with our random values
        for (idx = 0; idx< num_elements; idx++) {
            targets[idx] = _random.Next(1, (1 + max_value));
        }
        
        /// Debug output what target is
        
        if (printDebug) {
            Console.Write(" target is ");
            for (idx = 0; idx< num_elements; idx++) {
                Console.Write(targets[idx]);
            }
            Console.WriteLine(); 
        }
        
        for (attempts=0; attempts < max_attempts; attempts++) {
            
            isTooShort = false;
            isTooLong = false;
            isImproper = false;
            for (guess_idx = 0; guess_idx < num_elements; guess_idx++) { guesses[guess_idx] = 0; }
            guess_idx = 0; // re-init this
            
            
            // get user input
            my_guess_input = Console.ReadLine();
            
            if (printDebug) {
                Console.Write(" input "+my_guess_input+"  "); // debug what raw input was
            }
            
            // sanitize user input (strip white space, remove out of range input)
            // keep track if we get too much input and check if we got too little
            // walk down all the characters that we got 
            for (int i = 0; ((i < my_guess_input.Length) && (false == isTooLong)); i++) 
            {
                char ch = my_guess_input[i];
                if (Char.IsWhiteSpace(ch)) {
                    // ignore
                }
                else 
                if (Char.IsDigit(ch)) { // digits are good but zero or too large a value are not good
                    int_from_char = (int) Char.GetNumericValue(ch);
                    if (int_from_char < 1) {isImproper = true; int_from_char = 0; } // flag zero
                    if (int_from_char > max_value) {isImproper = true; int_from_char = 0; } // flag too large
                    
                    
                    if (guess_idx >= num_elements) { // but if we have too many, must quit
                            isTooLong = true; /// to exit for loop
                    } else {
                        guesses[guess_idx] = int_from_char; // place sanitized value
                    }
                    
                    // any improper input (including zero or too large a value) would leave following as zero
                    // So a non-zero means we must have had valid input, so only then do we step to next element
                    if (int_from_char != 0) {
                        guess_idx++; // step to next element in guess array
                    }
                    
                }
                else // if not whitespace and not a digit then must be something improper so note that
                {
                    isImproper = true;
                }
            }
            if (guess_idx < num_elements) isTooShort = true; // check for adequate input and flag if not
            
            
            /// debugging output
            if (printDebug) {
                Console.Write(" eval "+guesses[0]+guesses[1]+guesses[2]+guesses[3]+" ");
            }

            // call evaluation function with above players combination  and answer targets
            result = Evaluate( targets, guesses,  num_elements); // returns the missed guess count
            if (0 == result) { // if missed guess count is zero, they must have won
                if (printDebug) Console.Write(" zero result from evaluate ");
                isWon = true; // memorialze the victory
            }
            
            // optional feedback messages
            if (printFeedback) {
                if (isTooLong)  Console.Write(" input too long ");
                if (isTooShort) Console.Write(" input too short ");
                if (isImproper) Console.Write(" improper input ");
            }
            
            // if eval returns zero print win and return
            
            if (isWon) {
                Console.Write(" win "); /// optional win message
                attempts = max_attempts; // To quit game attempts loop
            }
            else 
            {
                if (printDebug) Console.Write(" attempt "+attempts); // debug printing
            }
            
            Console.WriteLine("");
            
        }
        
        if (false == isWon) { // if they have not won by now, they must have lost
            isLost = true; // in case anyone else cares memorialize the loss
        }
        // print "lose" and return
        if (isLost) {
            Console.WriteLine(" lose"); // per requirements although actual message text was not completely specified
            
            if (printFeedback) {
                Console.Write(" answer was ");
                for (idx = 0; idx< num_elements; idx++) {
                    Console.Write(targets[idx]);
                }
                Console.WriteLine(); // newline
            }

        }

    }
    
    /* the evaluation function */
    static int Evaluate( int[] target_arr, int[] guess_arr, int howbig)
    {
        int idx = 0;
        int missed_guess_count = 0;
        
        for (idx = 1; idx < max_value + 1; idx++)
        {
            count_avail_each_input[idx] = 0;
        }
        
/*  Logic here as follows

Zero out an array of count available of each input (1-6 in this case)
zero out overall missed guess count

First pass eval, for each location first check guess versus target.
If there is a match, emit a +.   If there is no match increment 
the element of the count available array by target value at that location
and increment an overall count of missed guesses

Second pass eval, for each location if there is no match, then provided that
the guess itself was non-zero, check if there is a non-zero count available 
corresponding to that guess.  If so, decrement that count available
and emit a -.

Return overall missed guess count to caller  (if zero they may want to print success)
*/
        for (idx = 0; idx < howbig; idx++) {
            if ((target_arr[idx] == guess_arr[idx])) { // match value in location
                Console.Write("+"); // right value in right location so output a '+' per problem statement
            } else {
        // if guess and answer do not match, then this digit of the answer is available to be matched with
        // that value in a different location of the guess.  Incrementing the counter for this digit for next pass.
                count_avail_each_input[target_arr[idx]]++;
                missed_guess_count++; // anytime answer and guess don't match we increment missed_guess_count
            }
        } 
        // since exact matches already taken care of 
        for (idx = 0; idx < howbig; idx++) {
            if ((guess_arr[idx] != 0) && (target_arr[idx] != guess_arr[idx])) { // match value in location
                if (count_avail_each_input[guess_arr[idx]] > 0) {
                    Console.Write("-");
                    count_avail_each_input[guess_arr[idx]]--;
                }
            }
        }
        
        return (missed_guess_count); // will be zero for a win, anything else for try again or eventual loss
    }

}

