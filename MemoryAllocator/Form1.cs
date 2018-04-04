using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryAllocator
{
    public partial class Form1 : Form
    {
        string algorithm;
        List<Hole> holes = new List<Hole>();
        List<Process> process = new List<Process>();
        List<Process> waitingList = new List<Process>();
        int pID = 1;
        int nHoles;

        public Form1()
        {
            InitializeComponent();
        }

        void error( string s) {
            string message = s ;
            string caption = "Error Detected in Input";
            MessageBox.Show(message, caption);
            //algorithm = "none";
            //break;
        }

        void sortByStart(List<Hole> l) {
            for (int i = 0; i < l.Count - 1; i++)
            {
                int minindex = i;
                for (int j = i + 1; j < l.Count; j++)
                {

                    if (l[j].getStartingAddress() < l[minindex].getStartingAddress())
                    {
                        minindex = j;
                    }
                }
                if (minindex != i)
                {
                    Hole temp = l[i];
                    l[i] = l[minindex];
                    l[minindex] = temp;
                }

            }
        }

        void sortBySize(List<Hole> l , string t) {
            if (t == "min")
            {
                for (int i = 0; i < l.Count - 1; i++)
                {
                    int minindex = i;
                    for (int j = i + 1; j < l.Count; j++)
                    {
                        if (l[j].getHoleSize() < l[minindex].getHoleSize())
                        {
                            minindex = j;
                        }
                    }
                    if (minindex != i)
                    {
                        Hole temp = l[i];
                        l[i] = l[minindex];
                        l[minindex] = temp;
                    }

                }
            }
            else if (t == "max") {
                for (int i = 0; i < l.Count - 1; i++)
                {
                    int maxindex = i;
                    for (int j = i + 1; j < l.Count; j++)
                    {
                        if (l[j].getHoleSize() > l[maxindex].getHoleSize())
                        {
                            maxindex = j;
                        }
                    }
                    if (maxindex != i)
                    {
                        Hole temp = l[i];
                        l[i] = l[maxindex];
                        l[maxindex] = temp;
                    }

                }
            
            }
        }

        void checkAdjacentHoles(List<Hole> l) {
            
            sortByStart(l);
            
            for (int i = 0; i < l.Count()-1 ; i++) {
                if (l[i].getStartingAddress() + l[i].getHoleSize() == l[i + 1].getStartingAddress()) { 
                    //adjacent holes
                    //original hole size
                    int holeSize = l[i + 1].getHoleSize();
                    //add size of adjacent hole to the original one
                    holeSize += l[i].getHoleSize();

                    l[i+1].setHoleSize(holeSize);
                    l[i + 1].setStartingAddress(l[i].getStartingAddress());

                    l[i].setHoleSize(0);
                    //l.RemoveAt(i + 1);
                }
            }
        }

        void allocateProcess(Process p, List<Hole> holes) {
            for (int j = 0; j < holes.Count(); j++)
            {
                if (p.getProcessSize() <= holes[j].getHoleSize())
                {
                    //bool flaggg = true;
                    if (p.getState() == "Waiting") {
                        //flaggg = false;
                        for (int i = 0; i < waitingList.Count; i++) {
                            if (waitingList[i].getProcessID() == p.getProcessID()) {
                                waitingList.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    
                    //update starting address of this process
                    int start = holes[j].getStartingAddress();
                    p.setStartingAddress(start);
                    p.setState("Allocated");

                    //update hole size
                    int holeSize = holes[j].getHoleSize();
                    holeSize -= p.getProcessSize();
                    holes[j].setHoleSize(holeSize);
                    holes[j].setStartingAddress(p.getStartingAddress() + p.getProcessSize());

                    allocateHoles(holes);

                    listBox1.Items.Add("P" + p.getProcessID().ToString() + " : " + p.getStartingAddress() + " -  Size : " + p.getProcessSize()).ToString();
                    
                    process.Add(p);

                    break;
                }
                else
                {
                    p.setState("Waiting");
                }
            }

            bool flag = true;
            
            //If it's already in the waiting list, don't add it again
            for (int j = 0; j < waitingList.Count; j++) {
                if (p.getProcessID() == waitingList[j].getProcessID()) {
                    flag = false;
                }
            }

            if (p.getState() == "Waiting" && flag)
            {
                listBox2.Items.Add("P" + p.getProcessID().ToString() + " -  Size : " + p.getProcessSize());
                waitingList.Add(p);
            }

        }

        void allocateHoles(List<Hole> holes) {

            listBox3.Items.Clear();
            int n = 0;
            for (int i = 0; i < holes.Count; i++)
            {
                sortByStart(holes);
                if (holes[i].getHoleSize() != 0)
                {
                    n++;
                    listBox3.Items.Add("Hole" + n.ToString() + " : " + holes[i].getStartingAddress().ToString() + " -  Size :" + holes[i].getHoleSize().ToString());
                }
            }
        
        }
        
        //ALLOCATE PROCESS
        private void button1_Click(object sender, EventArgs e)
        {
            algorithm = comboBox1.Text;
            
            if (holes.Count == 0) {
                error("Please Allocate Holes First ");
                algorithm = "none";
            }

            if (textBox7.Text == "")
            {
                error("Please Enter Correct Process Size");
                algorithm = "none";
            }

            if (algorithm != "none")
            {
                Process p = new Process();
                p.setProcessID(pID);
                pID++;

                p.setProcessSize(Int32.Parse(textBox7.Text));

                if (algorithm == "First Fit")
                {
                    sortByStart(holes);
                    allocateProcess(p, holes);
                }
                else if (algorithm == "Best Fit")
                {
                    sortBySize(holes, "min");
                    allocateProcess(p, holes);
                }
                else if (algorithm == "Worst Fit")
                {
                    sortBySize(holes, "max");
                    allocateProcess(p, holes);
                }
            }
        }
        
        //DEALLOCATION
        private void button3_Click(object sender, EventArgs e)
        {
            int pDeallocateID = Int32.Parse(textBox4.Text);
            bool flag = true;
            //Check if ID is correct
            for (int i = 0; i < process.Count; i++) {
                if (process[i].getProcessID() == pDeallocateID)
                {
                    flag = false;
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();

                    Hole h = new Hole();
                    h.setStartingAddress(process[i].getStartingAddress());
                    h.setHoleSize(process[i].getProcessSize());

                    holes.Add(h);

                    sortByStart(holes);

                    checkAdjacentHoles(holes);

                    int n = 0;
                    for (int j = 0; j < holes.Count; j++)
                    {
                        sortByStart(holes);
                        if (holes[j].getHoleSize() != 0)
                        {
                            n++;
                            listBox3.Items.Add("Hole" + n.ToString() + " : " + holes[j].getStartingAddress().ToString() + " -  Size :" + holes[j].getHoleSize().ToString());
                        }
                    }

                    process.RemoveAt(i);

                    for (int j = 0; j < process.Count; j++)
                    {
                        listBox1.Items.Add("P" + process[j].getProcessID().ToString() + " : " + process[j].getStartingAddress() + " -  Size : " + process[j].getProcessSize()).ToString();
                    }

                    for (int j = 0; j < waitingList.Count; j++) 
                    {
                        allocateProcess(waitingList[j],holes);                
                    }

                    for (int j = 0; j < waitingList.Count; j++)
                    {
                        listBox2.Items.Add("P" + waitingList[j].getProcessID().ToString() + " -  Size : " + waitingList[j].getProcessSize());
                    }
                    break;
                }
            }

            if (flag) {
                error("Process No. was not found!");
            }
        }

        //ALLOCATE HOLES
        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            textBox7.Text = "";
            textBox4.Text = "";
            holes.Clear();
            process.Clear();
            waitingList.Clear();
            pID = 1;

            //int nHoles=0;
            if (textBox1.Text != "" )
            {
                nHoles = Int32.Parse(textBox1.Text);
                bool flag = true;
                for (int i = 1; i < nHoles + 1 ; i++)
                {
                    Hole h = new Hole();
                    Control c = (TextBox)tableLayoutPanel2.GetControlFromPosition(0, i);
                    //if (c.Text != "")
                    //{
                    //    h.setHoleID(Int32.Parse(c.Text));
                    //}
                    //else 
                    //{
                    //    error("Missing Input");
                    //    flag = false;
                    //    break;
                    //}
                    
                    //c = (TextBox)tableLayoutPanel2.GetControlFromPosition(1, i);
                    if (c.Text != "")
                    {
                        h.setStartingAddress(Int32.Parse(c.Text));
                    }
                    else
                    {
                        error("Missing Input");
                        flag = false;
                        break;
                    }
                    c = (TextBox)tableLayoutPanel2.GetControlFromPosition(1, i);
                    
                    if (c.Text != "")
                    {
                        h.setHoleSize(Int32.Parse(c.Text));
                    }
                    else
                    {
                        error("Missing Input");
                        flag = false;
                        break;
                    }
                    holes.Add(h);
                }
                if (flag)
                {
                    checkAdjacentHoles(holes);

                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();

                    allocateHoles(holes);
                }
            }
            else
            {
                error("Please Enter No. of Holes");
            }
        }

        //ADD NEW ROW
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                TextBox c = new TextBox();
                tableLayoutPanel2.Controls.Add(c, i, tableLayoutPanel2.RowCount);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            textBox7.Text = "";
            textBox4.Text = "";
            holes.Clear();
            process.Clear();
            waitingList.Clear();
            pID = 1;
        }
   
    }
}
