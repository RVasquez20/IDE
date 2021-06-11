using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoIDE
{
    public partial class DOM : Form
    {
        string pal;
        string[] trozos1;
        public DOM()
        {
            InitializeComponent();
        }
      
        public void Graficar(int[] columna)
        {
            Graphics nodo;
            nodo = CreateGraphics();


            int tam = 580;

            int fila = columna.Length;

            int sum = columna.Sum();

            int k = 0;

            Font drawFont = new Font("Arial", 13);

            int y =30;
            int x = 50;
            int z = 0;
            int[] a = new int[50];//
            int[] b = new int[50];//
            Pen linea = new Pen(Color.Gold, 2);

            for (int i = 0; i < fila; i++)
            {
                x = (tam / (columna[i] + 1));
                z = x;
                for (int j = 0; j < columna[i]; j++)
                {

                    nodo.FillRectangle(Brushes.White, x, y, 78, 35);
                    nodo.DrawString(trozos1[k], drawFont, Brushes.Black, x + 13, y + 5);//PINTA EL TEXTO
                    a[k] = x;//CUENTA PUNTOS PARA LINEAS
                    b[k] = y;//CUENTA PUNTOS PARA LINEAS
                    x += z;
                    k++;//AYUDA A PINTAR EL TEXTO
                }

                y += 60;

            }

            int xi = 300;
            int yi = 60;
            int xf = xi;
            int yf = yi + 30;
            z = 0;
            int w = 0;

            for (int i = 1; i < fila; i++)//------
            {
                if (i != fila - 2)
                {
                    xi = ((tam + 60) / (columna[i] + 1));
                    z = xi;
                    xf = xi;
                    for (int j = 0; j < columna[i]; j++)
                    {

                        nodo.DrawLine(linea, xi, yi, xf, yf);
                        xi += z;
                        xf = xi;
                        //xf += z;

                    }
                }
                else
                {
                    xf = ((tam + 60) / (columna[i] + 1));
                    z = xf;
                    xi = ((tam + 60) / (columna[i - 1] + 1));
                    for (int j = 0; j < columna[i]; j++)
                    {

                        nodo.DrawLine(linea, xi, yi + 5, xf, yf);
                        xf += z;



                    }
                }
                yi = (yf + 30);
                yf = (yi + 30);


            }



        }//TERMINA LA FUNCION





        static string arbol(string pal)
        {
            string ncadena = "";
            int ln = (pal).Length;
            int parentesis1 = 0, parentesis2 = 0, inicio = 0, tamaño = 0, copia = 0;
            string nodo = "", cnodo = "", hijo = "";


            parentesis1 = pal.IndexOf("/");
            parentesis2 = pal.IndexOf(">", parentesis1);//**** 
            int resta = parentesis2 - parentesis1;
            nodo = pal.Substring(parentesis1 + 1, resta - 1);
            cnodo = nodo;
            nodo = "<" + nodo + ">";


            inicio = pal.IndexOf(nodo);
            tamaño = (nodo).Length;//LONGITUD DEL NOD

            copia = inicio + tamaño;
            resta = (parentesis1 - 1) - copia;

            hijo = pal.Substring(copia, resta);//DONE ESTA EL HIJO

            ncadena = pal.Substring(0, inicio) + cnodo + "(" + hijo + ")";
            resta = ln - (parentesis2 + 1);
            ncadena += pal.Substring(parentesis2 + 1, resta);

            return ncadena;

        }

        public int[] proceso(string array)
        {

            int tamaño = (array).Length;
            int fila = 0, j = 0, aux1 = 0, profi = 0;
            int i = 0, p = 0;
            char[] arbol = array.ToCharArray();

            int[] c = new int[20];
            char[] imprimiraux = new char[500];
            int[] imporden = new int[500];

            foreach (char ch in arbol)
            {

                if (ch == '(')
                {
                    aux1 = fila;
                    fila++;

                    if (aux1 < fila)
                    {

                        c[i] = fila;
                        i++;
                    }

                    aux1 = fila;



                }
                if (ch == ')')
                {
                    fila -= 1;
                    aux1 = fila;
                    imprimiraux[p] = '+';///
                    imporden[p] = aux1;///
                    p++;///



                }
                if (ch != '(' && ch != ')' && ch != ',' && ch != '.' && ch != ' ')
                {
                    j++;
                    imprimiraux[p] = ch;
                    imporden[p] = aux1;
                    p++;

                }

            }//TERMINA EL FOR

            int max = 0;
            for (int k = 0; k < c.Length; k++)
            {
                if (c[k] > max)
                {
                    max = c[k];
                }

            }

            profi = max + 1;
           
            string[] primera = new string[50];
            string juntar = "";

            for (int x = 0; x < profi; x++)
            {

                int variables = (imporden).Length;

               
                for (int y = 0; y < variables; y++)
                {
                    if (imporden[y] == x)
                    {

                       
                        juntar += imprimiraux[y].ToString();

                    }


                }
                primera[x] = juntar;
                juntar = "";
               
            }


            primera[profi - 1] = proceso2(array);


            int count = 0;
            int[] dev = new int[profi];
            for (int t = 0; t < profi; t++)
            {
                count = primera[t].Split('+').Length - 1;
                dev[t] = count;

            }

            ////////////////////////////////////////////////////////////////7

            string signo = primera[profi - 1];
            signo = signo.Substring(1);
            primera[profi - 1] = signo;

            string codigo = "";

            for (int t = 0; t < profi; t++)
            {

                codigo += primera[t];////

            }
            char[] delimitador = { '+', ' ' };
            trozos1 = codigo.Split(delimitador);


            return dev;





        }//TERMINA LA FUNCION 













        static string proceso2(string array)//
        {

            int tamaño = (array).Length;
            int fila = 0, j = 0, aux1 = 0, profi = 0;
            int i = 0, p = 0;
            char[] arbol = array.ToCharArray();

            int[] c = new int[20];
            char[] imprimiraux = new char[500];
            int[] imporden = new int[500];

            foreach (char ch in arbol)
            {

                if (ch == '(')
                {
                    aux1 = fila;
                    fila++;

                    if (aux1 < fila)
                    {

                        c[i] = fila;
                        i++;
                    }

                    aux1 = fila;

                    imprimiraux[p] = '+';///
                    imporden[p] = aux1;///
                    p++;///

                }
                if (ch == ')')
                {
                    fila -= 1;
                    aux1 = fila;




                }
                if (ch != '(' && ch != ')' && ch != ',' && ch != '.' && ch != ' ')
                {
                    j++;
                    imprimiraux[p] = ch;
                    imporden[p] = aux1;
                    p++;

                }

            }//TERMINA EL FOR

            int max = 0;
            for (int k = 0; k < c.Length; k++)
            {
                if (c[k] > max)
                {
                    max = c[k];
                }

            }

            profi = max + 1;
            string[] primera = new string[50];
            string juntar = "";

            for (int x = 0; x < profi; x++)
            {

                int variables = (imporden).Length;

                for (int y = 0; y < variables; y++)
                {
                    if (imporden[y] == x)
                    {

                        juntar += imprimiraux[y].ToString();

                    }


                }
                primera[x] = juntar;
                juntar = "";

            }
         
            return primera[profi - 1];

        }//TERMINA LA FUNCION

       

      
        private void DOM_Activated_1(object sender, EventArgs e)
        {
            pal = FormMenuPrincipal.variableCom;
            if (pal.Length > 5)
            {
                pal = pal.Replace("\n", "");
                pal = pal.Replace(" ", "");
                string regresa = "";
                bool i = true;
                while (i == true)
                {
                    regresa = arbol(pal);
                    pal = regresa;
                    i = pal.Contains("/");

                }

                int[] resivir = (proceso(pal));
                Graficar(resivir);
            }
        }

        private void DOM_Shown(object sender, EventArgs e)
        {
            pal = FormMenuPrincipal.variableCom;
            pal = pal.Replace("\n", "");
            pal = pal.Replace(" ", "");
            string regresa = "";
            bool i = true;


            while (i == true)
            {
                regresa = arbol(pal);
                pal = regresa;
                i = pal.Contains("/");

            }

            int[] resivir = (proceso(pal));
            Graficar(resivir);

        }

      

        


        
        }

      
    
}
