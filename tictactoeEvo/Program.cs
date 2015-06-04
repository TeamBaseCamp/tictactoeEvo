using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoeEvo
{
    class Program
    {
        static void Main(string[] args)
        {
            Nature ttt = new Nature();
            ttt.newPopulation(2);


            ttt.Fitness(1, 50);

            var x = ttt.getApex();

            string input = "";

            ttt.engine.recreate();

            int reaction;
            while (input != "end")
            {
                input = Console.ReadLine();

                if (input == "r")
                {
                    ttt.engine.recreate();
                    input = Console.ReadLine();
                }

                //parse own input 
                ttt.engine.set(Int32.Parse(input));

                printSpace(ttt.engine.space);

                Console.WriteLine("");

                reaction = x.react(ttt.engine.space);
                if (reaction == -1)
                    Console.WriteLine("dont know");
                else
                    ttt.engine.set(reaction);

                printSpace(ttt.engine.space);
            }

            Console.Read();
        }

        static void printSpace(int space)
        {
            for (int i = 8; i >= 0; --i)
            {
                var x = (space & (3 << (i * 2))) >> (i * 2);
                switch (x)
                {
                    case 0:
                        Console.Write("-");
                        break;
                    case 1:
                        Console.Write("x");
                        break;
                    case 2:
                        Console.Write("o");
                        break;
                    default:
                        Console.Write(x);
                        break;
                }
                if (i == 6 || i == 3 || i == 0)
                    Console.WriteLine();
            }
        }
    }
}
