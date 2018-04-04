using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryAllocator
{
    class Process
    {
        private int processID;
        private int processSize;
        private int startingAddress;
        private string state;

        public Process() { }

        public void setProcessID(int ID) { processID = ID; }
        public void setProcessSize(int size) { processSize = size; }
        public void setStartingAddress(int add) { startingAddress = add; }
        public void setState(string s) { state = s; }

        public int getProcessID() { return processID; }
        public int getProcessSize() { return processSize; }
        public int getStartingAddress() { return startingAddress; }
        public string getState() { return state; }
    }
}
