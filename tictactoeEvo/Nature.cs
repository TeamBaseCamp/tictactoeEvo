using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoeEvo {
public class Nature {
    public Physics engine;

    public class Physics {
        public int space;
        public int state;

        public Physics() {
            recreate();
        }

        public void recreate() {
            space = 0;
            state = 0;
        }

        //11 draw, 00 runing
        //01 P1, 10 P2
        //pos position 0..8
        public int set(int pos) {
            space |= (((state % 2) + 1) << (2 * pos));
            ++state;
            return checkTermination();
        }

        public bool isFreeSpace(int pos) {
            if ((space & (3 << (2 * pos))) == 0) {
                return true;
            }
            return false;
        }



        public int checkTermination() {
            if (state == 9)
                return 3; //draw
            #region player1
            //horizontal
            //                       010101 0^6
            if ((space & 0x3f000) == 0x15000)
                return 1;
            //                      0^3 010101 0^3
            if ((space & 0xFC0) == 0x540)
                return 1;

            //                      0^6 010101
            if ((space & 0x3F) == 0x15)
                return 1;

            //vertical
            //                       010101 0^6
            if ((space & 0x30C30) == 0x10410)
                return 1;
            //                      0^3 010101 0^3
            if ((space & 0xC30C) == 0x4104)
                return 1;

            //                      0^6 010101
            if ((space & 0x30C3) == 0x1041)
                return 1;

            //diagonal
            if ((space & 0x30303) == 0x10101)
                return 1;
            //
            if ((space & 0x3330) == 0x1110)
                return 1;
            #endregion
            #region player 2
            //horizontal
            //                       010101 0^6
            if ((space & 0x3f000) == 0x15000 << 1)
                return 2;
            //                      0^3 010101 0^3
            if ((space & 0xFC0) == 0x540 << 1)
                return 2;

            //                      0^6 010101
            if ((space & 0x3F) == 0x15 << 1)
                return 2;

            //vertical
            //                       010101 0^6
            if ((space & 0x30C30) == 0x10410 << 1)
                return 2;
            //                      0^3 010101 0^3
            if ((space & 0xC30C) == 0x4104 << 1)
                return 2;

            //                      0^6 010101
            if ((space & 0x30C3) == 0x1041 << 1)
                return 2;

            //diagonal
            if ((space & 0x30303) == 0x10101 << 1)
                return 2;
            //
            if ((space & 0x3330) == 0x1110 << 1)
                return 2;
            #endregion

            return 0; //running
        }
    }

    //TODO
    //public List<Indivdual> Population;
    public Indivdual[] Population;

    public Nature() {
        //Population = new List<Indivdual>();
        //Population = new Indivdual[2];

        engine = new Physics();
    }

    public void newPopulation(int count) {
        Population = new Indivdual[count];
    }


    public void Fitness(int generations, int geneCapacity) {
        Indivdual champion = new Indivdual(geneCapacity);
        Indivdual hero = new Indivdual(geneCapacity);

        int firstBattle = 0;
        int secondBattle = 0;

        int i = 0;

        int utilMax = 0;

        while (champion.genome.utilization < geneCapacity - 10) {
            if (utilMax < champion.genome.utilization) {
                utilMax = champion.genome.utilization;
                Console.WriteLine(utilMax);
            }

            ++i;
            firstBattle = evaluateWithGeneExtension(champion, hero);
            secondBattle = evaluateWithGeneExtension(hero, champion);

            //apex 2 lose
            if ((firstBattle == 2) || (secondBattle == 1))
                champion = hero;

            hero = new Indivdual(geneCapacity);
        }

        apex = champion;
    }
    Indivdual apex;

    public Indivdual getApex() {
        return apex;
    }


    public int evaluateWithGeneExtension(Indivdual first, Indivdual second) {
        engine.recreate();

        int result = 0;

        int reaction = 0;

        for (int i = 0; i < 5; ++i) {
            reaction = first.react(engine.space);
            if (reaction == -1)
                reaction = first.addRandomGene(engine);
            result = engine.set(reaction);

            if (result == 0x1 || result == 0x3)
                return result;

            reaction = second.react(engine.space);
            if (reaction == -1)
                reaction = second.addRandomGene(engine);
            result = engine.set(reaction);

            if (result == 0x2 || result == 0x3)
                return result;
        }

        //TODO
        return -1;
    }

    public class Indivdual {
        //TODO rausnehmen
        public Genome genome;

        public Indivdual(int geneCapacity) {
            genome = new Genome(geneCapacity);
        }

        public int addRandomGene(Physics p) {
            Random rnd = new Random();
            int pos = rnd.Next(9 - p.state);

            int j = 0;
            for (int i = 0; i < 9; ++i) {
                if (p.isFreeSpace(i)) {
                    if (j == pos) {
                        addGene(p.space, (Byte)i);
                        return i;
                    } else
                        ++j;
                }
            }
            return -1;
        }


        public void addGene(int code, Byte answer) {
            if (genome.utilization < genome.capacity) {
                //TODO answer 4 bit?
                if ((code << 8) + answer < 0) {
                    var todo = 0;
                }

                genome.genePool[genome.utilization] = (code << 8) + answer;
                ++genome.utilization;
            } else {
                //TODO
                var x = genome.utilization;
            }
        }

        public int react(int space) {
            int result = -1;
            //TODO perf

            for (int i = 0; i < genome.utilization; ++i) {
                //TODO 8 bit
                if ((genome.genePool[i] & 0x1FFFFF00) == (space << 8)) {
                    return genome.genePool[i] & 0xFF;
                }
            }

            return result;
        }

        public class Genome {
            //Gene 22 bit
            public Genome(int _capacity) {
                capacity = _capacity;
                genePool = new int[capacity];
                utilization = 0;
            }

            public int[] genePool;
            public int capacity;
            public int utilization;
        }
    }


}
}
