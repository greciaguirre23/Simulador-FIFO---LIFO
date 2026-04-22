using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Simulador_FIFO___LIFO
{
    internal class FIFOSimulatorPanel : Panel
    {
        private Queue<string> queue;
        private List<QueueElement> visualElements;

        private System.Windows.Forms.Timer animationTimer;
        private QueueElement animatingElement;
        private bool isEnqueueAnimation;

        private Queue<string> pendingItems;
        private System.Windows.Forms.Timer loadTimer;
        private bool isLoadingExample;

        private const int element_width = 100;
        private const int element_height = 50;
        private const int element_spacing = 20;

        private Panel Panel;
        private TextBox TxtValor;
        private Button BtnAgregar;
        private Button BtnEliminar;
        private Button BtnVer;
        private Button BtnEjemplo;
        private Label LblEstado;
        private Label LblCount;

        public FIFOSimulatorPanel()
        {
            queue = new Queue<string>();
            visualElements = new List<QueueElement>();

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 20;
            animationTimer.Tick += AnimationTimer_Tick;

            pendingItems = new Queue<string>();
            loadTimer = new System.Windows.Forms.Timer();
            loadTimer.Interval = 500;
            loadTimer.Tick += LoadTimer_Tick;

            SetupUI();
        }

        private void SetupUI()
        {
            this.BackColor = Color.FromArgb(247, 248, 245);

            Label title = new Label();
            title.Text = "COLA FIFO";
            title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(44, 62, 45);
            title.AutoSize = true;
            title.Location = new Point(20, 15);
            this.Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "El primero que entra es el primero que sale";
            subtitle.Font = new Font("Segoe UI", 10F);
            subtitle.ForeColor = Color.FromArgb(136, 136, 136);
            subtitle.AutoSize = true;
            subtitle.Location = new Point(20, 50);
            this.Controls.Add(subtitle);

            Panel = new Panel();
            Panel.Location = new Point(20, 90);
            Panel.Size = new Size(820, 160);
            Panel.BackColor = Color.FromArgb(210, 220, 205);
            Panel.Paint += Panel_Paint;
            this.Controls.Add(Panel);

            Label frontLabel = new Label();
            frontLabel.Text = "FRONT (Sale)";
            frontLabel.ForeColor = Color.FromArgb(139, 69, 19);
            frontLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            frontLabel.AutoSize = true;
            frontLabel.Location = new Point(20, 258);
            this.Controls.Add(frontLabel);

            Label rearLabel = new Label();
            rearLabel.Text = "REAR (Entra) ";
            rearLabel.ForeColor = Color.FromArgb(58, 90, 64);
            rearLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            rearLabel.AutoSize = true;
            rearLabel.Location = new Point(680, 258);
            this.Controls.Add(rearLabel);

            TxtValor = new TextBox();
            TxtValor.Location = new Point(20, 290);
            TxtValor.Size = new Size(180, 36);
            TxtValor.Font = new Font("Segoe UI", 12F);
            TxtValor.BackColor = Color.FromArgb(247, 248, 245);
            TxtValor.ForeColor = Color.FromArgb(44, 62, 45);
            TxtValor.BorderStyle = BorderStyle.FixedSingle;
            TxtValor.PlaceholderText = "Escribe un valor...";
            this.Controls.Add(TxtValor);

            
            BtnAgregar = new Button();
            BtnAgregar.Text = "Agregar +";
            BtnAgregar.Location = new Point(215, 290);
            BtnAgregar.Size = new Size(110, 36);
            BtnAgregar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            BtnAgregar.BackColor = Color.FromArgb(58, 90, 64);
            BtnAgregar.ForeColor = Color.White;
            BtnAgregar.FlatStyle = FlatStyle.Flat;
            BtnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(BtnAgregar);

            
            BtnEliminar = new Button();
            BtnEliminar.Text = "Eliminar -";
            BtnEliminar.Location = new Point(335, 290);
            BtnEliminar.Size = new Size(110, 36);
            BtnEliminar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            BtnEliminar.BackColor = Color.FromArgb(139, 69, 19);
            BtnEliminar.ForeColor = Color.White;
            BtnEliminar.FlatStyle = FlatStyle.Flat;
            BtnEliminar.Click += BtnEliminar_Click;
            this.Controls.Add(BtnEliminar);

            
            BtnVer = new Button();
            BtnVer.Text = "Seleccionar";
            BtnVer.Location = new Point(455, 290);
            BtnVer.Size = new Size(90, 36);
            BtnVer.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            BtnVer.BackColor = Color.FromArgb(163, 184, 153); ;
            BtnVer.ForeColor = Color.FromArgb(44, 62, 45);
            BtnVer.FlatStyle = FlatStyle.Flat;
            BtnVer.Click += BtnVer_Click;
            this.Controls.Add(BtnVer);

            BtnEjemplo = new Button();
            BtnEjemplo.Text = "Ver Ejemplo";
            BtnEjemplo.Location = new Point(555, 290);
            BtnEjemplo.Size = new Size(140, 36);
            BtnEjemplo.Font = new Font("Segoe UI", 10F);
            BtnEjemplo.BackColor = Color.FromArgb(232, 237, 230);
            BtnEjemplo.ForeColor = Color.FromArgb(58, 90, 64);
            BtnEjemplo.FlatStyle = FlatStyle.Flat;
            BtnEjemplo.Click += BtnEjemplo_Click;
            this.Controls.Add(BtnEjemplo);

            LblEstado = new Label();
            LblEstado.Text = "Ingrese un valor y de click en agregar";
            LblEstado.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            LblEstado.ForeColor = Color.FromArgb(136, 136, 136);
            LblEstado.AutoSize = true;
            LblEstado.Location = new Point(20, 345);
            this.Controls.Add(LblEstado);

            
            LblCount = new Label();
            LblCount.Text = "Total: 0";
            LblCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            LblCount.ForeColor = Color.Gold;
            LblCount.AutoSize = true;
            LblCount.Location = new Point(760, 345);
            this.Controls.Add(LblCount);
        }

     
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtValor.Text))
            {
                LblEstado.Text = "Error: Ingrese un valor primero";
                return;
            }

            if (visualElements.Count >= 8)
            {
                LblEstado.Text = "Error: Cola llena (maximo 8 elementos)";
                return;
            }

            string value = TxtValor.Text.Trim();
            queue.Enqueue(value);
            TxtValor.Clear();

            int targetX = 100 + visualElements.Count * (element_width + element_spacing);
            int targetY = Panel.Height / 2 - element_height / 2;

            animatingElement = new QueueElement
            {
                Value = value,
                X = Panel.Width,
                Y = targetY,
                TargetX = targetX,
                TargetY = targetY
            };

            isEnqueueAnimation = true;
            animationTimer.Start();

            LblEstado.Text = "Enqueue(" + value + ") - Agregado al REAR. Total: " + queue.Count;
            ActualizarContador();
        }

     
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (queue.Count == 0)
            {
                LblEstado.Text = "Error: La cola esta vacia";
                return;
            }

            string removed = queue.Dequeue();
            animatingElement = visualElements[0];
            visualElements.RemoveAt(0);

            for (int i = 0; i < visualElements.Count; i++)
                visualElements[i].TargetX = 100 + i * (element_width + element_spacing);

            animatingElement.TargetX = -element_width - 50;
            isEnqueueAnimation = false;
            animationTimer.Start();

            LblEstado.Text = "Dequeue() - " + removed + " salio del FRONT. Total: " + queue.Count;
            ActualizarContador();
        }

        // BOTON SELECCIONAR = PEEK
        private void BtnVer_Click(object sender, EventArgs e)
        {
            if (queue.Count == 0)
            {
                LblEstado.Text = "Error: La cola esta vacia";
                return;
            }
            string front = queue.Peek();
            LblEstado.Text = "Peek() - " + front + " esta en el FRONT (no se elimino)";

            Panel.Invalidate();
        }

        // BOTON VER 
        private void BtnEjemplo_Click(object sender, EventArgs e)
        {
            if (isLoadingExample) return;

            queue.Clear();
            visualElements.Clear();
            Panel.Invalidate();

            string[] ejemplos = { "A55", "A56", "A192", "B67", "B90" };
            pendingItems.Clear();
            foreach (string item in ejemplos)
                pendingItems.Enqueue(item);

            isLoadingExample = true;
            loadTimer.Start();
            LblEstado.Text = "Cargando ejemplo...";
        }

        // TIMER DE CARGA 
        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            if (pendingItems.Count > 0 && animatingElement == null)
            {
                string nextItem = pendingItems.Dequeue();
                queue.Enqueue(nextItem);

                int targetX = 100 + visualElements.Count * (element_width + element_spacing);
                int targetY = Panel.Height / 2 - element_height / 2;

                animatingElement = new QueueElement
                {
                    Value = nextItem,
                    X = Panel.Width,
                    Y = targetY,
                    TargetX = targetX,
                    TargetY = targetY
                };

                isEnqueueAnimation = true;
                animationTimer.Start();
            }

            if (pendingItems.Count == 0 && animatingElement == null)
            {
                loadTimer.Stop();
                isLoadingExample = false;
                LblEstado.Text = "Ejemplo cargado. Presiona Eliminar para ver la dinamica FIFO";
                ActualizarContador();
            }
        }

        // TIMER DE ANIMACION
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (animatingElement == null) return;

            animatingElement.X += (animatingElement.TargetX - animatingElement.X) * 0.15f;
            animatingElement.Y += (animatingElement.TargetY - animatingElement.Y) * 0.15f;

            foreach (var el in visualElements)
            {
                el.X += (el.TargetX - el.X) * 0.15f;
                el.Y += (el.TargetY - el.Y) * 0.15f;
            }

            if (Math.Abs(animatingElement.X - animatingElement.TargetX) < 1 &&
                Math.Abs(animatingElement.Y - animatingElement.TargetY) < 1)
            {
                animationTimer.Stop();

                if (isEnqueueAnimation)
                    visualElements.Add(animatingElement);

                animatingElement = null;
            }

            Panel.Invalidate();
        }

        // DIBUJO
        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < visualElements.Count; i++)
                DrawQueueElement(g, visualElements[i], i);

            if (animatingElement != null)
                DrawQueueElement(g, animatingElement, -1);
        }

        private void DrawQueueElement(Graphics g, QueueElement el, int index)
        {
            Rectangle rect = new Rectangle((int)el.X, (int)el.Y, element_width, element_height);

            using (var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                g.FillPath(shadow, GetRoundedRectangle(
                    new Rectangle(rect.X + 3, rect.Y + 3, element_width, element_height), 10));

            using (var brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(58, 90, 64),  
                    Color.FromArgb(44, 62, 45),
                LinearGradientMode.Vertical))
                g.FillPath(brush, GetRoundedRectangle(rect, 10));

            Color borderColor = (index == 0)
                ? Color.FromArgb(163, 184, 153) 
                : Color.FromArgb(216, 224, 212);
            using (var pen = new Pen(borderColor, 2.5f))
                g.DrawPath(pen, GetRoundedRectangle(rect, 10));

            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(el.Value,
                new Font("Segoe UI", 13F, FontStyle.Bold),
                Brushes.White, rect, sf);

            if (index >= 0)
            {
                g.DrawString("[" + index.ToString() + "]",
                    new Font("Segoe UI", 8F),
                    new SolidBrush(Color.FromArgb(136, 136, 136)),
                    new Point(rect.X + element_width / 2 - 10, rect.Bottom + 4));
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

        private void ActualizarContador()
        {
            LblCount.Text = "Total: " + queue.Count;
        }

        private class QueueElement
        {
            public string Value { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float TargetX { get; set; }
            public float TargetY { get; set; }
        }
    }
}