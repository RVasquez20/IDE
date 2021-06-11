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



        internal static string variableCom = "";
        //Iniciar form de DOM
        Form dom=new DOM();
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
            if (Titulo.Text.Contains("*")) {
                if (MessageBox.Show("¿Está seguro de cerrar?", "Alerta", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (MessageBox.Show("¿Desea Guardar antes de salir?", "Alerta", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        save();
                        Application.Exit();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            else
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
            if (Titulo.Text.Contains("*"))
            {
                DialogResult dialogo = MessageBox.Show("¿Desea Guardar los Cambios?", "Guardar Cambios",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogo == DialogResult.No)
                {
                    label2.Visible = false;
                    fastColoredTextBox1.Clear();
                    Titulo.Text = "Sin Titulo*";
                    archivo = null;
                }
                else
                {
                    save();
                    label2.Visible = false;
                    Titulo.Text = "Sin Titulo*";
                    fastColoredTextBox1.Clear();
                    archivo = null;
                }
            }
            else
            {
                label2.Visible = false;
                Titulo.Text = "Sin Titulo*";
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
            if (h != null)
            {


                h.Close();

                }
            if (dom!=null)
            {
                dom.Close();
            }
            

        }

        private void bUSCARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowFindDialog();
        }

        private void iMPRIMIRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(fastColoredTextBox1.Text, new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new PointF(100, 100));
        }

        private void fastColoredTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            int cont_abre_etiqueta = 0;
            int cont_cierra_etiqueta = 0;
            string texto = fastColoredTextBox1.Text;
            char[] array = texto.ToCharArray();//separa en letras
            string error = "Cierre todas las etiquetas";
            string[] stringSeparators = new string[] { " ", "<", ">" };
            string[] array2 = texto.Split(stringSeparators, StringSplitOptions.None);//separa las palabras
            string busc = "";

            if (e.KeyCode == Keys.Up)
            {
                foreach (var p in array2)
                {
                    busc = p;
                    for (int i = 0; i < Reservadas.Length; i++)//para comparar cuando se abre la etiqueta
                    {
                        if (busc == Reservadas[i].ToString())
                        {
                            cont_abre_etiqueta++;
                            int a = fastColoredTextBox1.Text.IndexOf(busc);


                        }
                    }
                    for (int j = 0; j < Cierre_Reservadas.Length; j++)//para comparar cuando se cierra la etiqueta
                    {
                        if (busc == Cierre_Reservadas[j].ToString())
                        {
                            cont_cierra_etiqueta++;
                        }
                    }
                    busc = "";
                }

                if (cont_abre_etiqueta > 0 && cont_cierra_etiqueta > 0)
                {
                    if (cont_abre_etiqueta == cont_cierra_etiqueta)
                    {
                        variableCom = fastColoredTextBox1.Text;
                        if (dom != null)
                        {
                            if (Application.OpenForms["DOM"] == null)
                            {

                                if (!dom.IsDisposed)
                                {
                                    save();
                                    dom.Refresh();
                                    dom.Activate();
                                    dom.Show();
                                    this.Focus();
                                }
                                else
                                {
                                    save();
                                    dom = new DOM();
                                    dom.Refresh();
                                    dom.Activate();
                                    dom.Show();
                                    this.Focus();
                                }
                            }
                            else
                            {
                                save();
                                dom.Refresh();
                                dom.Activate();
                                dom.Show();
                                this.Focus();
                            }
                        }
                        else
                        {
                            save();
                            dom = new DOM();
                            dom.Refresh();
                            dom.Activate();
                            dom.Show();
                            this.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show(error);
                    }
                }
            }

        }

        private void fastColoredTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            int cont_abre_etiqueta = 0;
            int cont_cierra_etiqueta = 0;
            string texto = fastColoredTextBox1.Text;
            char[] array = texto.ToCharArray();//separa en letras
            string error = "Cierre todas las etiquetas";
            string[] stringSeparators = new string[] { " ", "<", ">" };
            string[] array2 = texto.Split(stringSeparators, StringSplitOptions.None);//separa las palabras
            string busc = "";

            if (e.KeyCode == Keys.Delete)
            {
                foreach (var p in array2)
                {
                    busc = p;
                    for (int i = 0; i < Reservadas.Length; i++)//para comparar cuando se abre la etiqueta
                    {
                        if (busc == Reservadas[i].ToString())
                        {
                            cont_abre_etiqueta++;
                            int a = fastColoredTextBox1.Text.IndexOf(busc);


                        }
                    }
                    for (int j = 0; j < Cierre_Reservadas.Length; j++)//para comparar cuando se cierra la etiqueta
                    {
                        if (busc == Cierre_Reservadas[j].ToString())
                        {
                            cont_cierra_etiqueta++;
                        }
                    }
                    busc = "";
                }

                if (cont_abre_etiqueta > 0 && cont_cierra_etiqueta > 0)
                {
                    if (cont_abre_etiqueta == cont_cierra_etiqueta)
                    {
                        variableCom = fastColoredTextBox1.Text;
                        if (dom != null)
                        {
                            if (Application.OpenForms["DOM"] == null)
                            {

                                if (!dom.IsDisposed)
                                {
                                    save();
                                    dom.Refresh();
                                    dom.Activate();
                                    dom.Show();
                                    this.Focus();
                                }
                                else
                                {
                                    save();
                                    dom = new DOM();
                                    dom.Refresh();
                                    dom.Activate();
                                    dom.Show();
                                    this.Focus();
                                }
                            }
                            else
                            {
                                save();
                                dom.Refresh();
                                dom.Activate();
                                dom.Show();
                                this.Focus();
                            }
                        }
                        else
                        {
                            save();
                            dom = new DOM();
                            dom.Refresh();
                            dom.Activate();
                            dom.Show();
                            this.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show(error);
                    }
                }
            }
        }

        private void adelanteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Redo();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Copy();
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Cut();
        }

        private void pEGARToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Paste();
        }

        private void sELECIONARTODOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.SelectAll();
        }

        private void eLIMINARTODOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Clear();
        }

        private void rEEMPLAZARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowReplaceDialog();
        }

        private void fUENTEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formato = fontDialog1.ShowDialog();
            if (formato == DialogResult.OK)
            {
                fastColoredTextBox1.Font = fontDialog1.Font;
            }
        }

        private void iRAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowGoToDialog();
            
            
        }

      

      
        public Navegador h;
        private void pppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
            if (h == null)
            {
                string content = "";
                content = fastColoredTextBox1.Text;
                if (content.Equals(""))
                {
                    MessageBox.Show("No hay nada que mostrar escriba codiog para mostrar la web");
                }
                else
                {
                    h = new Navegador(content);
                    this.Invoke(new Action(() => { h.Refresh(); }));
                    h.Titulo.Text = Titulo.Text;
                    h.Show();

                }
            }
            else
            {
                if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.HTML)
                {

                    if (Application.OpenForms["Navegador"] == null && !fastColoredTextBox1.Text.Equals(""))
                    {
                        if (!h.IsDisposed)
                        {
                            h.Actualizar(fastColoredTextBox1.Text);
                            this.Invoke(new Action(() => { h.Refresh(); }));
                            h.Titulo.Text = Titulo.Text;
                            h.Show();
                            h.FormClosed += Logout;
                        }
                        else
                        {
                            h = new Navegador(fastColoredTextBox1.Text);
                            h.Titulo.Text = Titulo.Text;
                            h.Show();
                            this.Invoke(new Action(() => { h.Refresh(); }));
                            h.FormClosed += Logout;
                        }
                    }
                    else
                    {

                        h.Actualizar(fastColoredTextBox1.Text);
                        this.Invoke(new Action(() => { h.Refresh(); }));
                        h.Titulo.Text = Titulo.Text;
                        h.FormClosed += Logout;

                    }
                }
                else
                {
                    MessageBox.Show("NO SE PUEDE EJECUTAR");
                }
            }
        }
        private void Logout(object sender, FormClosedEventArgs e)
        {

            this.Focus();

        }

        private void aYUDAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Ayuda"] == null)
            {
                Ayuda MenuDeAyuda = new Ayuda();
                MenuDeAyuda.Show();
            }
        }

        private void AtrasoolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Undo();
        }

        bool div = false;

        private void gUARDARCOMOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "Texto|*.HTML";
            if (SaveFile.ShowDialog() == DialogResult.OK)
            {
                archivo = SaveFile.FileName;
                using (StreamWriter sw = new StreamWriter(SaveFile.FileName))
                {
                    sw.Write(fastColoredTextBox1.Text);
                }
            }
        }

        private void fastColoredTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == Convert.ToString("/"))
            {
                div = true;
            }
            if (e.KeyChar.ToString() == Convert.ToString(">") && div == true)
            {
                div = false;
            }
        }

       

       

        //METODO PARA HORA Y FECHA ACTUAL ----------------------------------------------------------
        private void tmFechaHora_Tick(object sender, EventArgs e)
        {
            lbFecha.Text = DateTime.Now.ToLongDateString();
            lblHora.Text = DateTime.Now.ToString("HH:mm:ssss");

        }
        


        

    }
}
