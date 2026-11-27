using System;
using NAudio.Wave;

namespace FlappyCat
{
    public class OnbellekSesKaynagi : ISampleProvider
    {
        private readonly OnbellekSes ses;
        private int konum;

        public OnbellekSesKaynagi(OnbellekSes ses)
        {
            this.ses = ses;
        }

        public WaveFormat WaveFormat
        {
            get { return ses.Bicim; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int kalan = ses.Ornekler.Length - konum;
            int adet = Math.Min(kalan, count);
            Array.Copy(ses.Ornekler, konum, buffer, offset, adet);
            konum += adet;
            return adet;
        }
    }

    public class DonguSesKaynagi : ISampleProvider
    {
        private readonly OnbellekSes ses;
        private int konum;

        public float Seviye { get; set; }

        public bool Sessiz { get; set; }

        public DonguSesKaynagi(OnbellekSes ses)
        {
            this.ses = ses;
            Seviye = 1f;
        }

        public WaveFormat WaveFormat
        {
            get { return ses.Bicim; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            float carpan = Sessiz ? 0f : Seviye;
            float[] ornekler = ses.Ornekler;
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = ornekler[konum] * carpan;
                konum++;
                if (konum >= ornekler.Length) konum = 0;
            }
            return count;
        }
    }
}
