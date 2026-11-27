using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace FlappyCat
{
    public class SesYoneticisi : IDisposable
    {
        private const int OrnekleHizi = 44100;
        private const int KanalSayisi = 2;

        private readonly Dictionary<string, OnbellekSes> efektler = new Dictionary<string, OnbellekSes>();
        private readonly MixingSampleProvider karistirici;
        private readonly DonguSesKaynagi muzik;
        private readonly WaveOutEvent cikis;
        private bool sesAcik = true;

        public bool SesAcik
        {
            get { return sesAcik; }
            set
            {
                sesAcik = value;
                if (muzik != null) muzik.Sessiz = !value;
            }
        }

        public SesYoneticisi(string klasor)
        {
            try
            {
                karistirici = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(OrnekleHizi, KanalSayisi))
                {
                    ReadFully = true
                };

                foreach (string ad in new[] { "tik", "miyav" })
                    efektler[ad] = new OnbellekSes(Path.Combine(klasor, ad + ".wav"));

                muzik = new DonguSesKaynagi(new OnbellekSes(Path.Combine(klasor, "muzik.wav"))) { Seviye = 0.55f };
                karistirici.AddMixerInput(muzik);

                cikis = new WaveOutEvent { DesiredLatency = 70, NumberOfBuffers = 3 };
                cikis.Init(karistirici);
                cikis.Play();
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException
                                       || ex is NAudio.MmException || ex is ArgumentException)
            {
                cikis = null;
            }
        }

        public void Cal(string ad)
        {
            OnbellekSes efekt;
            if (!sesAcik || cikis == null || !efektler.TryGetValue(ad, out efekt)) return;
            karistirici.AddMixerInput(new OnbellekSesKaynagi(efekt));
        }

        public void Dispose()
        {
            if (cikis == null) return;
            cikis.Stop();
            cikis.Dispose();
        }
    }
}
