// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("QvrhFfvCaM+pRwW+DK9OnKcorUYj98Bwri8z4HBwyfXGZ27fch6b2SMdLEWp2PpLEQcfFuLacYoafirwgJJXKcM0C06KtI64+rwc8d+31mQLJQMcvE5Em+wwOtsLft2DooooBcr+8Z/Wvb2KrqrvbWlcmTx47qj1wWRn7usNS94hNivGmfBbzPFV9eLStE+CM7sv7s4hAPkWgDVUciW14EcQgDd4lSyLwICUqk0aeWs7nn+ADm/Z1k6oEhUCPNB8R9fhUAlJO1CnFZa1p5qRnr0R3xFgmpaWlpKXlP5XyL7yUK+ucDyzUl5YqyW6Nk1UnlrxLgUWbhyd8p4mGph54c6vilIVlpiXpxWWnZUVlpaXCVJxLVippc8h/E4FAF6vZJWUlpeW");
        private static int[] order = new int[] { 7,5,4,6,13,13,11,9,10,10,10,11,12,13,14 };
        private static int key = 151;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
