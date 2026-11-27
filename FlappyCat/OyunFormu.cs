using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace FlappyCat
{
    /// <summary>
    /// Oyun penceresi. Şimdilik Timer ile çalışan döngüde arka planı,
    /// kayan bulutları ve bulut zemini çizer.
    /// </summary>
    public class OyunFormu : Form
    {
        private readonly Timer zamanlayici;
        private readonly Stopwatch kronometre = new Stopwatch();
        private readonly Random rastgele = new Random();
        private readonly List<Bulut> bulutlar = new List<Bulut>();
        private readonly Zemin zemin = new Zemin();
        private float zaman;

        public OyunFormu()
        {
            Text = "Flappy Cat";
            ClientSize = new Size(Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                Icon = new Icon(Path.Combine(Varliklar.Klasor, "ikon.ico"));
            }
            catch (Exception ex) when (ex is IOException || ex is ArgumentException)
            {
                // Simge yüklenemezse varsayılan simge kullanılır.
            }

            for (int i = 0; i < 5; i++)
            {
                Bitmap b = Varliklar.Bulutlar[rastgele.Next(Varliklar.Bulutlar.Length)];
                bulutlar.Add(new Bulut(b, 0.55f + (float)rastgele.NextDouble() * 0.35f, 0.75f,
                    0.25f + (float)rastgele.NextDouble() * 0.35f,
                    rastgele.Next(-40, Ayarlar.EkranGenislik), rastgele.Next(40, 430), rastgele));
            }
            zemin.Hiz = 1.5f;

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

            zaman += dt;
            foreach (Bulut b in bulutlar) b.Guncelle(dt);
            zemin.Guncelle(dt);
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Arka planı OnPaint içinde çiziyoruz (titremeyi önler).
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            g.DrawImage(Varliklar.Arkaplan, 0, 0, Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
            foreach (Bulut b in bulutlar) b.Ciz(g);
            zemin.Ciz(g);

            float orta = Ayarlar.EkranGenislik / 2f;
            float sallanma = (float)Math.Sin(zaman * 0.05f) * 4f;
            Cizim.GokkusagiYazi(g, "FLAPPY CAT", 54f, new PointF(orta, 200 + sallanma), 9f);

            Bitmap kedi = Varliklar.SecimGorselleri[KarakterTuru.Gri];
            g.DrawImage(kedi, orta - kedi.Width / 2f, 300 + sallanma * 2, kedi.Width, kedi.Height);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            zamanlayici.Stop();
            base.OnFormClosing(e);
        }
    }
}
