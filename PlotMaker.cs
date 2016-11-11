using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Plotmaker
{
    public partial class MainWindow : Window
    {

        #region Contructor

        public MainWindow()
        {
            InitializeComponent();
            tb_presnost.TextChanged += tb_presnost_TextChanged;
            tb_interval.TextChanged += tb_interval_TextChanged;
        }

        #endregion

        /*
         * Maturitní práce
         * 2013/2014
         * 
         * Václav Pelíšek
         * 
         * 
         * 
         * ////----------------BASICS----------------
        */

        // 1) DECLARING

        #region declaring

        static bool inputcheck = true;
        static int delka;
        static double parsehelper;
        static List<double> bodyY = new List<double>();
        static List<decimal> bodyX = new List<decimal>();
        static decimal pocetbodu;
        static decimal interval;
        static decimal step;
        static double maxY;
        static double minY;
        static double x0;
        static List<double> y0 = new List<double>();

        #endregion

        // 2) EVENTS

        #region event zadanichanged

        private void zadani_TextChanged(object sender, TextChangedEventArgs e)
        {
            Button_solution.IsEnabled = false;
            label_x0.Content = "Returns Y value for x=0: ";
            output_label.Content = "Additional Output";
        }

        #endregion

        #region events settings changed

        private void tb_interval_TextChanged(object sender, TextChangedEventArgs e)
        {
            output_label.Content = "Additional Output";
        }

        private void tb_presnost_TextChanged(object sender, TextChangedEventArgs e)
        {
            output_label.Content = "Additional Output";
        }

        #endregion

        #region event window_loaded

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            drawlines();
        }

        #endregion

        #region event main click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        // Restart:

            inputcheck = true;
            maxY = -Double.MaxValue;
            minY = Double.MaxValue;
            bodyX.Clear();
            bodyY.Clear();
            y0.Clear();

        // Text Loader:

            string defaulttext = zadani.Text;
            defaulttext = defaulttext.Replace("π", Math.PI.ToString());
            defaulttext = defaulttext.Replace("pi", Math.PI.ToString());
            defaulttext = defaulttext.Replace("e", Math.E.ToString());
            defaulttext = defaulttext.Replace(".", ",");
            if (defaulttext != "")
            {

        // Settings parser:

                bool tryparse = true;
                if (Decimal.TryParse(tb_presnost.Text, out pocetbodu) == false)
                {
                    tryparse = false;
                }
                if (Decimal.TryParse(tb_interval.Text, out interval) == false)
                {
                    tryparse = false;
                }

        // Point Creator:

                if (tryparse == true && pocetbodu != 0)
                {
                    step = 2 * interval / pocetbodu;
                    if (step > 0)
                    {
                        for (decimal i = -interval; i < interval; i += step)
                        {
                            string text = defaulttext.Replace("x", "(" + i.ToString() + ")");
                            string reseni = vyres(text);
                            if (inputcheck == false)
                            {
                                break;
                            }
                            bodyX.Add(i);
                            double pomocna;
                            if (Double.TryParse(reseni, out pomocna) == true)
                            {
                                bodyY.Add(pomocna);

        // Find and report X = 0 value

                                if (i == 0)
                                {
                                    x0 = pomocna;
                                }

        // Find max and min Y for scaling

                                if (pomocna > maxY && double.IsInfinity(pomocna) == false)
                                {
                                    maxY = pomocna;
                                }
                                if (pomocna < minY && double.IsInfinity(pomocna) == false)
                                {
                                    minY = pomocna;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

        // Output:

                        //if (bodyY.Count == pocetbodu)
                        {
                            draw();
                            Button_solution.IsEnabled = true;
                        }
                        //else
                        {
                            output_label.Content = "Invalid input.";
                        }
                    }
                    else
                    {
                        output_label.Content = "Invalid settings.";
                    }
                }
                else
                {
                    output_label.Content = "Invalid settings.";
                }
            }
            else
            {
                 output_label.Content = "Empty input.";
            }
        }

        #endregion

        #region event click 2

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            label_x0.Content = "Returns Y value for x=0: " + x0;
        }

        #endregion

        /*
         * ////----------------GRAPHICAL ENGINE----------------
        */

        // 1) DECLARING

        #region declaring

        private Line xAxis = new Line();
        private Line yAxis = new Line();
        private List<Line> lines = new List<Line>();
        private const double lineWidth = 2.0d;

        #endregion

        // 2) EVENTS

        #region draw default X and Y lines

        private void drawlines()
        {
            xAxis.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            xAxis.X1 = 0;
            xAxis.X2 = graphCanvas.ActualWidth;
            xAxis.Y1 = graphCanvas.ActualHeight / 2;
            xAxis.Y2 = graphCanvas.ActualHeight / 2;
            xAxis.StrokeThickness = lineWidth;
            graphCanvas.Children.Add(xAxis);

            yAxis.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            yAxis.X1 = graphCanvas.ActualWidth / 2;
            yAxis.X2 = graphCanvas.ActualWidth / 2;
            yAxis.Y1 = 0;
            yAxis.Y2 = graphCanvas.ActualHeight;
            yAxis.StrokeThickness = lineWidth;
            graphCanvas.Children.Add(yAxis);
        }

        #endregion

        #region graphic

        private void draw()
        {
        // Clear old

            if (lines != null)
            {
                foreach (Line tmp in lines)
                {
                    graphCanvas.Children.Remove(tmp);
                }
            }
            lines.Clear();

        // Set up scales

            Line tmpL;
            double yScale;
            if (Math.Abs(maxY) > Math.Abs(minY))
            {
                yScale = graphCanvas.ActualHeight / Math.Abs(maxY) / 2.0d;
            }
            else
            {
                yScale = graphCanvas.ActualHeight / Math.Abs(minY) / 2.0d;
            }
            decimal canvaswidth = Convert.ToDecimal(graphCanvas.ActualWidth);
            decimal xScale = canvaswidth / interval / 2;

         // Draw new

            for (int i = 1; i < bodyX.Count; i++)
            {
                if (double.IsNaN(bodyY[i]) == false && double.IsNaN(bodyY[i - 1]) == false && double.IsInfinity(bodyY[i]) == false && double.IsInfinity(bodyY[i - 1]) == false)               
                {
                    tmpL = new Line();
                    tmpL.Stroke = Brushes.Black;
                    tmpL.X1 = Convert.ToDouble((bodyX[i - 1] + interval) * xScale);
                    tmpL.X2 = Convert.ToDouble((bodyX[i] + interval) * xScale);
                    tmpL.Y1 = graphCanvas.ActualHeight / 2 - bodyY[i - 1] * yScale;
                    tmpL.Y2 = graphCanvas.ActualHeight / 2 - bodyY[i] * yScale;
                    tmpL.StrokeThickness = lineWidth;
                    graphCanvas.Children.Add(tmpL);
                    lines.Add(tmpL);
                }
                else
                {
                    continue;
                }
            }
        }

        #endregion

        /*
         * ////----------------EVAULATOR ENGINE----------------
        */

        // 1) FUNCTIONS PARSER

        #region functions

        static string vyres(string text)
        {
            while (text.Contains("ln") == true || text.Contains("abs") == true || text.Contains("sqrt") == true || text.Contains("sin") == true || text.Contains("cos") == true || text.Contains("tg") == true || text.Contains("cot") == true)
            {
                int[] index = new int[8];
                index[0] = text.LastIndexOf("sqrt");
                index[1] = text.LastIndexOf("sin");
                index[2] = text.LastIndexOf("cos");
                index[3] = text.LastIndexOf("ln");
                index[4] = text.LastIndexOf("tg");
                index[5] = text.LastIndexOf("cot");
                index[6] = text.LastIndexOf("abs");

        // Getting highest index:

                int maxindex = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (index[i] > maxindex)
                    {
                        maxindex = index[i];
                    }
                }

        // Functions solvers:

                #region Sqrt

                if (maxindex == index[0])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 4 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 4 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 4 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 4 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 4 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 5, druhazavorka - maxindex - 5);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Sqrt(funkcevysledek);
                            if (funkcevysledek == double.NaN)
                            {
                                text = double.NaN.ToString();
                                return text;
                            }
                            else
                            {
                                text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                                text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region Sin

                else if (maxindex == index[1])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 3 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 3 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 3 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 4, druhazavorka - maxindex - 4);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Sin(funkcevysledek);
                            text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                            text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region Cos

                else if (maxindex == index[2])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 3 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 3 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 3 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 4, druhazavorka - maxindex - 4);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Cos(funkcevysledek);
                            text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                            text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region Ln

                else if (maxindex == index[3])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 2 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 2 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 2 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 2 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 2 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 3, druhazavorka - maxindex - 3);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Log(funkcevysledek, Math.E);
                            if (funkcevysledek == double.NaN)
                            {
                                return double.NaN.ToString();
                            }
                            else
                            {
                                text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                                text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region Tg

                else if (maxindex == index[4])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 2 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 2 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 2 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 2 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 2 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 3, druhazavorka - maxindex - 3);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Tan(funkcevysledek);
                            text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                            text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region Cot

                else if (maxindex == index[5])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 3 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 3 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 3 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 4, druhazavorka - maxindex - 4);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Cos(funkcevysledek) / Math.Sin(funkcevysledek);
                            text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                            text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region Abs

                else if (maxindex == index[6])
                {
                    int druhazavorka = 0;
                    double funkcevysledek;
                    int otevrena = 0;
                    for (int i = 1; ; i++)
                    {
                        if (maxindex + 3 + i < text.Length)
                        {
                            if (text.ElementAt(maxindex + 3 + i) == '(')
                            {
                                otevrena++;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena != 0)
                            {
                                otevrena--;
                            }
                            else if (text.ElementAt(maxindex + 3 + i) == ')' && otevrena == 0)
                            {
                                druhazavorka = maxindex + 3 + i;
                                break;
                            }
                        }
                        else
                        {
                            inputcheck = false;
                            break;
                        }
                    }
                    if (inputcheck == true)
                    {
                        string funkce = text.Substring(maxindex + 4, druhazavorka - maxindex - 4);
                        funkcevysledek = Double.Parse(zjednodus(funkce));
                        if (inputcheck == true)
                        {
                            funkcevysledek = Math.Abs(funkcevysledek);
                            text = text.Remove(maxindex, druhazavorka - maxindex + 1);
                            text = text.Insert(maxindex, "(" + funkcevysledek.ToString() + ")");
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion


        // Additional space for future (sinh, cosh, ...)

                else if (maxindex == index[7])
                {

                }

        // Output:

            }
            return zjednodus(text);
        }

        #endregion

        // 2) BRACKETS PARSER

        #region brackets

        static string zjednodus(string text)
        {
            while (text.Contains("(") == true)
            {
                if (text.Contains(")") == true)
                {

            // Get indexes and exponentation bool:

                    int druhazavorka = text.IndexOf(")");
                    bool mocnenizavorky = false;
                    if (text.Length > druhazavorka + 1)
                    {
                        if (text.ElementAt(druhazavorka + 1) == '^')
                        {
                            mocnenizavorky = true;
                        }
                    }
                    int prvnizavorka = 0;
                    for (int j = druhazavorka; j > 0; j--)
                    {
                        if (text.ElementAt(j) == '(')
                        {
                            prvnizavorka = j;
                            break;
                        }
                    }

            // Solve:

                    string zavorka = text.Substring(prvnizavorka + 1, druhazavorka - prvnizavorka - 1);
                    string reseni_zavorky = vypocti(zavorka);
                    if (mocnenizavorky == false)
                    {
                        text = text.Remove(prvnizavorka, druhazavorka - prvnizavorka + 1);
                        text = text.Insert(prvnizavorka, reseni_zavorky.ToString());
                    }

            // And involve if needed:

                    else
                    {
                        double exponent = getnumberAF(druhazavorka + 1, text);
                        int exponentdelka = delka;
                        reseni_zavorky = (Math.Pow(Double.Parse(reseni_zavorky), exponent)).ToString();
                        text = text.Remove(prvnizavorka, (druhazavorka - prvnizavorka) + 2 + exponentdelka);
                        text = text.Insert(prvnizavorka, reseni_zavorky.ToString());
                    }
                }
                else
                {
                    inputcheck = false;
                    break;
                }
            }
            return vypocti(text);
        }

        #endregion

        // 3) MAIN PARSER

        // 3.1) main handler

        #region main

        static string vypocti(string text)
        {
            #region ^solver 100%

            while (text.Contains('^') == true)
            {

                // Fix operators:

                while (text.Contains("++") || text.Contains("+-") || text.Contains("-+") || text.Contains("--"))
                {
                    text = text.Replace("+-", "-");
                    text = text.Replace("-+", "-");
                    text = text.Replace("++", "+");
                    text = text.Replace("--", "+");
                }

                // Get numbers:

                int index = text.LastIndexOf('^');
                double vysledek = 0;
                double exponent = getnumberAF(index, text);
                int exponentdelka = delka;
                double zaklad = getnumberBF(index, text);
                int zakladdelka = delka;
                int skutecnadelka = zakladdelka + 1 + exponentdelka;

                // Check for negative base:

                bool flag = false;
                if (zaklad < 0)
                {
                    zaklad = Math.Abs(zaklad);
                    flag = true;
                }

                // 0^0 exception because C# can't do math:

                if (exponent == 0 && zaklad == 0)
                {
                    vysledek = double.NaN;
                    text = text.Remove(index - zakladdelka, skutecnadelka);
                    text = text.Insert(index - zakladdelka, vysledek.ToString());
                }

                // Solve:

                if (inputcheck == true)
                {
                    vysledek = Math.Pow(zaklad, exponent);
                }
                else
                {
                    break;
                }

                // And replace:

                int startindex;
                if (flag == false)
                {
                    startindex = index - zakladdelka;
                    skutecnadelka = exponent.ToString().Length + 1 + zaklad.ToString().Length;
                }
                else
                {
                    startindex = index - zakladdelka + 1;
                    skutecnadelka = exponentdelka + zakladdelka;
                }
                text = text.Remove(startindex, skutecnadelka);
                text = text.Insert(startindex, vysledek.ToString());
            }

            #endregion

            #region *solver 100%

            while (text.Contains('*'))
            {

                // Fix Operators:

                while (text.Contains("++") || text.Contains("+-") || text.Contains("-+") || text.Contains("--"))
                {
                    text = text.Replace("+-", "-");
                    text = text.Replace("-+", "-");
                    text = text.Replace("++", "+");
                    text = text.Replace("--", "+");
                }

                // Solve:

                int index = text.IndexOf('*');
                double vysledek = 0;
                double cinitel1 = getnumberAF(index, text);
                int cinitel1delka = delka;
                double cinitel2 = getnumberBF(index, text);
                int cinitel2delka = delka;
                if (inputcheck == true)
                {
                    vysledek = cinitel2 * cinitel1;
                }
                else
                {
                    text = "0";
                    break;
                }
                int startindex = index - cinitel2delka;
                int skutecnadelka = cinitel1delka + 1 + cinitel2delka;
                text = text.Remove(startindex, skutecnadelka);
                text = text.Insert(startindex, vysledek.ToString());
            }

            #endregion

            #region /solver 100%

            while (text.Contains('/'))
            {

                // Fix Operators:

                while (text.Contains("++") || text.Contains("+-") || text.Contains("-+") || text.Contains("--"))
                {
                    text = text.Replace("+-", "-");
                    text = text.Replace("-+", "-");
                    text = text.Replace("++", "+");
                    text = text.Replace("--", "+");
                }

                // Solve:

                int index = text.IndexOf('/');
                double vysledek = 0;
                double delitel = getnumberAF(index, text);
                int deliteldelka = delka;
                double delenec = getnumberBF(index, text);
                int delenecdelka = delka;
                if (inputcheck == true)
                {
                    vysledek = delenec / delitel;
                }
                else
                {
                    text = "0";
                    break;
                }
                int startindex = index - delenecdelka;
                int skutecnadelka = deliteldelka + 1 + delenecdelka;
                text = text.Remove(startindex, skutecnadelka);
                text = text.Insert(startindex, vysledek.ToString());
            }

            #endregion

            #region +-solver 100%

            while (text.Contains('-') || text.Contains('+'))
            {

                // Fix Operators:

                while (text.Contains("++") || text.Contains("+-") || text.Contains("-+") || text.Contains("--"))
                {
                    text = text.Replace("+-", "-");
                    text = text.Replace("-+", "-");
                    text = text.Replace("++", "+");
                    text = text.Replace("--", "+");
                }

                // Check:

                if (Double.TryParse(text, out parsehelper) == false)
                {

                    // Get indexes and exclude exponential notation:

                    int indexminus = int.MaxValue;
                    int indexplus = int.MaxValue;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (i > 1)
                        {
                            if (text.ElementAt(i) == '-' && text.ElementAt(i - 1) != 'E')
                            {
                                indexminus = i;
                                break;
                            }
                            if (text.ElementAt(i) == '+' && text.ElementAt(i - 1) != 'E')
                            {
                                indexplus = i;
                                break;
                            }
                        }
                        else
                        {
                            if (text.ElementAt(i) == '-' && i > 0)
                            {
                                indexminus = i;
                                break;
                            }
                            if (text.ElementAt(i) == '+' && i > 0)
                            {
                                indexplus = i;
                                break;
                            }
                        }
                    }

                    // And compare them:

                    int index;
                    if (indexminus < indexplus)
                    {
                        index = indexminus;
                    }
                    else if (indexplus < indexminus)
                    {
                        index = indexplus;
                    }
                    else if (indexplus == 0)
                    {
                        text = text.Remove(0, 1);
                        continue;
                    }
                    else
                    {
                        continue;
                    }

                    // Solve:

                    double vysledek = 0;
                    double cinitel1 = getnumberAF(index, text);
                    int cinitel1delka = delka;
                    double cinitel2 = getnumberBF(index, text);
                    int cinitel2delka = delka;
                    if (inputcheck == true)
                    {
                        if (indexminus < indexplus)
                        {
                            vysledek = cinitel2 - cinitel1;
                        }
                        else
                        {
                            vysledek = cinitel2 + cinitel1;
                        }
                    }
                    else
                    {
                        text = "0";
                        break;
                    }
                    int startindex = index - cinitel2delka;
                    int skutecnadelka = cinitel1delka + 1 + cinitel2delka;
                    text = text.Remove(startindex, skutecnadelka);
                    text = text.Insert(startindex, vysledek.ToString());
                }
                else
                {
                    text = parsehelper.ToString();
                    break;
                }
            }

            #endregion

            if (Double.TryParse(text, out parsehelper) == false)
            {
                inputcheck = false;
            }
            if (inputcheck == true)
            {
                return text;
            }
            else
            {
                return "0";
            }
        }

        #endregion

        // 3.2) assistants

        #region get Numbers Around Operators 100%

        // Get number after operator:

        static double getnumberAF(int index, string text)
        {
            delka = 0;
            double cislo = 0;
            bool flag = false;
            for (int i = 1; ; i++)
            {
                if (index + i < text.Length)
                {
                    byte pomocna;
                    if (Byte.TryParse(text.Substring(index + i, 1), out pomocna) == false)
                    {

        // Decimal point:

                        if (text.ElementAt(index + i) == ',' && flag == true)
                        {
                            inputcheck = false;
                            break;
                        }
                        else if (text.ElementAt(index + i) == ',' && flag == false)
                        {
                            delka++;
                            flag = true;
                        }

        // Minus operator:

                        else if (text.ElementAt(index + i) == '-' && i == 1)
                        {
                            delka++;
                            continue;
                        }
                        else if (text.ElementAt(index + i) == '-' && text.ElementAt(index + i - 1) == 'E')
                        {
                            delka++;
                            continue;
                        }

        // Exponential Notation:

                        else if (text.ElementAt(index + i) == 'E')
                        {
                            try
                            {
                                if ((text.ElementAt(index + i + 1) == '-' || text.ElementAt(index + i + 1) == '+'))
                                {
                                    delka++;
                                    continue;
                                }
                                else
                                {
                                    inputcheck = false;
                                    break;
                                }

                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                inputcheck = false;
                                break;
                            }
                        }

        // Plus Operator:

                        else if (text.ElementAt(index + i) == '+' && i == 1)
                        {
                            delka++;
                            continue;
                        }
                        else if (text.ElementAt(index + i) == '+' && text.ElementAt(index + i - 1) == 'E')
                        {
                            delka++;
                            continue;
                        }

        // Parse:

                        else
                        {
                            if (Double.TryParse(text.Substring(index + 1, i - 1), out cislo) == false)
                            {
                                inputcheck = false;
                            }
                            break;
                        }
                    }
                    else
                    {
                        delka++;
                        continue;
                    }
                }
                else if (i < 2)
                {
                    inputcheck = false;
                    break;
                }
                else
                {
                    if (Double.TryParse(text.Substring(index + 1, i - 1), out cislo) == false)
                    {
                        inputcheck = false;
                    }
                    break;
                }
            }

            // Return:

            if (inputcheck == true)
            {
                return cislo;
            }
            else
            {
                return 0;
            }
        }

        // Get number before operator:    

        static double getnumberBF(int index, string text)
        {
            delka = 0;
            double cislo = 0;
            bool flag = false;
            for (int i = 1; ; i++)
            {
                if (index - i >= 0)
                {
                    byte pomocna;
                    if (Byte.TryParse(text.Substring(index - i, 1), out pomocna) == false)
                    {

        // Decimal point:

                        if (text.ElementAt(index - i) == ',' && flag == true)
                        {
                            inputcheck = false;
                            break;
                        }
                        else if (text.ElementAt(index - i) == ',' && flag == false)
                        {
                            delka++;
                            flag = true;
                        }

        // Exponential notation:

                        else if (text.ElementAt(index - i) == 'E' && (text.ElementAt(index - i + 1) == '-' || text.ElementAt(index - i + 1) == '+'))
                        {
                            delka++;
                            continue;
                        }

        // Plus operator:

                        else if (text.ElementAt(index - i) == '+')
                        {
                            try
                            {
                                if (text.ElementAt(index - i - 1) == 'E')
                                {
                                    delka++;
                                    continue;
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                if (Double.TryParse(text.Substring(index - i + 1, index - (index - i + 1)), out cislo) == false)
                                {
                                    inputcheck = false;
                                }
                                break;
                            }
                        }

        // Minus Operator:

                        else if (text.ElementAt(index - i) == '-')
                        {
                            delka++;
                            try
                            {
                                if (text.ElementAt(index - i - 1) == 'E')
                                {
                                    continue;
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                if (Double.TryParse(text.Substring(index - i, index - (index - i)), out cislo) == false)
                                {
                                    inputcheck = false;
                                }
                                break;
                            }
                            if (Double.TryParse(text.Substring(index - i, index - (index - i)), out cislo) == false)
                            {
                                inputcheck = false;
                            }
                            break;
                        }

        // Parse:

                        else
                        {
                            if (Double.TryParse(text.Substring(index - i + 1, index - (index - i + 1)), out cislo) == false)
                            {
                                inputcheck = false;
                            }
                            break;
                        }
                    }
                    else
                    {
                        delka++;
                        continue;
                    }
                }
                else if (i > 1)
                {
                    if (Double.TryParse(text.Substring(index - i + 1, index - (index - i + 1)), out cislo) == false)
                    {
                        inputcheck = false;
                    }
                    break;
                }
                else
                {
                    inputcheck = false;
                    break;
                }
            }

        // Return:

            if (inputcheck == true)
            {
                return cislo;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
