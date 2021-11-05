#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("DSsPHRhOHxgIFlpra3d+O0l0dG/CLWTanE7CvIKiKVngw85qhWW6SXd+O1J1eDUqPSs/HRhOHxAIBlpr0gJp7kYVzmREgOk+GKFOlFZGFuquIbbvFBUbiRCqOg01b84nFsB5DS4pKi8rKC1BDBYoLispKyIpKi8rYjt6aGhudn5oO3p4eH5rb3p1eH47enV/O3h+aW9yfXJ4em9ydHU7a2Ras4PiytF9hz9wCsu4oP8AMdgEsLhqiVxITtq0NFqo4+D4a9b9uFcdGE4GFR8NHw8wy3Jcj20S5e9wlm9zdGlyb2IqDSsPHRhOHxgIFlprXmUEV3BLjVqS3295EAuYWpwokZpCvB4SZwxbTQoFb8iskDggXLjOdGt3fjtJdHRvO1haKwUMFistKy8pO1haK5kaOSsWHRIxnVOd7BYaGhqZGhsdEjGdU53seH8eGiua6SsxHS2CVzZjrPaXgMfobIDpbclsK1TaEzAdGh4eHBkaDQVzb29raCE0NGwEnpieAIImXCzpsoBblTfPqosJwyuZH6ArmRi4uxgZGhkZGhkrFh0Spe9ogPXJfxTQYlQvw7kl4mPkcNM3O3h+aW9yfXJ4em9+O2t0d3J4Ynl3fjtob3p1f3ppfztvfml2aDt6rACmiFk/CTHcFAatVodFeNNQmwwTRSuZGgodGE4GOx+ZGhMrmRofKx4bGJkaFBsrmRoRGZkaGhv/irISFh0SMZ1TnewWGhoeHhsYmRoaG0coLUEreSoQKxIdGE4fHQgZTkgqCG9yfXJ4em9+O3liO3p1YjtremlvHx0IGU5IKggrCh0YTh8RCBFaa2sxnVOd7BYaGh4eGyt5KhArEh0YTnyUE6877NC3Nzt0a60kGiuXrFjUdX87eHR1f3JvcnR1aDt0fTtuaH6bDzDLclyPbRLl73CWNVu97FxWZFLDbYQoD366bI/SNhkYGhsauJkaHPdmIpiQSDvII9+qpIFUEXDkMOcmPXw7kShx7BaZ1MXwuDTiSHFAf2xsNXpra3d+NXh0djR6a2t3fnh6s8dlOS7RPs7CFM1wz7k/OArsureUaJp73QBAEjSJqeNfU+t7I4UO7isKHRhOHxEIEVpra3d+O1J1eDUqHSsUHRhOBggaGuQfHisYGhrkKwY7dH07b3N+O29zfnU7emtrd3J4egSKwAVcS/Ae9kVinzbwLblMV073qitD90EfKZdzqJQGxX5o5HxFfqdrd347WH5pb3J9cnh6b3J0dTtabjVbvexcVmQTRSsEHRhOBjgfAysNaXp4b3J4fjtob3pvfnZ+dW9oNSt/LjgOUA5CBqiP7O2HhdRLodpDSxSGJugwUjMB0+XVrqIVwkUHzdAmNCua2B0TMB0aHh4cGRkrmq0BmqjbeChs7CEcN03wwRQ6FcGhaAJUrj0rPx0YTh8QCAZaa2t3fjtYfmlvP/nwyqxrxBRe+jzR6nZj9vyuDAyQApLF4lB37hywOSsZ8wMl40sSyHJ9cnh6b3J0dTtabm9zdGlyb2IqjoVhF79ckEDPDSwo0N8UVtUPcspJfndyenV4fjt0dTtvc3JoO3h+aWErmRptKxUdGE4GFBoa5B8fGBkaS7GRzsH/58sSHCyrbm46");
        private static int[] order = new int[] { 12,58,3,19,23,7,40,45,17,24,30,45,25,52,40,52,34,48,35,49,25,50,44,42,59,33,40,47,48,39,49,58,50,43,50,47,51,56,51,54,55,59,54,59,52,55,54,58,54,59,52,56,57,59,59,57,57,58,58,59,60 };
        private static int key = 27;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
