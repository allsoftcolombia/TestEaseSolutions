using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WFTestEaseSolutions
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region Vars
        List<Node> allNodes = new List<Node>();
         int[][] points;
         int runningValueID = 1;
         int runningLevel = 0;
        #endregion

        #region subs
        int[][] ReadMapFile()
        {
            var filePath = label1.Text; //"map4.txt";
            return File.ReadLines(filePath).Skip(1).Select(x => x.Split(' ').Select(y => int.Parse(y)).ToArray()).ToArray();
        }


         int[] GetTreeNodeValues(Node node)
        {
            var nodePath = new List<int>() { node.NodeValue };
            Node parentNode = null;
            do
            {
                parentNode = allNodes.Where(x => x.Id == node.ParentID).SingleOrDefault();
                if (parentNode != null)
                    nodePath.Add(parentNode.NodeValue);
                node = parentNode;
            } while (parentNode != null);
            return nodePath.ToArray();
        }

         void AddBaseNodeLevel(Point node, int nodeValue)
        {
            allNodes.Add(new Node(runningValueID++, node, nodeValue, 0, 0));
        }

         int AddSubNode()
        {
            int count = 0;
            allNodes.Where(x => x.Level == runningLevel).ToArray().ToList().ForEach(x =>
            {
                count += Addnextadjacent(Cardinalpoints.North, x) + Addnextadjacent(Cardinalpoints.South, x) + Addnextadjacent(Cardinalpoints.East, x) + Addnextadjacent(Cardinalpoints.West, x);
            });
            runningLevel++;
            return count;
        }

         int Addnextadjacent(Cardinalpoints dir, Node node)
        {
            Point adjacent = Findadjacent(dir, node.Loc);
            if (adjacent != null && points[adjacent.x][adjacent.y] < node.NodeValue)
            {
                allNodes.Add(new Node(runningValueID++, adjacent, points[adjacent.x][adjacent.y], node.Id, runningLevel + 1));
                return 1;
            }
            return 0;
        }

      
        Point Findadjacent(Cardinalpoints dir, Point node)
        {
            Point nextLoc = null;
            switch (dir)
            {
                case Cardinalpoints.North:
                    nextLoc = node.x - 1 >= 0 ? new Point(node.x - 1, node.y) : null;
                    break;
                case Cardinalpoints.South:
                    nextLoc = node.x + 1 < points.GetLength(0) ? new Point(node.x + 1, node.y) : null;
                    break;
                case Cardinalpoints.East:
                    nextLoc = node.y + 1 < points.GetLength(0) ? new Point(node.x, node.y + 1) : null;
                    break;
                case Cardinalpoints.West:
                    nextLoc = node.y - 1 >= 0 ? new Point(node.x, node.y - 1) : null;
                    break;
                default:
                    break;
            }
            return nextLoc;

        }
        #endregion

        #region Objects
        enum Cardinalpoints
        {
            North,
            South,
            East,
            West
        }

        public class Point
        {
            public int x { get; set; }
            public int y { get; set; }
            public Point(int p1, int p2)
            {
                x = p1;
                y = p2;
            }
        }

        public class Node
        {
            public int Id { get; set; }
            public Point Loc { get; set; }
            public int NodeValue { get; set; }
            public int ParentID { get; set; }
            public int Level { get; set; }
            public Node(int id, Point loc, int nodeValue, int parentId, int level)
            {
                Id = id;
                Loc = loc;
                NodeValue = nodeValue;
                ParentID = parentId;
                Level = level;
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            
           

        }


        
        private void button1_Click(object sender, EventArgs e)
        {

            this.label1.Text = "";

            this.openFileDialog1.FileName = "Select a text file";
            this.openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            this.openFileDialog1.Title = "Open Map text file";
           
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Text = openFileDialog1.FileName;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                points = ReadMapFile();
                for (int i = 0; i < points.Length; i++)
                {
                    for (int j = 0; j < points[i].Length; j++)
                    {
                        AddBaseNodeLevel(new Point(i, j), points[i][j]);
                    }
                }
                //go all over the tree  
                while (AddSubNode() > 0) { }
                var longroute = allNodes.OrderByDescending(x => x.Level).FirstOrDefault().Level;
                var steepestfallNodes = new List<int[]>();
                allNodes.Where(y => y.Level == longroute).OrderByDescending(x => x.Id).ToList().ForEach(y => steepestfallNodes.Add(GetTreeNodeValues(y)));
                var steepestfall = steepestfallNodes.OrderByDescending(x => x.Max() - x.Min()).FirstOrDefault();
                this.listBox1.Items.Add("Long route length is : " + (longroute + 1));
                this.listBox1.Items.Add("");
                this.listBox1.Items.Add("The most vertical fall is  : " + (steepestfall.Max() - steepestfall.Min()));
                this.listBox1.Items.Add("");
                this.listBox1.Items.Add("The route is :");
                this.listBox1.Items.Add("");
                steepestfall.ToList().ForEach(x => this.listBox1.Items.Add(x));
                watch.Stop();
               
            }
            catch (Exception ex)
            {
               MessageBox.Show("Sorry, we have a problem!" + ex.Message,"Alexander Mendoza", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }
    }

   













}

