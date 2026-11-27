using System.Drawing;

namespace FlappyCat
{
    /// <summary>
    /// Tıklanabilir buton. Görseli varsa tasarımdaki buton resmi çizilir,
    /// yoksa krem renkli kart üzerine ikon ve yazı çizilir.
    /// </summary>
    public class Buton
    {
        public RectangleF Alan { get; set; }
        public string Metin { get; set; }
        public Bitmap Gorsel { get; set; }
        public Bitmap Ikon { get; set; }
        public bool UzerindeMi { get; set; }

        public Buton(RectangleF alan, Bitmap gorsel)
        {
            Alan = alan;
            Gorsel = gorsel;
        }

        public Buton(RectangleF alan, string metin, Bitmap ikon)
        {
            Alan = alan;
            Metin = metin;
            Ikon = ikon;
        }

        public bool Icerir(Point nokta)
        {
            return Alan.Contains(nokta);
        }

        public void Ciz(Graphics g)
        {
            RectangleF r = Alan;
            if (UzerindeMi) r.Inflate(4, 3);   // fare üstündeyken hafifçe büyür

            if (Gorsel != null)
            {
                g.DrawImage(Gorsel, r);
                return;
            }

            Cizim.Kart(g, r, UzerindeMi ? Renkler.SariAcik : Renkler.Krem, 16f);

            float yaziX = r.X + r.Width / 2f;
            if (Ikon != null)
            {
                float ikonH = r.Height * 0.62f;
                float ikonW = Ikon.Width * ikonH / Ikon.Height;
                g.DrawImage(Ikon, r.X + 16, r.Y + (r.Height - ikonH) / 2f - 2, ikonW, ikonH);
                yaziX = r.X + 16 + ikonW + (r.Width - 16 - ikonW) / 2f;
            }
            Cizim.Yazi(g, Metin, 24f, new PointF(yaziX, r.Y + r.Height / 2f - 2), Renkler.Kontur, 0f);
        }
    }
}
