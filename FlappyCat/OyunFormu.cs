using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace FlappyCat
{
    public class OyunFormu : Form
    {
        private readonly Timer zamanlayici;
        private readonly Stopwatch kronometre = new Stopwatch();
        private readonly SesYoneticisi ses;
        private readonly OyunYoneticisi oyun;
        private readonly EkranArayuzu arayuz;
        private bool ziplamaTusuBasili;

        public OyunFormu()
        {
            Text = "Flappy Cat";
            ClientSize = new Size(Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                Icon = new Icon(Path.Combine(Varliklar.Klasor, "ikon.ico"));
            }
            catch (Exception ex) when (ex is IOException || ex is ArgumentException)
            {
            }

            ses = new SesYoneticisi(Path.Combine(Varliklar.Klasor, "Sounds"));
            oyun = new OyunYoneticisi(ses, new SkorDeposu());
            oyun.SkorDegisti += Oyun_SkorDegisti;
            arayuz = new EkranArayuzu(oyun);

            zamanlayici = new Timer { Interval = 15 };
            zamanlayici.Tick += Zamanlayici_Tick;
            kronometre.Start();
            zamanlayici.Start();
        }

        private void Zamanlayici_Tick(object sender, EventArgs e)
        {
            double ms = kronometre.Elapsed.TotalMilliseconds;
            kronometre.Restart();
            float dt = (float)Math.Min(ms / (1000.0 / 60.0), 3.0);

            oyun.Guncelle(dt);
            Invalidate();
        }

        private void Oyun_SkorDegisti(object sender, EventArgs e)
        {
            Text = "Flappy Cat  -  Skor: " + oyun.Skor;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            oyun.DunyayiCiz(g);
            arayuz.Ciz(g);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Space:
                case Keys.Enter:
                case Keys.Escape:
                    e.IsInputKey = true;
                    break;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (ZiplamaTusu(e.KeyCode))
            {
                if (ziplamaTusuBasili) return;
                ziplamaTusuBasili = true;
            }

            arayuz.TusBasildi(e.KeyCode);
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (ZiplamaTusu(e.KeyCode)) ziplamaTusuBasili = false;
        }

        private static bool ZiplamaTusu(Keys tus)
        {
            return tus == Keys.Space || tus == Keys.Up || tus == Keys.W;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) arayuz.FareTiklandi(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor istenen = arayuz.FareHareket(e.Location) ? Cursors.Hand : Cursors.Default;
            if (Cursor != istenen) Cursor = istenen;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (oyun.Durum == OyunDurumu.Oynuyor) oyun.Duraklat();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            zamanlayici.Stop();
            ses.Dispose();
            base.OnFormClosing(e);
        }
    }
}
