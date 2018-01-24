namespace Kornea.Audio.DSP
{
    public class RingBuffer
    {
        private float[] buffer;
        private int size;
        private int pos;

        public RingBuffer(int Size, int pos = 0)
        {
            size = Size;
            buffer = new float[Size];
        }

        public float Run(float inp)
        {
            float outp = buffer[pos];
            buffer[pos] = inp;
            pos = (pos + 1) % size;
            return outp;
        }

        public void apply_gain(float gain, int s)
        {
            int i = pos;
            while (s > 0)
            {
                if (i == 0) i = size - 1;
                else i--;

                buffer[i] = buffer[i] * gain;

                s--;
            }
        }

        public void Reset()
        {
            apply_gain(0.0f, size);
        }
    }
}