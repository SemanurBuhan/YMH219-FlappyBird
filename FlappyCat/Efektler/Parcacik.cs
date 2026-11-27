using System;
using System.Drawing;

namespace FlappyCat
{
    /// <summary>Gökkuşağı izinden dökülen küçük parıltı.</summary>
    public class Parcacik
    {
        private float x, y;
        private readonly float olcek;
        private readonly float toplamOmur;
        private float omur;

        public Parcacik(float x, float y, float olcek, float omur)
        {
            this.x = x;
            this.y = y;
            this.olcek = olcek;
            this.omur = omur;
            toplamOmur = omur;
        }

        public bool Bitti { get { return omur <= 0; } }

        public void Guncelle(float dt, float kayma)
        {
            x -= kayma * dt;
            y -= 0.15f * dt;
            omur -= dt;
        }

        public void Ciz(Graphics g, float yogunluk)
        {
            float oran = omur / toplamOmur;                       // 1 → 0
            float boyut = olcek * (float)Math.Sin(oran * Math.PI); // büyüyüp küçülür
            Cizim.MerkezdenCiz(g, Varliklar.Parilti, new PointF(x, y), boyut, yogunluk);
        }
    }
}
