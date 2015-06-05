using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoeEvo {
public class Nature {
    Indivdual apex;
    int _geneCapacity = -1;

    public Physics engine;
    public Indivdual[] _population;

    public Nature() {
        // _population = new List<Indivdual>();
        // _population = new Indivdual[2];
        engine = new Physics();
    }

    public class Physics {
        public Random rnd = new Random();
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

    // TODO
    // public List<Indivdual> _population;
    public void newPopulation(int count, int geneCapacity) {
        _geneCapacity = geneCapacity;
        _population = new Indivdual[count];
        for (int i = 0; i < _population.Length; i++) {
            _population[i] = new Indivdual(geneCapacity, count);
        }
    }

    public void Fitness(int generations, int anzBattle = 5) {
        int firstBattle = 0;
        int secondBattle = 0;
        int counter = 0;
        Indivdual beast = new Indivdual(_geneCapacity, _population.Length);

        while (beast.genome.utilization < _geneCapacity - 10) {
            
            Console.Clear();
            Console.WriteLine("Beast GenmoeUtilization: " + beast.genome.utilization);
            Console.WriteLine("Beast Level: " + beast.getLevel());
            Console.WriteLine();
            Console.WriteLine("     ---------------------------");
            Console.WriteLine();
            foreach (Indivdual opfer in _population) {
                Console.WriteLine("opfer GenmoeUtilization: " + opfer.genome.utilization);
                Console.WriteLine("opfer Level: " + opfer.getLevel());
                Console.WriteLine();
            }
            Console.WriteLine("rounds: " + counter++);

            foreach (Indivdual hero in _population) {
                foreach (Indivdual champion in _population) {
                    if (hero.getLevel() != champion.getLevel())
                        continue;
                    int winsChamp = 0;
                    // Console.WriteLine("fight!");
                    for (int battle = 0; battle < anzBattle; battle++ ) {
                        firstBattle = evaluateWithGeneExtension(champion, hero);
                        secondBattle = evaluateWithGeneExtension(hero, champion);
                        if (firstBattle == 1)
                            winsChamp++;
                        if (secondBattle == 2)
                            winsChamp++;

                    }
                    // Console.WriteLine("winsChamp: " + winsChamp);
                }
            }

            foreach (Indivdual bürger in _population)  // utf8 supp? wow
                if (bürger.getLevel() > beast.getLevel())
                    beast = bürger;
        }
        apex = beast;
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

            if (result == 1) { // (result == 0x1 || result == 0x3) {
                first.win();
                if (second.getLevel() >= first.getLevel())
                    second.loose();
                return result;
            }

            reaction = second.react(engine.space);
            if (reaction == -1)
                reaction = second.addRandomGene(engine);
            result = engine.set(reaction);

            if (result == 2) { // (result == 0x2 || result == 0x3) { // <- I dont get that one
                if (second.getLevel() <= first.getLevel())
                    first.loose();
                second.win();
                return result;
            }
        }

        //TODO
        return -1;
    }

    public class Indivdual {
        //TODO rausnehmen
        public Genome genome;
        int _level;
        int _capacity;
        int _popsize;

        public Indivdual(int geneCapacity, int popsize) {
            genome = new Genome(geneCapacity);
            _popsize = popsize;
            _level = 3;
            _capacity = geneCapacity;
        }

        public int getLevel() {
            return _level;
        }

        public void win() {
            if (_level < Math.Log(_popsize))
                _level++;
        }

        public void loose() {
            if (_level == 2)
                genome = new Genome(_capacity);
                _level = 3;
            if (! (--_level > 3))
                _level = 3;
        }

        public int addRandomGene(Physics p) {
            
            int pos = p.rnd.Next(9 - p.state);

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

    /* Getter and setter */
    public Indivdual getApex() {
        return apex;
    }
}
}
