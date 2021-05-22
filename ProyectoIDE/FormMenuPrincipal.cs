using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ProyectoIDE
{
    public partial class FormMenuPrincipal : Form
    {
        string archivo;
        //////////Etiquetas
        string[] Reservadas = new string[] {"address","applet","area","a","base","basefont","big","button","blockquote",
                                "body","br","b","caption","center","cite","code","div","dl","form","font","html","head",
                                "p","form","h1","h2","h3","h4","h5","h6","head","img","hr","i","input","link","li","map",
                                "marquee","menu","meta","option","param","p","script","title","type","select","small","strong",
                                "style","table","th","tr","ul","u","var","values","name","placeholder","section"};
        //////////Cierre Etiquetas
        string[] Cierre_Reservadas = new string[] {"/address","/applet","/area","/a","/base","/basefont","/big","/button","/blockquote",
                                "/body","/br","/b","/caption","/center","/cite","/code","/div","/dl","/form","/font","/html","/head",
                                "/p","/form","/h1","/h2","/h3","/h4","/h5","/h6","/head","/img","/hr","/i","/input","/link","/li","/map",
                                "/marquee","/menu","/meta","/option","/param","/p","/script","/title","/type","/select","/small","/strong",
                                "/style","/table","/th","/tr","/ul","/u","/var","/values","/name","/placeholder","/section"};
        /////////Atributos
        string[] Reservadas2 = new string[] {"bgcolor","size","color","face","selected","value","href","style","behavior",
                                "direction","width","heigth","hspace","scrollamount","scrolldelay","truespeed"};

        /////////Colores y Palabas varias
        string[] Reservadas3 = new string[] {"aqua","black","blue","fuchsia","gray","green","lime","maroon","navy","olive",
                                "purple","red","silver","teal","white","yellow","border",":","solid","left","rigth","up",
                                "down","truespeed","alternate"};

        /////////Simbolos
        string[] Reservadas4 = new string[] { "<", ">", "/" };

        //int posicion = 0;
        //importantes para buscar
        int t = 0;
        int nb_result = 0;

        internal static string variableCom = "";
        
        bool confi = true;
        //Constructor
        public FormMenuPrincipal()
        {
            InitializeComponent();
            //Estas lineas eliminan los parpadeos del formulario o controles en la interfaz grafica (Pero no en un 100%)
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            label2.Visible = false;
        }
        //METODO PARA REDIMENCIONAR/CAMBIAR TAMAÑO A FORMULARIO  TIEMPO DE EJECUCION ----------------------------------------------------------
        private int tolerance = 15;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        //----------------DIBUJAR RECTANGULO / EXCLUIR ESQUINA PANEL 
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));

            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);

            region.Exclude(sizeGripRectangle);
            this.panelContenedorPrincipal.Region = region;
            this.Invalidate();
        }
        //----------------COLOR Y GRIP DE RECTANGULO INFERIOR
        protected override void OnPaint(PaintEventArgs e)
        {

            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(55, 61, 69));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);

            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
        }
       
        //METODO PARA ARRASTRAR EL FORMULARIO---------------------------------------------------------------------
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void PanelBarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        //METODOS PARA CERRAR,MAXIMIZAR, MINIMIZAR FORMULARIO------------------------------------------------------
        int lx, ly;
        int sw, sh;
        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            lx = this.Location.X;
            ly = this.Location.Y;
            sw = this.Size.Width;
            sh = this.Size.Height;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
            btnMaximizar.Visible = false;
            btnNormal.Visible = true;

        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            this.Size = new Size(sw, sh);
            this.Location = new Point(lx, ly);
            btnNormal.Visible = false;
            btnMaximizar.Visible = true;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de cerrar?", "Alerta¡¡", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        //METODOS PARA ANIMACION DE MENU SLIDING--
        private void btnMenu_Click(object sender, EventArgs e)
        {
            //-------CON EFECTO SLIDING
            if (panelMenu.Width == 230)
            {
                this.tmContraerMenu.Start();
            }
            else if (panelMenu.Width == 55)
            {
                this.tmExpandirMenu.Start();
            }

            //-------SIN EFECTO 
            //if (panelMenu.Width == 55)
            //{
            //    panelMenu.Width = 230;
            //}
            //else

            //    panelMenu.Width = 55;
        }

        private void tmExpandirMenu_Tick(object sender, EventArgs e)
        {
            if (panelMenu.Width >= 230)
                this.tmExpandirMenu.Stop();
            else
                panelMenu.Width = panelMenu.Width + 5;
            
        }

        private void tmContraerMenu_Tick(object sender, EventArgs e)
        {
            if (panelMenu.Width <= 55)
                this.tmContraerMenu.Stop();
            else
                panelMenu.Width = panelMenu.Width - 5;
        }


    

      
       

        private void aBRIRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "Texto|*.HTML";
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                archivo = Openfile.FileName;
                using (StreamReader sr = new StreamReader(archivo))
                {
                    fastColoredTextBox1.Text = sr.ReadToEnd();
                }
                NamesFiles(Openfile.FileName);
                
            }
        }

        private void NamesFiles(string Nombre)
        {
            char[] delimiterChars = { '.', '\\' };
            string[] nombre = Nombre.Split(delimiterChars);
            int cantidad = 0;
            cantidad = nombre.Length;
            Titulo.Text = nombre[cantidad - 2];
            label2.Visible = true;
        }

        private void cerrarArchivo_Click(object sender, EventArgs e)
        { 
            fastColoredTextBox1.Clear();
            Titulo.Text = "Sin Titulo";
            label2.Visible = false;
            archivo = null;
        }

        private void nUEVOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogo = MessageBox.Show("¿Desea Guardar los Cambios?", "Guardar Cambios",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.No)
            {
                fastColoredTextBox1.Clear();
                archivo = null;
            }
            else
            {
                save();
                fastColoredTextBox1.Clear();
                archivo = null;
            }
        }
        private void save()//GUARDAR
        {
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "Texto|*.HTML";
            if (archivo != null)
            {
                using (StreamWriter sw = new StreamWriter(archivo))
                {
                    sw.Write(fastColoredTextBox1.Text);
                }
                Titulo.Text= Titulo.Text.Remove(Titulo.Text.Length - 1,1 );
            }
            else
            {
                if (SaveFile.ShowDialog() == DialogResult.OK)
                {
                    archivo = SaveFile.FileName;
                    using (StreamWriter sw = new StreamWriter(SaveFile.FileName))
                    {
                        sw.Write(fastColoredTextBox1.Text);
                    }
                NamesFiles(archivo);
                }
            }


        }

        private void gUARDARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (!Titulo.Text.Contains("*"))
            {
                Titulo.Text += "*";
            }
        }

   

        private void label2_Click_1(object sender, EventArgs e)
        {
            fastColoredTextBox1.Clear();
            Titulo.Text = "Sin Titulo*";
            label2.Visible = false;
            archivo = null;
        }

        private void bUSCARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowFindDialog();
        }

        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

        }


        //METODO PARA HORA Y FECHA ACTUAL ----------------------------------------------------------
        private void tmFechaHora_Tick(object sender, EventArgs e)
        {
            lbFecha.Text = DateTime.Now.ToLongDateString();
            lblHora.Text = DateTime.Now.ToString("HH:mm:ssss");
        }
        


        

    }
}
