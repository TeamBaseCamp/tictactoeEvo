using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoeEvo {
class Program {
    static void Main(string[] args) {
        Nature mama = new Nature();
        mama.newPopulation(19, 50);
        mama.Fitness(1);

        var boss = mama.getApex();
        string input = "";
        mama.engine.recreate();

        int reaction;
        while (input != "end") {
            input = Console.ReadLine();

            if (input == "r") {
                mama.engine.recreate();
                input = Console.ReadLine();
            }

            //parse own input
            mama.engine.set(Int32.Parse(input));
            printSpace(mama.engine.space);
            Console.WriteLine("");

            reaction = boss.react(mama.engine.space);
            if (reaction == -1)
                Console.WriteLine("dont know");
            else
                mama.engine.set(reaction);

            printSpace(mama.engine.space);
        }

        Console.Read();
    }

    static void printSpace(int space) {
        for (int i = 8; i >= 0; --i) {
            var x = (space & (3 << (i * 2))) >> (i * 2);
            switch (x) {
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
