using System.Collections.Generic;
using NAudio.Wave;

namespace FlappyCat
{
    /// <summary>
    /// Ses dosyasını bir kez okuyup belleğe alır.
    /// Böylece her çalışta diskten okunmaz ve ses anında başlar.
    /// </summary>
    public class OnbellekSes
    {
        public float[] Ornekler { get; private set; }
        public WaveFormat Bicim { get; private set; }

        public OnbellekSes(string dosyaYolu)
        {
            using (var okuyucu = new AudioFileReader(dosyaYolu))
            {
                Bicim = okuyucu.WaveFormat;
                var tumu = new List<float>();
                var tampon = new float[Bicim.SampleRate * Bicim.Channels];
                int okunan;
                while ((okunan = okuyucu.Read(tampon, 0, tampon.Length)) > 0)
                {
                    for (int i = 0; i < okunan; i++) tumu.Add(tampon[i]);
                }
                Ornekler = tumu.ToArray();
            }
        }
    }
}
