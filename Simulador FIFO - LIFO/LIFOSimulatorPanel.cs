using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulador_FIFO___LIFO
{
        internal class LIFOSimulatorPanel : Panel
        {
           
            private Stack<string> stack; 
            private List<StackElement> visualElements; 

         
            private System.Windows.Forms.Timer animationTimer;
            private System.Windows.Forms.Timer loadTimer;

            private StackElement animatingElement;
            private bool isPushAnimation;
            private bool isLoadingExample;

            private Queue<string> pendingItems;

           
            private const int element_width = 150;
            private const int element_height = 46;
            private const int element_spacing = 10;
            private const int max_elements = 8;

           
            private DoubleBufferedPanel Canvas;
            private TextBox TxtValor;
            private Button BtnAgregar;
            private Button BtnEliminar;
            private Button BtnElegir;
            private Button BtnEjemplo;
            private Label LblEstado;
            private Label LblCount;

            public LIFOSimulatorPanel()
            {
                stack = new Stack<string>();
                visualElements = new List<StackElement>();

                pendingItems = new Queue<string>();

                animationTimer = new System.Windows.Forms.Timer();
                animationTimer.Interval = 20;
                animationTimer.Tick += AnimationTimer_Tick;

                loadTimer = new System.Windows.Forms.Timer();
                loadTimer.Interval = 500;
                loadTimer.Tick += LoadTimer_Tick;

                SetupUI();
            }

       
            private void SetupUI()
            {
                this.BackColor = Color.FromArgb(18, 18, 18);
                this.Dock = DockStyle.Fill;

              
                Label title = new Label();
                title.Text = "PILA LIFO";
                title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(0, 188, 212);
                title.AutoSize = true;
                title.Location = new Point(20, 15);
                this.Controls.Add(title);

                Label subtitle = new Label();
                subtitle.Text = "El último que entra es el primero que sale";
                subtitle.Font = new Font("Segoe UI", 10F);
                subtitle.ForeColor = Color.FromArgb(150, 150, 150);
                subtitle.AutoSize = true;
                subtitle.Location = new Point(20, 50);
                this.Controls.Add(subtitle);

                
                Panel infoPanel = new Panel();
                infoPanel.Location = new Point(600, 15);
                infoPanel.Size = new Size(260, 110);
                infoPanel.BackColor = Color.FromArgb(30, 30, 30);
                infoPanel.BorderStyle = BorderStyle.FixedSingle;
                this.Controls.Add(infoPanel);

                string[] infoText =
                {
                "• Visualización vertical",
                "• Animación caída desde arriba ↓",
                "• Animación salida hacia arriba ↑",
                "• Operaciones: Push, Pop, Peek"
            };

                for (int i = 0; i < infoText.Length; i++)
                {
                    Label lbl = new Label();
                    lbl.Text = infoText[i];
                    lbl.ForeColor = Color.Gainsboro;
                    lbl.Font = new Font("Segoe UI", 9.5F);
                    lbl.AutoSize = true;
                    lbl.Location = new Point(12, 12 + (i * 22));
                    infoPanel.Controls.Add(lbl);
                }

               
                Canvas = new DoubleBufferedPanel();
                Canvas.Location = new Point(20, 140);
                Canvas.Size = new Size(840, 360);
                Canvas.BackColor = Color.FromArgb(30, 30, 30);
                Canvas.Paint += Canvas_Paint;
                this.Controls.Add(Canvas);

               
                Label topLabel = new Label();
                topLabel.Text = "TOP (Sale)";
                topLabel.ForeColor = Color.FromArgb(255, 100, 100);
                topLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                topLabel.AutoSize = true;
                topLabel.Location = new Point(20, 505);
                this.Controls.Add(topLabel);

                
                Label baseLabel = new Label();
                baseLabel.Text = "BASE (Entra)";
                baseLabel.ForeColor = Color.FromArgb(100, 255, 100);
                baseLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                baseLabel.AutoSize = true;
                baseLabel.Location = new Point(760, 505);
                this.Controls.Add(baseLabel);

                
                TxtValor = new TextBox();
                TxtValor.Location = new Point(20, 535);
                TxtValor.Size = new Size(180, 36);
                TxtValor.Font = new Font("Segoe UI", 12F);
                TxtValor.BackColor = Color.FromArgb(45, 45, 45);
                TxtValor.ForeColor = Color.White;
                TxtValor.BorderStyle = BorderStyle.FixedSingle;
                this.Controls.Add(TxtValor);

              

             
                // BOTÓN AGREGAR
            
                BtnAgregar = new Button();
                BtnAgregar.Text = "Agregar +";
                BtnAgregar.Location = new Point(215, 535);
                BtnAgregar.Size = new Size(110, 36);
                BtnAgregar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                BtnAgregar.BackColor = Color.FromArgb(0, 100, 0);
                BtnAgregar.ForeColor = Color.White;
                BtnAgregar.FlatStyle = FlatStyle.Flat;
                BtnAgregar.Click += BtnAgregar_Click;
                this.Controls.Add(BtnAgregar);

              
                // BOTÓN ELIMINAR
               
                BtnEliminar = new Button();
                BtnEliminar.Text = "Eliminar -";
                BtnEliminar.Location = new Point(335, 535);
                BtnEliminar.Size = new Size(110, 36);
                BtnEliminar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                BtnEliminar.BackColor = Color.FromArgb(139, 0, 0);
                BtnEliminar.ForeColor = Color.White;
                BtnEliminar.FlatStyle = FlatStyle.Flat;
                BtnEliminar.Click += BtnEliminar_Click;
                this.Controls.Add(BtnEliminar);

              
                // BOTÓN ELEGIR = PEEK
                
                BtnElegir = new Button();
                BtnElegir.Text = "Elegir";
                BtnElegir.Location = new Point(455, 535);
                BtnElegir.Size = new Size(90, 36);
                BtnElegir.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                BtnElegir.BackColor = Color.FromArgb(180, 140, 0);
                BtnElegir.ForeColor = Color.White;
                BtnElegir.FlatStyle = FlatStyle.Flat;
                BtnElegir.Click += BtnElegir_Click;
                this.Controls.Add(BtnElegir);

              
                // BOTÓN VER EJEMPLO
              
                BtnEjemplo = new Button();
                BtnEjemplo.Text = "Ver Ejemplo";
                BtnEjemplo.Location = new Point(555, 535);
                BtnEjemplo.Size = new Size(140, 36);
                BtnEjemplo.Font = new Font("Segoe UI", 10F);
                BtnEjemplo.BackColor = Color.FromArgb(50, 50, 150);
                BtnEjemplo.ForeColor = Color.White;
                BtnEjemplo.FlatStyle = FlatStyle.Flat;
                BtnEjemplo.Click += BtnEjemplo_Click;
                this.Controls.Add(BtnEjemplo);

               
                // LABEL ESTADO
                
                LblEstado = new Label();
                LblEstado.Text = "Ingrese un valor y dé click en agregar";
                LblEstado.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
                LblEstado.ForeColor = Color.FromArgb(180, 180, 180);
                LblEstado.AutoSize = true;
                LblEstado.Location = new Point(20, 590);
                this.Controls.Add(LblEstado);

                
                // LABEL CONTADOR
               
                LblCount = new Label();
                LblCount.Text = "Total: 0";
                LblCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                LblCount.ForeColor = Color.Gold;
                LblCount.AutoSize = true;
                LblCount.Location = new Point(760, 590);
                this.Controls.Add(LblCount);
            }

         
            // BOTÓN AGREGAR = PUSH
           
            private void BtnAgregar_Click(object sender, EventArgs e)
            {
               
            }

            // BOTÓN ELIMINAR = POP
            
            private void BtnEliminar_Click(object sender, EventArgs e)
            {
                
            }

     
            // BOTÓN ELEGIR = PEEK
            
            private void BtnElegir_Click(object sender, EventArgs e)
            {
                
            }

            // BOTÓN VER EJEMPLO
           
            private void BtnEjemplo_Click(object sender, EventArgs e)
            {
                
            }

          
            // TIMER DE CARGA DE EJEMPLOS
         
            private void LoadTimer_Tick(object sender, EventArgs e)
            {
                if (pendingItems.Count > 0 && animatingElement == null)
                {
                    string nextItem = pendingItems.Dequeue();
                    stack.Push(nextItem);

                    int targetX = Canvas.Width / 2 - element_width / 2;
                    int targetY = Canvas.Height - 25 - element_height -
                                  (visualElements.Count * (element_height + element_spacing));

                    animatingElement = new StackElement
                    {
                        Value = nextItem,
                        X = targetX,
                        Y = -element_height - 30,
                        TargetX = targetX,
                        TargetY = targetY,
                        Highlight = false
                    };

                    isPushAnimation = true;
                    animationTimer.Start();
                    ActualizarContador();
                }

                if (pendingItems.Count == 0 && animatingElement == null)
                {
                    loadTimer.Stop();
                    isLoadingExample = false;
                    LblEstado.Text = "Ejemplo cargado. Presiona Eliminar para ver la dinámica LIFO.";
                }
            }

         
            // TIMER DE ANIMACIÓN
         
            private void AnimationTimer_Tick(object sender, EventArgs e)
            {
                if (animatingElement == null)
                    return;

                animatingElement.X += (animatingElement.TargetX - animatingElement.X) * 0.18f;
                animatingElement.Y += (animatingElement.TargetY - animatingElement.Y) * 0.18f;

                if (Math.Abs(animatingElement.X - animatingElement.TargetX) < 1 &&
                    Math.Abs(animatingElement.Y - animatingElement.TargetY) < 1)
                {
                    animationTimer.Stop();

                    if (isPushAnimation)
                        visualElements.Add(animatingElement);

                    animatingElement = null;
                }

                Canvas.Invalidate();
            }

           
            // DIBUJO
           
            private void Canvas_Paint(object sender, PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                DrawStackGuide(g);

                for (int i = 0; i < visualElements.Count; i++)
                    DrawStackElement(g, visualElements[i], i);

                if (animatingElement != null)
                    DrawStackElement(g, animatingElement, -1);
            }

            private void DrawStackGuide(Graphics g)
            {
                int centerX = Canvas.Width / 2;
                int stackWidth = element_width + 20;

                Rectangle area = new Rectangle(centerX - stackWidth / 2, 20, stackWidth, Canvas.Height - 40);

                using (Pen pen = new Pen(Color.FromArgb(90, 90, 90), 2))
                {
                    g.DrawLine(pen, area.Left, area.Top, area.Left, area.Bottom);
                    g.DrawLine(pen, area.Right, area.Top, area.Right, area.Bottom);
                    g.DrawLine(pen, area.Left, area.Bottom, area.Right, area.Bottom);
                }
            }

            private void DrawStackElement(Graphics g, StackElement el, int index)
            {
                Rectangle rect = new Rectangle((int)el.X, (int)el.Y, element_width, element_height);

                using (var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    GraphicsPath shadowPath = GetRoundedRectangle(
                        new Rectangle(rect.X + 3, rect.Y + 3, rect.Width, rect.Height), 10);
                    g.FillPath(shadow, shadowPath);
                }

                using (var brush = new LinearGradientBrush(
                    rect,
                    Color.FromArgb(0, 63, 127),
                    Color.FromArgb(0, 31, 63),
                    LinearGradientMode.Vertical))
                {
                    GraphicsPath fillPath = GetRoundedRectangle(rect, 10);
                    g.FillPath(brush, fillPath);
                }

                Color borderColor;

                if (el.Highlight)
                    borderColor = Color.FromArgb(253, 185, 19);
                else if (index == visualElements.Count - 1)
                    borderColor = Color.FromArgb(255, 100, 100);
                else
                    borderColor = Color.FromArgb(0, 150, 200);

                using (var pen = new Pen(borderColor, 2.5f))
                {
                    GraphicsPath borderPath = GetRoundedRectangle(rect, 10);
                    g.DrawPath(pen, borderPath);
                }

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(
                    el.Value,
                    new Font("Segoe UI", 12F, FontStyle.Bold),
                    Brushes.White,
                    rect,
                    sf);

                if (index >= 0)
                {
                    g.DrawString(
                        "[" + index.ToString() + "]",
                        new Font("Segoe UI", 8F),
                        new SolidBrush(Color.FromArgb(150, 150, 150)),
                        new Point(rect.Right + 8, rect.Y + 14));
                }
            }

            private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();

                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                return path;
            }

            private void LimpiarResaltado()
            {
                foreach (var item in visualElements)
                    item.Highlight = false;
            }

            private void ActualizarContador()
            {
                LblCount.Text = "Total: " + stack.Count;
            }

        
            private class StackElement
            {
                public string Value { get; set; }
                public float X { get; set; }
                public float Y { get; set; }
                public float TargetX { get; set; }
                public float TargetY { get; set; }
                public bool Highlight { get; set; }
            }
            private class DoubleBufferedPanel : Panel
            {
                public DoubleBufferedPanel()
                {
                    this.DoubleBuffered = true;
                    this.ResizeRedraw = true;
                }
            }
        }
}
