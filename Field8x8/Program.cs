using System;
using System.Collections;
using System.Threading;

namespace Field8x8
{
    class Program
    {
        class Rectangle
        {
            public int width { get; set; }
            public int height { get; set; }
            public int[] upperleft_corner { get; set; }
            public int[] lowerleft_corner { get; set; }
            public int[] upperright_corner { get; set; }
            public int[] lowerright_corner { get; set; }
            public Rectangle(int x, int y, int corner)
            {
                width = 1;
                height = 1;
                upperleft_corner = new int[2];
                lowerleft_corner = new int[2];
                upperright_corner = new int[2];
                lowerright_corner = new int[2];
                switch (corner)
                {
                    case 1: { upperleft_corner[0] = x; upperleft_corner[1] = y; break; }
                    case 2: { upperright_corner[0] = x; upperright_corner[1] = y; break; }
                    case 3: { lowerleft_corner[0] = x; lowerleft_corner[1] = y; break; }
                    case 4: { lowerright_corner[0] = x; lowerright_corner[1] = y; break; }
                }
            }
            public int[] RPoint(int corner)
            {
                int[] ThisCorner = new int[2];
                switch(corner)
                {
                    case 1: { ThisCorner = upperleft_corner; break; }
                    case 2: { ThisCorner = upperright_corner; break; }
                    case 3: { ThisCorner = lowerleft_corner; break; }
                    case 4: { ThisCorner = lowerright_corner; break; }
                }
                return ThisCorner;
            }
            public void FixingCorner(int corner)
            {
                switch(corner)
                {
                    case 1:
                        {
                            lowerright_corner[0] = upperleft_corner[0] + height - 1;
                            lowerright_corner[1] = upperleft_corner[1] + width - 1;
                            upperright_corner[0] = upperleft_corner[0];
                            upperright_corner[1] = lowerright_corner[1];
                            lowerleft_corner[0] = lowerright_corner[0];
                            lowerleft_corner[1] = upperleft_corner[1];
                            break;
                        }
                    case 2:
                        {
                            lowerleft_corner[0] = upperright_corner[0] + height - 1;
                            lowerleft_corner[1] = upperright_corner[1] - width + 1;
                            upperleft_corner[0] = upperright_corner[0];
                            upperleft_corner[1] = lowerleft_corner[1];
                            lowerright_corner[0] = lowerleft_corner[0];
                            lowerright_corner[1] = upperright_corner[1];
                            break;
                        }
                    case 3:
                        {
                            upperright_corner[0] = lowerleft_corner[0] - height + 1;
                            upperright_corner[1] = lowerleft_corner[0] + width - 1;
                            lowerright_corner[0] = lowerleft_corner[0];
                            lowerright_corner[1] = upperright_corner[1];
                            upperleft_corner[0] = upperright_corner[0];
                            upperleft_corner[1] = lowerleft_corner[1];
                            break;
                        }
                    case 4:
                        {
                            upperleft_corner[0] = lowerright_corner[0] - height + 1;
                            upperleft_corner[1] = lowerright_corner[1] - width + 1;
                            lowerleft_corner[0] = lowerright_corner[0];
                            lowerleft_corner[1] = upperleft_corner[1];
                            upperright_corner[0] = upperleft_corner[0];
                            upperright_corner[1] = lowerright_corner[1];
                            break;
                        }
                }
            }
        }
        class Field
        {
            public Nullable<int>[,] field { get; set; }
            public int[] TLcounter { get; set;}
            public int[] TRcounter { get; set; }
            public int[] BLcounter { get; set; }
            public int[] BRcounter { get; set; }
            public int count { get; set; }
            public ArrayList rectangles { get; set; }
            private readonly object CheckLock = new object();

            public Field()
            {
                field = new Nullable<int>[8, 8];
                TLcounter = new int[2] { 0, 0 };
                TRcounter = new int[2] { 0, 7 };
                BLcounter = new int[2] { 7, 0 };
                BRcounter = new int[2] { 7, 7 };
                count = 0;
                rectangles = new ArrayList();
            }

