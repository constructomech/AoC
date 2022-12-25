using System.Collections.Immutable;
using System.Diagnostics;

// Base 5, but the digits are 2, 1, 0, minus (written -), and double-minus (written =). 
//  Minus is worth -1, and double-minus is worth -2."


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");
var sum = 0L;
foreach (var snafuNum in input) {
    var value = SnafuToDec(snafuNum);
    sum += value;
}
string result = DecToSnafu(sum);

// Implement here


watch.Stop();
Console.WriteLine($"Result: {result}, Completed in {watch.ElapsedMilliseconds}ms");


long SnafuToDec(string snafuNum)
{
    long accum = 0;

    for (int i = snafuNum.Length - 1, place = 0; i >= 0; i--, place++) 
    {
        long placeValue = (long)Math.Pow(5, place);

        switch(snafuNum[i]) {
            case '2':
                accum += placeValue * 2;
                break;
            case '1':
                accum += placeValue;
                break;
            case '0':
                break;
            case '-':
                accum -= placeValue;
                break;
            case '=':
                accum -= placeValue * 2;
                break;
        }
    }
    return accum;
}

string DecToSnafu(long value)
{
    string result = "";
    int maxPlace = 0;
    long placeValue = 0;
    while (placeValue * 2 < value)
    {
        placeValue = (long)Math.Pow(5, maxPlace);
        maxPlace++;
    }

    long accum = 0L;
    for (var place = maxPlace; place >= 0; place--)
    {
        placeValue = (long)Math.Pow(5, place);

        // Calculate range correction by future places
        var delta = 0L;
        for (var testPlace = place -1; testPlace >= 0; testPlace--) 
        {
            var testPlaceValue = (long)Math.Pow(5, testPlace);
            delta += testPlaceValue * 2;
        }

        for (var i = 2; i >= -2; i--) 
        {
            var digitValue = placeValue * i;

            if (Math.Abs(accum + digitValue - value) <= delta) 
            {
                accum += digitValue;
                switch(i)
                {
                    case -2: result += '='; break;
                    case -1: result += '-'; break;
                    case 0:  if (result.Length != 0) result += '0'; break;
                    case 1:  result += '1'; break;
                    case 2:  result += '2'; break;
                }
                break;
            }
        }                
    }

    return result;
}