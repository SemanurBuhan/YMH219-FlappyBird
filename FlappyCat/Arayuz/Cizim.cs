using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FlappyCat
{
    /// <summary>
    /// Konturlu yazı, yuvarlak köşeli kart ve saydam görsel gibi ortak çizim işlemleri.
    /// </summary>
    public static class Cizim
    {
        private static FontFamily yaziAilesi;
        private static FontStyle yaziStili;

        public static FontFamily YaziAilesi
        {
            get
            {
                if (yaziAilesi == null) YaziAilesiniBul();
                return yaziAilesi;
            }
        }

        private static void YaziAilesiniBul()
        {
            string[] adaylar = { "Segoe UI Black", "Arial Black", "Segoe UI", "Arial" };
            foreach (string ad in adaylar)
            {
                try
                {
                    var aile = new FontFamily(ad);
                    yaziAilesi = aile;
                    yaziStili = ad.Contains("Black") || !aile.IsStyleAvailable(FontStyle.Bold)
                        ? FontStyle.Regular : FontStyle.Bold;
                    return;
                }
                catch (ArgumentException)
                {
                    // Bu yazı tipi bilgisayarda yok, sıradakini dene.
                }
            }
            yaziAilesi = FontFamily.GenericSansSerif;
            yaziStili = FontStyle.Bold;
        }

        /// <summary>Metni yol (path) olarak oluşturur; hiza: 0 = sol, 0.5 = orta, 1 = sağ.</summary>
        private static GraphicsPath YaziYolu(string metin, float boyut, PointF nokta, float hiza)
        {
            var yol = new GraphicsPath();
            yol.AddString(metin, YaziAilesi, (int)yaziStili, boyut, PointF.Empty, StringFormat.GenericTypographic);
            RectangleF b = yol.GetBounds();
            using (var m = new Matrix())
            {
                m.Translate(nokta.X - b.X - b.Width * hiza, nokta.Y - b.Y - b.Height / 2f);
                yol.Transform(m);
            }
            return yol;
        }

        /// <summary>Kalın kenarlıklı (tasarımdaki gibi) yazı çizer. nokta.Y yazının dikey ortasıdır.</summary>
        public static void Yazi(Graphics g, string metin, float boyut, PointF nokta, Color dolgu,
                                float konturKalinligi = 6f, float hiza = 0.5f, int saydamlik = 255)
        {
            using (GraphicsPath yol = YaziYolu(metin, boyut, nokta, hiza))
            using (var firca = new SolidBrush(Color.FromArgb(saydamlik * dolgu.A / 255, dolgu)))
            {
                YoluCiz(g, yol, firca, konturKalinligi, saydamlik);
            }
        }

        /// <summary>Gökkuşağı renklerinde dolgulu yazı.</summary>
        public static void GokkusagiYazi(Graphics g, string metin, float boyut, PointF nokta,
                                         float konturKalinligi = 7f, float hiza = 0.5f, int saydamlik = 255)
        {
            using (GraphicsPath yol = YaziYolu(metin, boyut, nokta, hiza))
            {
                RectangleF b = yol.GetBounds();
                using (var firca = new LinearGradientBrush(
                           new PointF(b.Left - 1, 0), new PointF(b.Right + 1, 0), Color.Red, Color.Blue))
                {
                    var karisim = new ColorBlend(Renkler.Gokkusagi.Length);
                    for (int i = 0; i < Renkler.Gokkusagi.Length; i++)
                    {
                        karisim.Colors[i] = Color.FromArgb(saydamlik, Renkler.Gokkusagi[i]);
                        karisim.Positions[i] = i / (float)(Renkler.Gokkusagi.Length - 1);
                    }
                    firca.InterpolationColors = karisim;
                    YoluCiz(g, yol, firca, konturKalinligi, saydamlik);
                }
            }
        }

        private static void YoluCiz(Graphics g, GraphicsPath yol, Brush firca, float kontur, int saydamlik)
        {
            if (kontur > 0)
            {
                using (var kalem = new Pen(Color.FromArgb(saydamlik, Renkler.Kontur), kontur))
                {
                    kalem.LineJoin = LineJoin.Round;
                    g.DrawPath(kalem, yol);
                }
            }
            g.FillPath(firca, yol);
        }

        public static GraphicsPath YuvarlakDikdortgen(RectangleF r, float yaricap)
        {
            float d = yaricap * 2f;
            var yol = new GraphicsPath();
            yol.AddArc(r.X, r.Y, d, d, 180, 90);
            yol.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            yol.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            yol.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            yol.CloseFigure();
            return yol;
        }

        /// <summary>Tasarımdaki butonlar gibi gölgeli, kalın kenarlıklı yuvarlak kart.</summary>
        public static void Kart(Graphics g, RectangleF r, Color dolgu, float yaricap = 16f, float kontur = 4f, bool altSerit = true)
        {
            RectangleF golge = r;
            golge.Offset(0, 5);
            using (GraphicsPath yol = YuvarlakDikdortgen(golge, yaricap))
            using (var firca = new SolidBrush(Renkler.Golge))
            {
                g.FillPath(firca, yol);
            }

            using (GraphicsPath yol = YuvarlakDikdortgen(r, yaricap))
            {
                using (var firca = new SolidBrush(dolgu))
                    g.FillPath(firca, yol);

                // Alt kısımda hafif koyu şerit (tasarımdaki 3B buton hissi)
                if (altSerit)
                {
                    RectangleF alt = new RectangleF(r.X, r.Bottom - r.Height * 0.28f, r.Width, r.Height * 0.28f);
                    GraphicsState durum = g.Save();
                    g.SetClip(yol);
                    using (var firca = new SolidBrush(Color.FromArgb(28, Renkler.Kontur)))
                        g.FillRectangle(firca, alt);
                    g.Restore(durum);
                }

                using (var kalem = new Pen(Renkler.Kontur, kontur))
                    g.DrawPath(kalem, yol);
            }
        }

        /// <summary>Görseli verilen saydamlıkla (0..1) çizer.</summary>
        public static void SaydamGorsel(Graphics g, Image gorsel, RectangleF hedef, float saydamlik)
        {
            if (saydamlik >= 0.99f)
            {
                g.DrawImage(gorsel, hedef);
                return;
            }
            if (saydamlik <= 0.01f) return;

            var matris = new ColorMatrix { Matrix33 = saydamlik };
            using (var ozellik = new ImageAttributes())
            {
                ozellik.SetColorMatrix(matris);
                g.DrawImage(gorsel,
                    new[] { hedef.Location, new PointF(hedef.Right, hedef.Top), new PointF(hedef.Left, hedef.Bottom) },
                    new RectangleF(0, 0, gorsel.Width, gorsel.Height), GraphicsUnit.Pixel, ozellik);
            }
        }

        /// <summary>Görseli merkezine göre ölçekleyerek çizer.</summary>
        public static void MerkezdenCiz(Graphics g, Image gorsel, PointF merkez, float olcek, float saydamlik = 1f)
        {
            float w = gorsel.Width * olcek, h = gorsel.Height * olcek;
            SaydamGorsel(g, gorsel, new RectangleF(merkez.X - w / 2f, merkez.Y - h / 2f, w, h), saydamlik);
        }
    }
}