            public void EnterFieldNumbers()
            {
                Random rand = new Random();
                Console.WriteLine("\nEnter the integer values for field 8x8");
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Console.Write($"value {i+1}{j+1} : ");
                        try
                        {
                            field[i, j] = Convert.ToInt32(Console.ReadLine());
                        }
                        catch
                        {
                            field[i, j] = rand.Next(1, 10);
                            Console.WriteLine(field[i,j]);
                        }
                    }
                }
                Console.WriteLine("Thanks!");
            }
            public void FieldDisplay()
            {
                Console.WriteLine("FIELD OF RECTANGLES : ");
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        string symbol = (field[i,j] == 0) ? "██" : ((field[i,j] == null) ? "░░" : "▒▒");
                        Console.Write(symbol);
                    }
                    Console.WriteLine();
                }
            }
            public bool[] Conditions(int counter)
            {
                bool[] borders = new bool[2];
                switch(counter)
                {
                    case 1: { borders[0] = TLcounter[0] <= 3; borders[1] = TLcounter[1] <= 3; break; }
                    case 2: { borders[0] = TRcounter[0] <= 3; borders[1] = TRcounter[1] >= 4; break; }
                    case 3: { borders[0] = BLcounter[0] >= 4; borders[1] = BLcounter[1] <= 3; break; }
                    case 4: { borders[0] = BRcounter[0] >= 4; borders[1] = BRcounter[1] >= 4; break; }
                }
                return borders;
            }
            public void SearchForRectangles(object start)
            {
                int corner = Convert.ToInt32(start);
                while (Conditions(corner)[0])
                {
                    switch(corner)
                    {
                        case 1: { TLcounter[1] = 0; break; }
                        case 2: { TRcounter[1] = 7; break; }
                        case 3: { BLcounter[1] = 0; break; }
                        case 4: { BRcounter[1] = 7; break; }
                    }
                    while (Conditions(corner)[1])
                    {
                        int i = 0;
                        int j = 0;
                        switch(corner)
                        {
                            case 1: { i = TLcounter[0]; j = TLcounter[1]; break; }
                            case 2: { i = TRcounter[0]; j = TRcounter[1]; break; }
                            case 3: { i = BLcounter[0]; j = BLcounter[1]; break; }
                            case 4: { i = BRcounter[0]; j = BRcounter[1]; break; }
                        }
                        if (field[i,j] == 0)
                        {
                            Rectangle NewRectangle = new Rectangle(i,j,corner);
                            WidthOfRectangle(NewRectangle,corner);
                            HeightOfRectangle(NewRectangle,corner);
                            NewRectangle.FixingCorner(corner);

                            bool unique = true;
                            lock (CheckLock)
                            {
                                foreach (Rectangle rect in rectangles)
                                {
                                    if (rect.RPoint(1)[0] == NewRectangle.RPoint(1)[0] & rect.RPoint(1)[1] == NewRectangle.RPoint(1)[1])
                                    {
                                        switch (corner)
                                        {
                                            case 1: { TLcounter[1] += NewRectangle.width; break; }
                                            case 2: { TRcounter[1] -= NewRectangle.width; break; }
                                            case 3: { BLcounter[1] += NewRectangle.width; break; }
                                            case 4: { BRcounter[1] -= NewRectangle.width; break; }
                                        }
                                        unique = false;
                                        break;
                                    }
                                }
                                if (unique)
                                {
                                    rectangles.Add(NewRectangle);
                                    CleaningField(NewRectangle);
                                    Console.WriteLine($"\nCounter{corner} found a rectangle...");
                                    FieldDisplay();
                                }
                            }
                            if (unique)
                            {
                                count++;
                                switch (corner)
                                {
                                    case 1: { TLcounter[1] += NewRectangle.width; break; }
                                    case 2: { TRcounter[1] -= NewRectangle.width; break; }
                                    case 3: { BLcounter[1] += NewRectangle.width; break; }
                                    case 4: { BRcounter[1] -= NewRectangle.width; break; }
                                }
                            }
                        }
                        switch (corner)
                        {
                            case 1: { TLcounter[1]++; break; }
                            case 2: { TRcounter[1]--; break; }
                            case 3: { BLcounter[1]++; break; }
                            case 4: { BRcounter[1]--; break; }
                        }
                    }
                    switch (corner)
                    {
                        case 1: { TLcounter[0]++; break; }
                        case 2: { TRcounter[0]++; break; }
                        case 3: { BLcounter[0]--; break; }
                        case 4: { BRcounter[0]--; break; }
                    }
                }
            }
            public void WidthOfRectangle(Rectangle rect, int corner)
            {
                int i = rect.RPoint(corner)[0];
                int j = rect.RPoint(corner)[1];
                while (true)
                {
                    j = (corner == 1 | corner == 3) ? j + 1 : j - 1;
                    try
                    {
                        if (field[i, j] == 0)
                            rect.width++;
                        else
                            break;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        break;
                    }
                }
            }
            public void HeightOfRectangle(Rectangle rect, int corner)
            {
                int i = rect.RPoint(corner)[0];
                int j = rect.RPoint(corner)[1];
                while (true)
                {
                    i = (corner == 1 | corner == 2) ? i + 1 : i - 1;
                    try
                    {
                        if (field[i, j] == 0)
                            rect.height++;
                        else
                            break;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        break;
                    }
                }
            }
            public void CleaningField(Rectangle rect)
            {
                int x = rect.RPoint(1)[0];
                int y = rect.RPoint(1)[1];
                for (int i = x-1; i <= x + rect.height; i++)
                {
                    for (int j = y-1; j <= y + rect.width; j++)
                    {
                        try
                        {
                            field[i, j] = null;
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Field field1 = new Field();
            field1.EnterFieldNumbers();
            field1.FieldDisplay();
            Console.WriteLine("\nSearch for rectangles...\n");

            Thread threadTLcounter = new Thread(new ParameterizedThreadStart(field1.SearchForRectangles));
            Thread threadTRcounter = new Thread(new ParameterizedThreadStart(field1.SearchForRectangles));
            Thread threadBLcounter = new Thread(new ParameterizedThreadStart(field1.SearchForRectangles));
            Thread threadBRcounter = new Thread(new ParameterizedThreadStart(field1.SearchForRectangles));

            threadTLcounter.Start(1);
            threadTRcounter.Start(2);
            threadBLcounter.Start(3);
            threadBRcounter.Start(4);

            threadTLcounter.Join();
            threadTRcounter.Join();
            threadBLcounter.Join();
            threadBRcounter.Join();

            Console.WriteLine($"\nCount of rectangles: {field1.count}");

            Console.ReadKey();
        }
    }
}
