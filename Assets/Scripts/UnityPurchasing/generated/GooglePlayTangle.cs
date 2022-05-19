// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("f/xygFQFaJNUZ8llI3wZrQClyM2Ba0S8vBNkeE+Xt2k1JFmq1MqRVOmwpGebbm9XFQjM9a8jE5MRmqsHqtaG41WgVGchZALlv19C7fzvHcN6ok1IT3DVe1GoljTVsvMXzWd1Nan48PdGWRKW8nR22b1ND2MFUE3pTP719uBUtIUfxTxLY30LCDMrp9AWUbsValGjQxedVEi4orCkYpQWJdYtSe75VreWI4gwGertKKAveI4ERWNnda6Zo7Oh7vqkp52b39vWAg5f7W5NX2JpZkXpJ+mYYm5ubmpvbO1uYG9f7W5lbe1ubm/FydWBtZkF6TgqEj5/nI12qfXPFkN1rGx0lusqw/vbM8Kg5R2g6YOzVbS4wSl5LokNpfpuI5L2AG1sbm9u");
        private static int[] order = new int[] { 8,6,4,10,9,7,7,13,12,9,12,13,13,13,14 };
        private static int key = 111;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
