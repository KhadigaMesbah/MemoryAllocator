using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryAllocator
{
    class Hole
    {
        private int holeID;
        private int startingAddress;
        private int holeSize;

        public Hole() { }
        public void setHoleID(int ID) { this.holeID = ID; }
        public void setStartingAddress(int add) { this.startingAddress = add; }
        public void setHoleSize (int size) {this.holeSize = size;}
        public int getHoleID() { return this.holeID; }
        public int getStartingAddress() { return this.startingAddress;  }
        public int getHoleSize() { return this.holeSize; }

    }
}
