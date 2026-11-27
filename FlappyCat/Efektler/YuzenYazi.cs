using System;
using System.Drawing;

namespace FlappyCat
{
    public class YuzenYazi
    {
        private readonly string metin;
        private readonly float x;
        private float y;
        private readonly float boyut;
        private readonly Color renk;
        private readonly bool gokkusagi;
        private readonly float toplamOmur;
        private float omur;

        public YuzenYazi(string metin, float x, float y, float boyut, Color renk, float omur, bool gokkusagi = false)
        {
            this.metin = metin;
            this.x = x;
            this.y = y;
            this.boyut = boyut;
            this.renk = renk;
            this.omur = omur;
            this.gokkusagi = gokkusagi;
            toplamOmur = omur;
        }

        public bool Bitti { get { return omur <= 0; } }

        public void Guncelle(float dt)
        {
            y -= 0.6f * dt;
            omur -= dt;
        }

        public void Ciz(Graphics g)
        {
            float gecen = toplamOmur - omur;
            float buyume = 1f + 0.35f * Math.Max(0f, 1f - gecen / 8f);
            int saydamlik = (int)(255 * Math.Min(1f, omur / 18f));
            float b = boyut * buyume;

            if (gokkusagi)
                Cizim.GokkusagiYazi(g, metin, b, new PointF(x, y), 7f, 0.5f, saydamlik);
            else
                Cizim.Yazi(g, metin, b, new PointF(x, y), renk, 5f, 0.5f, saydamlik);
        }
    }
}
